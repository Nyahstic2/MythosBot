// First Party

// Third Party
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MythosBot;

public class Logger
{
    public Logger(DiscordSocketClient client, CommandService service)
    {
        if (client is not null)
            client.Log += LogMessage;
        if (service is not null)
            service.Log += LogMessage;
    }

    public async Task LogMessage(LogMessage arg)
    {
        Console.ForegroundColor = arg.Severity == LogSeverity.Info || arg.Severity == LogSeverity.Verbose ? ConsoleColor.Cyan : ConsoleColor.Yellow;
        if (arg.Exception is CommandException cmdExc)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Comandos/Exceção] Um erro aconteceu enquanto tentava executar o comando {cmdExc.Command}.");
            
            Console.Error.WriteLine(cmdExc);
        }
        else if (arg.Exception is not null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[Exceção] {arg.Exception.Message}");
            Console.WriteLine(arg.Exception);
        }
        else
        {
            Console.WriteLine($"[Genérico/{arg.Severity}] {arg.Message}");
        }
        Console.ResetColor();
        await Task.Delay(0);
    }
}
