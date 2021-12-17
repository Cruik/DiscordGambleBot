using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Model;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Bot.Modules
{
    // public class SniffModule  : ModuleBase<SocketCommandContext>
    // {
    //     private readonly CommandService _service;
    //     private readonly BotConfiguration _config;
    //     private readonly ITextService _textService;
    //     private readonly DiscordSocketClient _discord;
    //     private readonly IDataService _dataService;
    //
    //     public SniffModule(DiscordSocketClient discord, ITextService textService, IDataService dataService, CommandService service, BotConfiguration config)
    //     {
    //         _service = service;
    //         _config = config;
    //         _textService = textService;
    //         _discord = discord;
    //         _dataService = dataService;
    //     }
    //
    //     [Command("sniff")]
    //     [Summary("Check Words from Messages x days ago")]
    //     public async Task SniffMessages(int messages)
    //     {
    //
    //         await ReplyAsync($"counting!!!!!!");
    //
    //         var messagesAsync = Context.Channel.GetMessagesAsync(messages);
    //         var enumerator = messagesAsync.GetAsyncEnumerator();
    //         var errorCnt = 0;
    //         var cnt = 0;
    //         ulong lastid = 0;
    //         try
    //         {
    //             while(await enumerator.MoveNextAsync())
    //             {
    //                 var item = enumerator.Current;
    //
    //                 foreach(var m in item)
    //                 {
    //                     try
    //                     {
    //                         var msg = (RestUserMessage)m;
    //                         bool isSenderNotBot = msg.Author.Id != _discord.CurrentUser.Id;
    //
    //                         if(isSenderNotBot)
    //                         {
    //                             _textService.ProcessMessage(msg);
    //                             if (lastid == 0)
    //                             {
    //                                 lastid = msg.Id;
    //                             }
    //                         }
    //                     }
    //                     catch(Exception ex)
    //                     {
    //                         errorCnt++;
    //                     }
    //                 }
    //
    //                 cnt += item.Count;
    //             }
    //         }
    //         finally
    //         {
    //             await enumerator.DisposeAsync();
    //             await ReplyAsync($"I sniffed on {cnt} messages - {errorCnt} errors! \n Use ! snifflist for results!!!");
    //             //await ReplyAsync($" Use ! snifflist for results!!");
    //
    //             var export = new ExportDictionary()
    //             {
    //                 UserMessageInfo = _textService.GetDictionaryInfo(),
    //                 LastMessageId = lastid
    //
    //         };
    //             // serialize JSON to a string and then write string to a file
    //             File.WriteAllText(@"E:\discord.json", JsonConvert.SerializeObject(export));
    //             
    //         }
    //
    //     }
    //
    //     [Command("sniffdelete")]
    //     [Summary("Delete History")]
    //     public async Task DeleteSnifferCache()
    //     {
    //         _textService.ClearCache();
    //         await ReplyAsync("Cache Cleared");
    //     }
    //
    //     [Command("snifflist")]
    //     [Summary("List the top 5 words of every User")]
    //     public async Task ShowSniffResults()
    //     {
    //         Dictionary<string, UserMessageInfo> dic = _textService.GetDictionaryInfo();
    //
    //         foreach(var user in dic)
    //         {
    //             var umi = user.Value;
    //             await ReplyAsync($"User: '{user.Key}' - {umi.Words.Count} words in {umi.MessageCount} messages!! Top Words:");
    //             var orderByDescending = umi.Words.Where(x => x.Value >= 40).OrderByDescending(x => x.Value);
    //             var wordsCount = 1;
    //             StringBuilder sb = new StringBuilder();
    //
    //             foreach(var mostWords in orderByDescending)
    //             {
    //                 sb.AppendLine($"No{wordsCount}. '{mostWords.Key}' - '{mostWords.Value}' times!!");
    //                 if(sb.Length >= 1950)
    //                 {
    //                     await ReplyAsync($"{sb.ToString()}");
    //                     sb.Clear();
    //                 }
    //                 wordsCount++;
    //             }
    //
    //             await ReplyAsync($"{sb.ToString()}");
    //             await ReplyAsync($"------------------");
    //         }
    //     }
    //
    //
    //     [Command("snifflist")]
    //     [Summary("List the top 5 words of every User")]
    //     public async Task ShowSniffResults(string discordUser)
    //     {
    //         var dic = _textService.GetDictionaryInfo();
    //         UserMessageInfo userMessageInfo = null;
    //
    //         if(dic.ContainsKey(discordUser))
    //         {
    //             userMessageInfo = dic[discordUser];
    //         }
    //         else
    //         {
    //
    //             await ReplyAsync($"Unknown User. pls use full user with #number");
    //         }
    //
    //         await ReplyAsync($"User: '{discordUser}' - {userMessageInfo.Words.Count} words in {userMessageInfo.MessageCount} messages!! Top Words:");
    //         var orderDescending = userMessageInfo.Words.Where(x => x.Value >=40).OrderByDescending(x => x.Value);
    //         var wordsCount = 1;
    //
    //         StringBuilder sb = new StringBuilder();
    //
    //         foreach(var words in orderDescending)
    //         {
    //             sb.AppendLine($"No{wordsCount}. '{words.Key}' - '{words.Value}' times!!");
    //             if (sb.Length >= 1950)
    //             {
    //                 await ReplyAsync($"{sb.ToString()}");
    //                 sb.Clear();
    //             }
    //             wordsCount++;
    //         }
    //
    //         await ReplyAsync($"{sb.ToString()}");
    //         await ReplyAsync($"------------------");
    //     }
    //
    //     [Command("sniffdata")]
    //     [Summary("connect to data")]
    //     public async Task SniffData()
    //     {
    //
    //         var list = await _dataService.Get();
    //
    //         foreach (var name in list)
    //         {
    //             await ReplyAsync($"{name}");
    //
    //         }
    //         await ReplyAsync($"------------------");
    //     }
    //}
}