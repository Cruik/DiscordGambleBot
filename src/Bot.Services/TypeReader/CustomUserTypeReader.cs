
// Please note that the library already supports type reading
// primitive types such as bool. This example is merely used
// to demonstrate how one could write a simple TypeReader.

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Bot.Services.TypeReader
{
    public class CustomUserTypeReader : Discord.Commands.TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            IUser result;
            //<@!697872736911097887>
            //string userId = input.Replace()
            //ulong.TryParse()
            //result = context.Message.men.GetUserAsync(input);
            //return Task.FromResult(TypeReaderResult.FromSuccess(result));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, "Input could not be parsed as a IUser."));
        }
    }
}