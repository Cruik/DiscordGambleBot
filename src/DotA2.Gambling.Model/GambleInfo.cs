using System.Collections.Generic;

namespace DotA2.Gambling.Model
{
    public class GambleInfo
    {
        public GambleInfo()
        {
            Bets = new List<Bet>();
        }
        public Gamble Gamble{ get; set; }

        public List<Bet> Bets { get; set; }
    }
}