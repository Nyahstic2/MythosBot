using Newtonsoft.Json;

namespace MythosBot;

public static class CurrentLocation
{
    public static string Here()
    {
        return Environment.CurrentDirectory;
    }
}

public static class FolderDatabase
{
    public static void Recriar()
    {
        if (!Directory.Exists(CurrentLocation.Here() + "/Database"))
        {
            Directory.CreateDirectory(CurrentLocation.Here() + "/Database");
        }
    }

    public static bool GuildaJaExiste(ulong id, bool criarCasoNaoEncontrar = true)
    {
        if (Directory.Exists(CurrentLocation.Here() + "/Database/" + id))
        {
            return true;
        }
        else
        {
            if (criarCasoNaoEncontrar)
            {
                Directory.CreateDirectory(CurrentLocation.Here() + "/Database/" + id);
                return true;
            }
            return false;
        }
    }

    public static Personagem[] ListarPersonagensParaGuilda(ulong id)
    {
        if (Directory.Exists(CurrentLocation.Here() + "/Database/" + id))
        {
            var personagens = Directory.GetFiles(CurrentLocation.Here() + "/Database/" + id, "*.json");
            Personagem[] lista = new Personagem[personagens.Length];
            for (int i = 0; i < personagens.Length; i++)
            {
                lista[i] = JsonConvert.DeserializeObject<Personagem>(File.ReadAllText(personagens[i]));
            }
            return lista;
        }
        else
        {
            return null;
        }
    }

    internal static void AdicionarPersonagem(ulong id, Personagem personagem)
    {
        if (Directory.Exists(CurrentLocation.Here() + "/Database/" + id))
        {
            var path = CurrentLocation.Here() + "/Database/" + id + "/" + personagem.Nome + ".json";
            File.WriteAllText(path, JsonConvert.SerializeObject(personagem, Formatting.Indented));
        }
        else
        {
            Directory.CreateDirectory(CurrentLocation.Here() + "/Database/" + id);
            AdicionarPersonagem(id, personagem);
        }
    }
}