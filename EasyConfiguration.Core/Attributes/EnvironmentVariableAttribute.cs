namespace EasyConfiguration.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class EnvironmentVariableAttribute : Attribute
	{
		public EnvironmentVariableAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}