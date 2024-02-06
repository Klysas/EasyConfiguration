namespace EasyConfiguration.Core.Attributes
{
	public sealed class DefaultBoolAttribute : AbstractDefaultAttribute
	{
		public DefaultBoolAttribute(bool value)
		{
			Value = value;
		}

		public bool Value { get; private set; }
	}
}