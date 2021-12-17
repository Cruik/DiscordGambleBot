using System.Collections.Generic;
using Discord.Rest;
using Discord.WebSocket;

namespace Bot.Model
{
    public interface ITextService
    {
        void ClearCache();
        Dictionary<string, UserMessageInfo> GetDictionaryInfo();
        void ProcessMessage(SocketUserMessage socketUserMessage);
        void ProcessMessage(RestUserMessage restUserMessage);
    }
}