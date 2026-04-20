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
            string host = _config["Smtp:Host"]!;
            int port = int.Parse(_config["Smtp:Port"]!);
            string gonderen = _config["Smtp:GonderenEmail"]!;
            string sifre = _config["Smtp:GonderenSifre"]!;

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
            mail.To.Add(req.AliciEmail);
            sc.Send(mail);
            return Ok(new { mesaj = "E-posta basariyla gonderildi!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { hata = ex.Message });
        }
    }
}
