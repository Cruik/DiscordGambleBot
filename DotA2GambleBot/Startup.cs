using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Bot.Model;
using Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DotA2.Gambling.Context;
using DotA2.Gambling.Model;
using DotA2GamblingMachine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotA2GambleBot
{
    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            var isDevelopment = string.IsNullOrEmpty(environment) ||
                                environment.ToLower() == "development";


            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.docker.json", optional: true)
                .AddEnvironmentVariables();

            //only add secrets in development
            if(isDevelopment)
            {
                builder.AddUserSecrets<Program>(true);
            }

            Configuration = builder.Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }
        public async Task RunAsync()
        {
            var services = new ServiceCollection();             // Create a new instance of a service collection
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();     // Build the service provider
            provider.GetRequiredService<LoggingService>();      // Start the logging service
            provider.GetRequiredService<CommandHandler>(); 		// Start the command handler service

            await provider.GetRequiredService<StartupService>().StartAsync();       // Start the startup service
            await Task.Delay(-1);                               // Keep the program alive
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var botConfigSection = Configuration.GetSection("BotConfiguration");
            BotConfiguration botConfiguration = botConfigSection.Get<BotConfiguration>();

            var botSecretSection = Configuration.GetSection("BotSecrets");
            BotSecrets botSecrets = botSecretSection.Get<BotSecrets>();

            botConfiguration.DefaultConnection = botSecrets.ConnectionString;
            botConfiguration.Token = botSecrets.DiscordBotToken;

            services.AddSingleton<BotConfiguration>(botConfiguration);
            
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       // Add discord to the collection
                LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
                MessageCacheSize = 1000             // Cache 1,000 messages per channel
            }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {                                       // Add the command service to the collection
                    LogLevel = LogSeverity.Verbose,     // Tell the logger to give Verbose amount of info
                    DefaultRunMode = RunMode.Async,     // Force all commands to run async by default
                }))
                .AddSingleton<CommandHandler>() // Add the command handler to the collection
                .AddSingleton<StartupService>() // Add startupservice to the collection
                .AddSingleton<LoggingService>();
            
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IGambler, Gambler>();
            services.AddScoped<IDbReader, BotDbContext>();
            services.AddScoped<IDbWriter, BotDbContext>();
        }

    }
}