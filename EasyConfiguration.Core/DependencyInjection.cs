using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace EasyConfiguration.Core
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSingleton<ConfigurationManager>();
			services.AddSingleton(_ =>
			{
				return new IniFileManager(configuration["SettingsIniFilePath"]);
			});

			return services;
		}
	}
}