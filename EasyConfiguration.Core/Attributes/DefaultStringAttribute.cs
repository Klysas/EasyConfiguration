namespace EasyConfiguration.Core.Attributes
{
	public sealed class DefaultStringAttribute : AbstractDefaultAttribute
	{
		public DefaultStringAttribute(string value)
		{
			Value = value;
		}

		public string Value { get; private set; }
	}
}