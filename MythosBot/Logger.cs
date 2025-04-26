using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace MythosBot;

public class Logger
{
    public static Stopwatch stopwatch = new Stopwatch();
    private static StreamWriter LogFile;
    public Logger(DiscordSocketClient client, CommandService service)
    {
        if (!stopwatch.IsRunning)
            stopwatch.Start();

        if (!Directory.Exists(@".\Logs")) Directory.CreateDirectory(@".\Logs");

        LogFile ??= new StreamWriter(@$".\Logs\log{DateTime.Now.Ticks}.txt");
        LogFile.AutoFlush = true;

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
        var time = string.Format("[{0:00}:{1:00}:{2:00}.{3:000} -", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);

        if (arg.Exception is CommandException cmdExc)
        {
            WriteLog($"{time}- Comandos/Exceção] Um erro aconteceu enquanto tentava executar o comando {cmdExc.Command}.");
        }
        else if (arg.Exception is not null)
        {
            WriteLog($"{time}- Exceção] {arg.Exception.Message}");
            WriteLog(arg.Exception);
        }
        else
        {
            WriteLog($"{time}- Outros/{arg.Severity}] {arg.Message}");
        }
        Console.ResetColor();
        await Task.Delay(0);
    }

    private void WriteLog(Exception exception)
    {
        Console.WriteLine(exception);
        LogFile.WriteLine(exception);
    }

    private void WriteLog(string message)
    {
        Console.WriteLine(message);
        LogFile.WriteLine(message);
    }
}
