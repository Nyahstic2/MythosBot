using MythosBot.Languages;

namespace MythosBot;

class Program
{
    static void Main(string[] args)
    {
        new LanguageManager();
        var langMan = LanguageManager.Instance;

        Console.WriteLine($"Hello {langMan.GetToken("bot.name")}");
    }
}
