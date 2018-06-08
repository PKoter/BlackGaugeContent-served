using System;
using System.Data;
using System.Data.Common;
using JetBrains.Annotations;

namespace Bgc.Data.Extensions
{
	public interface IGenerateMapping
	{
		Action<DbDataReader, T> CreatePropertyMapping<T>([NotNull] IDataRecord reader);

	}
}
