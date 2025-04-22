// Third Party
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MythosBot
{
    internal class CommandHandler
    {
        private Logger logger;
        private DiscordSocketClient bot;
        private CommandService commandService;
        static private Dictionary<ulong, Action<SocketGuildUser, SocketCommandContext>> Callbacks = new Dictionary<ulong, Action<SocketGuildUser, SocketCommandContext>>();
        static public Dictionary<ulong, int> CallbacksTimeout { get; private set; } = new Dictionary<ulong, int>();

        public CommandHandler(DiscordSocketClient bot, CommandService commandService)
        {
            logger = new Logger(null, commandService);
            this.bot = bot;
            this.commandService = commandService;
        }

        internal static bool AddMessageCallback(SocketUser user, Action<SocketGuildUser, SocketCommandContext> callback, int maxMessagesBeforeTimeout)
        {
            if (Callbacks.ContainsKey(user.Id)){
                return false;
            }
            else
            {
                Callbacks.Add(user.Id, callback);
                CallbacksTimeout.Add(user.Id, maxMessagesBeforeTimeout);
                return true;
            }
        }

        internal static bool RemoveMessageCallback(SocketGuildUser user)
        {

            if (Callbacks.ContainsKey(user.Id))
            {

                Callbacks.Remove(user.Id);
                CallbacksTimeout.Remove(user.Id);
                return true;
            }
            else
            {
                return false;
            }
        }

        internal async Task InstallCommands()
        {
            bot.MessageReceived += HandleCommandAsync;
            commandService.Log += logger.LogMessage;
            await commandService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage smsg)
        {
            var lg = new Logger(null, null);
            var message = smsg as SocketUserMessage;
            if (message is null) return;

            int argPos = 0;

            if(
               (message.HasStringPrefix(".", ref argPos) || message.HasMentionPrefix(bot.CurrentUser, ref argPos))  // Verifica se a mensagem começa com o prefixo ou menção do bot
               &&
               (!message.Author.IsBot || !message.Author.IsWebhook) // Mas essa mensagem não vem de um bot ou webhook
            )
            {
                var context = new SocketCommandContext(bot, message);
                var result = await commandService.ExecuteAsync(context, argPos, null); // Então executamos o comando
                if (!result.IsSuccess) // Falhamos em executar o comando
                {
                    var Embed = new EmbedBuilder();
                    Embed.WithColor(Color.Red);
                    Embed.WithTitle("O comando falhou :(");
                    Embed.AddField("Motivo do erro:", result.ErrorReason);
                    Embed.WithFooter($"Comando: {context.Message.Content}");
                    Embed.WithCurrentTimestamp();
                    Embed.WithAuthor(context.User.Username, context.User.GetAvatarUrl(), context.User.GetAvatarUrl());
                    await context.Channel.SendMessageAsync(embed: Embed.Build());
                }
            }
            else
            {
                if (Callbacks.ContainsKey(message.Author.Id) && (!message.Author.IsBot || !message.Author.IsWebhook)) //Ainda ignoramos webhooks
                {
                    var context = new SocketCommandContext(bot, message);
                    var usr = context.User as SocketGuildUser;
                    Callbacks[message.Author.Id].Invoke(usr, context);
                    CallbacksTimeout[message.Author.Id] = CallbacksTimeout[message.Author.Id] - 1;
                    await lg.LogMessage(new LogMessage(LogSeverity.Info, "Callback de Comando", $"Executando callback de um comando para o usuário {usr.Username}, falta {CallbacksTimeout[usr.Id]} callback(s)"));
                    if (CallbacksTimeout[message.Author.Id] == 0)
                    {
                        RemoveMessageCallback(usr);
                    }
                }
            }
        }
    }
}