using System.Text.Json.Serialization;

namespace KriptoWebApi.Models;

public class IslemRequest
{
    [JsonPropertyName("metin")]
    public string Metin { get; set; } = "";

    [JsonPropertyName("yontem")]
    public string Yontem { get; set; } = "";

    [JsonPropertyName("anahtar")]
    public string Anahtar { get; set; } = "";

    [JsonPropertyName("sifrele")]
    public bool Sifrele { get; set; } = true;
}

public class EmailRequest
{
    [JsonPropertyName("aliciEmail")]
    public string AliciEmail { get; set; } = "";

    [JsonPropertyName("sifreliMetin")]
    public string SifreliMetin { get; set; } = "";
}
