using Dapper.Contrib.Extensions;
using DotA2.Gambling.Model;

namespace DotA2.Gambling.Context
{
    [Table("Accounts")]
    public class AccountEntity
    {
        public AccountEntity(Account account)
        {
            Id = account.Id;
            GuildId = account.GuildId;
            DiscordUserId = account.DiscordUserId;
            Balance = account.Balance;
        }
        [Key]
        public int Id { get; set; }
        public ulong GuildId { get; set; }
        public int DiscordUserId { get; set; }
        public int Balance { get; set; }
    }
}