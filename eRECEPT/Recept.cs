using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Deployment.Internal.CodeSigning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using eRECEPT.CUERLekar;
using System.Xml.Linq;

namespace eRECEPT
{

    public enum Notifikace { EMAIL = 0, SMS = 1, BEZNOTIFIKACE = 99 }
    public enum Pohlavi { M = 0, F = 1 }
    public enum Jednotka { g = 0, ks = 1 }
    public enum StavElektronickehoReceptu { KE_SCHVALENI = 0, ZAMITNUTY = 1, PREDEPSANY = 2, PRIPRAVOVANY = 3, CASTECNE_VYDANY = 4, PLNE_VYDANY = 5, NEDOKONCENY_VYDEJ = 6, UZAVRENY = 7 }
    public enum UpozornitLekare { PRISTI_NAVSTEVA = 0, BEZODKLADNE = 1 }
    public enum Uhrada { PACIENT = 0, ZAKLADNI = 1, ZVYSENA = 2, PACIENT_ZAM = 3, ZAKLADNI_ZAM = 4, ZVYSENA_ZAM = 5 }
    public enum Prostredi { TEST = 0, PRODUKCE = 1 }
    public enum DruhLeku { REGISTROVANY = 0, NEREGISTROVANY = 1, INDIVIDUALNI = 2 }
    public enum Rodina { NE = 0, ANO = 1 }
    public class Recept
    {
        internal byte[] _pkcs12bytes;
        internal String _pkcs12password;
        internal byte[] _globalCertificate;
        internal String _globalPass;
        internal String _userPass;
        internal RSACryptoServiceProvider _key;
        internal X509Certificate2 _certificate;
        internal Guid _uuid_zpravy = Guid.NewGuid();
        internal DateTime _Odeslano = DateTime.Now;
        internal Prostredi _prostredi = Prostredi.TEST;
        internal List<PredpisLP> _predpisLP = new List<PredpisLP>();
        internal string _IdZpravy { get; set; }
        //ID lékaře od SUKLu
        public string LekarIdErp { get; set; }
        internal string verze { get; set; } = "201704A";
        public string SwKlienta { get; set; } = "DRDATANET100";

        internal DateTime _DatumVystaveni;
        internal String _Dop_Jmeno_Jmena;
        internal int _platnost = 14;


        //platnost Receptu


        //příznak receptu Akutní
        internal int? Akutni { get; set; }
        //datum vystavení
        internal DateTime DatumVystaveni;
        //Doporučující lékař - jméno/jména
        internal string Dop_Jmeno_Jmena;
        //Doporučující lékař - přijmení
        internal string Dop_Jmeno_Prijmeni;
        //doporučující lékař - identifikační číslo pracoviště PZS
        internal string Dop_Pzs_Icp;
        //doporučující lékař - identifikační číslo zařízení PZS
        internal string Dop_Pzs_Icz;

        //doporučující lékař - identifikační číslo DIČ
        internal string Dop_Pzs_Dic;

        //doporučující lékař - identifikační číslo pracoviště IČ
        internal string Dop_Pzs_Ic;
        //doporučující lékaře - název zdravotnického zařízení
        internal string Dop_Pzs_Nazev;
        //doporučující lékaře - telefon zdravotnického zařízení
        internal string Dop_Pzs_Telefon;

        //doporučující lékaře - odbornost
        internal string Dop_Odbornost_Kod;

        //ID elektronického předpisu GUID – vzniklý ve starém dočasném řešení (odpovídá poli EIDE z DR VZP přímo, nebo po případné konverzi z čárového kódu)
        internal string IdDokladGuid;

        //ID elektronického předpisu zkrácený - vzniklý v novém finálním řešení (odpovídá poli EIDE z DR VZP)
        internal string IdDokladuNew;
        //počet celkových výdajů
        internal int? Opakovani { get; set; }
        //hmotnost pacienta
        internal double? Pacient_Hmotnost { get; set; }
        //číslo pojištěnce
        internal string Pacient_CP;
        //email pacienta
        internal string Pacient_Email;
        //způsob notifikace paceinta
        internal Notifikace Pacient_Notifikace = Notifikace.EMAIL;
        internal string Pacient_Veznice;
        internal Pohlavi Pacient_Pohlavi = Pohlavi.M; //pohlací pacienta
        internal string Pacient_Telefon;
        internal int? Papirovy { get; set; } //příznak digitalizovaného předpisu (digitalizované předpisy nemají uvedené položky)
        internal DateTime PlatnostDo { get; set; } //platnost předpisu do
        //Poznámka k předpisu
        internal string Pozn;
        //předepisující lékař - email lékaře
        internal string Pred_Email;
        //předepisující lékař - identifikační číslo pracoviště v rámci PZS
        internal string Pred_icp;
        //předepisující lékař - identifikační číslo zařízení PZS
        internal string Pred_pzs;
        //předepisující lékař - název oddělení poskytovatele zdravotních služeb
        internal string Pred_oddeleni;
        //předepisující lékař - jméno/jména
        internal string Pred_lekar_jmeno_jmena;
        //předepisující lékař - příjmení
        internal string Pred_lekar_jmeno_prijmeni;
        //předepisující lékař - část obce
        internal string Pred_pzs_adresa_castobce;
        //předepisující lékař - číslo evidenční
        internal string Pred_pzs_adresa_ce;
        //předepisující lékař - číslo orientační
        internal string Pred_pzs_adresa_co;
        //předepisující lékař - číslo popisné
        internal string Pred_pzs_adresa_cp;
        //předepisující lékař - obec
        internal string Pred_pzs_adresa_obec;
        //předepisující lékař - okres
        internal string Pred_pzs_adresa_okres;
        //předepisující lékař - PSČ
        internal string Pred_pzs_adresa_psc;
        //předepisující lékař - ulice
        internal string Pred_pzs_adresa_ulice;
        /// <summary>
        /// předepisující lékař – DIČ poskytovatele zdravotních služeb
        /// </summary>
        internal string Pred_pzs_dic;
        /// <summary>
        /// předepisující lékař – IČ poskytovatele zdravotních služeb
        /// </summary>
        internal string Pred_pzs_ic;
        //předepisující lékař - kód poskytovatele zdravotních služeb
        internal string Pred_pzs_kod;
        //předepisující lékař - název poskytovatele zdravotních služeb
        internal string Pred_pzs_nazev;
        //předepisující lékař – telefon poskytovatele zdravotních služeb
        internal string Pred_pzs_telefon;
        //předepisující lékař – telefon
        internal string Pred_telefon;
        //předepisující lékař - ID odbornosti
        internal string Pred_odbornost_kod;
        internal int? Preshranicni;
        internal string Rev_lekar_kod;
        internal string Rev_lekar_jmeno_jmena;
        internal string Rev_lekar_jmeno_prijmeni;
        internal DateTime? Rev_schvaleno { get; set; }
        internal string Rev_telefon;
        //příznak Pro potřebu rodiny
        internal Rodina Rodina = Rodina.NE;
        //stav receptu
        internal StavElektronickehoReceptu Stav = StavElektronickehoReceptu.PREDEPSANY;
        //příznak akceptace upozornění lékaře od lékárníka přes CÚeR
        internal UpozornitLekare UpozornitLekare = UpozornitLekare.PRISTI_NAVSTEVA;
        internal DateTime? VypisDo { get; set; }
        internal DateTime Zalozeni { get; set; }
        internal DateTime Zmena { get; set; }
        public DateTime? Zruseni_datumcaszruseni { get; set; }
        internal string Zruseni_duvodzruseni;
        internal string Zp_id;
        internal string Adresa_castobce;
        internal string Adresa_ce;
        internal string Adresa_co;
        internal string Adresa_cp;
        internal string Adresa_obec;
        internal string Adresa_okres;
        internal string Adresa_psc;
        internal string Adresa_ulice;
        internal string _autorizacniKod;


        internal DateTime DatumNarozeni { get; set; }
        internal string Jmeno_jmena { get; set; }
        internal string Jmeno_prijmeni { get; set; }
        /// <summary>
        /// Předepsané recepty objekty
        /// </summary>
        public List<PredpisLP> PredpisLP
        {
            get { return _predpisLP; }
            set { _predpisLP = value; }



        }
        public Recept predespat(PredpisLP value)
        {
            PredpisLP.Add(value);
            return this;
        }




