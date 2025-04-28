using System.Collections.Generic;

using Discord.WebSocket;
using Discord.Commands;
using Discord;
using System.Reflection;

namespace MythosBot;

class Program
{
    public static Configuration configuration = new Configuration();
    private static DiscordSocketClient bot;
    public static CommandService commands = new CommandService();


    static bool podeSerFechado = false;
    static void Main(string[] args)
    {
        var cleaner = new Thread(new ThreadStart(CleanTempFiles));
        Console.Title = Consts.GetVersion();
        AppDomain.CurrentDomain.ProcessExit += (a, b) => {
            Shutdown();
        };

        if (ConfiguraçãoExiste())
        {
            IniciarBot();
        }
        else
        {
            PromptDoToken();
        }

        bool ConfiguraçãoExiste()
        {
            return File.Exists("config.json");
        }
       
        void PromptDoToken()
        {
            Console.Clear();
            Console.WriteLine("Para o bot executar corretamente, você precisa fornecer o token do bot");
            Console.WriteLine("Você pode encontrar o token do bot na página de desenvolvedores do Discord");
            Console.WriteLine("https://discord.com/developers/applications");
            Console.Write("Digite o token do bot> ");
            var token = Console.ReadLine();
            if (string.IsNullOrEmpty(token) || token.Length < 72)
            {
                Console.WriteLine("Você precisa inserir pelo menos algo.");
                Console.WriteLine("Tente novamente.");
                Thread.Sleep(1000);
                PromptDoToken();
            }
            else
            {
                File.WriteAllText("config.json", $"(\"token\":\"{token}\")".Replace('(', '{').Replace(')', '}'));
                Console.WriteLine("Token salvo com sucesso");
                Console.WriteLine("Pressione qualquer tecla para iniciar o bot...");
                Console.ReadKey(true);
                Console.Clear();
                IniciarBot();
            }
        }

        void IniciarBot()
        {
            cleaner.Start();
            InitBot().GetAwaiter().GetResult();
        }

    }

    public static void CleanTempFiles()
    {
        var lg = new Logger(null, null);
        while (true)
        {
            var files = new List<string>();
            lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", "Iniciando limpeza de arquivos...")).GetAwaiter().GetResult();
            if (!Directory.Exists(@".\Database"))
            {
                lg.LogMessage(new LogMessage(LogSeverity.Warning, "Cleaner", "A pasta Database não existe.")).GetAwaiter().GetResult();
                Thread.Sleep(150 * 1000); // 2 Minutos e Meio
                continue;
            }

            var folders = Directory.EnumerateDirectories(@".\Database");
            foreach (var folder in folders)
            {
                files.AddRange(Directory.EnumerateFiles(folder));
            }
            files = files.Where(x => x.Contains("temp") && x.EndsWith(".deleteme")).ToList();
            if (files.Count == 0)
            {
                lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", "Nenhum arquivo encontrado")).GetAwaiter().GetResult();
            }
            else
            {
                foreach (var tempFile in files)
                {
                    try
                    {
                        File.Delete(tempFile);
                        lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", $"Deletado {tempFile}")).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        lg.LogMessage(new LogMessage(LogSeverity.Error, "Cleaner", $"Não consegui deletar {tempFile}, talvez consiga mais tarde...")).GetAwaiter().GetResult();
                    }
                }
                lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", "Limpeza finalizada.")).GetAwaiter().GetResult();
            }
            Thread.Sleep(600 * 1000); // 10 minutos
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

        Thread.Sleep(250);
    }
}
