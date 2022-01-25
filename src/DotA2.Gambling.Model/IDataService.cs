using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotA2.Gambling.Model
{
    public interface IDataService
    {
        int AddUser(DiscordUser user);
        int CreateAccount(Account user);
        int CreateGamble(Gamble gamble);
        int Bet(Bet bet);
        Gamble GetGambleByName(string userName);
        GambleInfo GetGambleInfoByName(string userName);
        int EndGamble(string userName, Prediction prediction, string matchId);
        int EndGamble(int gambleId, Prediction prediction, string matchId);
        int CalculateGambleResult(int gambleId);
        int CloseGamble(string userName);
        DiscordUser GetUserByName(string userUsername);
        Account GetAccount(DiscordUser discordUser);
        Account GetAccount(string discordUserName);
        List<Account> GetLeaderBoard();
        List<Bet> GetAllBetsForGamble(int id);
        bool DoesUnfinishedGambleExist(string userUsername);
        Bet GetBetInfoById(int id);
        List<Gamble> GetAllGambles();
        void ResetEvaluation();
        void SubtractBetFromBalance(int accountId, int bet);
    }
}