        public bool Send()
        {
            return true;
        }
        public void SetStav(StavElektronickehoReceptu value)
        {
            Stav = value;
        }
        public StavElektronickehoReceptu GetStav()
        {
            return Stav;
        }


        //Rodina
        public Recept rodina(String val)
        {
            return rodina(int.Parse(val));
        }
        public Recept rodina(int val)
        {
            if (val == 0) return rodina(Rodina.NE);
            else if (val == 1) return rodina(Rodina.ANO);
            else throw new ArgumentException("Rodina může být buť 0 nebo 1");
        }
        public Recept rodina(Rodina val)
        {

            Rodina = val;
            return this;
        }

        //Stav ereceptu
        public Recept stav(String val)
        {
            return stav(int.Parse(val));
        }
        public Recept stav(int val)
        {
            if (val == 0) return stav(StavElektronickehoReceptu.KE_SCHVALENI);
            else if (val == 1) return stav(StavElektronickehoReceptu.ZAMITNUTY);
            else if (val == 2) return stav(StavElektronickehoReceptu.PREDEPSANY);
            else if (val == 3) return stav(StavElektronickehoReceptu.PRIPRAVOVANY);
            else if (val == 4) return stav(StavElektronickehoReceptu.CASTECNE_VYDANY);
            else if (val == 5) return stav(StavElektronickehoReceptu.PLNE_VYDANY);
            else if (val == 6) return stav(StavElektronickehoReceptu.NEDOKONCENY_VYDEJ);
            else if (val == 7) return stav(StavElektronickehoReceptu.UZAVRENY);
            else throw new ArgumentException("Stav receptu může být 0 až 7");
        }
        public Recept stav(StavElektronickehoReceptu val)
        {
            Stav = val;
            return this;
        }


        //Upozornit lekare
        public Recept upozornitLekare(String val)
        {
            return upozornitLekare(int.Parse(val));
        }
        public Recept upozornitLekare(int val)
        {
            if (val == 0) return upozornitLekare(UpozornitLekare.PRISTI_NAVSTEVA);
            else if (val == 1) return upozornitLekare(UpozornitLekare.BEZODKLADNE);
            else throw new ArgumentException("Upozornit lékaře může nabývat hodnoty 0 a 1");
        }
        public Recept upozornitLekare(UpozornitLekare val)
        {
            UpozornitLekare = val;
            return this;
        }

        //Notifikace
        public Recept notifikace(String val)
        {
            return notifikace(int.Parse(val));
        }
        public Recept notifikace(int val)
        {
            if (val == 0) return notifikace(Notifikace.EMAIL);
            else if (val == 1) return notifikace(Notifikace.SMS);
            else if (val == 9) return notifikace(Notifikace.BEZNOTIFIKACE);
            else throw new ArgumentException("Notifikace mohou nabývat hodnota 0 a 1");
        }
        public Recept notifikace(Notifikace val)
        {
            Pacient_Notifikace = val;
            return this;
        }



        //Produkční prostředí - provoz

        public Recept prostredi(Prostredi val)
        {
            _prostredi = val;
            return this;
        }


        public Recept prostredi(int val)
        {
            if (val == 0) return prostredi(Prostredi.TEST);
            else if (val == 1) return prostredi(Prostredi.PRODUKCE);
            else throw new ArgumentException("only 0 and 1 is allowed as int value");
        }

        public Recept prostredi(String val)
        {
            return prostredi(int.Parse(val));
        }

        public Recept pohlavi(Pohlavi val)
        {
            Pacient_Pohlavi = val;
            return this;
        }

        public Recept pohlavi(int val)
        {
            if (val == 0) return pohlavi(Pohlavi.M);
            else if (val == 1) return pohlavi(Pohlavi.F);
            else throw new ArgumentException("pohlaví může být buď 0 nebo 1");
        }

        public Recept pohlavi(String val)
        {
            return pohlavi(int.Parse(val));
        }




        public Recept certificate(X509Certificate2 val)
        {
            _certificate = val;
            return this;
        }
        public Recept key(RSACryptoServiceProvider val)
        {
            _key = val;
            return this;
        }

        public Recept Pkcs12(String p12Filename)
        {
            return Pkcs12(File.ReadAllBytes(p12Filename));
        }

        public Recept Pkcs12(byte[] p12bytes)
        {
            _pkcs12bytes = p12bytes;
            return this;
        }


        public Recept Pkcs12Password(String Password)
        {
            _pkcs12password = Password;
            return this;
        }

        public Recept GlobalCertificate(String p12Filename)
        {
            return GlobalCertificate(File.ReadAllBytes(p12Filename));

        }

        public Recept GlobalCertificate(byte[] p12bytes)
        {
            _globalCertificate = p12bytes;
            return this;
        }
        public Recept GlobalPass(String Password)
        {
            _globalPass = Password;
            return this;
        }
        public Recept userPass(String val)
        {
            _userPass = val;
            return this;
        }
        public AppPingZep Ping()
        {
            return new AppPingZep(this);
        }

        public Recept uuid_zpravy(Guid val)
        {
            _uuid_zpravy = val;
            return this;
        }
        public Recept uuid_zpravy(String val)
        {
            _uuid_zpravy = new Guid(val);
            return this;
        }
        public Recept opakovani(int val)
        {
            Opakovani = val;
            return this;
        }
        public Recept autorizacniKod(String val)
        {
            _autorizacniKod = val;
            return this;
        }

        public Recept odeslano(DateTime val)
        {
            _Odeslano = val;
            return this;
        }

        public Recept jmenoPrijmeni(String val)
        {

            Jmeno_prijmeni = val;
            return this;
        }
        public Recept jmenoJmena(String val)
        {
            Jmeno_jmena = val;
            return this;
        }
        public Recept datumNarozeni(DateTime val)
        {
            DatumNarozeni = val;
            return this;
        }
        public Recept adresaUlice(String val)
        {
            Adresa_ulice = val;
            return this;
        }
        public Recept adresaCp(String val)
        {
            Adresa_cp = val;
            return this;
        }
        public Recept adresaObec(String val)
        {
            Adresa_obec = val;
            return this;
        }
        public Recept adresaPSC(String val)
        {
            Adresa_psc = val;
            return this;
        }
        public Recept pacientCP(String val)
        {
            Pacient_CP = val;
            return this;
        }
        public Recept zpId(String val)
        {
            Zp_id = val;
            return this;
        }


        public Recept pacientTelefon(String val)
        {
            Pacient_Telefon = val;
            return this;
        }
        public Recept pacientEmail(String val)
        {

            Pacient_Email = val;
            return this;
        }
        public Recept pacientVeznice(String val)
        {
            Pacient_Veznice = val;
            return this;
        }
        public Recept pacientHmotnost(int val)
        {
            Pacient_Hmotnost = val;
            return this;
        }
        public Recept lekarId(String val)
        {
            LekarIdErp = val;
            return this;
        }
        public Recept lekarIcp(String val)
        {
            Pred_icp = val;
            return this;
        }
        public Recept lekarPzs(String val)
        {
            Pred_pzs = val;
            return this;
        }
        public Recept lekarTelefon(String val)
        {
            Pred_telefon = val;
            return this;
        }
        public Recept lekarOdbornost(String val)
        {
            Pred_odbornost_kod = val;
            return this;
        }
        public Recept poznamka(String val)
        {
            Pozn = val;
            return this;
        }
        public Recept datumVystaveni(DateTime val)
        {
            _DatumVystaveni = val;
            return this;
        }

        public Recept idDokladuNew(String val)
        {
            IdDokladuNew = val;
            return this;
        }
        public Recept duvodZruseni(String val)
        {
            Zruseni_duvodzruseni = val;
            return this;
        }
     
        public AppPingZep recept()
        {
            return new AppPingZep(this);
        }
        public ZalozitRecept zalozit()
        {
            return new ZalozitRecept(this);
        }
        public ZrusitRecept zrusit()
        {
            return new ZrusitRecept(this);
        }

