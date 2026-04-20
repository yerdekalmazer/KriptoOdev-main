using Microsoft.AspNetCore.Mvc;
using KriptoWebApi.Models;
using KriptoWebApi.Services;
using System.Net;
using System.Net.Mail;

namespace KriptoWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KriptoController : ControllerBase
{
    private readonly KriptoService _krpto;
    private readonly IConfiguration _config;

    public KriptoController(KriptoService krpto, IConfiguration config)
    {
        _krpto = krpto;
        _config = config;
    }

    [HttpPost("islem")]
    public IActionResult Islem([FromBody] IslemRequest req)
    {
        try
        {
            string sonuc = _krpto.IslemYap(req.Metin, req.Yontem, req.Anahtar, req.Sifrele);
            return Ok(new { sonuc });
        }
        catch (Exception ex)
        {
            return BadRequest(new { hata = ex.Message });
        }
    }

    [HttpPost("email-gonder")]
    public IActionResult EmailGonder([FromBody] EmailRequest req)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(req.AliciEmail))
                return BadRequest(new { hata = "Alici e-posta adresi bos olamaz." });

            string host = _config["Smtp:Host"] ?? "smtp.gmail.com";
            int port = int.TryParse(_config["Smtp:Port"], out int p) ? p : 587;
            string gonderen = _config["Smtp:GonderenEmail"] ?? "";
            string sifre = _config["Smtp:GonderenSifre"] ?? "";

            if (string.IsNullOrWhiteSpace(gonderen))
                return BadRequest(new { hata = "Gonderen e-posta adresi appsettings.json'da tanimli degil." });

            var sc = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(gonderen, sifre),
                EnableSsl = true
            };
            var mail = new MailMessage
            {
                From = new MailAddress(gonderen),
                Subject = "Sifreli Mesaj",
                Body = req.SifreliMetin
            };
            mail.To.Add(req.AliciEmail.Trim());
            sc.Send(mail);
            return Ok(new { mesaj = "E-posta basariyla gonderildi!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { hata = ex.Message });
        }
    }
}
