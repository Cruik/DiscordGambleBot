namespace DotA2.Gambling.Model
{
    public class Bet
    {
        public int Id { get; set; }
        public int BettingAccountId { get; set; }
        public Account BettingAccount { get; set; }
        public int GambleId { get; set; }
        public Gamble Gamble { get; set; }
        public int Amount { get; set; }
        public Prediction Prediction { get; set; }
        public string Better { get; set; }
        public bool IsEvaluated { get; set; }
    }
}