// First Party
using System.Diagnostics;

// Third Party
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MythosBot;

public class Logger
{
    public static Stopwatch stopwatch = new Stopwatch();
    public Logger(DiscordSocketClient client, CommandService service)
    {
        if (!stopwatch.IsRunning)
            stopwatch.Start();

        if (client is not null)
            client.Log += LogMessage;
        if (service is not null)
            service.Log += LogMessage;
    }

    public async Task LogMessage(LogMessage arg)
    {
        switch (arg.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.Magenta;
                break;
            default:
                break;
        }

        var ts = stopwatch.Elapsed;
        Console.Write("[{0:00}:{1:00}:{2:00}.{3:000} -", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

        if (arg.Exception is CommandException cmdExc)
        {
            Console.WriteLine($"- Comandos/Exceção] Um erro aconteceu enquanto tentava executar o comando {cmdExc.Command}.");
            
            Console.Error.WriteLine(cmdExc);
        }
        else if (arg.Exception is not null)
        {
            Console.WriteLine($"- Exceção] {arg.Exception.Message}");
            Console.WriteLine(arg.Exception);
        }
        else
        {
            Console.WriteLine($"- Genérico/{arg.Severity}] {arg.Message}");
        }
        Console.ResetColor();
        await Task.Delay(0);
    }
}
