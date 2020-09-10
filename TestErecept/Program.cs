using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eRECEPT;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Xml;

namespace TestErecept
{
    class Program
    {

        static void Main(string[] args)
        {



            //var cert = new X509Certificate2(, eet.EETPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);




            //var has = cert.HasPrivateKey;




            AppPingZep data;

            data = AppPingZep.Recept()
                .Pkcs12(zdroj.vet)
                .Pkcs12Password("1234")
                .GlobalCertificate(zdroj.suklTEST00000910487)
                .GlobalPass("Test1234")
                .prostredi(Prostredi.TEST)
                .odeslano(DateTime.Now)
                .lekarId("b8234b9c-b7cd-11e7-abc4-cec278b6b50a")
                .recept();



            String signed = data.GenerateSoapRequest();
            string response = "";
            try
            {
                response = data.sendRequest(signed);
            }
            catch (Exception ex)
            {

            }


            PredpisLP lek, lek2;
            lek = new PredpisLP();
            lek2 = new PredpisLP();

            SlozkyLP slozky;
            slozky = new SlozkyLP();
            slozky.mnozstviSlozky(5);
            slozky.nazevSlozky("Ac. salic");
            slozky.jednotka(Jednotka.g);
            //slozky.surovinaSlozky("9000006");

            SlozkyLP slozky2;
            slozky2 = new SlozkyLP();
            slozky2.mnozstviSlozky(100);
            slozky2.nazevSlozky("Vaselini ad");
            slozky2.jednotka(Jednotka.g);


            //lek.kod("0086148");
            //lek.atc("J01CR02");
            //lek.nazev("AUGMENTIN 625 MG");
            //lek.sila("500MG/125MG");
            //lek.forma("TBL FLM");
            //lek.baleni("21 II");
            lek.cestapodani("IHN");

            lek.postupPripravyLeku("Smíchej, ať vznikne mast");
            lek.nazev("Mast na ztvrdlou kůži");


            lek.diagnoza("I10");
            lek.navod("3x denně potírat postižené místo");
            lek.mnozstvi(1);
            lek.druhLeku(DruhLeku.REGISTROVANY);
            lek.uhrada(Uhrada.PACIENT);
            //lek.nezamenovat(1);
            lek.idpolozky(1);
            lek.druhLeku(DruhLeku.INDIVIDUALNI);

            lek2.kod("0047741");
            lek2.atc("C07AB07");
            lek2.nazev("RIVOCOR 10");
            lek2.sila("10MG");
            lek2.forma("TBL FLM");
            lek2.baleni("30");
            lek2.cestapodani("POR");
            //lek2.diagnoza("I10");
            lek2.navod("Pacient poučen,vydat pouze do vlastních rukou pacienta,prosím ověřit totožnost");
            lek2.mnozstvi(3);
            lek2.druhLeku(DruhLeku.REGISTROVANY);
            lek2.uhrada(Uhrada.ZAKLADNI);
            lek2.idpolozky(2);



            lek.Slozka(slozky);
            lek.Slozka(slozky2);


            ZalozitRecept _recept;
            _recept = ZalozitRecept.Recept()
                .Pkcs12(zdroj.vet)
                .Pkcs12Password("1234")
                .GlobalCertificate(zdroj.suklTEST00000910487)
                .GlobalPass("Test1234")
                .userPass("Test1234")
                .prostredi(Prostredi.TEST)
                .odeslano(DateTime.Now)
            .datumVystaveni(DateTime.Now)
            .rodina(Rodina.NE)
            .opakovani(3)
            .jmenoPrijmeni("Pavlíček")
            .jmenoJmena("Miroslav")
            .datumNarozeni(new DateTime(1983, 02, 16))
            .adresaUlice("Slévačská")
            .adresaCp("905")
            .adresaObec("Praha 9")
            .adresaPSC("19800")
            .pacientCP("8302160108")
            .zpId("207")
            .pacientTelefon("776085686")
            .pacientEmail("mira@alt64.cz")
            
                .notifikace(Notifikace.EMAIL)
          
            

            .pacientVeznice("Pracovní činnost")
            .pacientHmotnost(200)
            .pohlavi(Pohlavi.M)
            .lekarId("b8234b9c-b7cd-11e7-abc4-cec278b6b50a")
            .lekarIcp("12345678")
            .lekarPzs("00000910487")
            .lekarTelefon("123456789")
            .lekarOdbornost("001")
            .poznamka("Pacient použen, vydat pouze do vlastních rukou pacienta, prosím ověřit totožnost")
            .upozornitLekare(UpozornitLekare.PRISTI_NAVSTEVA)
            .stav(StavElektronickehoReceptu.PREDEPSANY)
            .doporucujiciJmeno("Zuzana")
            .doporucujiciPrijmeni("Buriánková")
            .doporucujiciNazevPzs("MUDr. Zuzana Buriánková - PLDD-Pediatrie")
            .doporucujiciOdbornost("002")
            .doporucujiciIcp("29718001")
            .doporucujiciIc("29718001")
            .doporucujiciTelefon("233930435")
                         .predespat(lek)
             .predespat(lek2)


            .zalozit();

            String podpis = _recept.GenerateSoapRequest();
            string odpoved = "";

            string idDokladuPodani = null;
            string idLpZdroj;
            string idLp;
            string idZpravyPodani = null;
            string idPodani;
            DateTime prijato;

            try
            {
                odpoved = _recept.sendRequest(podpis);
                zpracujRecept zprep = new zpracujRecept();
                zprep.odpovedXML(odpoved);
                zprep.odpoved();

                idDokladuPodani = zprep.idDokladuPodani;
                idZpravyPodani = zprep.idZpravyPodani;

                if (zprep.chyba == true)
                {
                    Console.WriteLine(zprep.doporuceni);
                }
                //XmlDocument xml = new XmlDocument();


                //xml.LoadXml(odpoved);
                //var nsmgr = new XmlNamespaceManager(xml.NameTable);
                //nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                //nsmgr.AddNamespace("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
                //nsmgr.AddNamespace("erp", "http://www.sukl.cz/erp/201704");
                //XmlNodeList xnList = xml.SelectNodes("soap:Envelope/soap:Body/erp:ZalozeniPredpisuOdpoved/erp:Doklad",nsmgr);
                //foreach (XmlNode xn in xnList)
                //{
                //    XmlNode dokladNode = xn.SelectSingleNode("erp:ID_Dokladu",nsmgr);
                //    idDokladuPodani = dokladNode.InnerText;
                //    XmlNode lpNode = xn.SelectSingleNode("erp:LP", nsmgr);
                //    if (lpNode != null)
                //    {
                //        idLpZdroj = lpNode["erp:ID_LP_Zdroj"].InnerText;
                //        idLp = lpNode["erp:ID_LP"].InnerText;
                //    }
                //}
                //XmlNode ZpravaList = xml.SelectSingleNode("soap:Envelope/soap:Body/erp:ZalozeniPredpisuOdpoved/erp:Zprava", nsmgr);
                //idZpravyPodani = ZpravaList["erp:ID_Zpravy"].InnerText;
                //idPodani = ZpravaList["erp:ID_Podani"].InnerText;
                //prijato = DateTime.Parse(ZpravaList["erp:Prijato"].InnerText);

            }
            catch (Exception ex)
            {

            }


            ZrusitRecept delRep;
            delRep = ZrusitRecept.Recept()
                .lekarId("b8234b9c-b7cd-11e7-abc4-cec278b6b50a")
                .lekarPzs("00000910487")
                .idDokladuNew(idDokladuPodani)
                .autorizacniKod(idZpravyPodani)
                 .datumVystaveni(DateTime.Now)
                 .Pkcs12(zdroj.vet)
                .Pkcs12Password("1234")
                .GlobalCertificate(zdroj.suklTEST00000910487)
                .GlobalPass("Test1234")
                .duvodZruseni("Testování")
                .prostredi(Prostredi.TEST)
                .zrusit();


            String podZrus = delRep.GenerateSoapRequest();
            string reakce = "";
            try
            {
                XmlDocument xmlReakce = new XmlDocument();
                reakce = delRep.sendRequest(podZrus);
                xmlReakce.LoadXml(reakce);
            }
            catch (Exception ex)
            {

            }



            Console.WriteLine(_recept);
            Console.ReadKey();


        }
    }
}
;