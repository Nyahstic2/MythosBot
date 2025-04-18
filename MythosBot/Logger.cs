// First Party

// Third Party
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// Our Libraries
using MythosBot.Languages;

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
        var langMan = LanguageManager.Instance;
        if (arg.Exception is CommandException cmdExc)
        {
            Console.WriteLine($"[{langMan.GetToken("log.command")}/{langMan.GetToken("log.command.exception")}]" + // Expande para -> [Comando/Exceção] ##COMMANDNAME## (será substituído pelo nome do comando) encontrou um erro enquanto executava.
                                $" {langMan.GetToken("log.command.failed").Replace("##COMMANDNAME##", $"{cmdExc.Command.Aliases.First()}")}.");
            Console.WriteLine(cmdExc);
        }
        else
        {
            Console.WriteLine($"[{langMan.GetToken("log.generic")}/{arg.Severity}] {arg.Message}");
        }
        await Task.Delay(0);
    }
}
