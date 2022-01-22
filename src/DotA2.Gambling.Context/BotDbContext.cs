using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Bot.Model;
using Dapper;
using DapperExtensions;
using DotA2.Gambling.Model;
using DiscordUser = DotA2.Gambling.Model.DiscordUser;

namespace DotA2.Gambling.Context
{
    public class BotDbContext : IDbReader, IDbWriter
    {
        private readonly string _connectionString;

        public BotDbContext(BotConfiguration botConfiguration)
        {
            _connectionString = botConfiguration.DefaultConnection;
        }


        public DiscordUser GetUserId(string name)
        {
            string sql = "SELECT * FROM DiscordUser WHERE Name = @Name ";
            var paramList = new { Name = name };

            DiscordUser user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                user = connection.Query<DiscordUser>(sql, paramList).FirstOrDefault();
            }

            return user;
        }

        public Gamble GetGamble(string name)
        {
            string sql = @"SELECT [Id]
      ,[Name]
      ,[BetTime]
      ,[Odds]
      ,[Result]
      ,[IsOpen]
      ,[MatchId]
        ,[IsGameFinished]
	   FROM [DotA2GambleBot].[dbo].[Gambles] g
	where g.Name = @Name and g.IsOpen = 1
    Order By Id DESC";
            var paramList = new { Name = name };

            Gamble gamble = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                gamble = connection.Query<Gamble>(sql, paramList).FirstOrDefault();
            }

            return gamble;
        }

        public Gamble GetGamble(int id)
        {
            string sql = @"SELECT [Id]
      ,[Name]
      ,[BetTime]
      ,[Odds]
      ,[Result]
      ,[IsOpen]
      ,[MatchId]
        ,[IsGameFinished]
	   FROM [DotA2GambleBot].[dbo].[Gambles] g
	where g.Id = @Id";
            var paramList = new { Id = id };

            Gamble gamble = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                gamble = connection.Query<Gamble>(sql, paramList).FirstOrDefault();
            }

            return gamble;
        }

        public List<Bet> GetBetsForGamble(int gambleId)
        {
            string sql = @"SELECT b.[Id]
      ,[BettingAccountId]
      ,[GambleId]
      ,[Amount]
      ,[Prediction]
,[IsEvaluated]
        ,dc.[Name] as Better
            FROM[DotA2GambleBot].[dbo].[Bets] b
                join Accounts a on a.Id = b.BettingAccountId
            join DiscordUser dc on dc.Id = a.DiscordUserId
            Join Gambles g on g.Id = b.GambleId
            where g.Id = @Id";
            var paramList = new { Id = gambleId };

            List<Bet> bets = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                bets = connection.Query<Bet>(sql, paramList).ToList();
            }

            return bets;
        }

        public List<Bet> GetBetsForGamble(string gambleName)
        {
            string sql = @"SELECT b.[Id]
      ,[BettingAccountId]
      ,[GambleId]
      ,[Amount]
      ,[Prediction]
,[IsEvaluated]
        ,dc.[Name] as Better
            FROM[DotA2GambleBot].[dbo].[Bets] b
                join Accounts a on a.Id = b.BettingAccountId
            join DiscordUser dc on dc.Id = a.DiscordUserId
            Join Gambles g on g.Id = b.GambleId
            where g.Name = @Name";
            var paramList = new { Name = gambleName };

            List<Bet> bets = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                bets = connection.Query<Bet>(sql, paramList).ToList();
            }

            return bets;
        }

        public Account GetAccount(string name)
        {
            string sql = @"SELECT * FROM Accounts a
            Join Discorduser dc on dc.Id = a.DiscordUserId
            where dc.Name = @Name";
            var paramList = new { Name = name };

            Account user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                user = connection.Query<Account>(sql, paramList).FirstOrDefault();
            }

            return user;
        }

        public Account GetAccount(int id)
        {
            string sql = @"SELECT * FROM Accounts a
            Join Discorduser dc on dc.Id = a.DiscordUserId
            where a.Id = @Id";
            var paramList = new { Id = id };

            Account user = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                user = connection.Query<Account>(sql, paramList).FirstOrDefault();
            }

            return user;
        }

        public List<Account> GetLeaderBoard()
        {
            string sql = @"SELECT  a.[Id]
      ,[DiscordUserId]
      ,[Balance],
	   dc.[Id] as dcId
      ,[Name]
      ,[Discriminator]
      ,[RichKid]
      ,[Emoji]
            FROM[DotA2GambleBot].[dbo].[Accounts] a
			join DiscordUser dc on dc.Id = a.DiscordUserId
Order By Balance DESC";

            List<Account> accounts = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                accounts = connection.Query<Account, DiscordUser, Account>(sql, (account, user) =>
                {
                    account.DiscordUser = user;
                    return account;
                }, splitOn: "dcId").ToList();
            }

            return accounts;
        }

        public bool DoesUnfinishedGambleExist(string userName)
        {
            string sql = @"Select * from Gambles where Name = @Name and IsGameFinished = 0";

            var paramList = new
            {
                Name = userName
            };

            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                id = connection.Query<int>(sql, paramList).FirstOrDefault();
            }

            bool doesExist = id > 0;

            return doesExist;
        }

        public GambleInfo GetGambleInfo(string userName)
        {
            string sql = @"DECLARE @Id int;
Select @Id = id from Gambles WHERE Name Like(@Name) and IsGameFinished = 0 order by Id DESC
SELECT TOP 1 [Id],[Name]
      ,[BetTime]
      ,[Odds]
      ,[Result]
      ,[IsOpen]
      ,[MatchId]
      ,[IsGameFinished]
      ,[IsArchived]	  from Gambles where Id = @Id;

Select b.[Id]
      ,[BettingAccountId]
      ,[GambleId]
      ,[Amount]
      ,[Prediction]
      ,[IsEvaluated],
	  dc.Name as Better
	  from Bets b
	  join Accounts a on a. Id = b.BettingAccountId
	  join DiscordUser dc on dc.Id = a.DiscordUserId where gambleId = @Id";

            var paramList = new { Name = userName };
            GambleInfo gambleInfo = new GambleInfo();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var multi = connection.QueryMultiple(sql, paramList))
                {
                    gambleInfo.Gamble = multi.Read<Gamble>().FirstOrDefault();
                    gambleInfo.Bets = multi.Read<Bet>().ToList();
                }
            }

            return gambleInfo;
        }

        public Bet GetBetInfoById(int id)
        {
            string sql = @"SELECT [Id]
      ,[BettingAccountId]
      ,[GambleId]
      ,[Amount]
      ,[Prediction]
      ,[IsEvaluated]
  FROM [DotA2GambleBot].[dbo].[Bets]
  where Id = @Id";

            var paramList = new { Id = id };

            Bet bet = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                bet = connection.Query<Bet>(sql, paramList).FirstOrDefault();
            }

            return bet;
        }

        public int InsertGamble(Gamble gamble)
        {
            string sql = @"INSERT INTO Gambles ([Name]
                ,[BetTime]
            ,[Odds]
            ,[Result]
            ,[IsOpen]
            ,[MatchId],
            [IsGameFinished])
