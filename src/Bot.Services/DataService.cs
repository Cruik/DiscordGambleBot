using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotA2.Gambling.Context;
using DotA2.Gambling.Model;

namespace Bot.Services
{
    public class DataService : IDataService
    {
        private readonly IDbReader _dataReader;
        private readonly IDbWriter _dataWriter;

        public DataService(IDbReader dataReader, IDbWriter dataWriter)
        {
            _dataReader = dataReader;
            _dataWriter = dataWriter;
        }

        public int AddUser(DiscordUser user)
        {
            var userId = _dataWriter.AddUser(user);
            
            if (userId > 0)
            {
                var account = new Account()
                {
                    Balance = 200,
                    DiscordUserId = userId,

                };
                _dataWriter.CreateAccount(account);
            }

            return userId;
        }

        public int CreateAccount(Account account)
        {
            return _dataWriter.CreateAccount(account);
        }

        public int CreateGamble(Gamble gamble)
        {
            var id = _dataWriter.InsertGamble(gamble);

            return id;
        }

        public int Bet(Bet bet)
        {
            var id = _dataWriter.InsertBet(bet);

            return id;
        }
        
        public Gamble GetGambleByName(string userName)
        {
            return _dataReader.GetGamble(userName);
        }

        public int EndGamble(string userName, Prediction prediction, string matchId)
        {
            return _dataWriter.EndGamble(userName, prediction, matchId);
        }

        public int CloseGamble(string name)
        {
            return _dataWriter.CloseGamble(name);
        }

        public DiscordUser GetUserByName(string username)
        {
            return _dataReader.GetUserId(username);
        }

        public Account GetAccount(DiscordUser discordUser)
        {
            return _dataReader.GetAccount(discordUser.Name);
        }

        public Account GetAccount(string discordUserName)
        {
            return _dataReader.GetAccount(discordUserName);
        }

        public void CalculateResults()
        {

        }

        public List<Account> GetLeaderBoard()
        {
            var result = _dataReader.GetLeaderBoard();

            return result;
        }

        public List<Bet> GetAllBetsForGamble(int id)
        {
            return _dataReader.GetBetsForGamble(id);
        }

        public bool DoesUnfinishedGambleExist(string userUsername)
        {
            return _dataReader.DoesUnfinishedGambleExist(userUsername);
        }
    }
}