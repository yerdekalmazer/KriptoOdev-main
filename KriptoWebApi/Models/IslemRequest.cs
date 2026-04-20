namespace KriptoWebApi.Models;

public class IslemRequest
{
    public string Metin { get; set; } = "";
    public string Yontem { get; set; } = "";
    public string Anahtar { get; set; } = "";
    public bool Sifrele { get; set; } = true;
}

public class EmailRequest
{
    public string AliciEmail { get; set; } = "";
    public string SifreliMetin { get; set; } = "";
}
