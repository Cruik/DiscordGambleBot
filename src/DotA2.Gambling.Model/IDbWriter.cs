namespace DotA2.Gambling.Model
{
    public interface IDbWriter
    {
        int AddUser(DiscordUser user);
        int CreateAccount(Account account);
        int InsertGamble(Gamble gamble);
        int InsertBet(Bet bet);

        int CloseGamble(string name);
        int EndGamble(string userName, Prediction prediction, string matchId);
        int CalculateGambleResult(int gambleId);
    }
}