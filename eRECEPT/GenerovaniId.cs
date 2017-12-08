using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eRECEPT
{
    public class GenerovaniId
    {
        private String base32Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private String verzeDruh = "P"; //nastavení druhu a verze dokladu - jeden znak z Base32 znakové sady P = předpis verze 201701, V = výdej verze 201701, ...
        public String Generuj()
        {

            //zjištění pořadového čísla měsíce (od ledna 2016) 
            DateTime Dnes = DateTime.Now;
            DateTime PrvniMesic = new DateTime(2016, 1, 1);
            int PocetMesicu = Math.Abs((Dnes.Month) + 12 * (Dnes.Year - PrvniMesic.Year));



            //generování náhodnéo čísla
            Random Generator = new Random();
            byte[] NahodneCislo = new byte[5];
            Generator.NextBytes(NahodneCislo);

            

            return PocetMesicu.ToString();
        }
    }
}
