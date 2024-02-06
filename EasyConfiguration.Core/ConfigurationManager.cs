using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using EasyConfiguration.Core.Attributes;
using EasyConfiguration.Core.Exceptions;
using EasyConfiguration.Core.Extensions;


namespace EasyConfiguration.Core
{
	public class ConfigurationManager
	{
		//========================================================
		//	Fields
		//========================================================

		private readonly ILogger<ConfigurationManager> _logger;
		private readonly IniFileManager _iniFileManager;

		private readonly List<AbstractConfig> _configs;

		private readonly bool _iniFileNotFound;

		//========================================================
		//	Constructors
		//========================================================

		public ConfigurationManager() :
			this(new IniFileManager(Path.GetFullPath("settings.ini")))
		{ }

		public ConfigurationManager(IniFileManager iniFileManager) :
			this(new NullLogger<ConfigurationManager>(), iniFileManager)
		{ }

		public ConfigurationManager(ILogger<ConfigurationManager> logger, IniFileManager iniFileManager)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_iniFileManager = iniFileManager ?? throw new ArgumentNullException(nameof(iniFileManager));
			_configs = new List<AbstractConfig>();

			_iniFileNotFound = !_iniFileManager.TryLoad();
			InitializeConfigs();
		}

		//========================================================
		//	Methods
		//========================================================
		//--------------------------------------------------------
		//	Public
		//--------------------------------------------------------

		public T GetConfig<T>() where T : AbstractConfig
		{
			return _configs.FirstOrDefault(c => c.GetType() == typeof(T)) as T ?? throw new ConfigurationNotFoundException(typeof(T));
		}

		//--------------------------------------------------------
		//	Private
		//--------------------------------------------------------

		private AbstractConfig CreateAndInitialize(Type configType)
		{
			_logger.LogDebug("Initializing '{type}' ...", configType.ToString());

			var instance = (AbstractConfig)Activator.CreateInstance(configType)!;

			foreach (var property in instance.GetType().GetProperties())
			{
				var defaultValue = GetDefaultAttributeValueOrThrow(property, configType);
				if (_iniFileNotFound) SetIniFileValue(property, defaultValue);

				var value = (GetEnvironmentVariableValue(property) ?? GetIniFileValue(property)) ?? defaultValue;
				value = Convert.ChangeType(value, property.PropertyType);

				ValidateRegexFormat(property, value);

				property.SetValue(instance, value);
			}

			return instance;
		}

		private static object GetDefaultAttributeValueOrThrow(PropertyInfo property, Type configType)
		{
			var defaultAttribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType.BaseType == typeof(AbstractDefaultAttribute));
			if (defaultAttribute is null)
				throw new ConfigurationAttributeException($"'{property.Name}' property of '{configType}' class must have default value attribute.");

			var defaultValue = defaultAttribute.ConstructorArguments[0].Value!;
			if (defaultValue.GetType() != property.PropertyType)
				throw new ConfigurationAttributeException($"'{property.Name}' property is of '{property.PropertyType}' type, but default value attribute is of '{defaultValue.GetType()}' type.");

			return defaultValue;
		}

		private object? GetIniFileValue(PropertyInfo property)
		{
			object? value = null;

			var iniFileAttribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(IniFileAttribute));
			if (iniFileAttribute is not null)
			{
				var iniKey = (string)(iniFileAttribute.ConstructorArguments[0].Value ?? property.Name);
				var iniSection = iniFileAttribute.ConstructorArguments[1].Value as string;

				value = _iniFileManager.GetValue(iniKey, iniSection);

				if (value != null)
					_logger.LogDebug("Found ini file variable: '{key}', value: '{value}'", iniKey, value);
			}

			return value;
		}

		private object? GetEnvironmentVariableValue(PropertyInfo property)
		{
			object? value = null;

			var environmentVariableAttribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(EnvironmentVariableAttribute));
			if (environmentVariableAttribute is not null)
			{
				var environmentVariableName = (string)environmentVariableAttribute.ConstructorArguments[0].Value!;
				value = Environment.GetEnvironmentVariable(environmentVariableName);

				if (value is not null)
					_logger.LogInformation("Found environment variable: '{key}'", environmentVariableName);
			}

			return value;
		}

		private void InitializeConfigs()
		{
			var baseType = typeof(AbstractConfig);
			var types = AppDomain.CurrentDomain.GetAssemblies()
						.SelectMany(a => a.GetTypes())
						.Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract);

			_logger.LogDebug("Found {count} Config classes.", types.Count());

			types.ForEach(type => _configs.Add(CreateAndInitialize(type)));
			if (_iniFileNotFound && !_iniFileManager.IsEmpty) _iniFileManager.Save();
		}

		private void SetIniFileValue(PropertyInfo property, object value)
		{
			var iniFileAttribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(IniFileAttribute));
			if (iniFileAttribute is not null)
			{
				var iniKey = (string)(iniFileAttribute.ConstructorArguments[0].Value ?? property.Name);
				var iniSection = iniFileAttribute.ConstructorArguments[1].Value as string;
				var iniComment = iniFileAttribute.ConstructorArguments[2].Value as string;

				_iniFileManager.SetValue(iniKey, value, iniSection, iniComment);
			}
		}

		private static void ValidateRegexFormat(PropertyInfo property, object? value)
		{
			var regexFormatAttribute = property.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(RegexFormatAttribute));
			if (regexFormatAttribute is not null && property.PropertyType != typeof(string))
				throw new ConfigurationAttributeException($"'{typeof(RegexFormatAttribute)}' attribute can be set only on 'string' type properties. Remove attribute from '{property.Name}' property.");

			if (regexFormatAttribute is not null && value is string stringValue)
			{
				var regexPattern = (string)regexFormatAttribute.ConstructorArguments[0].Value!;
				var exceptionMessage = (string)regexFormatAttribute.ConstructorArguments[1].Value!;
				if (!System.Text.RegularExpressions.Regex.IsMatch(stringValue, regexPattern))
					throw new ConfigurationException(string.Format(exceptionMessage, value));
			}
		}
	}
}