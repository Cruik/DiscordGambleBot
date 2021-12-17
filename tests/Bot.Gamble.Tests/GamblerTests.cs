using DotA2.Gambling.Model;
using FluentAssertions;
using Xunit;
using IDataReader = System.Data.IDataReader;

namespace Bot.Gamble.Tests
{
    public class GamblerTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public GamblerTests(TestsFixture fixture)
        {
            _fixture = fixture;
        }

       [Fact]
        public void CompleteGambleExample()
        {
            var gambler = _fixture.Gambler;

            var gamble = gambler.StartGamble("Cruik");
            gamble.Should().NotBeNull();
            var betId = gambler.Bet("Cruik", "Cruik", Prediction.Win, 20);

            betId.Should().BeGreaterThan(0);
            var betId2 = gambler.Bet("Cruik", "Cruik", Prediction.Loss, 40);
            betId2.Should().BeGreaterThan(0);

            gambler.CloseGamble("Cruik");

            var test = @"test";

            test.Should().NotBeEmpty();
        }
       [Fact]
        public void AddUser()
        {
            var dataService = _fixture.DataService;

            var user = new DiscordUser()
            {
                Discriminator = "1111",
                Name = "Test",
                RichKid = false
            };

            dataService.AddUser(user);

        }
       [Fact]
        public void EndGamble()
        {
            var dataService = _fixture.DataService;

            var user = new DiscordUser()
            {
                Discriminator = "1111",
                Name = "Test",
                RichKid = false
            };

            _fixture.Context.CalculateGambleResult(3);

        }
    }
}
