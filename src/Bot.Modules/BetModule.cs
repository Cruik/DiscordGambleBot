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
            var caller = Context.User;
            if(IsUserRegistered(caller).GetAwaiter().GetResult())
            {
                if(!_dataService.DoesUnfinishedGambleExist(user.Username))
                {
                    var gamble = _gambler.StartGamble(user.Username);

                    if(gamble != null)
                    {
                        var msgText = $"> The bet is now open for player: '**{user.Username}**' \n > Odds are: **{gamble.Odds}**";
                        await ReplyAsync(msgText);

                        await ReplyAsync($"> Place your bet !gamble win @{user.Username} 10");
                        await ReplyAsync($"> Place your bet !gamble loss @{user.Username} 10");

                        Thread.Sleep(1000 * 60);
                        msgText = $"> The bet is open for 1 more minute! \n player: '**{user.Username}**' \n > Odds are: **{gamble.Odds}**";
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
                    List<Bet> bets = _dataService.GetAllBetsForGamble(id);

                    var msgText = $@"> The bet is now closed for player: '{user.Username}'";
                    var msg = await ReplyAsync(msgText);
                    await ReplyAsync($"> 💰💰💰 {bets.Count} Bets placed 💰💰💰 \n -----------------------------------");

                    foreach(var bet in bets)
                    {
                        await ReplyAsync($"> '{bet.Better}' predicted -> '{bet.Prediction}' - {bet.Amount} {_tokenSymbol} ");
                    }
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

            //var msg = Context.Channel;
            //var better = Context.User;
            var caller = Context.User;
            var betId = _gambler.Bet(user.Username, caller.Username, prediction, token);

            if(betId > 0)
            {
                await ReplyAsync("Bet accepted!");
            }
            else
            {
                await ReplyAsync("Too late to apologize! No Bets possible!");
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

            }
            else
            {
                await ReplyAsync("You are already registered!");
            }
        }

        [Command("end")]
        [Summary("End Gamble and set Result of Match.\n @Username is needed. \n result[win/loss] \n matchId(Opendota) ")]
        public async Task EndGamble(IUser user, string result, string matchId)
        {
            var caller = Context.User;
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();

            if(isRegistered)
            {
                var gamble = _dataService.GetGambleByName(user.Username);
                if(gamble != null && gamble.Result != null)
                {

                }
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
            else
            {
                await ReplyAsync("You are already registered!");
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
            bool isRegistered = IsUserRegistered(caller).GetAwaiter().GetResult();
            var prefix = _config.Prefix;
            
            if(isRegistered)
            {
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

                            if (parameters != null)
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

}