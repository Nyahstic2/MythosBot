namespace MythosBot;

public static class Consts
{
    public static bool IS_ALPHA = false;
    public static bool IS_BETA = true;
    public static int VERSION_MAJOR = 1;
    public static int VERSION_MINOR = 0;
    public static int VERSION_PATCH = 1;

    public static string GetVersion()
    {
        return $"MythosBot v{Consts.VERSION_MAJOR}.{Consts.VERSION_MINOR}{(Consts.VERSION_PATCH == 0 ? "" : $".{Consts.VERSION_PATCH}")}{(Consts.IS_ALPHA ? "a" : Consts.IS_BETA ? "b" : "")}";
    }
}
