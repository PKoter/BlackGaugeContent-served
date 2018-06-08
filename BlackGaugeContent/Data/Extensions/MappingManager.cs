using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using JetBrains.Annotations;

namespace Bgc.Data.Extensions
{
	// singleton
	public class MappingManager
	{
		private readonly ReaderWriterLockSlim       _lock;
		private readonly IGenerateMapping           _generator;
		private readonly Dictionary<string, object> _mappings;

		public MappingManager([NotNull] IGenerateMapping generator)
		{
			_generator = generator;
			_mappings  = new Dictionary<string, object>();
			_lock      = new ReaderWriterLockSlim();
		}

		[CanBeNull]
		public Action<DbDataReader, T> GetSpMappingFor<T>([NotNull] IDataRecord reader, string spName)
		{
			_lock.EnterReadLock();
			var exist = _mappings.TryGetValue(spName, out object mappingObj);
			_lock.ExitReadLock();
			if (exist)
				return mappingObj as Action<DbDataReader, T>;
				
			return CreateMapping<T>(reader, spName);
		}

		private Action<DbDataReader, T> CreateMapping<T>([NotNull] IDataRecord reader, string spName)
		{
			try 
			{
				_lock.EnterWriteLock();
				// there may be situation, when multiple writers exit read and are queued. 
				// after first that is granted and finished,
				// mapping is updated, so following writers should check and return.
				if(_mappings.TryGetValue(spName, out object mappingObj))
					return mappingObj as Action<DbDataReader, T>;

				var mapping = AddMapping<T>(reader, spName);
				return mapping;
			}
			finally{
				_lock.ExitWriteLock();
			}
		}

		[NotNull]
		private Action<DbDataReader, T> AddMapping<T>([NotNull] IDataRecord reader, string spName)
		{
			var mapping = _generator.CreatePropertyMapping<T>(reader);
			_mappings.Add(spName, mapping);
			return mapping;
		}

	}
}
