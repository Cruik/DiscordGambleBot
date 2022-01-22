using System;
using System.Collections.Generic;
using System.Net.Http;
using DotA2.Gambling.Model;
using FluentAssertions;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
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
            var dataService = _fixture.DataService;
            var userName = "Bravious";
            var gamble = gambler.StartGamble(userName);
            gamble.Id.Should().BeGreaterThan(0);
            var betId = gambler.Bet(userName, "Cruik", Prediction.Win, 20);
            betId.Should().BeGreaterThan(0);
            var bet = dataService.GetBetInfoById(betId);
            var betId2 = gambler.Bet(userName, "ikonoklast", Prediction.Loss, 40);
            betId2.Should().BeGreaterThan(0);

            gambler.CloseGamble(userName);
            
            var gambleInfo = dataService.GetGambleInfoByName(userName);

            gambleInfo.Bets.Count.Should().Be(2);

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
                Discriminator = "2904",
                Name = "GraViTy | Justus",
                RichKid = false
            };

            _fixture.Gambler.EndGamble(user.Name, Prediction.Win,"-");

        }
       [Fact]
        public void GetGambleInfo()
        {
            var dataService = _fixture.DataService;

            var gambleId = 2;
            
            _fixture.DataService.EndGamble("Bravious",Prediction.Loss,string.Empty);

        }

       [Fact]
        public void GetSteamlist()
        {
            string key = @"A1C861231018E0170221CEBDA449F0FA";
            // factory to be used to generate various web interfaces
            var webInterfaceFactory = new SteamWebInterfaceFactory(key);

            // this will map to the ISteamUser endpoint
            // note that you have full control over HttpClient lifecycle here
            var steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamUser>(new HttpClient());

            var friendsListResponse = steamInterface.GetFriendsListAsync((ulong)76561198123890391);
            var friendsList = friendsListResponse.GetAwaiter().GetResult().Data;
            List<UserStatus> invalidStatus = new List<UserStatus>
            {
                UserStatus.Offline,UserStatus.Unknown
            };
            foreach (var friendModel in friendsList)
            {
                var playerSummaryResponse = steamInterface.GetPlayerSummaryAsync(friendModel.SteamId).GetAwaiter().GetResult();
                var playerSummaryData = playerSummaryResponse.Data;

                if (playerSummaryData != null && !invalidStatus.Contains(playerSummaryData.UserStatus))
                {
                    var nickname = playerSummaryData.Nickname;
                    var playingGame = playerSummaryData.PlayingGameName;

                    if (playingGame == "Dota 2")
                    {

                    }
                }
            }
        }
    }
}
