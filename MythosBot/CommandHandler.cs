// Third Party
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Our Libraries
using MythosBot.Languages;

namespace MythosBot
{
    internal class CommandHandler
    {
        private Logger logger;
        private DiscordSocketClient bot;
        private CommandService commandService;

        public CommandHandler(DiscordSocketClient bot, CommandService commandService)
        {
            logger = new Logger(null, commandService);
            this.bot = bot;
            this.commandService = commandService;
        }

        internal async Task InstallCommands()
        {
            bot.MessageReceived += HandleCommandAsync;
            commandService.Log += logger.LogMessage;
            await commandService.AddModulesAsync(System.Reflection.Assembly.GetEntryAssembly(), null);
        }

        private async Task HandleCommandAsync(SocketMessage smsg)
        {
            var langMan = LanguageManager.Instance;
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
                    Embed.WithTitle(langMan.GetToken("bot.command.failed"));
                    Embed.AddField(langMan.GetToken("bot.command.failedmotive"), result.ErrorReason);
                    Embed.WithFooter($"Comando: {context.Message.Content}");
                    Embed.WithCurrentTimestamp();
                    Embed.WithAuthor(context.User.Username, context.User.GetAvatarUrl(), context.User.GetAvatarUrl());
                    await context.Channel.SendMessageAsync(embed: Embed.Build());
                }
            }
        }
    }
}