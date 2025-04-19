// First Party
using System.Collections.Generic;

// Third Party
using Discord.WebSocket;
using Discord.Commands;
using Discord;

namespace MythosBot;

class Program
{
    public static Configuration configuration = new Configuration();
    private static DiscordSocketClient bot;
    private static CommandService commands = new CommandService();

    static void Main(string[] args)
    {
        Console.Title = "\"Por favor *NÃO* precione o botão fechar, use \"Control+C\" para parar corretamente o bot";

        // Inicia o bot
        InitBot().GetAwaiter().GetResult();
    }

    static async Task InitBot()
    {
        var token = configuration.GetToken();
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };
        bot = new DiscordSocketClient(config);
        var logger = new Logger(bot, null);
        var commandHandler = new CommandHandler(bot, commands);

        bot.Ready += GoOnline;
        Console.CancelKeyPress += HandleShutdown;

        await bot.LoginAsync(TokenType.Bot, token);
        await commandHandler.InstallCommands();
        await bot.StartAsync();

        await Task.Delay(-1); // Mantém o bot ativo
    }

    private async static Task GoOnline()
    {
        await bot.SetStatusAsync(UserStatus.Online);
        await bot.SetGameAsync("MythosBot está online!");
    }

    private static void HandleShutdown(object? sender, ConsoleCancelEventArgs e)
    {
        bot.SetStatusAsync(UserStatus.Offline).GetAwaiter().GetResult();
        bot.LogoutAsync().GetAwaiter().GetResult();
        bot.StopAsync().GetAwaiter().GetResult();

        configuration.SaveConfig();

        Thread.Sleep(1000);
        Environment.Exit(0);
    }
}
