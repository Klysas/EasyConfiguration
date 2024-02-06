namespace EasyConfiguration.Core.Exceptions
{
	public class ConfigurationNotFoundException : ConfigurationException
	{
		//========================================================
		//	Constructors
		//========================================================

		public ConfigurationNotFoundException()
			: base()
		{ }

		public ConfigurationNotFoundException(string? message)
			: base(message)
		{ }

		public ConfigurationNotFoundException(Type? type)
			: this(type, type?.ToString())
		{ }

		public ConfigurationNotFoundException(Type? type, string? message)
			: base(message)
		{
			Type = type;
		}

		//========================================================
		//	Properties
		//========================================================

		public Type? Type { get; private set; }
	}
}