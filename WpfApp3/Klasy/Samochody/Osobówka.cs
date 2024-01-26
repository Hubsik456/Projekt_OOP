using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3.Klasy
{
    internal class Osobówka : Samochód
    {
        // Pola

        // Właściwości

        // Konstruktory
        public Osobówka(string _ID, string _Źródło, string _Marka, string _Model, string _Kolor, Rodzaj_Napędu _Napęd, Skrzynia_Biegów _Skrzynia_Biegów) : base(_ID, _Źródło, _Marka, _Model, _Kolor, _Napęd, _Skrzynia_Biegów)
        {
            Typ = "Osobówka";
        }

        // Metody
        /// <summary>
        /// Metod pomocniczna do metody "Symulacja" z klasy "Samochód". Odpowiada za hamowanie samochodu.
        /// </summary>
        public override void Hamowanie()
        {
            Napęd.Prędkość -= 3.5f;
            Napęd.RPM -= 100;

            if (Napęd.Prędkość < 0)
            {
                Napęd.Prędkość = 0;
            }

            if (Napęd.RPM < 1000)
            {
                Napęd.RPM = 1000;
            }
        }

        /// <summary>
        /// Metod pomocniczna do metody "Symulacja" z klasy "Samochód". Odpowiada za hamowanie pasywne (kiedy nie jest wciśnięty przycisk przyśpieszanie czy przycisk hamowanie) samochodu.
        /// </summary>
        public override void Hamowanie_Silnikiem()
        {
            Napęd.Prędkość -= 0.35f;
            Napęd.RPM -= 100;


            if (Napęd.Prędkość < 0)
            {
                Napęd.Prędkość = 0;
            }

            if (Napęd.RPM < 1000)
            {
                Napęd.RPM = 1000;
            }
        }
    }
}
