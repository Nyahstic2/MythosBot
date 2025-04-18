// Third Party
using Discord;
using Discord.Commands;

using MythosBot.Languages;

namespace MythosBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            var langMan = LanguageManager.Instance;
            var Embed = new EmbedBuilder();
            Embed.WithColor(Color.Green);
            Embed.WithTitle(langMan.GetToken("bot.name"));
            Embed.WithDescription(langMan.GetToken("bot.ping.description"));
            Embed.WithFooter(langMan.GetToken("bot.ping.footer"));
            Embed.WithCurrentTimestamp();
            await Context.Message.ReplyAsync(embed: Embed.Build());
        }

        [Command("personagem", false, "", ["p", "c", "char", "character", "persona", "sona", "s"])]
        public async Task Personagem(string comando, string nome = "",[Remainder] string? extras = "")
        {
            switch (comando.ToLower())
            {
                case "l":
                case "list":
                case "lista":
                case "todos":
                case "all":
                case "listar":
                    await Context.Message.ReplyAsync("Listagem dos personagens:");
                    break;

                default:
                    await Context.Message.ReplyAsync($"sub-comando {comando} desconhecido");
                    break;
            }
        }
    }
}
