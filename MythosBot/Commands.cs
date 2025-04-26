
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NAudio.Wave;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

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

        [Command("personagem", Aliases = ["p", "c", "char", "character", "persona"])]
        public async Task GerenciarSona(string comando, [Remainder] string nome = "")
        {
            if (nome.Length > 128) nome = nome.Substring(0, 127);
            switch (comando.ToLower())
            {
                case "n":
                case "new":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para o personagem.");
                        break;
                    }
                    var personagem = new Personagem();
                    personagem.Nome = nome;
                    personagem.Autor = Context.User.Id;
                    personagem.Id = DateTime.Now.Ticks;
                    if (!FolderDatabase.ContemPersonagemComEsteNome(Context.Guild.Id,nome))
                    {
                        FolderDatabase.AdicionarPersonagem(Context.Guild.Id, personagem);
                        await Context.Message.ReplyAsync($"Personagem `{nome}` criado com sucesso!\nPara poder editar o personagem, por favor use .edit <nome do personagem>");
                    }
                    else
                    {
                        await Context.Message.ReplyAsync($"O Personagem `{nome}` já existe!");
                    }
                    break;

                case "r":
                case "rm":
                case "remover":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para poder deletar o personagem");
                        break;
                    }
                    var personagemParaDeletar = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).First(x => x.Nome == nome);
                    if (personagemParaDeletar is not null)
                    {
                        if (personagemParaDeletar.Autor != Context.User.Id || Context.Guild.OwnerId != Context.User.Id)
                        {
                            await Context.Message.ReplyAsync($"Você precisa ser dono do personagem `{nome}` para poder deletar.");
                            return;
                        }
                        FolderDatabase.RemoverPersonagem(Context.Guild.Id, personagemParaDeletar.Nome);
                        await Context.Message.ReplyAsync($"Personagem `{nome}` foi excluido!");
                    }
                    break;

                case "listar":
                case "l":
                    await Context.Message.ReplyAsync("Listagem dos personagens:");
                    var personagens = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id);
                    if (personagens is null || personagens.Length == 0)
                    {
                        await Context.Channel.SendMessageAsync("Nenhum personagem encontrado.");
                        return;
                    }
                    var stBuilder = new StringBuilder();
                    foreach (var sona in personagens)
                    {
                        stBuilder.Append($"- \t {sona.Nome} \n");
                    }
                    await Context.Channel.SendMessageAsync(stBuilder.ToString());
                    break;

                case "i":
                case "informações":
                case "informação":
                case "sobre":
                case "s":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para o personagem.");
                        break;
                    }
                    var personagemInfo = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).FirstOrDefault(p => nome.Equals(p.Nome));
                    if (personagemInfo is null)
                    {
                        await Context.Message.ReplyAsync($"Personagem `{nome}` não encontrado.");
                        break;
                    }
                    else
                    {
                        var infoEmbed = new EmbedBuilder();
                        infoEmbed.WithColor(Color.Blue);
                        string autor = Context.Client.GetUser(personagemInfo.Autor) is null ? "???" : Context.Client.GetUser(personagemInfo.Autor).Username;
                        string personalidade = personagemInfo.Personalidade ?? "Nada aqui...";
                        string historia = personagemInfo.Historia ?? "Nada aqui...";
                        infoEmbed.WithTitle($"Informações do personagem de {autor}");
                        infoEmbed.AddField("Nome", personagemInfo.Nome, true);
                        infoEmbed.AddField("Idade", personagemInfo.Idade, true);
                        infoEmbed.AddField("Altura", personagemInfo.Altura, true);
                        infoEmbed.AddField("PE (Pontos de Esforço)",personagemInfo.PontosDeEsforço,true);
                        infoEmbed.AddField("Sanidade", personagemInfo.Sanidade, true);
                        infoEmbed.AddField("Defesa", personagemInfo.Defesa, true);
                        infoEmbed.AddField("Fome", personagemInfo.Defesa, true);
                        infoEmbed.AddField("Inventário", personagemInfo.Inventário, true);
                        infoEmbed.AddField("Medo", personagemInfo.Medo, true);
                        if (personalidade.Length < 1023) infoEmbed.AddField("Personalidade", personalidade, false);
                        else infoEmbed.AddField("Personalidade", "Será escrito separado", false);
                        if (historia.Length < 1023) infoEmbed.AddField("Historia", historia, false);
                        else infoEmbed.AddField("Historia", "Será escrito separado", false);

                            // Obtendo todas as propriedades públicas da instância
                        var pType = personagemInfo.GetType();
                        var sstBuilder = new StringBuilder();
                        var props = pType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).ToList();
                        props.RemoveRange(0, 13);
                        // Primeiro passe: Encontrar o maior tamanho do conjunto nome + valor individualmente
                        int maxEntrySize = 0;
                        foreach (var prop in props)
                        {
                            if (prop.Name == "Nome") continue;
                            string entry = $"{prop.Name}: {prop.GetValue(personagemInfo) ?? "N/A"}";
                            maxEntrySize = Math.Max(maxEntrySize, entry.Length);
                        }

                        // Adicionando uma margem para espaçamento
                        maxEntrySize += 5;

                        // Segundo passe: Aplicar o espaçamento calculado entre os grupos
                        while (props.Count >= 10) // Processa dois grupos de cinco atributos por vez
                        {
                            var propsGroup1 = props.Take(5).ToArray();
                            props.RemoveRange(0, 5);
                            var propsGroup2 = props.Take(5).ToArray();
                            props.RemoveRange(0, 5);

                            for (int i = 0; i < 5; i++)
                            {
                                string entry1 = $"{propsGroup1[i].Name}: {propsGroup1[i].GetValue(personagemInfo) ?? "N/A"}".PadRight(maxEntrySize);
                                string entry2 = $"{propsGroup2[i].Name}: {propsGroup2[i].GetValue(personagemInfo) ?? "N/A"}";

                                sstBuilder.AppendLine(entry1 + entry2);
                            }
                            sstBuilder.AppendLine(); // Adiciona uma linha extra para separar grupos
                        }

                        // Se houver atributos restantes, processa-os separadamente
                        if (props.Count > 0)
                        {
                            foreach (var prop in props)
                            {
                                string entry = $"{prop.Name}: {prop.GetValue(personagemInfo) ?? "N/A"}".PadRight(maxEntrySize);
                                sstBuilder.AppendLine(entry);
                            }
                        }

                        await Context.Message.ReplyAsync(embed: infoEmbed.Build());
                        if (personalidade.Length > 1023) await Context.Channel.SendMessageAsync("==Personalidade==\n"+ personalidade);
                        if (historia.Length > 1023) await Context.Channel.SendMessageAsync("==História==\n" + historia);
                        await Context.Channel.SendMessageAsync("Atributos (muito grande para encaixar num embed): \n" + sstBuilder.ToString());

                    }
                    break;

                case "e":
                case "exportar":
                    var personagemParaExportar = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).First(x => x.Nome == nome);

                    var exporte = FolderDatabase.ExportarPersonagem(Context.Guild.Id, nome);

                    var arq = new FileAttachment(exporte.Item2, exporte.Item1 + ".json");
                    await Context.Channel.SendFileAsync(arq);
                    arq.Stream.Close();
                    break;

                case "im":
                case "importar":
                    if (Context.Message.Attachments.Count == 0)
                    {
                        await Context.Message.ReplyAsync("Você precisa pelo menos me mandar um arquivo!");
                        return;
                    }

                    int erros = 0;
                    int acertos = 0;
                    int arquivo = 0;
                    foreach (var attatchment in Context.Message.Attachments)
                    {
                        arquivo++;
                        await Context.Channel.SendMessageAsync($"Arquivo {arquivo}:");
                        if (attatchment.ContentType.StartsWith("application/json"))
                        {
                            using var client = new HttpClient();
                            HttpResponseMessage msg = await client.GetAsync(attatchment.Url);
                            msg.EnsureSuccessStatusCode();

                            var arqBaixado = await msg.Content.ReadAsStringAsync();

                            try
                            {
                                var configuracoes = new JsonSerializerSettings
                                {
                                    NullValueHandling = NullValueHandling.Ignore,
                                    MissingMemberHandling = MissingMemberHandling.Error
                                };

                                var personagemConvertido = JsonConvert.DeserializeObject<Personagem>(arqBaixado, configuracoes);

                                if (personagemConvertido is null)
                                {
                                    erros++;
                                    await Context.Channel.SendMessageAsync("Você me mandou um arquivo vazio!");
                                    await Context.Message.AddReactionAsync(Emoji.Parse("❌"));
                                    continue;
                                }

                                if (personagemConvertido.Nome is null)
                                {
                                    erros++;
                                    await Context.Channel.SendMessageAsync("O personagem precisa de um nome!");
                                    await Context.Message.AddReactionAsync(Emoji.Parse("❌"));
                                    continue;
                                }

                                if (personagemConvertido.Nome == "")
                                {
                                    erros++;
                                    await Context.Channel.SendMessageAsync("O personagem precisa de um nome!");
                                    await Context.Message.AddReactionAsync(Emoji.Parse("❌"));
                                    continue;
                                }

                                else
                                {
                                    await Context.Channel.SendMessageAsync($"Importando `{personagemConvertido.Nome}`...");
                                    if (FolderDatabase.ContemPersonagemComEsteNome(Context.Guild.Id, personagemConvertido.Nome))
                                    {
                                        erros++;
                                        await Context.Channel.SendMessageAsync("Este personagem já existe!\n");
                                        await Context.Message.AddReactionAsync(Emoji.Parse("❌"));
                                    }
                                    else
                                    {
                                        bool donoNãoExiste = personagemConvertido.Autor == 0 || (Context.Guild.GetUser(personagemConvertido.Autor) is null);
                                        if (donoNãoExiste) //Caso o personagem não tenha autor ou o autor não está neste servidor
                                            personagemConvertido.Autor = Context.User.Id; //Passamos a ser o autor
                                        if (nome is null)
                                            personagemConvertido.Autor = Context.User.Id;
                                        personagemConvertido.Id = DateTime.Now.Ticks;
                                        FolderDatabase.AdicionarPersonagem(Context.Guild.Id, personagemConvertido);
                                        await Context.Channel.SendMessageAsync($"Personagem `{personagemConvertido.Nome}` foi importado com sucesso!{(donoNãoExiste ? "\nE você passa ser o dono deste personagem por eu não ter encontrado o dono deste personagem" : "")}");
                                        acertos++;
                                    }
                                }
                            }
                            catch (JsonSerializationException ex)
                            {
                                erros++;
                                await Context.Channel.SendMessageAsync("O arquivo JSON tem um formato que não é compatível com um personagem!");
                                await Context.Message.AddReactionAsync(Emoji.Parse("❌"));
                            }
                        }
                        else
                        {
                            erros++;
                            await Context.Channel.SendMessageAsync($"Eu preciso de um arquivo JSON!\nVocê me mandou um arquivo {MimeToString.PegarOTipoDoArquivo(attatchment.ContentType.Split(';')[0])}");
                            continue;
                        }
                    }
                    await Context.Channel.SendMessageAsync($"Pronto!" +
                        $"\nDentre {Context.Message.Attachments.Count} Arquivo(s), eu consegui importar" +
                        $" {acertos} Arquivo(s), e eu não consegui importar" +
                        $" {erros} Arquivo(s).");
                    break;
                default:
                    await Context.Message.ReplyAsync($"Sub-comando {comando} desconhecido.");
                    break;
            }
        }


        [Command("edit", Aliases = ["e", "ed"])]
        public async Task EditPersonagem([Remainder] string nome)
        {
            var sona = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id)
                .FirstOrDefault(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));

            if (sona == null)
            {
                await ReplyAsync($"Personagem `{nome}` não encontrado.");
                return;
            }

            if(sona.Autor != Context.User.Id)
            {
                if (Context.Guild.OwnerId != Context.User.Id)
                {
                    await ReplyAsync("O personagem deve ser editado pelo criador, não outra pessoa!");
                    return;
                }
            }

            if (CommandHandler.AddMessageCallback(Context.User, async (usr, ctx) => await EditLoop(usr, ctx, sona), 100))
            {
                await ReplyAsync($"Editando o personagem `{nome}`.\n" +
                    $"Envie mensagens no formato `atributo=valor`.\n" +
                    $"Digite `!` ou `!pronto` para finalizar.\n" +
                    $"Digite `?` ou `?ajuda` para saber quais atributos podem ser modificados\n" +
                    $"-# Você terá 100 mensagens para poder editar o personagem, mais que isso eu irei ignorar.");
            }
            else
            {
                await ReplyAsync("Você já está editando um personagem.");
            }
        }

        int erros = 0;
        private async Task EditLoop(SocketGuildUser usr, SocketCommandContext ctx, Personagem sona)
        {
            if (ctx.Message.Content.ToLower().Equals("!")  || ctx.Message.Content.ToLower().Equals("!pronto"))
            {
                CommandHandler.RemoveMessageCallback(usr);
                FolderDatabase.AtualizarPersonagem(ctx.Guild.Id, sona);
                await ctx.Channel.SendMessageAsync($"Edição do personagem `{sona.Nome}` finalizada com sucesso!");
                return;
            }

            if (ctx.Message.Content.ToLower().Equals("?") || ctx.Message.Content.ToLower().Equals("?ajuda"))
            {
                await ctx.Channel.SendMessageAsync(sona.AtributosDisponiveis());
                return;
            }

            var match = Regex.Match(ctx.Message.Content, @"(\w+)\s*=\s*([^|]+)");
            if (!match.Success)
            {
                erros++;
                if (erros == 2)
                {
                    await ctx.Channel.SendMessageAsync("Formato inválido. Use `atributo=valor`.");
                    erros = 0;
                }
                return;
            }

            string atributo = match.Groups[1].Value.Trim();

            ///HACKHACK: Adivinha quem decidiu fazer algo diferente?
            if (atributo.ToLower() == "pontos de esforço" || atributo.ToLower() == "pe") atributo = "PontosDeEsforço";

            string valor = match.Groups[2].Value.Trim();

            if (atributo.ToLower() == "altura")
            {
                valor = valor.Replace(".", ",");
            }

            PropertyInfo propriedade = sona.GetType().GetProperty(atributo, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (propriedade != null)
            {
                if (!Attribute.IsDefined(propriedade, typeof(EditavelAttribute)))
                {
                    await ctx.Channel.SendMessageAsync($"Atributo '{atributo}' não é editável, tente outro");
                    return;
                }

                try
                {
                    object valorConvertido = Convert.ChangeType(valor, propriedade.PropertyType);
                    propriedade.SetValue(sona, valorConvertido);
                    await ctx.Channel.SendMessageAsync($"Atributo '{atributo}' atualizado para '{valor}'.");
                    erros = 0;
                }
                catch
                {
                    await ctx.Channel.SendMessageAsync($"Falha ao definir '{atributo}'. Tipo incompatível.");
                }
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Atributo '{atributo}' não reconhecido.");
            }
        }

        [Command("IniciarServer", Aliases = ["is", "init"])]
        [RequireOwner]
        public async Task IniciarDB()
        {
            await Context.Message.ReplyAsync("Iniciando banco de dados...");
            if (!FolderDatabase.GuildaJaExiste(Context.Guild.Id))
            {
                await Context.Channel.SendMessageAsync("Banco de dados criado com sucesso!");
            }
            else
            {
                await Context.Channel.SendMessageAsync("O banco de dados já existia.");
            }
        }

        [Command("ajuda", Aliases = ["help", "a", "h"])]
        public async Task Ajuda([Remainder][Optional] string? comando)
        {
            if (comando is null)
            {
                await Context.Message.ReplyAsync("Ainda está sendo feita");
            }
            else
            {
                await Context.Message.ReplyAsync("Argumentativo você!\nAinda está sendo feita");
            }
        }
    }
}
