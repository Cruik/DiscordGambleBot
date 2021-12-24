using System.Collections.Generic;

namespace DotA2.Gambling.Model
{
    public interface IDbReader
    {
        DiscordUser GetUserId(string name);
        Gamble GetGamble(string name);
        List<Bet> GetBetsForGamble(string gambleName);
        List<Bet> GetBetsForGamble(int gambleId);
        Account GetAccount(string name);
        List<Account> GetLeaderBoard();
        bool DoesUnfinishedGambleExist(string userUsername);
        GambleInfo GetGambleInfo(string userName);
        Bet GetBetInfoById(int id);
    }
}