OUTPUT INSERTED.Id VALUES(@Name,@BetTime,@Odds,@Result,@IsOpen,@MatchId, @IsGameFinished)";

            var paramList = new
            {
                Name = gamble.Name,
                gamble.BetTime,
                gamble.Odds,
                gamble.Result,
                gamble.IsOpen,
                gamble.MatchId,
                gamble.IsGameFinished
            };

            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                id = connection.Query<int>(sql, paramList).FirstOrDefault();
            }

            return id;
        }

        public int InsertBet(Bet bet)
        {
            string sql = @"INSERT INTO Bets([BettingAccountId]
                ,[GambleId]
            ,[Amount]
            ,[Prediction]) 
OUTPUT INSERTED.Id VALUES(@BettingAccountId, @GambleId,@Amount,@Prediction)";
            var paramList = new { bet.BettingAccountId, bet.GambleId, bet.Amount, bet.Prediction };
            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        id = connection.Query<int>(sql, paramList, transaction: transaction).FirstOrDefault();

                        UpdateAccountBalance(bet.BettingAccountId, bet.Amount, connection, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        id = -1;
                        transaction.Rollback();
                    }
                }

                connection.Close();
            }

            return id;
        }

        public int CloseGamble(string name)
        {
            string sql = "UPDATE [dbo].[Gambles] Set [IsOpen] = 0  OUTPUT inserted.Id WHERE Name = @Name";
            var paramList = new { Name = name };
            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                id = connection.QueryFirst<int>(sql, paramList);
            }

            return id;
        }

        public int EndGamble(string userName, Prediction result, string matchId)
        {
            string sql =
                "UPDATE [dbo].[Gambles] Set [Result] = @Result, [MatchId]=@MatchId, [IsGameFinished]=1 OUTPUT inserted.Id WHERE Name Like(@Name) and IsOpen=0 and IsArchived = 0";
            var paramList = new { Name = userName, Result = result, MatchId = matchId };
            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                id = connection.Query<int>(sql, paramList).FirstOrDefault();
            }

            if (id > 0)
            {
                CalculateGambleResult(id);
            }

            return id;
        }

        public int EndGamble(int gambleId, Prediction result, string matchId)
        {
            string sql =
                "UPDATE [dbo].[Gambles] Set [Result] = @Result, [MatchId]=@MatchId, [IsGameFinished]=1 OUTPUT inserted.Id WHERE Id = @Id and IsOpen=0 and IsArchived = 0";

            var paramList = new { Id = gambleId, Result = result, MatchId = matchId };
            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                id = connection.Query<int>(sql, paramList).FirstOrDefault();
            }

            if (id > 0)
            {
                CalculateGambleResult(id);
            }

            return id;
        }

        public int CalculateGambleResult(int gambleId)
        {
            string sqlGetGambleResult = @"Select 
g.Odds,
b.Prediction,
g.Result,
(Amount * g.Odds) as Win,
(Balance + (Amount * g.Odds)) as NewBalance ,
b.BettingAccountId
from Bets b 
join Gambles g on g.Id = b.GambleId
join Accounts a on b.BettingAccountId = a.Id
where b.GambleId = @Id";
            var sqlGetGambleResultParam = new { Id = gambleId };

            string updateBalanceSql = @"UPDATE [dbo].[Accounts]
   SET [Balance] = @NewBalance
 WHERE Id = @AccountId";

            string archiveBetSql = @"UPDATE [dbo].[Bets]
   SET [IsEvaluated] = 1
 WHERE GambleId = @Id";

            int id = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        List<GambleResult> gambleResult = connection
                            .Query<GambleResult>(sqlGetGambleResult, sqlGetGambleResultParam, transaction)
                            .ToList();
                        int affectedRows = -1;
                        foreach (var result in gambleResult)
                        {
                            if (result.Prediction == result.Result)
                            {
                                var updateBalanceSqlParam = new
                                {
                                    AccountId = result.BettingAccountId, NewBalance = result.NewBalance,
                                    Win = result.Win
                                };

                                affectedRows = connection.Execute(updateBalanceSql, updateBalanceSqlParam, transaction);
                            

                                if (affectedRows <= 0)
                                {
                                    transaction.Rollback();
                                }
                            }
                        }

                        affectedRows = connection.Execute(@"Update Gambles Set [IsArchived] = 1 where Id = @Id",
                            sqlGetGambleResultParam,
                            transaction);

                        if (affectedRows <= 0)
                        {
                            transaction.Rollback();
                        }

                        affectedRows = connection.Execute(archiveBetSql, sqlGetGambleResultParam,
                            transaction);
                        if (affectedRows <= 0)
                        {
                            transaction.Rollback();
                        }
                        else
                        {
                            transaction.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }

            return 0;
        }

        public int UpdateAccountBalance(int accountId, int placedBet, SqlConnection sqlConnection,
            SqlTransaction transaction)
        {
            string sql = @"DECLARE @OldBalance INT;
SET @OldBalance = 0;

Select @OldBalance = Balance from Accounts where Id = @Id

INSERT INTO [dbo].[BalanceLog]
           ([AccountId]
           ,[Old]
           ,[LogDate])
     VALUES
           (@Id
           ,@OldBalance
           ,getdate())

UPDATE [dbo].[Accounts]
   SET [Balance] = @OldBalance - @PlacedBet
OUTPUT INSERTED.Id
 WHERE  Id = @Id";

            var paramList = new { Id = accountId, PlacedBet = placedBet };
            int id = -1;

            id = sqlConnection.QueryFirst<int>(sql, paramList, transaction: transaction);

            return id;
        }

        public int CreateAccount(Account account)
        {
            string sql = @"INSERT INTO Accounts ([DiscordUserId] ,[Balance])
OUTPUT INSERTED.Id
VALUES(@DiscordUserId,@Balance)";

            var paramList = new { account.DiscordUserId, account.Balance };
            int userId = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                userId = connection.QueryFirst<int>(sql, paramList);
            }

            return userId;
        }

        public int AddUser(DiscordUser user)
        {
            string sql = @"INSERT INTO DiscordUser([Name]
                ,[Discriminator],[RichKid],[Emoji])
            OUTPUT INSERTED.Id
            VALUES(@Name,@Discriminator,@RichKid,@Emoji)";
            var paramList = new { Name = user.Name, user.Discriminator, user.RichKid, user.Emoji };
            int userId = -1;

            using (var connection = new SqlConnection(_connectionString))
            {
                userId = connection.Query<int>(sql, paramList).FirstOrDefault();
            }

            return userId;
        }
    }
}