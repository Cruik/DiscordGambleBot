using System;
using Bot.Model;
using Bot.Services;
using DotA2.Gambling.Context;
using DotA2.Gambling.Model;
using DotA2GamblingMachine;
using Microsoft.Extensions.Configuration;

namespace Bot.Gamble.Tests
{
    public class TestsFixture : IDisposable
    {
        public IDataService DataService;
        public BotDbContext Context;
        public IGambler Gambler;
        public TestsFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            var botConfigSection = config.GetSection("BotConfiguration");
            BotConfiguration botConfiguration = botConfigSection.Get<BotConfiguration>();
            var context = new BotDbContext(botConfiguration);

            IDbReader reader =context;
            IDbWriter writer =context;

            DataService = new DataService(reader,writer);
            Gambler = new Gambler(DataService);
            Context = context;
        }

        public void Dispose()
        {
        }
    }
}