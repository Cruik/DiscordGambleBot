using System;
using DotA2.Gambling.Model;

namespace DotA2GamblingMachine
{
    public class Gambler : IGambler
    {
        private readonly IDataService _dataService;

        public Gambler(IDataService dataService)
        {
            _dataService = dataService;
        }

        public Gamble StartGamble(string userName)
        {
            var random = new Random();
            var value = random.Next(110, 200);

            float odds = value / 100f;

            var gamble = new Gamble()
            {
                Name = userName,
                BetTime = DateTime.Now,
                Odds = odds,
                IsOpen = true
            };

            if (!_dataService.DoesUnfinishedGambleExist(gamble.Name))
            {
                gamble.Id = _dataService.CreateGamble(gamble);
            }
            else
            {
                //Todo log open gamble exists
            }

            if (gamble.Id <= 0)
            {
                gamble = null;
            }

            return gamble;
        }

        public int Bet(string userName, string better, Prediction prediction, int amount)
        {
            int betId;

            Gamble gamble = _dataService.GetGambleByName(userName);
            var betAccount = _dataService.GetAccount(better);

            if(gamble != null && gamble.IsOpen)
            {
                var bet = new Bet()
                {
                    Amount = amount,
                    GambleId = gamble.Id,
                    BettingAccountId = betAccount.Id,
                    Prediction = prediction
                };

                betId = _dataService.Bet(bet);
            }
            else
            {
                betId = -1;
            }

            return betId;
        }

        public int CloseGamble(string userName)
        {
            var id = _dataService.CloseGamble(userName);

            return id;
        }

        public int EndGamble(string userName, Prediction result, string matchId)
        {
            var id = _dataService.EndGamble(userName, result, matchId);

            return id;
        }
    }
}