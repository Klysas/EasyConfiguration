namespace EasyConfiguration.Core.Attributes
{
	public sealed class DefaultIntAttribute : AbstractDefaultAttribute
	{
		public DefaultIntAttribute(int value)
		{
			Value = value;
		}

		public int Value { get; private set; }
	}
}