        public String formatDate(DateTime date, String format = "l")
        {

            string ret = date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
            if (format == "l")
            {
                ret = date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
            }
            else if (format == "s")
            {
                ret = date.ToString("yyyy-MM-dd");
            }




            return ret;
        }

        public static DateTime parseDate(String date)
        {
            return DateTime.Parse(date);
        }

        public static string byte2hex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", data[i]));
            }
            return sb.ToString();
        }

        void loadP12(byte[] p12data, string password)
        {
            X509Certificate2Collection col = new X509Certificate2Collection();
            col.Import(p12data, password, X509KeyStorageFlags.Exportable);
            foreach (X509Certificate2 cert in col)
            {
                if (cert.HasPrivateKey)
                {
                    _certificate = cert;
                    RSACryptoServiceProvider tmpKey = (RSACryptoServiceProvider)cert.PrivateKey;
                    RSAParameters keyParams = tmpKey.ExportParameters(true);
                    CspParameters p = new CspParameters();
                    p.ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider";
                    _key = new RSACryptoServiceProvider(p);
                    _key.ImportParameters(keyParams);
                }

            }

            if (_key == null || _certificate == null) throw new ArgumentException("key and/or certificate still missing after p12 processing");
        }



    }

    /// <summary>
    /// Třída zabývající se jednotlivými léky
    /// </summary>
    public class PredpisLP
    {


        internal int _Mnozstvi;
        internal string _Navod;
        internal int _Nezamenovat;
        internal int _Prekroceni;
        internal Uhrada _uhrada { get; set; }
        internal DruhLeku _druhLeku { get; set; }
        internal string _ZadankaZP;
        internal string _Diagnoza_kod;
        internal string _atc_kod;
        internal string _kod;
        internal string _nazev;
        internal string _forma;
        internal string _sila;
        internal string _cesta_podani;
        internal string _baleni;
        //Identifikátor položky přidělený CUER
        internal string _Idlp;
        //ID elektronického předpisu GUID – vzniklý ve starém dočasném řešení (odpovídá poli EIDE z DR VZP přímo, nebo po případné konverzi z čárového kódu)
        internal string _IdDokladGuid;
        //ID elektronického předpisu zkrácený - vzniklý v novém finálním řešení (odpovídá poli EIDE z DR VZP)
        internal string _IdDokladuNew;
        internal string _Pridruzenadiagnoza_kod;
        internal string _postuppripravy;
        internal int _IdLpZdroj;
        internal List<SlozkyLP> _slozkyLP = new List<SlozkyLP>();


        public int MnozstviLeku { get { return _Mnozstvi; } set { _Mnozstvi = value; } }
        public string NavodLeku { get { return _Navod; } set { if (value.Length <= 80 && value.Length >= 1) { _Navod = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public int NezamenovatLek { get { return _Nezamenovat; } set { _Nezamenovat = value; } }
        public int Prekroceni { get { return _Prekroceni; } set { _Prekroceni = value; } }
        public string ZadankaZP { get { return _ZadankaZP; } set { if (value.Length <= 8) { _ZadankaZP = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Diagnoza_kod { get { return _Diagnoza_kod; } set { if (value.Length <= 5) { _Diagnoza_kod = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Atc_kod { get { return _atc_kod; } set { if (value.Length <= 7) { _atc_kod = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Kod_leku { get { return _kod; } set { if (value.Length <= 7) { _kod = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string NazevLeku { get { return _nazev; } set { if (value.Length <= 146) { _nazev = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string FormaLeku { get { return _forma; } set { if (value.Length <= 27) { _forma = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string SilaLeku { get { return _sila; } set { if (value.Length <= 24) { _sila = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Cesta_podani { get { return _cesta_podani; } set { if (value.Length <= 15) { _cesta_podani = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Hvlpreg_baleni { get { return _baleni; } set { if (value.Length <= 22) { _baleni = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Idlp { get { return _Idlp; } set { if (value.Length <= 36) { Idlp = value; } else { throw new ArgumentOutOfRangeException(); } } }
        //ID elektronického předpisu GUID – vzniklý ve starém dočasném řešení (odpovídá poli EIDE z DR VZP přímo, nebo po případné konverzi z čárového kódu)
        public string IdDokladGuid { get { return _IdDokladGuid; } set { if (value.Length < 37) { _IdDokladGuid = value; } else { throw new ArgumentOutOfRangeException(); } } }
        //ID elektronického předpisu zkrácený - vzniklý v novém finálním řešení (odpovídá poli EIDE z DR VZP)
        public string IdDokladuNew { get { return _IdDokladuNew; } set { if (value.Length < 13) { _IdDokladuNew = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Pridruzenadiagnoza_kod { get { return _Pridruzenadiagnoza_kod; } set { if (value.Length <= 5) { _Pridruzenadiagnoza_kod = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Postuppripravy { get { return _postuppripravy; } set { if (value.Length <= 4000) { _postuppripravy = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string UhradaLeku { get { return uhradaText(_uhrada); } }


        public int idPolozkyLeku { get { return _IdLpZdroj; } set { _IdLpZdroj = value; } }

        public List<SlozkyLP> SlozkyLP
        {
            get { return _slozkyLP; }
            set { _slozkyLP = value; }



        }

        public void Slozka(SlozkyLP val)
        {
            SlozkyLP.Add(val);
        }


        public static PredpisLP PridatLP()
        {
            return new PredpisLP();
        }

        public PredpisLP uhrada(Uhrada val)
        {
            _uhrada = val;
            return this;
        }
        public String uhradaText(Uhrada val)
        {
            if (val == Uhrada.PACIENT) return "Pacient";
            else if (val == Uhrada.ZVYSENA) return "Zvýšená";
            else if (val == Uhrada.ZAKLADNI) return "Základní";
            else if (val == Uhrada.PACIENT_ZAM) return "Zaměstnavatel pacienta";
            else if (val == Uhrada.ZAKLADNI_ZAM) return "Zaměstnavatel základní";
            else if (val == Uhrada.ZVYSENA_ZAM) return "Zaměstnavatel zvýšená";
            else throw new ArgumentException("Může nabývat hodnota jen podle úhrady");
        }

        public PredpisLP uhrada(int val)
        {
            if (val == 0) return uhrada(Uhrada.PACIENT);
            else if (val == 1) return uhrada(Uhrada.ZAKLADNI);
            else if (val == 2) return uhrada(Uhrada.ZVYSENA);
            else if (val == 3) return uhrada(Uhrada.PACIENT_ZAM);
            else if (val == 4) return uhrada(Uhrada.ZAKLADNI_ZAM);
            else if (val == 5) return uhrada(Uhrada.ZVYSENA_ZAM);
            else throw new ArgumentException("only 0 to 5 is allowed as int value");
        }
        public PredpisLP uhrada(String val)
        {
            return uhrada(int.Parse(val));
        }


        public PredpisLP druhLeku(DruhLeku val)
        {
            _druhLeku = val;
            return this;
        }
        public PredpisLP druhLeku(int val)
        {
            if (val == 0) return druhLeku(DruhLeku.REGISTROVANY);
            else if (val == 1) return druhLeku(DruhLeku.NEREGISTROVANY);
            else if (val == 2) return druhLeku(DruhLeku.INDIVIDUALNI);
            else throw new ArgumentException("Může nabývat hodnota 0 až 2");

        }
        public PredpisLP druhLeku(String val)
        {
            return druhLeku(int.Parse(val));
        }


        public PredpisLP navod(String val)
        {
            NavodLeku = val;
            return this;
        }
        public PredpisLP mnozstvi(int val)
        {
            MnozstviLeku = val;
            return this;

        }
        public PredpisLP kod(string val)
        {
            _kod = val;
            return this;
        }
        public PredpisLP atc(string val)
        {
            _atc_kod = val;
            return this;
        }
        public PredpisLP nazev(string val)
        {
            _nazev = val;
            return this;
        }
        public PredpisLP sila(string val)
        {
            _sila = val;
            return this;
        }
        public PredpisLP forma(string val)
        {
            _forma = val;
            return this;
        }
        public PredpisLP cestapodani(string val)
        {
            _cesta_podani = val;
            return this;
        }

        public PredpisLP baleni(string val)
        {
            Hvlpreg_baleni = val;
            return this;
        }

        public PredpisLP diagnoza(string v)
        {
            Diagnoza_kod = v;
            return this;
        }
        public PredpisLP idpolozky(int v)
        {
            idPolozkyLeku = v;
            return this;
        }

        public PredpisLP nezamenovat(int v)
        {
            NezamenovatLek = v;
            return this;
        }

        public PredpisLP postupPripravyLeku(string v)
        {
            Postuppripravy = v;
            return this;
        }

        public PredpisLP()
        {
        }
    }


    public class SlozkyLP
    {
        internal double _mnozstvi;
        internal Jednotka _jednotka;
        internal string _nazev;
        internal string _surovina;

        public double Mnozstvi { get { return _mnozstvi; } set { if (value > 0) { _mnozstvi = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Nazev { get { return _nazev; } set { if (value.Length <= 100 && value.Length >= 1) { _nazev = value; } else { throw new ArgumentOutOfRangeException(); } } }
        public string Surovina { get { return _surovina; } set { if (value.Length <= 7) { _surovina = value; } else { throw new ArgumentOutOfRangeException(); } } }

        public SlozkyLP jednotka(Jednotka val)
        {
            _jednotka = val;
            return this;
        }


        public SlozkyLP jednotka(int val)
        {
            if (val == 0) return jednotka(Jednotka.g);
            else if (val == 1) return jednotka(Jednotka.ks);
            else throw new ArgumentException("only 0 to 1 is allowed as int value");
        }
        public SlozkyLP jednotka(String val)
        {
            return jednotka(int.Parse(val));
        }

        public SlozkyLP mnozstviSlozky(double val)
        {
            Mnozstvi = val;
            return this;

        }

        public SlozkyLP nazevSlozky(string val)
        {
            Nazev = val;
            return this;

        }

        public SlozkyLP surovinaSlozky(string val)
        {
            Surovina = val;
            return this;

        }



    }
    public class ZrusitRecept
    {
        X509Certificate2 _certificate; public X509Certificate2 certificate { get { return _certificate; } }
        byte[] _globalCertificate; public Byte[] globalCertificate { get { return _globalCertificate; } }
        Guid _IdZpravy; public Guid IdZpravy { get { return _IdZpravy; } }
        String _SwKlienta; public String SwKlienta { get { return _SwKlienta; } }
        DateTime _Odeslano; public DateTime Odeslano { get { return _Odeslano; } }
        String _Verze; public String Verze { get { return _Verze; } }
        String _globalPass; public String GlobalPass { get { return _globalPass; } }

        String _userPass; public String UserPass { get { return _userPass; } }
        RSACryptoServiceProvider _key; RSACryptoServiceProvider key { get { return _key; } }
        Prostredi? _prostredi; public Prostredi? prostredi { get { return _prostredi; } }

        DateTime _datumVystaveni; public DateTime datumVystaveni { get { return _datumVystaveni; } }

        String _IdDokladuNew; public String idDokladuNew { get { return _IdDokladuNew; } }

        //jedná se o položku ID zprávy
        String _autorizacniKod; public String autorizacniKod { get { return _autorizacniKod; } }

        string _duvodZruseni; public String duvodZruseni { get { return _duvodZruseni; } }

        string _uzivatel; public string uzivatel { get { return _uzivatel; } }
        string _pracoviste; public string pracoviste { get { return _pracoviste; } }

        string _lekarId; public string lekarId { get { return _lekarId; } }
        internal ZrusitRecept(Recept build)
        {

            _certificate = build._certificate;
            _IdZpravy = build._uuid_zpravy;
            _SwKlienta = build.SwKlienta;
            _key = build._key;
            _Verze = build.verze;
            _Odeslano = build._Odeslano;
            _globalCertificate = build._globalCertificate;
            _globalPass = build._globalPass;
            _userPass = build._userPass;
            _prostredi = build._prostredi;
            _datumVystaveni = build._DatumVystaveni;
            _IdDokladuNew = build.IdDokladuNew;
            _autorizacniKod = build._autorizacniKod;
            _duvodZruseni = build.Zruseni_duvodzruseni;
            _uzivatel = build.LekarIdErp;
            _pracoviste = build.Pred_pzs;
            _lekarId = build.LekarIdErp;

            if (build._pkcs12bytes != null)
            {
                if (build._pkcs12password == null)
                {
                    throw new ArgumentException("Nalezena PKCS12 data, bohužel nenalezeno heslo prosím doplňte heslo.");
                }

                loadP12(build._pkcs12bytes, build._pkcs12password);
            }

        }

        public static Recept Recept()
        {
            return new Recept();
        }

        void loadP12(byte[] p12data, string password)
        {
            X509Certificate2Collection col = new X509Certificate2Collection();
            col.Import(p12data, password, X509KeyStorageFlags.Exportable);
            foreach (X509Certificate2 cert in col)
            {
                if (cert.HasPrivateKey)
                {
                    _certificate = cert;
                    RSACryptoServiceProvider tmpKey = (RSACryptoServiceProvider)cert.PrivateKey;
                    RSAParameters keyParams = tmpKey.ExportParameters(true);
                    CspParameters p = new CspParameters();
                    p.ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider";
                    _key = new RSACryptoServiceProvider(p);
                    _key.ImportParameters(keyParams);
                }

            }

            if (_key == null || _certificate == null) throw new ArgumentException("key and/or certificate still missing after p12 processing");
        }

        public String GenerateSoapRequest()
        {
            //Začít skládat XML
            try
            {
                //Nastavení SHA256
                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                //Příprava prázdnéh dokumentu
                XmlDocument doc = new XmlDocument();
                //Úprava pro přeskakování býlých míst
                doc.PreserveWhitespace = false;
                //Načtení dokumentu - úprava s binárního uložení

                //Načtení šablony
                String template = UTF8Encoding.UTF8.GetString(templates.ZruseniPredpisu);

                //Vyplnění šablony
                template = replacePlaceholders(template, null, null);



                //Očištění od přebytečných tagů
                template = removeUnusedPlaceholders(template);
                //smazt prázdné tagy
                var xd = XDocument.Parse(template);
                xd.Descendants().Attributes().Where(a => string.IsNullOrWhiteSpace(a.Value)).Remove();
                xd.Descendants()
                  .Where(e => (e.Attributes().All(a => a.IsNamespaceDeclaration))
                            && string.IsNullOrWhiteSpace(e.Value))
                  .Remove();
                //Hotové XML pro podpis
                doc.LoadXml(xd.ToString());


                return doc.InnerXml;

            }
            catch (Exception e)
            {
                throw new ArgumentException("Error while generating soap request", e);
            }

        }

        protected String nastavProstedi(Prostredi val)
        {
            if (val == Prostredi.TEST)
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
            else
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
        }

        private String replacePlaceholders(String src, String digest, string signature)
        {
            Recept f = new eRECEPT.Recept();
            try
            {
                if (certificate != null) src = src.Replace("${certb64}", Convert.ToBase64String(certificate.GetRawCertData()));
                if (IdZpravy != null) src = src.Replace("${IdZpravy}", IdZpravy.ToString());
                if (Odeslano != null) src = src.Replace("${Odeslano}", f.formatDate(Odeslano));
                if (Verze != null) src = src.Replace("${Verze}", Verze);
                if (digest != null) src = src.Replace("${digest}", digest);
                if (signature != null) src = src.Replace("${signature}", signature);
                if (certificate != null) src = src.Replace("${subjekt}", certificate.Subject.ToString());
                if (SwKlienta != null) src = src.Replace("${Swklienta}", SwKlienta);
                if (datumVystaveni != null) src = src.Replace("${datumVystaveni}", f.formatDate(datumVystaveni, "s"));
                if (uzivatel != null) src = src.Replace("${uzivatel}", uzivatel);
                if (pracoviste != null) src = src.Replace("${pracoviste}", pracoviste);
                if (idDokladuNew != null) src = src.Replace("${idDokladu}", idDokladuNew);
                if (autorizacniKod != null) src = src.Replace("${autorizacniid}", autorizacniKod);
                if (datumVystaveni != null) src = src.Replace("${datumZruseni}", f.formatDate(datumVystaveni, "s"));
                if (duvodZruseni != null) src = src.Replace("${duvodZruseni}", duvodZruseni);


                return src;
            }
            catch (Exception e)
            {
                throw new ArgumentException("replacement processing got wrong", e);
            }
        }

        private String removeUnusedPlaceholders(String src)
        {
            src = Regex.Replace(src, " [a-z_0-9]+=\"\\$\\{[0-9_a-z]+\\}\"", "");
            src = Regex.Replace(src, "\\$\\{[a-b_0-9]+\\}", "");
            src = Regex.Replace(src, "\\${[a-zA-Z0-9]*}", "");
            return src;
        }


        public String sendRequest(String requestBody)
        {
            String serviceUrl = nastavProstedi(_prostredi.GetValueOrDefault());

            X509Certificate2Collection certificates = new X509Certificate2Collection();
            try
            {
                certificates.Import(_globalCertificate, _globalPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch (Exception ex)
            {

            }


            NetworkCredential cred = new NetworkCredential(lekarId.ToString(), UserPass.ToString());
            String encoded = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(lekarId.ToString() + ":" + UserPass.ToString()));
            //enable minimal versions of TLS required by eRecept
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

            byte[] content = UTF8Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceUrl);
            


            req.ContentType = "text/xml;charset=UTF-8";
            req.ContentLength = content.Length;
            req.PreAuthenticate = true;
            //req.Credentials = cred;
            //req.Headers.Add("SOAPAction", serviceUrl);
            req.Method = "POST";
            req.Headers.Add("Authorization", "BASIC " + encoded);


            req.ClientCertificates = certificates;

            try
            {
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(content, 0, content.Length);
                reqStream.Close();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            String responseString = "";
            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    StreamReader rdr = new StreamReader(respStream, Encoding.UTF8);
                    responseString = rdr.ReadToEnd();
                }

            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return text;

                    }
                }
            }

            return responseString;






        }
    }



    public class ZalozitRecept
    {
        X509Certificate2 _certificate; public X509Certificate2 certificate { get { return _certificate; } }
        byte[] _globalCertificate; public Byte[] globalCertificate { get { return _globalCertificate; } }
        Guid _IdZpravy; public Guid IdZpravy { get { return _IdZpravy; } }
        String _SwKlienta; public String SwKlienta { get { return _SwKlienta; } }
        DateTime _Odeslano; public DateTime Odeslano { get { return _Odeslano; } }
        String _Verze; public String Verze { get { return _Verze; } }
        String _globalPass; public String GlobalPass { get { return _globalPass; } }
        String _userPass; public String UserPass { get { return _userPass; } }
        RSACryptoServiceProvider _key; RSACryptoServiceProvider key { get { return _key; } }
        Prostredi? _prostredi; public Prostredi? prostredi { get { return _prostredi; } }

        DateTime _datumVystaveni; public DateTime datumVystaveni { get { return _datumVystaveni; } }
        int _platnost; public int platnost { get { return _platnost; } }
        Rodina _rodina; public Rodina rodina { get { return _rodina; } }
        string _pacPrijmeni; public string pacPrijmeni { get { return _pacPrijmeni; } }
        String _pacJmeno; public string pacJmeno { get { return _pacJmeno; } }
        DateTime _datumNarozeni; public DateTime datumNarozeni { get { return _datumNarozeni; } }
        String _ulice; public string ulice { get { return _ulice; } }
        string _cisloPopisne; public string cisloPopisne { get { return _cisloPopisne; } }
        string _nazevObce; public string nazevObce { get { return _nazevObce; } }
        string _psc; public string psc { get { return _psc; } }
        string _cisloPojistence; public string cisloPojistence { get { return _cisloPojistence; } }
        string _kodPojistovny; public string kodPojistovny { get { return _kodPojistovny; } }
        string _telefonPacient; public string telefonPacient { get { return _telefonPacient; } }
        string _emailPacient; public string emailPacient { get { return _emailPacient; } }
        Notifikace _notifikace; public Notifikace notifikace { get { return _notifikace; } }
        string _vezeni; public string vezeni { get { return _vezeni; } }
        double? _hmotnost; public double? hmotnost { get { return _hmotnost; } }
        Pohlavi _pohlavi; public Pohlavi pohlavi { get { return _pohlavi; } }
        String _idLekare; public String idLekare { get { return _idLekare; } }
        String _icp; public String icp { get { return _icp; } }
        String _pzs; public string pzs { get { return _pzs; } }
        String _telefon; public string telefon { get { return _telefon; } }
        String _odb; public string odb { get { return _odb; } }
        List<PredpisLP> _predpisLP; public List<PredpisLP> predpisLP { get { return _predpisLP; } }
        String _poznamka; public string poznamka { get { return _poznamka; } }

        int? _opakovani; public int? opakovani { get { return _opakovani; } }

        String _lekarId; public string lekarId { get { return _lekarId.ToUpper(); } }
        UpozornitLekare _upozornitLekare; public UpozornitLekare upozornitLekare { get { return _upozornitLekare; } }
        StavElektronickehoReceptu _stav; public StavElektronickehoReceptu stav { get { return _stav; } }
        internal ZalozitRecept(Recept build)
        {
            _certificate = build._certificate;
            _IdZpravy = build._uuid_zpravy;
            _SwKlienta = build.SwKlienta;
            _key = build._key;
            _Verze = build.verze;
            _Odeslano = build._Odeslano;
            _globalCertificate = build._globalCertificate;
            _globalPass = build._globalPass;
            _userPass = build._userPass;
            _prostredi = build._prostredi;
            _datumVystaveni = build._DatumVystaveni;
            _platnost = build._platnost;
            _rodina = build.Rodina;
            _pacPrijmeni = build.Jmeno_prijmeni;
            _pacJmeno = build.Jmeno_jmena;
            _datumNarozeni = build.DatumNarozeni;
            _ulice = build.Adresa_ulice;
            _cisloPopisne = build.Adresa_cp;
            _nazevObce = build.Adresa_obec;
            _psc = build.Adresa_psc;
            _cisloPojistence = build.Pacient_CP;
            _kodPojistovny = build.Zp_id;
            _telefonPacient = build.Pacient_Telefon;
            _emailPacient = build.Pacient_Email;
            _notifikace = build.Pacient_Notifikace;
            _vezeni = build.Pacient_Veznice;
            _hmotnost = build.Pacient_Hmotnost;
            _pohlavi = build.Pacient_Pohlavi;
            _idLekare = build.LekarIdErp;
            _icp = build.Pred_icp;
            _pzs = build.Pred_pzs;
            _telefon = build.Pred_telefon;
            _odb = build.Pred_odbornost_kod;
            _predpisLP = build._predpisLP;
            _poznamka = build.Pozn;
            _upozornitLekare = build.UpozornitLekare;
            _stav = build.Stav;
            _opakovani = build.Opakovani;
            _lekarId = build.LekarIdErp;

            if (build._pkcs12bytes != null)
            {
                if (build._pkcs12password == null)
                {
                    throw new ArgumentException("Nalezena PKCS12 data, bohužel nenalezeno heslo prosím doplňte heslo.");
                }

                loadP12(build._pkcs12bytes, build._pkcs12password);
            }



        }

        public static Recept Recept()
        {
            return new Recept();
        }



        public String GenerateSoapRequest()
        {
            //Začít skládat XML
            try
            {
                //Nastavení SHA256
                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                //Příprava prázdnéh dokumentu
                XmlDocument doc = new XmlDocument();
                //Úprava pro přeskakování býlých míst
                doc.PreserveWhitespace = false;
                //Načtení dokumentu - úprava s binárního uložení

                //Načtení šablony
                String template = UTF8Encoding.UTF8.GetString(templates.ZalozeniPredpisu);

                //Vyplnění šablony
                template = replacePlaceholders(template, null, null);



                //Příprava léků
                if (predpisLP.Count() < 3 && predpisLP.Count() > 0)
                {
                    int pocetLeku = 1;
                    foreach (PredpisLP rec in predpisLP)
                    {
                        string lek = UTF8Encoding.UTF8.GetString(templates.ZalozeniPredpisuLek);
                        if (rec.MnozstviLeku != null) lek = lek.Replace("${mnozstvi}", rec.MnozstviLeku.ToString());
                        if (rec.NavodLeku != null) lek = lek.Replace("${navod}", rec.NavodLeku.ToString());
                        if (rec.Diagnoza_kod != null) lek = lek.Replace("${diagnoza}", rec.Diagnoza_kod.ToString());
                        if (rec._uhrada != null) lek = lek.Replace("${uhrada}", rec._uhrada.ToString());
                        if (rec._IdLpZdroj > 0) { lek = lek.Replace("${lpzdroj}", rec._IdLpZdroj.ToString()); } else { lek = lek.Replace("${lpzdroj}", pocetLeku.ToString()); }
                        if (rec.NezamenovatLek >= 0 && rec.NezamenovatLek < 2) lek = lek.Replace("${nezamenovat}", rec.NezamenovatLek.ToString());




                        //tady se bude rozhodovat jestli je to klasický recept nebo magistralita

                        if (rec._druhLeku == DruhLeku.REGISTROVANY)
                        {
                            string pol = UTF8Encoding.UTF8.GetString(templates.ZalozeniPredpisuReg);
                            if (rec.Kod_leku != null) pol = pol.Replace("${kodSukl}", rec.Kod_leku);
                            if (rec.Atc_kod != null) pol = pol.Replace("${atc}", rec.Atc_kod);
                            if (rec.NazevLeku != null) pol = pol.Replace("${nazevLeku}", rec.NazevLeku);
                            if (rec.FormaLeku != null) pol = pol.Replace("${formaLeku}", rec.FormaLeku);
                            if (rec.SilaLeku != null) pol = pol.Replace("${sila}", rec.SilaLeku);
                            if (rec.Cesta_podani != null) pol = pol.Replace("${cestaPodani}", rec.Cesta_podani);
                            if (rec.Hvlpreg_baleni != null) pol = pol.Replace("${baleni}", rec.Hvlpreg_baleni);
                            lek = lek.Replace("${druhReceptu}", pol);
                        }
                        else if (rec._druhLeku == DruhLeku.INDIVIDUALNI)
                        {
                            string iplp = UTF8Encoding.UTF8.GetString(templates.ZalozeniPredpisuIPLP);
                            if (rec._postuppripravy != null) iplp = iplp.Replace("${postupPripravy}", rec._postuppripravy);
                            if (rec._nazev != null) iplp = iplp.Replace("${nazev}", rec._nazev);
                            if (rec._cesta_podani != null) iplp = iplp.Replace("${cesta}", rec._cesta_podani);

                            int pocetSlozek = 1;
                            foreach (SlozkyLP slozka in rec.SlozkyLP)
                            {
                                string slotmp = UTF8Encoding.UTF8.GetString(templates.ZalozeniPredpisuSlozka);
                                if (slozka._mnozstvi > 0) slotmp = slotmp.Replace("${mnozstvi}", slozka._mnozstvi.ToString());
                                if (slozka._jednotka != null) slotmp = slotmp.Replace("${jednotka}", slozka._jednotka.ToString());
                                if (slozka._nazev != null) slotmp = slotmp.Replace("${nazev}", slozka._nazev.ToString());
                                if (slozka._surovina != null) slotmp = slotmp.Replace("${surovina}", slozka._surovina.ToString());
                                iplp = iplp.Replace("${slozka" + pocetSlozek + "}", slotmp);
                                pocetSlozek++;
                            }

                            lek = lek.Replace("${druhReceptu}", iplp);

                        }






                        template = template.Replace("${lek" + pocetLeku + "}", lek);




                        pocetLeku++;
                    }
                }
                //Očištění od přebytečných tagů
                template = removeUnusedPlaceholders(template);
                //smazt prázdné tagy
                var xd = XDocument.Parse(template);
                xd.Descendants().Attributes().Where(a => string.IsNullOrWhiteSpace(a.Value)).Remove();
                xd.Descendants()
                  .Where(e => (e.Attributes().All(a => a.IsNamespaceDeclaration))
                            && string.IsNullOrWhiteSpace(e.Value))
                  .Remove();
                //Hotové XML pro podpis
                doc.LoadXml(xd.ToString());
                //Vytvoření podepsaného dokumentu
                SignedXml signedXml = new SignedXml(doc);
                //Podpisový klíč
                signedXml.SigningKey = _key;
                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
                //Reference podpisu
                Reference reference = new Reference();
                reference.Uri = "";
                //reference.AddTransform(new XmlDsigExcC14NTransform());
                reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
                //Obálka podpisu
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                //Přidat referenci k podepisovanému XML
                signedXml.AddReference(reference);
                //Vytvořit nový objekt pro klíč
                KeyInfo keyInfo = new KeyInfo();
                //Načít x509 certifikát
                X509Certificate MSCert = _certificate;
                //Načít certifikát do Klíče X509 
                KeyInfoX509Data x509Data = new KeyInfoX509Data(MSCert);
                x509Data.AddSubjectName(_certificate.Subject.ToString());
                //přidat do keyinfo
                keyInfo.AddClause(x509Data);

                //Přidat informaci o klíči do podepsaného xmlobjektu
                signedXml.KeyInfo = keyInfo;
                //vypočítat podpis
                signedXml.ComputeSignature();
                //získání xml reprezentace podpisu a uložení do objekdu xml
                XmlElement xmlDigitalSignature = signedXml.GetXml();
                //připojit podpis k xml dokumentu
                doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

                if (doc.FirstChild is XmlDeclaration)
                {
                    doc.RemoveChild(doc.FirstChild);
                }

                //Uložení xml dokumentu na místo

                //Příprava obálky
                XmlDocument enve = new XmlDocument();
                string envelope = UTF8Encoding.UTF8.GetString(templates.Envelope);
                envelope = envelope.Replace("${body}", doc.InnerXml);
                enve.LoadXml(envelope);

                var xmlReady = enve.InnerXml;

                return xmlReady;

            }
            catch (Exception e)
            {
                throw new ArgumentException("Error while generating soap request", e);
            }

        }
        void loadP12(byte[] p12data, string password)
        {
            X509Certificate2Collection col = new X509Certificate2Collection();
            col.Import(p12data, password, X509KeyStorageFlags.Exportable);
            foreach (X509Certificate2 cert in col)
            {
                if (cert.HasPrivateKey)
                {
                    _certificate = cert;
                    RSACryptoServiceProvider tmpKey = (RSACryptoServiceProvider)cert.PrivateKey;
                    RSAParameters keyParams = tmpKey.ExportParameters(true);
                    CspParameters p = new CspParameters();
                    p.ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider";
                    _key = new RSACryptoServiceProvider(p);
                    _key.ImportParameters(keyParams);
                }

            }

            if (_key == null || _certificate == null) throw new ArgumentException("key and/or certificate still missing after p12 processing");
        }
        private String replacePlaceholders(String src, String digest, string signature)
        {
            Recept f = new eRECEPT.Recept();
            try
            {
                if (certificate != null) src = src.Replace("${certb64}", Convert.ToBase64String(certificate.GetRawCertData()));
                if (IdZpravy != null) src = src.Replace("${IdZpravy}", IdZpravy.ToString());
                if (Odeslano != null) src = src.Replace("${Odeslano}", f.formatDate(Odeslano));
                if (Verze != null) src = src.Replace("${Verze}", Verze);
                if (digest != null) src = src.Replace("${digest}", digest);
                if (signature != null) src = src.Replace("${signature}", signature);
                if (certificate != null) src = src.Replace("${subjekt}", certificate.Subject.ToString());
                if (SwKlienta != null) src = src.Replace("${Swklienta}", SwKlienta);
                if (datumVystaveni != null) src = src.Replace("${datumVystaveni}", f.formatDate(datumVystaveni, "s"));
                if (datumVystaveni != null) src = src.Replace("${platnyDo}", f.formatDate(datumVystaveni.AddDays(platnost), "s"));
                if (rodina != null) src = src.Replace("${rodina}", String.Format("{0}", (int)rodina));
                if (pacPrijmeni != null) src = src.Replace("${prijmeni}", pacPrijmeni);
                if (pacJmeno != null) src = src.Replace("${jmeno}", pacJmeno);
                if (datumNarozeni != null) src = src.Replace("${datumNarozeni}", f.formatDate(datumNarozeni, "s"));
                if (ulice != null) src = src.Replace("${ulice}", ulice);
                if (cisloPopisne != null) src = src.Replace("${cisloPopisne}", cisloPopisne);
                if (nazevObce != null) src = src.Replace("${nazevObce}", nazevObce);
                if (psc != null) src = src.Replace("${psc}", psc);
                if (cisloPojistence != null) src = src.Replace("${cisloPojistence}", cisloPojistence);
                if (kodPojistovny != null) src = src.Replace("${kodPojistovny}", kodPojistovny);
                if (telefonPacient != null) src = src.Replace("${telefonPacienta}", telefonPacient);
                if (emailPacient != null && notifikace != Notifikace.BEZNOTIFIKACE) src = src.Replace("${emailPacienta}", emailPacient);
                if (notifikace != Notifikace.BEZNOTIFIKACE) src = src.Replace("${notifikace}", notifikace.ToString());
                if (vezeni != null) src = src.Replace("${vezeni}", vezeni);
                if (hmotnost != null) src = src.Replace("${hmotnost}", hmotnost.ToString());
                if (pohlavi != null) src = src.Replace("${pohlavi}", pohlavi.ToString());
                if (idLekare != null) src = src.Replace("${idLekare}", idLekare);
                if (icp != null) src = src.Replace("${icp}", icp);
                if (pzs != null) src = src.Replace("${pzs}", pzs);
                if (telefon != null) src = src.Replace("${telefon}", telefon);
                if (odb != null) src = src.Replace("${odbornost}", odb);
                if (poznamka != null) src = src.Replace("${poznamka}", poznamka);
                if (upozornitLekare != null) src = src.Replace("${upozornitLekare}", upozornitLekare.ToString());
                if (stav != null) src = src.Replace("${stav}", stav.ToString());
                if (opakovani > 0) src = src.Replace("${opakovani}", opakovani.ToString());

                return src;
            }
            catch (Exception e)
            {
                throw new ArgumentException("replacement processing got wrong", e);
            }
        }

        private String removeUnusedPlaceholders(String src)
        {
            src = Regex.Replace(src, " [a-z_0-9]+=\"\\$\\{[0-9_a-z]+\\}\"", "");
            src = Regex.Replace(src, "\\$\\{[a-b_0-9]+\\}", "");
            src = Regex.Replace(src, "\\${[a-zA-Z0-9]*}", "");
            return src;
        }

        public String sendRequest(String requestBody)
        {
            String serviceUrl = nastavProstedi(_prostredi.GetValueOrDefault());

            X509Certificate2Collection certificates = new X509Certificate2Collection();
            try
            {
                certificates.Import(_globalCertificate, _globalPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch (Exception ex)
            {

            }


            NetworkCredential cred = new NetworkCredential(lekarId.ToString(), UserPass.ToString());
            String encoded = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(lekarId.ToString() + ":" + UserPass.ToString()));
            //enable minimal versions of TLS required by eRecept
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

            byte[] content = UTF8Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceUrl);



            req.ContentType = "text/xml;charset=UTF-8";
            req.ContentLength = content.Length;
            req.PreAuthenticate = true;
            //req.Credentials = cred;
            //req.Headers.Add("SOAPAction", serviceUrl);
            req.Method = "POST";
            req.Headers.Add("Authorization", "BASIC " + encoded);


            req.ClientCertificates = certificates;

            try
            {
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(content, 0, content.Length);
                reqStream.Close();

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            String responseString = "";
            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    StreamReader rdr = new StreamReader(respStream, Encoding.UTF8);
                    responseString = rdr.ReadToEnd();
                }

            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        return text;

                    }
                }
            }

            return responseString;






        }

        protected String nastavProstedi(Prostredi val)
        {
            if (val == Prostredi.TEST)
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
            else
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
        }
    }







    public class AppPingZep
    {
        X509Certificate2 _certificate; public X509Certificate2 certificate { get { return _certificate; } }

        byte[] _globalCertificate; public Byte[] globalCertificate { get { return _globalCertificate; } }
        Guid _IdZpravy; public Guid IdZpravy { get { return _IdZpravy; } }
        String _SwKlienta; public String SwKlienta { get { return _SwKlienta; } }
        DateTime _Odeslano; public DateTime Odeslano { get { return _Odeslano; } }

        String _Verze; public String Verze { get { return _Verze; } }

        String _globalPass; public String GlobalPass { get { return _globalPass; } }

        String _userPass; public String UserPass { get { return _userPass; } }
        RSACryptoServiceProvider _key; RSACryptoServiceProvider key { get { return _key; } }

        Prostredi? _prostredi; public Prostredi? prostredi { get { return _prostredi; } }

        String _lekarId; public String lekarId { get { return _lekarId; } }
        internal AppPingZep(Recept build)
        {
            _certificate = build._certificate;
            _IdZpravy = build._uuid_zpravy;
            _SwKlienta = build.SwKlienta;
            _key = build._key;
            _Verze = build.verze;
            _Odeslano = build._Odeslano;
            _globalCertificate = build._globalCertificate;
            _globalPass = build._globalPass;
            _userPass = build._userPass;
            _prostredi = build._prostredi;
            _lekarId = build.LekarIdErp;


            if (build._pkcs12bytes != null)
            {
                if (build._pkcs12password == null)
                {
                    throw new ArgumentException("Nalezena PKCS12 data, bohužel nenalezeno heslo prosím doplňte heslo.");
                }
                loadP12(build._pkcs12bytes, build._pkcs12password);
            }

            //if (key != null)
            //    computeCodes(key);

        }

        public static Recept Recept()
        {
            return new Recept();
        }


        public String formatDate(DateTime date)
        {
            string ret = date.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz");
            return ret;
        }

        public static DateTime parseDate(String date)
        {
            return DateTime.Parse(date);
        }

        public static string byte2hex(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", data[i]));
            }
            return sb.ToString();
        }

        void loadP12(byte[] p12data, string password)
        {
            X509Certificate2Collection col = new X509Certificate2Collection();
            col.Import(p12data, password, X509KeyStorageFlags.Exportable);
            foreach (X509Certificate2 cert in col)
            {
                if (cert.HasPrivateKey)
                {
                    _certificate = cert;
                    RSACryptoServiceProvider tmpKey = (RSACryptoServiceProvider)cert.PrivateKey;
                    RSAParameters keyParams = tmpKey.ExportParameters(true);
                    CspParameters p = new CspParameters();
                    p.ProviderName = "Microsoft Enhanced RSA and AES Cryptographic Provider";
                    _key = new RSACryptoServiceProvider(p);
                    _key.ImportParameters(keyParams);
                }

            }

            if (_key == null || _certificate == null) throw new ArgumentException("key and/or certificate still missing after p12 processing");
        }

        public String GenerateSoapRequest()
        {

            try
            {
                //Nastavení SHA256
                CryptoConfig.AddAlgorithm(typeof(RSAPKCS1SHA256SignatureDescription), "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");
                //Příprava prázdnéh dokumentu
                XmlDocument doc = new XmlDocument();
                //Úprava pro přeskakování býlých míst
                doc.PreserveWhitespace = false;
                //Načtení dokumentu - úprava s binárního uložení
                String template = UTF8Encoding.UTF8.GetString(templates.AppPingZEPDotazN);
                template = replacePlaceholders(template, null, null);
                doc.LoadXml(template);
                //Vytvoření podepsaného dokumentu
                SignedXml signedXml = new SignedXml(doc);
                //Podpisový klíč
                signedXml.SigningKey = _key;
                signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
                //Reference podpisu
                Reference reference = new Reference();
                reference.Uri = "";
                //reference.AddTransform(new XmlDsigExcC14NTransform());
                reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";
                //Obálka podpisu
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                //Přidat referenci k podepisovanému XML
                signedXml.AddReference(reference);
                //Vytvořit nový objekt pro klíč
                KeyInfo keyInfo = new KeyInfo();
                //Načít x509 certifikát
                X509Certificate MSCert = _certificate;
                //Načít certifikát do Klíče X509 
                KeyInfoX509Data x509Data = new KeyInfoX509Data(MSCert);
                x509Data.AddSubjectName(_certificate.Subject.ToString());
                //přidat do keyinfo
                keyInfo.AddClause(x509Data);

                //Přidat informaci o klíči do podepsaného xmlobjektu
                signedXml.KeyInfo = keyInfo;
                //vypočítat podpis
                signedXml.ComputeSignature();
                //získání xml reprezentace podpisu a uložení do objekdu xml
                XmlElement xmlDigitalSignature = signedXml.GetXml();
                //připojit podpis k xml dokumentu
                doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

                if (doc.FirstChild is XmlDeclaration)
                {
                    doc.RemoveChild(doc.FirstChild);
                }

                //Uložení xml dokumentu na místo

                //Příprava obálky
                XmlDocument enve = new XmlDocument();
                string envelope = UTF8Encoding.UTF8.GetString(templates.AppPingZEPDotazS);
                envelope = envelope.Replace("${body}", doc.InnerXml);
                enve.LoadXml(envelope);

                var xmlReady = enve.InnerXml;

                return xmlReady;

            }
            catch (Exception e)
            {
                throw new ArgumentException("Error while generating soap request", e);
            }

        }

        private string ReplacePrefix(String src)
        {

            src = src.Replace("AppPingZEPDotaz", "v2301:AppPingZEPDotaz");
            src = src.Replace("Zprava", "v2301:Zprava");
            src = src.Replace("ID_Zpravy", "v2301:ID_Zpravy");
            src = src.Replace("Verze", "v2301:Verze");
            src = src.Replace("Odeslano", "v2301:Odeslano");
            src = src.Replace("SW_Klienta", "v2301:SW_Klienta");


            return src;
        }

        private String replacePlaceholders(String src, String digest, string signature)
        {
            try
            {
                if (certificate != null) src = src.Replace("${certb64}", Convert.ToBase64String(certificate.GetRawCertData()));
                if (IdZpravy != null) src = src.Replace("${IdZpravy}", IdZpravy.ToString());
                if (Odeslano != null) src = src.Replace("${Odeslano}", formatDate(Odeslano));
                if (Verze != null) src = src.Replace("${Verze}", Verze);
                if (digest != null) src = src.Replace("${digest}", digest);
                if (signature != null) src = src.Replace("${signature}", signature);
                if (certificate != null) src = src.Replace("${subjekt}", certificate.Subject.ToString());
                if (SwKlienta != null) src = src.Replace("${Swklienta}", SwKlienta);


                return src;
            }
            catch (Exception e)
            {
                throw new ArgumentException("replacement processing got wrong", e);
            }
        }

        private String removeUnusedPlaceholders(String src)
        {
            src = Regex.Replace(src, " [a-z_0-9]+=\"\\$\\{[0-9_a-z]+\\}\"", "");
            src = Regex.Replace(src, "\\$\\{[a-b_0-9]+\\}", "");
            return src;
        }
        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }


        public String sendRequest(String requestBody)
        {
            String serviceUrl = nastavProstedi(_prostredi.GetValueOrDefault());

            X509Certificate2Collection certificates = new X509Certificate2Collection();
            try
            {
                certificates.Import(_globalCertificate, _globalPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            }
            catch (Exception ex)
            {

            }


            NetworkCredential cred = new NetworkCredential(lekarId.ToString(), UserPass.ToString());
            String encoded = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(lekarId.ToString() + ":" + UserPass.ToString()));
            //enable minimal versions of TLS required by eRecept
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            ServicePointManager.Expect100Continue = true;
            //ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

            byte[] content = UTF8Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(serviceUrl);



            req.ContentType = "text/xml;charset=UTF-8";
            req.ContentLength = content.Length;
            req.PreAuthenticate = true;
            //req.Credentials = cred;
            //req.Headers.Add("SOAPAction", serviceUrl);
            req.Method = "POST";
            req.Headers.Add("Authorization", "BASIC " + encoded);


            req.ClientCertificates = certificates;

            try
            {
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(content, 0, content.Length);
                reqStream.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            String responseString = "";
            try
            {
                using (WebResponse resp = req.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    StreamReader rdr = new StreamReader(respStream, Encoding.UTF8);
                    responseString = rdr.ReadToEnd();
                }

            }
            catch (WebException e)
            {
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        Console.WriteLine(text);
                        Console.ReadKey();
                    }
                }
            }

            return responseString;









        }



        protected String nastavProstedi(Prostredi val)
        {
            if (val == Prostredi.TEST)
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
            else
                return "https://lekar-soap.test-erecept.sukl.cz/cuer/Lekar";
        }
    }

    public class zpracujRecept
    {
        internal string _odpovedXML { get; set; }
        internal string _kodChyby; public string kodChyba { get { return _kodChyby; } }
        internal string _obsahChyby; public string obsahChyby { get { return _obsahChyby; } }
        internal string _popis; public string popis { get { return _popis; } }
        internal string _doporuceni; public string doporuceni { get { return _doporuceni; } }
        internal string _idDokladuPodani; public string idDokladuPodani { get { return _idDokladuPodani; } }
        internal List<idLP> _idLp; public List<idLP> idLpList { get { return _idLp; } }
        internal string _idZpravyPodani; public string idZpravyPodani { get { return _idZpravyPodani; } }
        internal string _idPodani; public string idPodani { get { return _idPodani; } }
        internal DateTime _prijato; public DateTime prijato { get { return _prijato; } }
        internal bool _chyba; public bool chyba { get { return _chyba; } }


        public class idLP
        {
            internal string _idLpZdroj; public string idLpZdroj { get { return _idLpZdroj; } }
            internal string _idLp; public string idLp { get { return _idLp; } }
        }


        public zpracujRecept odpovedXML(String val)
        {
            _odpovedXML = val;
            return this;
        }


        public zpracujRecept odpoved()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(_odpovedXML);
            var nsmgr = new XmlNamespaceManager(xml.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("erp", "http://www.sukl.cz/erp/201704");
            _chyba = false;


            XmlNode chybaList = xml.SelectSingleNode("soap:Envelope/soap:Body/soap:Fault", nsmgr);
            if (chybaList != null)
            {
                _chyba = true;
                _obsahChyby = chybaList["faultstring"].InnerText;

                XmlNode detailList = xml.SelectSingleNode("soap:Envelope/soap:Body/soap:Fault/detail/erp:Chyba", nsmgr);
                try
                {
                    _kodChyby = detailList["erp:Kod"].InnerText;
                    _popis = detailList["erp:Popis"].InnerText;
                    _doporuceni = detailList["erp:Doporuceni"].InnerText;
                }
                catch
                {
                    _kodChyby = "";
                    _popis = _obsahChyby;
                    _doporuceni = "";
                }
        
               
            
                return this;
            }




            XmlNodeList xnList = xml.SelectNodes("soap:Envelope/soap:Body/erp:ZalozeniPredpisuOdpoved/erp:Doklad", nsmgr);
            foreach (XmlNode xn in xnList)
            {
                XmlNode dokladNode = xn.SelectSingleNode("erp:ID_Dokladu", nsmgr);
                _idDokladuPodani = dokladNode.InnerText;
                XmlNodeList lpNode = xn.SelectNodes("erp:LP", nsmgr);
                _idLp = new List<idLP>();
                foreach (XmlNode xnlp in lpNode)
                {


                    if (xnlp != null)
                    {
                        idLP xidLp = new idLP();
                        xidLp._idLp = xnlp["erp:ID_LP_Zdroj"].InnerText;
                        xidLp._idLpZdroj = xnlp["erp:ID_LP"].InnerText;

                        idLpList.Add(xidLp);
                    }
                }
            }
            XmlNode ZpravaList = xml.SelectSingleNode("soap:Envelope/soap:Body/erp:ZalozeniPredpisuOdpoved/erp:Zprava", nsmgr);
            try
            {
                _idZpravyPodani = ZpravaList["erp:ID_Zpravy"].InnerText;
                _idPodani = ZpravaList["erp:ID_Podani"].InnerText;
                _prijato = DateTime.Parse(ZpravaList["erp:Prijato"].InnerText);

            }
            catch (Exception ex)
            {

            }



            return this;
        }



    }


}