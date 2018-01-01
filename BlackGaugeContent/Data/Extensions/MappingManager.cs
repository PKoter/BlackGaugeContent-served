using System;
using System.Collections.Generic;
using System.Data.Common;
using JetBrains.Annotations;

namespace Bgc.Data.Extensions
{
	public class MappingManager
	{
		private readonly MappingGenerator _generator;
		private readonly Dictionary<string, object> _mappings;

		public MappingManager()
		{
			_generator = new MappingGenerator();
			_mappings  = new Dictionary<string, object>();
		}

		[CanBeNull]
		public Action<DbDataReader, T> GetSpMappingFor<T>(DbDataReader reader, string spName)
		{
			if (!_mappings.TryGetValue(spName, out object mappingObj))
			{
				var mapping = _generator.CreatePropertyMapping<T>(reader);
				_mappings.Add(spName, mapping);
				return mapping;
			}
			return mappingObj as Action<DbDataReader, T>;
		}

	}
}
