using System;

namespace Bot.Model
{
    public class DiscordMessage
    {
        public DUser Author { get; set; }
        public DiscordChannel Channel { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}