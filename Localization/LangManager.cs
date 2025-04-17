using Newtonsoft.Json;

namespace MythosBot.Languages;

public class LanguageManager{
    public static LanguageManager Instance { get; private set; }

    private Dictionary<string, string> _tokens = new Dictionary<string, string>();
    private Dictionary<string, string> _tokensFallback = new Dictionary<string, string>();

    public LanguageManager(){
        Instance ??= this;
        string json = File.ReadAllText($"{CurrentLocation.Here()}/Assets/Languages/ptbr.json");
        var toks = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        foreach (var token in toks)
        {
            _tokens.Add(token.Key.ToUpper(), token.Value);
        }
        _tokensFallback = _tokens;
    }

    public void LoadLanguage(string lang){
        _tokens.Clear();
        string json = File.ReadAllText($"{CurrentLocation.Here()}/Assets/Languages/{lang}.json").ToUpper();
        _tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }

    public string GetToken(string id){
        if (_tokens.TryGetValue(id.ToUpper(), out string token)){
            return token;
        }
        else {
            return _tokensFallback.TryGetValue(id.ToUpper(), out string fallbackToken) ? fallbackToken : $"Token {id} not found.";
        }
    }
}