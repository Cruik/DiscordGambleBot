using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bot.Model;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Bot.Services
{
    public class TextService : ITextService
    {
        private readonly IMessageMapper _mapper;
        private Dictionary<string, UserMessageInfo> _dictionary;

        public TextService(IMessageMapper mapper)
        {
            _mapper = mapper;
            _dictionary = new Dictionary<string, UserMessageInfo>();
        }

        public void ProcessMessage(SocketUserMessage socketUserMessage)
        {
            var dMessage = _mapper.ToDiscordMessage(socketUserMessage);

            var list = GetWordsFromMessage(dMessage.Message);

            var key = $"{dMessage.Author.Name}#{dMessage.Author.Discriminator}";
            
        }

        public void ProcessMessage(RestUserMessage restUserMessage)
        {
            var words = GetWordsFromMessage(restUserMessage.Content);

            var userNameKey = $"{restUserMessage.Author.Username}#{restUserMessage.Author.Discriminator}";

            if(!_dictionary.ContainsKey(userNameKey))
            {
                UserMessageInfo umi = new UserMessageInfo();

                _dictionary.Add(userNameKey, umi);

            }

            var messageInfoForUser = _dictionary[userNameKey];

            foreach(var word in words)
            {
                var newWord = word.Replace("?","");
                newWord = newWord.Replace(".","");
                newWord = newWord.Replace("@","");
                newWord = newWord.Replace(",","");
                

                var key = newWord.Trim();
                if(messageInfoForUser.Words.ContainsKey(key))
                {
                    messageInfoForUser.Words[key]++;
                }
                else
                {
                    messageInfoForUser.Words.Add(key, 1);
                }
            }
            
            messageInfoForUser.MessageCount++;
        }

        private List<string> GetWordsFromMessage(string message)
        {
            var words = new List<string>();

            var textParts = message.Split(' ').ToList();
            
            words.AddRange(textParts.Where(x => x.Length >= 2 && !x.StartsWith(":") && !x.StartsWith("!") && !int.TryParse(x, out int s)));

            return words;
        }

        public Dictionary<string, UserMessageInfo> GetDictionaryInfo()
        {
            throw new NotImplementedException();

        }

        public void ClearCache()
        {
            _dictionary = new Dictionary<string, UserMessageInfo>();
        }
    }
}