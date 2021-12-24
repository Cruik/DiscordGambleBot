using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Bot.Model;
using Bot.Modules;
using Bot.Services.TypeReader;
using DapperExtensions;
using Microsoft.Extensions.Configuration;

namespace Bot.Services
{
    public class StartupService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly BotConfiguration _botConfig;

        // DiscordSocketClient, CommandService, and IConfigurationRoot are injected automatically from the IServiceProvider
        public StartupService(
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            BotConfiguration botConfig)
        {
            _provider = provider;
            _botConfig = botConfig;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            string discordToken = _botConfig.Token;     // Get the discord token from the config file
            if(string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
            await _discord.StartAsync();                                // Connect to the websocket

            await _discord.DownloadUsersAsync(_discord.Guilds);

            var moduleAssembly = Assembly.GetAssembly(typeof(HelpModule));
            await _commands.AddModulesAsync(moduleAssembly, _provider);     // Load commands and modules into the command service
            
        }
    }
}