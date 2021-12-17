namespace DotA2.Gambling.Model
{
    public interface IGambler
    {
        Gamble StartGamble(string userName);
        int Bet(string userName, string better, Prediction prediction, int amount);
        int CloseGamble(string userName);
        int EndGamble(string userName, Prediction result, string matchId);
    }
}