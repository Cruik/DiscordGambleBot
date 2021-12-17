namespace DotA2.Gambling.Model
{
    public class DiscordUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Discriminator { get; set; }
        public bool RichKid { get; set; }
        public string Emoji { get; set; }
    }
}