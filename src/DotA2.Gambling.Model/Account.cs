using System.Collections.Generic;

namespace DotA2.Gambling.Model
{
    public class Account
    {
        public Account()
        {
            Bets = new List<Bet>();
        }
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public int DiscordUserId { get; set; }
        public DiscordUser DiscordUser { get; set; }
        public int Balance { get; set; }
        public List<Bet> Bets { get; set; }
    }
}