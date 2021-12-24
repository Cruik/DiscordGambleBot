using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bot.Model;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly BotConfiguration _botConfig;
        private readonly IServiceProvider _provider;
        private bool _sentOnlyOnce = true;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            BotConfiguration config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _botConfig = config;
            _provider = provider;
            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            

            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot

            bool isMsgEmtpy = msg == null;

            if(isMsgEmtpy)
            {
                return;     // Ignore self when checking commands
            }
            else
            {
                bool isSenderBot = msg.Author.Id == _discord.CurrentUser.Id;
                if ((isSenderBot) && msg.Content != ("!gamble help"))
                {
                    return;
                }
            }

            var context = new SocketCommandContext(_discord, msg);     // Create the command context
            var regex = new Regex(@"[!]\w+");

            int argPos = 0;     // Check if the message has a valid command prefix
            bool isMsgValid = msg.Content.Length > 1 && regex.IsMatch(msg.Content);
            try
            {
               
                if(isMsgValid && (msg.HasStringPrefix(_botConfig.Prefix, ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos)))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command
                    var caller = context.User;
                    if(!result.IsSuccess)
                    {
                        // If not successful, reply with the error.
                        await context.Channel.SendMessageAsync($"Error. Did you use the command correctly?");
                        await context.Channel.SendMessageAsync($"!gamble help");
                    }
                }
                else
                {
                    //todo Handle all other messages
                }
            }
            catch(Exception ex)
            {
                await context.Channel.SendMessageAsync($"Error occured!");
                
                ulong botCreatorId = 324652745099313154;

                await context.Client.GetUser(botCreatorId).SendMessageAsync(ex.Message);
            }
        }
    }
}