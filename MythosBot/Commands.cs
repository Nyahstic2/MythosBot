// Third Party
using Discord;
using Discord.Commands;

using System.Text;

namespace MythosBot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            var Embed = new EmbedBuilder();
            Embed.WithColor(Color.Green);
            Embed.WithTitle("MythosBot");
            Embed.WithDescription("O bot está online e funcionando!");
            Embed.WithFooter("Comando executado com sucesso.");
            Embed.WithCurrentTimestamp();
            await Context.Message.ReplyAsync(embed: Embed.Build());
        }

        [Command("personagem", false, "", ["p", "c", "char", "character", "persona", "sona", "s"])]
        public async Task GerenciarSona(string comando, [Remainder] string nome = "")
        {
            if (nome.Length > 128) nome = nome.Substring(0, 127);
            switch (comando.ToLower())
            {
                case "c":
                case "criar":
                case "novo":
                case "new":
                case "n":
                case "add":
                case "adicionar":
                case "a":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para o personagem.");
                        break;
                    }
                    var personagem = new Personagem();
                    personagem.Nome = nome;
                    FolderDatabase.AdicionarPersonagem(Context.Guild.Id, personagem);

                    await Context.Message.ReplyAsync($"Personagem {nome} criado com sucesso!");
                    break;

                case "l":
                case "list":
                case "lista":
                case "todos":
                case "all":
                case "listar":
                    await Context.Message.ReplyAsync("Listagem dos personagens:");
                    var personagens = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id);
                    var stBuilder = new StringBuilder();
                    foreach (var sona in personagens)
                    {
                        stBuilder.Append($"- \t {sona.Nome} \n");
                    }
                    await Context.Channel.SendMessageAsync(stBuilder.ToString());
                    if (personagens is null || personagens.Length == 0)
                    {
                        await Context.Channel.SendMessageAsync("Nenhum personagem encontrado.");
                    }
                    break;

                case "i":
                case "info":
                case "informações":
                case "informação":
                case "sobre":
                case "s":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para o personagem.");
                        break;
                    }
                    var personagemInfo = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).FirstOrDefault(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
                    if (personagemInfo is null)
                    {
                        await Context.Message.ReplyAsync($"Personagem {nome} não encontrado.");
                        break;
                    }
                    else
                    {
                        var infoEmbed = new EmbedBuilder();
                        infoEmbed.WithColor(Color.Blue);
                        infoEmbed.WithTitle($"Informações do personagem {personagemInfo.Nome}");
                        infoEmbed.WithFields(
                            new EmbedFieldBuilder
                            {
                                Name = "Idade",
                                Value = personagemInfo.Idade.ToString(),
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "Personalidade",
                                Value = personagemInfo.Personalidade ?? "N/A",
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "Relação",
                                Value = Personagem.ValorParaRelação(personagemInfo.Relação),
                                IsInline = false
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "História Curta",
                                Value = personagemInfo.HistoriaCurta ?? "N/A",
                                IsInline = true
                            },
                            new EmbedFieldBuilder
                            {
                                Name = "História Longa",
                                Value = personagemInfo.HistoriaLonga ?? "N/A",
                                IsInline = true
                            }
                        );
                        await Context.Message.ReplyAsync(embed: infoEmbed.Build());
                    }
                    break;

                default:
                    await Context.Message.ReplyAsync($"Sub-comando {comando} desconhecido.");
                    break;
            }
        }

        [Command("IniciarServer", Aliases = ["is", "init"])]
        [RequireOwner]
        public async Task IniciarDB()
        {
            await Context.Message.ReplyAsync("Iniciando banco de dados...");
            if (!FolderDatabase.GuildaJáExiste(Context.Guild.Id))
            {
                await Context.Channel.SendMessageAsync("Banco de dados criado com sucesso!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("O banco de dados já existia.");
            }
        }
    }
}
