using Bot.Model;
using Discord.WebSocket;

namespace Bot.Services
{
    public interface IMessageMapper
    {
        DiscordMessage ToDiscordMessage(SocketMessage message);
        DUser ToDiscordUser(SocketUser user);
        DiscordChannel ToDiscordChannel(ISocketMessageChannel channel);
    }
}