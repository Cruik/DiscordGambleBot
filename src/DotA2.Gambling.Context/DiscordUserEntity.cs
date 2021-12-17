using System.Data.SqlClient;
using System.Threading;
using Bot.Model;
using Dapper;
using Dapper.Contrib.Extensions;
using DotA2.Gambling.Model;

namespace DotA2.Gambling.Context
{
    [Table("DiscordUser")]
    public class DiscordUserEntity
    {
        public DiscordUserEntity(DiscordUser discordUser)
        {
            Name = discordUser.Name;
            Discriminator = discordUser.Discriminator;
            RichKid = discordUser.RichKid;
            Emoji = discordUser.Emoji;
        }
        
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public bool RichKid { get; set; }
        public string Emoji { get; set; }
    }
}