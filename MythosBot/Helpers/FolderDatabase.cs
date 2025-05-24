using Discord;
using Newtonsoft.Json;
using System.Text;

namespace MythosBot;

public static class FolderDatabase
{
    public static void Recriar()
    {
        if (!Directory.Exists(CaminhoDaDatabase()))
        {
            Directory.CreateDirectory(CaminhoDaDatabase());
        }
    }

    public static bool GuildaJaExiste(ulong id, bool criarCasoNaoEncontrar = true)
    {
        if (Directory.Exists(CaminhoDaDatabase() + id))
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
    public static string CaminhoDaDatabase()
    {
        return CurrentLocation.Here() + "/Database/";
    }
    public static Personagem[] ListarPersonagensParaGuilda(ulong id)
    {
        if (Directory.Exists(CaminhoDaDatabase() + id))
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
        if (Directory.Exists(CaminhoDaDatabase() + id))
        {
            var path = CaminhoDaDatabase() + id + "/" + personagem.Id + ".json";
            File.WriteAllText(path, JsonConvert.SerializeObject(personagem, Formatting.Indented));
        }
        else
        {
            Directory.CreateDirectory(CaminhoDaDatabase() + id);
            AdicionarPersonagem(id, personagem);
        }
    }

    internal static void RemoverPersonagem(ulong id, string nome)
    {
        var personagem = ListarPersonagensParaGuilda(id).First(p => p.Nome.Equals(nome));
        if (Directory.Exists(CaminhoDaDatabase() + id))
        {
            var path = CaminhoDaDatabase() + id + "/" + personagem.Id + ".json";
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
        string directoryPath = CaminhoDaDatabase() + id;

        if (!Directory.Exists(directoryPath))
        {
            // Se a pasta n�o existir, n�o h� personagens para atualizar
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
        string directoryPath = CaminhoDaDatabase() + id;

        var jsonBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personagem, Formatting.Indented));
        var jsonSize = jsonBytes.Length;

        var val = new Random().Next(int.MinValue, int.MaxValue);
        using var file = File.Open($"{directoryPath}/temp{val}.deleteme", FileMode.OpenOrCreate);
        file.Write(jsonBytes, 0, jsonSize);
        file.Close();

        return (personagem == null) ? ("", "") : (personagem.Nome, $"{directoryPath}/temp{val}.deleteme");
    }

    internal static bool LimparArquivosTemporários(ulong id)
    {
        var lg = new Logger(null, null);
        var files = Directory.EnumerateFiles(CaminhoDaDatabase() + id).ToList();
        files = files.Where(x => x.Contains("temp") && x.EndsWith(".deleteme")).ToList();
        if (files.Count == 0)
        {
            lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", "Nenhum arquivo encontrado")).GetAwaiter().GetResult();
            return false;
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
                    lg.LogMessage(new LogMessage(LogSeverity.Error, "Cleaner", $"N�o consegui deletar {tempFile}, talvez consiga mais tarde...")).GetAwaiter().GetResult();
                }
            }
            lg.LogMessage(new LogMessage(LogSeverity.Info, "Cleaner", "Limpeza finalizada.")).GetAwaiter().GetResult();
            return true;
        }
    }
}