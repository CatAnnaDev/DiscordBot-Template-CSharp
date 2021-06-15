using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotHumEncore.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("avatar")]
        public async Task GetAvatar()
        {
            ushort size = 512;
            await ReplyAsync(CDN.GetUserAvatarUrl(Context.User.Id, Context.User.AvatarId, size, ImageFormat.Auto));
        }

        [Command("react")]
        public async Task reactAsync(string pMessage, string pEmoji)
        {
            var message = await Context.Channel.SendMessageAsync(pMessage);
            var emoji = new Emoji(pEmoji);
            await message.AddReactionAsync(emoji);
        }
    }
}
