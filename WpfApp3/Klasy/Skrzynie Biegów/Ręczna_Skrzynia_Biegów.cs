using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3.Klasy
{
    internal class Ręczna_Skrzynia_Biegów : Skrzynia_Biegów
    {
        public Ręczna_Skrzynia_Biegów()
        {
            Typ = "Ręczna";
        }

        /// <summary>
        /// Bardziej rozbudowana metoda odpowiedzialna za zmianę biegów oraz sprawdzanie tego jak są one zmieniane.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        /// <param name="Nowy_Bieg">Bieg jaki ma zostać ustawiony</param>
        public override void Zmiana_Biegu(MainWindow GUI, int Nowy_Bieg)
        {
            if (Nowy_Bieg < Obecny_Bieg)
            {
                Obecny_Bieg = Nowy_Bieg;
            }
            else if ((Nowy_Bieg - 1) == Obecny_Bieg)
            {
                Obecny_Bieg = Nowy_Bieg;
            }
            else if (Nowy_Bieg == Obecny_Bieg)
            {
                GUI.Log("Błąd: Ten bieg jest już wybrany", Wyświetl_Czas: true);
            }
            else
            {
                GUI.Log("Błąd: Próba zmiany o kilka biegów na raz", Wyświetl_Czas: true);
            }
        }
    }
}
