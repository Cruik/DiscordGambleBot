using System.Linq;
using System.Threading.Tasks;
using Bot.Model;
using Discord;
using Discord.Commands;

namespace Bot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;
        private readonly BotConfiguration _config;

        public HelpModule(CommandService service, BotConfiguration config)
        {
            _service = service;
            _config = config;
        }

        [Command("helpsniffle")]
        public async Task HelpAsync()
        {
            
            string prefix = _config.Prefix;
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach(var module in _service.Modules)
            {
                string description = null;
                foreach(var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if(result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n";
                    
                }

                if(!string.IsNullOrWhiteSpace(description))
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}