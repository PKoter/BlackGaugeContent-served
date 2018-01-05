using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Bgc.Data.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Testing.UnitTests.Data
{
	[TestClass]
	public class MappingManager_Tests : InstanceTestHelper
	{
		[TestMethod]
		public void GetSpMappingFor_IsThreadSafe()
		{
			// arrange tings
			Action<DbDataReader, ModelI> action  = (a, b) => {};
			var reader    = Substitute.For<IDataRecord>();
			var generator = Substitute.For<IGenerateMapping>();
			generator.CreatePropertyMapping<ModelI>(reader).Returns(action);
			var manager   = new MappingManager(generator);


			const int taskCount = 5;
			const string spName = "Test";
			var tasks = new Task<Action<DbDataReader, ModelI>>[taskCount];
			for (int i = 0; i < taskCount; i++)
			{
				tasks[i] = Task.Factory.StartNew(() => manager.GetSpMappingFor<ModelI>(reader, spName));
			}

			for (int i = 0; i < taskCount; i++)
				tasks[i].Wait();

			var results = tasks.Select(t => t.Result).ToList();
			var expected = Enumerable.Repeat(action, taskCount).ToList();

				// all mapping method refer to this one
			CollectionAssert.AllItemsAreNotNull(results);
			CollectionAssert.AllItemsAreInstancesOfType(results, action.GetType());
			CollectionAssert.AreEqual(expected, results);

			var mappingDictionary = GetPrivateField<Dictionary<string, object>>(manager, "_mappings");
				// no method was created, even tho some of threads may entered write mode
			Assert.AreEqual(1, mappingDictionary.Count);
			Assert.AreEqual(action, mappingDictionary[spName] as Action<DbDataReader, ModelI>);
		}

		private class ModelI
		{
		}


	}
}
