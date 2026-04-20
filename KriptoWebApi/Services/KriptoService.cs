namespace KriptoWebApi.Services;

public class KriptoService
{
    private const string Alfabe = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZ";

    public string Temizle(string metin)
    {
        if (string.IsNullOrEmpty(metin)) return "";
        metin = metin.Replace("i", "İ").Replace("ı", "I").ToUpper();
        string sonuc = "";
        foreach (char c in metin)
            if (Alfabe.Contains(c)) sonuc += c;
        return sonuc;
    }

    public string Kaydirmali(string m, int k, bool sifrele)
    {
        string s = "";
        foreach (char c in m)
        {
            int i = Alfabe.IndexOf(c);
            int yeni = sifrele ? (i + k) % 29 : (i - k + 29) % 29;
            s += Alfabe[yeni];
        }
        return s;
    }

    public string Dogrusal(string m, int k, bool sifrele)
    {
        int a = 3;
        int aTersi = 10;
        string s = "";
        foreach (char c in m)
        {
            int x = Alfabe.IndexOf(c);
            int yeni = sifrele ? (a * x + k) % 29 : (aTersi * (x - k + 29)) % 29;
            s += Alfabe[yeni];
        }
        return s;
    }

    public string YerDegistirme(string m)
    {
        string s = "";
        foreach (char c in m)
            s += Alfabe[28 - Alfabe.IndexOf(c)];
        return s;
    }

