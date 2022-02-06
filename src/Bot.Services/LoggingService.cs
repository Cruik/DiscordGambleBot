using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bot.Model;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace Bot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        private string _logDirectory { get; }

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {

            _discord = discord;
            _commands = commands;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            var criticalLogLevel = new List<LogSeverity>()
            {
                LogSeverity.Error, LogSeverity.Critical
            };
           
            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
           
            if (criticalLogLevel.Contains(msg.Severity))
            {
                
                Log.Logger.Error(logText);
            }
            else
            {
                Log.Logger.Debug(logText);
            }
            return null;
        }
    }
}
