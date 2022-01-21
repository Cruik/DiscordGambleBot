using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bot.Model;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DotA2.Gambling.Model;

namespace Bot.Modules
{
    [Group("gamble")]
    public class BetModule : ModuleBase<SocketCommandContext>
    {
        private readonly IGambler _gambler;
        private readonly IDataService _dataService;
        private string _tokenSymbol;
        private readonly CommandService _service;
        private readonly BotConfiguration _config;

        public BetModule(IGambler gambler, IDataService dataService, CommandService service, BotConfiguration config)
        {
            _gambler = gambler;
            _dataService = dataService;
            _service = service;
            _config = config;
            _tokenSymbol = "📀";
        }

        [Command("start")]

        [Summary("Starts a gamble. @Username is needed.")]
        public async Task StartBet(IUser user)
        {
            string userName = user.Username;
            var caller = Context.User;
            if(IsUserRegistered(caller).GetAwaiter().GetResult())
            {
                if(!_dataService.DoesUnfinishedGambleExist(userName))
                {
                    var gamble = _gambler.StartGamble(userName);

                    if(gamble != null)
                    {
                        var builder = new EmbedBuilder()
                        {
                            Color = new Color(114, 137, 218),
                            Description = "Time to loose some MMR"
                        };

                        var msgText = $"The bet is now open for player: '**{userName}**' \n > Odds are: **{gamble.Odds}**";

                        builder.AddField(x =>
                        {
                            x.Name = $"!!Attention!!";
                            x.Value = msgText;
                            x.IsInline = false;
                        });

                        builder.AddField(x =>
                        {
                            x.Name = $"WIN ";
                            x.Value = $"!gamble win **@{userName}** 10";
                            x.IsInline = false;
                        });

                        builder.AddField(x =>
                        {
                            x.Name = $"LOSS";
                            x.Value = $"!gamble loss **@{userName}** 10";
                            x.IsInline = false;
                        });

                        await ReplyAsync("", false, builder.Build());
                        Thread.Sleep(1000 * 60 * 2);
                        msgText = $"> The bet is open for 1 more minute! \n player: '**{userName}**' \n > Odds are: **{gamble.Odds}**";
                        await ReplyAsync(msgText);
                        Thread.Sleep(1000 * 60);

                        await CloseBet(user);
                    }
                    else
                    {
                        await ReplyAsync($"Error?!");
                    }
                }
                else
                {

                    await ReplyAsync($"The Last Game for this player is not over yet!");
                }
            }

        }

        [Command("close")]
        [Summary("Closes a gamble. Bets no longer possible. @Username is needed.")]
        public async Task CloseBet(IUser user)
        {

            var caller = Context.User;
            if(IsUserRegistered(caller).GetAwaiter().GetResult())
            {
                var id = _gambler.CloseGamble(user.Username);

                if(id > 0)
                {
                    var gambleInfo = _dataService.GetGambleInfoByName(user.Username);

                    var builder = new EmbedBuilder()
                    {
                        Color = new Color(114, 137, 218),
                        Description = "No more Bets possible"
                    };

                    var msgText = $@"The bet is now closed for player: '{gambleInfo.Gamble.Name}'";
                    builder.AddField(x =>
                    {
                        x.Name = $"{msgText}";
                        x.Value = "-";
                        x.IsInline = false;
                    });

                    builder.AddField(x =>
                    {
                        x.Name = $"💰💰💰 {gambleInfo.Bets.Count} Bets placed 💰💰💰";
                        x.Value = "-";
                        x.IsInline = false;
                    });

                    foreach(var bet in gambleInfo.Bets)
                    {
                        builder.AddField(x =>
                        {
                            x.Name = $"'{bet.Better}' predicted -> '{bet.Prediction}' - {bet.Amount} {_tokenSymbol} ";
                            x.Value = "-";
                            x.IsInline = false;
                        });
                    }

                    await ReplyAsync("", false, builder.Build());
                }
            }
        }

        [Command("win")]
        [Summary("Place WIN bet on gamble. @Username is needed. amount of token default 10")]
        public async Task Win(IUser user, int token = 10)
        {
            var caller = Context.User;
            if(IsUserRegistered(caller).GetAwaiter().GetResult())
            {
                await SetBet(user, Prediction.Win, token);
            }
        }

        [Command("loss")]
        [Summary("Place LOSS bet on gamble. @Username is needed. amount of token default 10")]
        public async Task Loss(IUser user, int token = 10)
        {
            var caller = Context.User;

            if(IsUserRegistered(caller).GetAwaiter().GetResult())
            {
                await SetBet(user, Prediction.Loss, token);
            }
        }

        private async Task SetBet(IUser user, Prediction prediction, int token)
        {
            var caller = Context.User;

            var callerAccount = _dataService.GetAccount(caller.Username);
            if(callerAccount != null)
            {
                var canPlaceBet = callerAccount.Balance - token >= 0;

                if(canPlaceBet)
                {
                    var betId = _gambler.Bet(user.Username, caller.Username, prediction, token);

                    if(betId > 0)
                    {
                        await ReplyAsync("Bet accepted!");
                    }
                    else
                    {
                        await ReplyAsync("Too late to apologize! No Bets possible!");
                        await ReplyAsync("https://tenor.com/view/im-afraid-its-too-late-scared-nervous-concerned-running-out-of-time-gif-15989937");
                    }
                }
                else
                {
                    await ReplyAsync($"{caller.Username} you have not enough to place this bet!");
                    await ReplyAsync("https://tenor.com/view/patrick-poor-ugly-thing-spongbob-gif-13658543");
                }
            }
        }