    public string SayiAnahtar(string m, int k, bool sifrele)
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
                    matris[i, j] = idx < m.Length ? m[idx++] : 'X';
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
                    matris[i, j] = idx < m.Length ? m[idx++] : 'X';
            string s = "";
            for (int i = 0; i < satir; i++)
                for (int j = 0; j < sutun; j++) s += matris[i, j];
            return s.Replace("X", "");
        }
    }

    public string Permutasyon(string m)
    {
        char[] arr = m.ToCharArray();
        for (int i = 0; i < arr.Length - 1; i += 2)
        {
            (arr[i], arr[i + 1]) = (arr[i + 1], arr[i]);
        }
        return new string(arr);
    }

    public string Rota(string m)
    {
        char[] arr = m.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }

    public string Zigzag(string m, bool sifrele)
    {
        if (sifrele)
        {
            string s1 = "", s2 = "";
            for (int i = 0; i < m.Length; i++)
                if (i % 2 == 0) s1 += m[i]; else s2 += m[i];
            return s1 + s2;
        }
        else
        {
            int orta = (m.Length + 1) / 2;
            string s1 = m[..orta], s2 = m[orta..];
            string sonuc = "";
            for (int i = 0; i < s1.Length; i++)
            {
                sonuc += s1[i];
                if (i < s2.Length) sonuc += s2[i];
            }
            return sonuc;
        }
    }

    public string Vigenere(string m, string anahtar, bool sifrele)
    {
        anahtar = Temizle(anahtar);
        if (string.IsNullOrEmpty(anahtar)) throw new Exception("Vigenère için metin anahtar giriniz (örn: ANAHTAR).");
        string s = "";
        for (int i = 0; i < m.Length; i++)
        {
            int mi = Alfabe.IndexOf(m[i]);
            int ki = Alfabe.IndexOf(anahtar[i % anahtar.Length]);
            int yeni = sifrele ? (mi + ki) % 29 : (mi - ki + 29) % 29;
            s += Alfabe[yeni];
        }
        return s;
    }

    private int ModTers(int a, int mod)
    {
        a = ((a % mod) + mod) % mod;
        for (int x = 1; x < mod; x++)
            if ((a * x) % mod == 1) return x;
        throw new Exception("Matrisin tersi alınamıyor (determinant sıfır veya 29 ile ortak çarpanı var).");
    }

    private char[] OlusturTabloTR(string anahtar)
    {
        const string alfabe30 = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZX";
        string s = "";
        foreach (char c in Temizle(anahtar))
            if (!s.Contains(c)) s += c;
        foreach (char c in alfabe30)
            if (!s.Contains(c)) s += c;
        return s.ToCharArray();
    }

    public string DortKare(string giris, string anahtarStr, bool sifrele)
    {
        const int SUTUN = 6;
        const string alfabe30 = "ABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZX";
        string[] ak = anahtarStr.Trim().Split(new char[] { ',', ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
        if (ak.Length < 2) throw new Exception("Dört Kare için iki anahtar girin (örn: ANAHTAR1,ANAHTAR2).");

        char[] t1 = alfabe30.ToCharArray();
        char[] t2 = OlusturTabloTR(ak[0].Trim());
        char[] t3 = OlusturTabloTR(ak[1].Trim());
        char[] t4 = alfabe30.ToCharArray();

        string m = Temizle(giris);
        if (m.Length % 2 != 0) m += 'X';
        string s = "";
        for (int i = 0; i < m.Length; i += 2)
        {
            if (!sifrele)
            {
                int i1 = Array.IndexOf(t2, m[i]);
                int i2 = Array.IndexOf(t3, m[i + 1]);
                s += t1[(i1 / SUTUN) * SUTUN + i2 % SUTUN];
                s += t4[(i2 / SUTUN) * SUTUN + i1 % SUTUN];
            }
            else
            {
                int i1 = Array.IndexOf(t1, m[i]);
                int i2 = Array.IndexOf(t4, m[i + 1]);
                s += t2[(i1 / SUTUN) * SUTUN + i2 % SUTUN];
                s += t3[(i2 / SUTUN) * SUTUN + i1 % SUTUN];
            }
        }
        return s;
    }

    public string Hill(string m, string anahtarStr, bool sifrele)
    {
        if (m.Length % 2 != 0) m += Alfabe[0];
        string[] p = anahtarStr.Trim().Split(new char[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        if (p.Length != 4) throw new Exception("Hill şifresi için 4 sayı girin (örn: 2 3 5 7).");
        int a = int.Parse(p[0]) % 29, b = int.Parse(p[1]) % 29;
        int c = int.Parse(p[2]) % 29, d = int.Parse(p[3]) % 29;
        if (!sifrele)
        {
            int det = ((a * d - b * c) % 29 + 29) % 29;
            int di = ModTers(det, 29);
            int na = di * d % 29;
            int nb = (di * ((-b % 29) + 29)) % 29;
            int nc = (di * ((-c % 29) + 29)) % 29;
            int nd = di * a % 29;
            a = na; b = nb; c = nc; d = nd;
        }
        string s = "";
        for (int i = 0; i < m.Length; i += 2)
        {
            int x = Alfabe.IndexOf(m[i]);
            int y = Alfabe.IndexOf(m[i + 1]);
            s += Alfabe[(a * x + b * y) % 29];
            s += Alfabe[(c * x + d * y) % 29];
        }
        return s;
    }

    public string IslemYap(string metin, string yontem, string anahtar, bool sifrele)
    {
        string m = Temizle(metin);

        return yontem switch
        {
            "Vigenère şifreleme" => Vigenere(m, anahtar, sifrele),
            "Hill şifreleme (2x2)" => Hill(m, anahtar, sifrele),
            "Dört Kare şifreleme" => DortKare(metin, anahtar, sifrele),
            _ => NumericKeyOperation(m, yontem, anahtar, sifrele)
        };
    }

    private string NumericKeyOperation(string m, string yontem, string anahtar, bool sifrele)
    {
        int k = int.TryParse(anahtar, out int val) ? val : 3;
        return yontem switch
        {
            "Kaydırmalı şifreleme" => Kaydirmali(m, k, sifrele),
            "Doğrusal şifreleme" => Dogrusal(m, k, sifrele),
            "Yer değiştirme şifreleme" => YerDegistirme(m),
            "Sayı anahtarlı şifreleme" => SayiAnahtar(m, k, sifrele),
            "Permütasyon şifreleme" => Permutasyon(m),
            "Rota şifreleme" => Rota(m),
            "Zigzag şifreleme" => Zigzag(m, sifrele),
            _ => throw new Exception("Geçersiz yöntem seçimi.")
        };
    }
}
