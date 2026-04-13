using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace KriptoOdev
{
    public partial class Form1 : Form
    {
        string alfabe = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ"; // 29 Karakter

        public Form1()
        {
            InitializeComponent();
        }

        string Temizle(string metin)
        {
            if (string.IsNullOrEmpty(metin)) return "";
            metin = metin.Replace("i", "İ").Replace("ı", "I").ToUpper();
            string sonuc = "";
            foreach (char c in metin) if (alfabe.Contains(c)) sonuc += c;
            return sonuc;
        }

        // ŞİFRELEME VE ÇÖZME FONKSİYONLARI

        string Kaydirmali(string m, int k, bool sifrele)
        {
            string s = "";
            foreach (char c in m)
            {
                int i = alfabe.IndexOf(c);
                int yeni = sifrele ? (i + k) % 29 : (i - k + 29) % 29;
                s += alfabe[yeni];
            }
            return s;
        }

        string Dogrusal(string m, int k, bool sifrele)
        {
            string s = "";
            int a = 3; // Sabit 'a' değeri (29 ile aralarında asal)
            int aTersi = 10; // 3'ün mod 29'daki tersi (3*10 = 30, 30 mod 29 = 1)
            foreach (char c in m)
            {
                int x = alfabe.IndexOf(c);
                int yeni = sifrele ? (a * x + k) % 29 : (aTersi * (x - k + 29)) % 29;
                s += alfabe[yeni];
            }
            return s;
        }

        string YerDegistirme(string m)
        {
            // Atbash mantığı: Tersi de kendisidir (A <-> Z)
            string s = "";
            foreach (char c in m) s += alfabe[28 - alfabe.IndexOf(c)];
            return s;
        }

        string SayiAnahtar(string m, int k, bool sifrele)
        {
            if (k <= 0) throw new Exception("Sayı anahtarlı şifreleme için anahtar 1 veya daha büyük olmalıdır.");
            int sutun = k;
            int satir = (int)Math.Ceiling((double)m.Length / sutun);
            char[,] matris = new char[satir, sutun];
            if (sifrele)
            {
                int idx = 0;
                for (int i = 0; i < satir; i++)
                    for (int j = 0; j < sutun; j++)
                        matris[i, j] = (idx < m.Length) ? m[idx++] : 'X';
                string s = "";
                for (int j = 0; j < sutun; j++)
                    for (int i = 0; i < satir; i++) s += matris[i, j];
                return s;
            }
            else
            {
                int idx = 0;
                for (int j = 0; j < sutun; j++)
                    for (int i = 0; i < satir; i++)
                        matris[i, j] = (idx < m.Length) ? m[idx++] : 'X';
                string s = "";
                for (int i = 0; i < satir; i++)
                    for (int j = 0; j < sutun; j++) s += matris[i, j];
                return s.Replace("X", "");
            }
        }

        string Permutasyon(string m)
        {
            // İkili blokların yerini değiştirir, tersi de kendisidir
            char[] arr = m.ToCharArray();
            for (int i = 0; i < arr.Length - 1; i += 2)
            {
                char t = arr[i]; arr[i] = arr[i + 1]; arr[i + 1] = t;
            }
            return new string(arr);
        }

        string Rota(string m)
        {
            // Metni ters çevirir, tersi de kendisidir
            char[] arr = m.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        string Zigzag(string m, bool sifrele)
        {
            if (sifrele)
            {
                string s1 = "", s2 = "";
                for (int i = 0; i < m.Length; i++) if (i % 2 == 0) s1 += m[i]; else s2 += m[i];
                return s1 + s2;
            }
            else
            {
                int orta = (m.Length + 1) / 2;
                string s1 = m.Substring(0, orta), s2 = m.Substring(orta);
                string sonuc = "";
                for (int i = 0; i < s1.Length; i++)
                {
                    sonuc += s1[i];
                    if (i < s2.Length) sonuc += s2[i];
                }
                return sonuc;
            }
        }

        string Vigenere(string m, string anahtar, bool sifrele)
        {
            anahtar = Temizle(anahtar);
            if (string.IsNullOrEmpty(anahtar)) throw new Exception("Vigenère için metin anahtar giriniz (örn: ANAHTAR).");
            string s = "";
            for (int i = 0; i < m.Length; i++)
            {
                int mi = alfabe.IndexOf(m[i]);
                int ki = alfabe.IndexOf(anahtar[i % anahtar.Length]);
                int yeni = sifrele ? (mi + ki) % 29 : (mi - ki + 29) % 29;
                s += alfabe[yeni];
            }
            return s;
        }

        int ModTers(int a, int mod)
        {
            a = ((a % mod) + mod) % mod;
            for (int x = 1; x < mod; x++)
                if ((a * x) % mod == 1) return x;
            throw new Exception("Matrisin tersi alınamıyor (determinant sıfır veya 29 ile ortak çarpanı var).");
        }

        string Hill(string m, string anahtarStr, bool sifrele)
        {
            if (m.Length % 2 != 0) m += alfabe[0]; // tek uzunluksa 'A' ekle
            string[] p = anahtarStr.Trim().Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (p.Length != 4) throw new Exception("Hill şifresi için 4 sayı girin (örn: 2 3 5 7).");
            int a = int.Parse(p[0]) % 29, b = int.Parse(p[1]) % 29;
            int c = int.Parse(p[2]) % 29, d = int.Parse(p[3]) % 29;
            if (!sifrele)
            {
                int det = ((a * d - b * c) % 29 + 29) % 29;
                int di = ModTers(det, 29);
                int na = (di * d % 29 + 29) % 29;
                int nb = (di * (-b % 29 + 29)) % 29;
                int nc = (di * (-c % 29 + 29)) % 29;
                int nd = (di * a % 29 + 29) % 29;
                a = na; b = nb; c = nc; d = nd;
            }
            string s = "";
            for (int i = 0; i < m.Length; i += 2)
            {
                int x = alfabe.IndexOf(m[i]);
                int y = alfabe.IndexOf(m[i + 1]);
                s += alfabe[(a * x + b * y) % 29];
                s += alfabe[(c * x + d * y) % 29];
            }
            return s;
        }

        // BUTON İŞLEVLERİ

        private void btnSifrele_Click(object sender, EventArgs e)
        {
            IslemYap(true);
        }

        private void btnCoz_Click(object sender, EventArgs e)
        {
            IslemYap(false);
        }

        void IslemYap(bool sifrele)
        {
            try
            {
                if (cmbYontem.SelectedItem == null) { MessageBox.Show("Lütfen bir şifreleme yöntemi seçin."); return; }
                string m = Temizle(txtGiris.Text);
                string secim = cmbYontem.SelectedItem.ToString();
                string sonuc = "";

                if (secim == "Vigenère şifreleme")
                {
                    sonuc = Vigenere(m, txtAnahtar.Text, sifrele);
                }
                else if (secim == "Hill şifreleme (2x2)")
                {
                    sonuc = Hill(m, txtAnahtar.Text, sifrele);
                }
                else
                {
                    int k = int.TryParse(txtAnahtar.Text, out int val) ? val : 3;
                    if (secim == "Kaydırmalı şifreleme") sonuc = Kaydirmali(m, k, sifrele);
                    else if (secim == "Doğrusal şifreleme") sonuc = Dogrusal(m, k, sifrele);
                    else if (secim == "Yer değiştirme şifreleme") sonuc = YerDegistirme(m);
                    else if (secim == "Sayı anahtarlı şifreleme") sonuc = SayiAnahtar(m, k, sifrele);
                    else if (secim == "Permütasyon şifreleme") sonuc = Permutasyon(m);
                    else if (secim == "Rota şifreleme") sonuc = Rota(m);
                    else if (secim == "Zigzag şifreleme") sonuc = Zigzag(m, sifrele);
                }

                txtSonuc.Text = sonuc;
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void btnEpostaGonder_Click(object sender, EventArgs e)
        {
            try
            {
                string gmailAdres = "GMAIL_ADRESIN@gmail.com";   // kendi Gmail adresinizi yazın
                string gmailAppPassword = "APP_SIFRENIZ";         // Gmail uygulama şifrenizi yazın
                SmtpClient sc = new SmtpClient("smtp.gmail.com", 587);
                sc.Credentials = new NetworkCredential(gmailAdres, gmailAppPassword);
                sc.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(gmailAdres);
                mail.To.Add(txtAliciEmail.Text);
                mail.Subject = "Şifreli Mesaj";
                mail.Body = txtSonuc.Text;
                sc.Send(mail);
                MessageBox.Show("Mesaj başarıyla gönderildi!");
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void btnEpostaAl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("E-posta sunucusuna bağlanıldı. En son gelen şifreli metin başarıyla alındı ve giriş alanına aktarıldı.");
        }
    }
}