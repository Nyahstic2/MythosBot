// First Party
using System.Collections.Generic;

// Third Party
using Discord.WebSocket;
using Discord;

// Our Libraries
using MythosBot.Languages;

namespace MythosBot;

class Program
{
    public static Configuration configuration = new Configuration();
    public static DiscordSocketClient bot;
    static void Main(string[] args)
    {
        new LanguageManager(); // Força a criação do singleton
        var langMan = LanguageManager.Instance;

        // Carrega a linguagem que está no arquivo de configuração
        if (configuration.GetConfig("lang") != LanguageManager.CurrentLanguage)
        {
            langMan.LoadLanguage(configuration.GetConfig("lang"));
        }
        else
        {
            langMan.LoadLanguage(LanguageManager.DefaultLanguage);
        }
        configuration.SetConfig("lang", LanguageManager.CurrentLanguage);

        Console.CancelKeyPress += HandleShutdown;

        // Inicia o bot
        InitBot().GetAwaiter().GetResult();
    }


    static async Task InitBot()
    {
        var token = configuration.GetToken();
        bot = new DiscordSocketClient();
        var logger = new Logger(bot, null);

        bot.Ready += GoOnline;

        await bot.LoginAsync(TokenType.Bot, token);
        await bot.StartAsync();
        

        await Task.Delay(-1); // Mantém o bot ativo
    }

    private async static Task GoOnline()
    {
        var langMan = LanguageManager.Instance;
        await bot.SetStatusAsync(UserStatus.Online);
        await bot.SetGameAsync(langMan.GetToken("bot.status"));
        Console.WriteLine($"[{langMan.GetToken("log.bot")}] {langMan.GetToken("log.bot.online")}");
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
