using Newtonsoft.Json;
using System.Text;

namespace MythosBot;

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
            var path = CurrentLocation.Here() + "/Database/" + id + "/" + personagem.Id + ".json";
            File.WriteAllText(path, JsonConvert.SerializeObject(personagem, Formatting.Indented));
        }
        else
        {
            Directory.CreateDirectory(CurrentLocation.Here() + "/Database/" + id);
            AdicionarPersonagem(id, personagem);
        }
    }

    internal static void RemoverPersonagem(ulong id, string nome)
    {
        var personagem = ListarPersonagensParaGuilda(id).First(p => p.Nome.Equals(nome));
        if (Directory.Exists(CurrentLocation.Here() + "/Database/" + id))
        {
            var path = CurrentLocation.Here() + "/Database/" + id + "/" + personagem.Id + ".json";
            File.Delete(path);
        }
        else
        {
            return;
        }
    }

    internal static bool ContemPersonagemComEsteNome(ulong id,string nome)
    {
        try
        {
            var a = ListarPersonagensParaGuilda(id).First(x => x.Nome == nome) != null;
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    internal static void AtualizarPersonagem(ulong id, Personagem sona)
    {
        string directoryPath = CurrentLocation.Here() + "/Database/" + id;

        if (!Directory.Exists(directoryPath))
        {
            // Se a pasta não existir, não há personagens para atualizar
            return;
        }

        var arquivosPersonagem = Directory.GetFiles(directoryPath, "*.json");

        foreach (var arquivo in arquivosPersonagem)
        {
            var personagem = JsonConvert.DeserializeObject<Personagem>(File.ReadAllText(arquivo));

            if (personagem.Nome.Equals(sona.Nome, StringComparison.OrdinalIgnoreCase))
            {
                // Atualiza os dados e sobrescreve o arquivo
                File.WriteAllText(arquivo, JsonConvert.SerializeObject(sona, Formatting.Indented));
                return;
            }
        }
    }

    internal static (string, string) ExportarPersonagem(ulong id, string nome)
    {
        var personagem = ListarPersonagensParaGuilda(id).First(x => x.Nome == nome);
        string directoryPath = CurrentLocation.Here() + "/Database/" + id;

        var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personagem, Formatting.Indented));
        var jsonSize = jsonBytes.Length;

        var val = new Random().Next(int.MinValue, int.MaxValue);
        using var file = File.Open($"{directoryPath}/temp{val}.deleteme", FileMode.OpenOrCreate);
        file.Write(jsonBytes, 0, jsonSize);
        file.Close();

        return (personagem == null) ? ("", "") : (personagem.Nome, $"{directoryPath}/temp{val}.deleteme");
    }
}