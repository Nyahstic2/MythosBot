
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
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
                    personagem.Autor = Context.User.Id;
                    personagem.Id = DateTime.Now.Ticks;
                    if (!FolderDatabase.ContemPersonagemComEsteNome(Context.Guild.Id,nome))
                    {
                        FolderDatabase.AdicionarPersonagem(Context.Guild.Id, personagem);
                        await Context.Message.ReplyAsync($"Personagem {nome} criado com sucesso!\nPara poder editar o personagem, por favor use .edit <nome do personagem>");
                    }
                    else
                    {
                        await Context.Message.ReplyAsync($"O Personagem {nome} já existe!");
                    }
                    break;

                case "r":
                case "remover":
                case "rm":
                case "del":
                case "deletar":
                case "excluir":
                    if (string.IsNullOrEmpty(nome))
                    {
                        await Context.Message.ReplyAsync("Você precisa fornecer um nome para poder deletar o personagem");
                        break;
                    }
                    var personagemParaDeletar = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).First(x => x.Nome == nome);
                    if (personagemParaDeletar is not null)
                    {
                        if (personagemParaDeletar.Autor != Context.User.Id)
                        {
                            await Context.Message.ReplyAsync($"Você precisa ser dono do personagem {nome} para poder deletar.");
                            return;
                        }
                        FolderDatabase.RemoverPersonagem(Context.Guild.Id, personagemParaDeletar.Nome);
                        await Context.Message.ReplyAsync($"Personagem {nome} foi excluido!");
                    }
                    break;

                case "l":
                case "list":
                case "lista":
                case "todos":
                case "all":
                case "listar":
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
                    var personagemInfo = FolderDatabase.ListarPersonagensParaGuilda(Context.Guild.Id).FirstOrDefault(p => nome.Equals(p.Nome));
                    if (personagemInfo is null)
                    {
                        await Context.Message.ReplyAsync($"Personagem {nome} não encontrado.");
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
                await ReplyAsync($"Personagem '{nome}' não encontrado.");
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
                await ReplyAsync($"Editando o personagem '{nome}'.\n" +
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
                await ctx.Channel.SendMessageAsync($"Edição do personagem '{sona.Nome}' finalizada com sucesso!");
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

        [Command("test")]
        public async Task AdicionarCallback()
        {
            if (CommandHandler.AddMessageCallback(Context.User, DoThisLoop, 32)) 
            {
                await Context.Message.ReplyAsync("Me envie mensagens, eu irei parar de responder quando eu ver uma mensagem que diz !done");
            }
            else
            {
                await Context.Message.ReplyAsync("Você já está nesse teste.");
            }
        }
        
        [Command("calc")]
        public async Task Calculadora()
        {
            if (CommandHandler.AddMessageCallback(Context.User, calc, -1)) 
            {
                await Context.Message.ReplyAsync("Me envie mensagens, eu irei parar de responder quando eu ver uma mensagem que diz !done");
            }
            else
            {
                await Context.Message.ReplyAsync("Você já está nesse teste.");
            }
        }

        public async void DoThisLoop(SocketGuildUser usr, SocketCommandContext ctx)
        {
            if (ctx.Message.Content.Contains("!done")) CommandHandler.RemoveMessageCallback(usr);
            await ctx.Channel.SendMessageAsync(ctx.Message.Content + $"\n-# falta {CommandHandler.CallbacksTimeout[usr.Id] - 1} mensagens para eu parar de te ouvir.");
        }

        
        Dictionary<ulong, List<int>> adds = new Dictionary<ulong, List<int>>();
        public async void calc(SocketGuildUser usr, SocketCommandContext ctx)
        {
            
        }
    }
}
