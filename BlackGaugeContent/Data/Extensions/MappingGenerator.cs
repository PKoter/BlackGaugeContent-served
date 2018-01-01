using System;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Bgc.Data.Extensions
{
	public class MappingGenerator
	{
		[NotNull]
		public Action<DbDataReader, T> CreatePropertyMapping<T>([NotNull] DbDataReader reader)
		{
			var type = typeof(T);
			var dynMethod = IlGenPropertyMapping(reader, type);
			return (Action<DbDataReader, T>)dynMethod.CreateDelegate(typeof(Action<DbDataReader, T>));
		}

		[NotNull]
		private DynamicMethod IlGenPropertyMapping([NotNull] DbDataReader reader, [NotNull] Type t)
		{
			var readerType = typeof(DbDataReader);
			var mapMethod = new DynamicMethod("_sqlMap", null, new[] { readerType, t });
			var ilGen = mapMethod.GetILGenerator();
			var indexer    = readerType
				.GetProperty("Item", typeof(object), new[] {typeof(int)}).GetMethod;

			var colNumber = reader.FieldCount;
			for(int i = 0; i < colNumber; i++)
			{
				var colName  = reader.GetName(i);
				var property = t.GetProperty(colName);
				if(property == null || property.CanWrite == false)
					throw new NotImplementedException($"property {colName} is not implemented or is read only!");

				PropertyMapping(ilGen, indexer, property, i);
			}
			ilGen.Emit(OpCodes.Ret);

			return mapMethod;
		}

		private static void PropertyMapping([NotNull] ILGenerator ilGen, [NotNull] MethodInfo readerIndexer, [NotNull] PropertyInfo objProperty, int columnIndex)
		{
			// codes based on il output of hardcoded method
			ilGen.Emit(OpCodes.Ldarg_1);
			ilGen.Emit(OpCodes.Ldarg_0);
			ilGen.Emit(OpCodes.Ldc_I4, columnIndex);
			ilGen.EmitCall(OpCodes.Callvirt, readerIndexer, null);

			var setterType = objProperty.PropertyType;
			ilGen.Emit(setterType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, setterType);

			var setter = objProperty.SetMethod;
			ilGen.EmitCall(OpCodes.Callvirt, setter, null);
		}
	}
}
