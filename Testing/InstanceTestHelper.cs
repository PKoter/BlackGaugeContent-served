using System.Reflection;
using JetBrains.Annotations;

namespace Testing
{
	public class InstanceTestHelper
	{
		[Pure]
		[NotNull]
		public static T GetPrivateField<T>([NotNull] object instance, [NotNull] string name)
		{
			return (T)instance.GetType()
				.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(instance);
		}
	}
}
