
using System.Collections.Generic;


using Discord.WebSocket;
using Discord.Commands;
using Discord;

namespace MythosBot;

class Program
{
    public static Configuration configuration = new Configuration();
    private static DiscordSocketClient bot;
    private static CommandService commands = new CommandService();


    static bool podeSerFechado = false;
    static void Main(string[] args)
    {
        AppDomain.CurrentDomain.ProcessExit += (a, b) => {
            Shutdown();
        };

        // Inicia o bot
        if (args.Length == 0) IniciarBot();
        else
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "--ask-token")
                {
                    if (!File.Exists("config.json"))
                    {
                        Console.WriteLine("Para o bot executar corretamente, você precisa fornecer o token do bot");
                        Console.WriteLine("Você pode encontrar o token do bot na página de desenvolvedores do Discord");
                        Console.WriteLine("https://discord.com/developers/applications");
                        Console.WriteLine("Digite o token do bot:");
                        var token = Console.ReadLine();
                        if (string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine("Você não digitou um token válido");
                            Console.WriteLine("Pressione qualquer tecla para sair...");
                            Console.ReadKey();
                            Environment.Exit(-1);
                        }
                        else
                        {
                            File.WriteAllText("config.json", $"(\"token\":\"{token}\")".Replace('(', '{').Replace(')', '}'));
                            Console.WriteLine("Token salvo com sucesso");
                            Console.WriteLine("Pressione qualquer tecla para iniciar o bot...");
                            Console.ReadKey();
                            Console.Clear();
                            IniciarBot();
                        }
                    }
                    else
                    {
                        IniciarBot();
                    }
                }
            }
        }

        void IniciarBot() {
            InitBot().GetAwaiter().GetResult();
        }
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
        podeSerFechado = true;
        await bot.SetStatusAsync(UserStatus.Online);
        await bot.SetGameAsync("Criando, Editando e Excluindo personagems!");
    }

    private static void HandleShutdown(object? sender, ConsoleCancelEventArgs? e)
    {
        if (!podeSerFechado)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("O bot não pode ser fechado agora, tente novamente mais tarde");
            Console.ResetColor();
            e.Cancel = true;
            return;
        }
        Shutdown();

        Environment.Exit(0);
    }

    private static void Shutdown()
    {

        bot.SetStatusAsync(UserStatus.Offline).GetAwaiter().GetResult();
        bot.LogoutAsync().GetAwaiter().GetResult();
        bot.StopAsync().GetAwaiter().GetResult();

        configuration.SaveConfig();

        Thread.Sleep(1000);
    }
}
