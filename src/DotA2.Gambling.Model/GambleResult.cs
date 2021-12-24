namespace DotA2.Gambling.Model
{
    public class GambleResult
    {
        public float Odds { get; set; }
        public float Win { get; set; }
        public float NewBalance { get; set; }
        public int BettingAccountId { get; set; }
    }
}