        [Command("register")]
        [Summary("Register for Gambling")]
        public async Task Register()
        {
            var caller = Context.User;
            var acc = _dataService.GetAccount(caller.Username);

            if(acc == null)
            {
                var discordUser = ToDiscordUser(caller);
                var result = _dataService.AddUser(discordUser);

                if(result > 0)
                {
                    await ReplyAsync("Successful, now you can bet!");
                }
                else
                {
                    await ReplyAsync("Error?!");
                }
            }
            else
            {
                await ReplyAsync("You are already registered!");
            }
        }

        [Command("balance")]
        [Summary("Shows your account balance")]
        public async Task GetBalance()
        {
            var caller = Context.User;
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();

            if(isRegistered)
            {
                var discordUser = ToDiscordUser(caller);
                Account account = _dataService.GetAccount(discordUser);
                if(account != null)
                {
                    await ReplyAsync($"> You have {account.Balance} {_tokenSymbol} in your account!");
                }

            }
        }

        [Command("board")]
        [Summary("Shows Leaderboard")]
        public async Task GetLeaderboard()
        {
            var caller = Context.User;
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();

            if(isRegistered)
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Description = "Leaderboard"
                };

                var discordUser = ToDiscordUser(caller);
                List<Account> accounts = _dataService.GetLeaderBoard();
                var cnt = 1;

                foreach(var account in accounts)
                {
                    builder.AddField(x =>
                    {
                        x.Name = $"{cnt}. {account.DiscordUser.Name}";
                        x.Value = $"{account.Balance} {_tokenSymbol}";
                        x.IsInline = false;
                    });

                    //await ReplyAsync($"> {cnt}. {account.DiscordUser.Name} -- {account.Balance} {_tokenSymbol}");
                    cnt++;
                }

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("info")]
        [Summary("Show Infos for open Gamble. @Username needed")]
        public async Task GetGambleInfo(IUser user)
        {
            var caller = Context.User;
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();

            if(isRegistered)
            {
                var gamble = _dataService.GetGambleByName(user.Username);

                if(gamble != null)
                {
                    //todo get list of all bets
                    await ReplyAsync("> Gamble Info");
                    await ReplyAsync($"> {gamble.Name} - Odds : '{gamble.Odds}'");
                }
                else
                {
                    await ReplyAsync($"> No open Game for {user.Username}");
                }

            }
            else
            {
                await ReplyAsync("You are already registered!");
            }
        }

        [Command("end")]
        [Summary("End Gamble and set Result of Match.\n @Username is needed. \n result[win/loss] \n")]
        public async Task EndGamble(IUser user, string result, string matchId = "-")
        {
            var caller = Context.User;
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();

            if(isRegistered)
            {
                var gamble = _dataService.GetGambleInfoByName(user.Username);

                if(gamble != null)
                {
                    if(result.Equals("win", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var gambleId = _dataService.EndGamble(user.Username, Prediction.Win, matchId);
                        await ReplyAsync($"{user.Username} won his Match");
                        await ReplyAsync("https://tenor.com/view/-gif-4519334");

                        if(gambleId > 0)
                        {
                            await GetLeaderboard();
                        }
                    }
                    else if(result.Equals("loss", StringComparison.InvariantCultureIgnoreCase))
                    {
                        await ReplyAsync($"{user.Username} lost his Match");
                        await ReplyAsync($"https://tenor.com/view/supernatural-loser-yousuck-insult-diss-gif-12572295");
                        var gambleId = _dataService.EndGamble(user.Username, Prediction.Loss, matchId);

                        if(gambleId > 0)
                        {
                            await GetLeaderboard();
                        }
                    }
                    else
                    {
                        await ReplyAsync("> Not a valid result. WIN or LOSS!! Nothing between!!!");
                    }
                }
            }
        }

        private async Task<bool> IsUserRegistered(IUser user)
        {
            var registeredUser = _dataService.GetUserByName(user.Username);

            bool isRegistered = registeredUser != null;

            if(!isRegistered)
            {
                await ReplyAsync("You are not registered\nRegister first! -> !gamble register");
            }

            return isRegistered;
        }

        public DiscordUser ToDiscordUser(SocketUser user)
        {
            var dUser = new DiscordUser();
            dUser.Name = user.Username;
            dUser.Discriminator = user.Discriminator;
            return dUser;
        }

        [Command("help")]
        [Summary("Shows Help")]
        public async Task Help()
        {
            var caller = Context.User;
            var prefix = _config.Prefix;

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Help",
                ThumbnailUrl = @"https://i.pinimg.com/564x/8a/8b/50/8a8b50da2bc4afa933718061fe291520.jpg",
            };
            var betModule = _service.Modules.FirstOrDefault(x => x.Name == "gamble");

            if(betModule != null)
            {

                foreach(var cmd in betModule.Commands)
                {
                    string command = null;
                    var result = await cmd.CheckPreconditionsAsync(Context);

                    if(result.IsSuccess)
                    {
                        var parameters = cmd.Parameters;
                        string parameterString = string.Empty;

                        if(parameters != null)
                        {
                            parameterString = string.Join(" ", parameters.Select(x => x.Name).ToList());
                        }
                        command += $"{prefix}{cmd.Aliases.First()} {parameterString}\n";
                    }

                    if(!string.IsNullOrWhiteSpace(command))
                    {
                        builder.AddField(x =>
                        {
                            x.Name = $"{command}\n";
                            x.Value = $"{cmd.Summary}";
                            x.IsInline = false;
                        });

                    }
                }
            }
            await ReplyAsync("", false, builder.Build());

        }
    }

}