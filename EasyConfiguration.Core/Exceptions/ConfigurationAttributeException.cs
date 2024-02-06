namespace EasyConfiguration.Core.Exceptions
{
	public class ConfigurationAttributeException : ConfigurationException
	{
		public ConfigurationAttributeException()
			: base()
		{ }

		public ConfigurationAttributeException(string? message)
			: base(message)
		{ }
	}
}