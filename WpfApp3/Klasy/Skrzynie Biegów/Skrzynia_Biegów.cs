using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Interfejsy;

namespace WpfApp3.Klasy
{
    internal abstract class Skrzynia_Biegów : I_Skrzynia_Biegów
    {
        // Pola
        private string typ;
        private int obecny_bieg;

        // Właściwości
        public string Typ
        {
            set { typ = value; }
            get { return typ; }
        }
        public int Obecny_Bieg
        {
            set { obecny_bieg = value; }
            get { return obecny_bieg; }
        }


        // Konstruktory
        public Skrzynia_Biegów()
        {
            Typ = "N/A";
            Obecny_Bieg = 0;
        }

        // Metody

        /// <summary>
        /// Metoda odpowiedzialna za zmianę biegów oraz sprawdzanie tego jak są one zmieniane.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        /// <param name="Nowy_Bieg">Bieg jaki ma zostać ustawiony</param>
        public virtual void Zmiana_Biegu(MainWindow GUI, int Nowy_Bieg)
        {
            if (Obecny_Bieg == Nowy_Bieg)
            {
                GUI.Log("Błąd: Ten bieg jest już wybrany", Wyświetl_Czas: true);
            }
            else
            {
                Obecny_Bieg = Nowy_Bieg;
            }
            
        }
    }
}
