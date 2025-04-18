using Newtonsoft.Json;

namespace MythosBot;

class Configuration
{
    private Dictionary<string, string> _configPairs;

    public Configuration()
    {
        _configPairs = new Dictionary<string, string>();
        var file = File.ReadAllText($"{CurrentLocation.Here()}/config.json");
        var json = JsonConvert.DeserializeObject<Dictionary<string,string>>(file);
        _configPairs = json;
    }

    private bool tokenAlreadyRead = false;
    public string GetToken()
    {
        if (tokenAlreadyRead) return ".";
        var token = _configPairs["token"];
        tokenAlreadyRead = true;

        return token;
    }

    public string GetConfig(string key)
    {
        if (_configPairs.TryGetValue(key, out string value))
        {
            _configPairs.Remove(key);
            return value;
        }
        else
        {
            return "???";
        }
    }

    public void SetConfig(string key, string value)
    {
        if (_configPairs.ContainsKey(key))
        {
            _configPairs[key] = value;
        }
        else
        {
            _configPairs.Add(key, value);
        }
    }

    public void SaveConfig()
    {
        var json = JsonConvert.SerializeObject(_configPairs, Formatting.Indented);
        File.WriteAllText($"{CurrentLocation.Here()}/config.json", json);
    }
}