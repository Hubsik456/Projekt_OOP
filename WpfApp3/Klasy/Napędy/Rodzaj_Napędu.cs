using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Interfejsy;

namespace WpfApp3.Klasy
{
    internal abstract class Rodzaj_Napędu : I_Napęd
    {
        // Pola
        private bool czy_silnik_jest_włączony;
        private string typ;
        private float prędkość;
        private float max_ilość_paliwa;
        private float obecna_ilość_paliwa;
        private float max_ilośc_oleju;
        private float obecna_ilośc_oleju;
        private float rpm;
        private float współczynnik_a; // Przyśpieszanie
        private float współczynnik_b; // Hamowanie
        private float współczynnik_c; // Olej

        // Właściwości
        public bool Czy_Silnik_Jest_Włączony
        {
            set { czy_silnik_jest_włączony = value; }
            get { return czy_silnik_jest_włączony; }
        }
        public string Typ
        {
            set { typ = value; }
            get { return typ; }
        }
        public float Prędkość
        {
            set { prędkość = value; }
            get { return prędkość; }
        }
        public float Max_Ilość_Paliwa
        {
            set { max_ilość_paliwa = value; }
            get { return max_ilość_paliwa; }
        }
        public float Obecna_Ilość_Paliwa
        {
            set { obecna_ilość_paliwa = value; }
            get { return obecna_ilość_paliwa; }
        }
        public float Max_Ilość_Oleju
        {
            set { max_ilośc_oleju = value; }
            get { return max_ilośc_oleju ; }
        }
        public float Obecna_Ilość_Oleju
        {
            set { obecna_ilośc_oleju = value; }
            get { return obecna_ilośc_oleju; }
        }
        public float RPM
        {
            set { rpm = value; }
            get { return rpm; }
        }
        public float Współczynnik_A
        {
            set { współczynnik_a = value; }
            get { return współczynnik_a; }
        }
        public float Współczynnik_B
        {
            set { współczynnik_b = value; }
            get { return współczynnik_b; }
        }
        public float Współczynnik_C
        {
            set { współczynnik_c = value; }
            get { return współczynnik_c; }
        }

        // Konstruktory
        public Rodzaj_Napędu(float _Współczynnik_A, float _Współczynnik_B, float _Współczynnik_C)
        {
            Czy_Silnik_Jest_Włączony = false;
            Typ = "N/A";
            Prędkość = 0f;
            Max_Ilość_Paliwa = 100f;
            Obecna_Ilość_Paliwa = Max_Ilość_Paliwa;
            Max_Ilość_Oleju = 10f;
            Obecna_Ilość_Oleju = Max_Ilość_Oleju;
            RPM = 0f;
            Współczynnik_A = _Współczynnik_A;
            Współczynnik_B = _Współczynnik_B;
            Współczynnik_C = _Współczynnik_C;
        }

        // Metody

        /// <summary>
        /// Metoda obsługująca włączanie i wyłączanie silnika
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public virtual void Silnik(MainWindow GUI)
        {
            if (Czy_Silnik_Jest_Włączony) // Wyłączenie silnika
            {
                Czy_Silnik_Jest_Włączony = false;
            }
            else // Włączenie silnika
            {
                Czy_Silnik_Jest_Włączony = true;
                Prędkość = 0f;
                RPM = 1000;
                GUI.Pauza = false;
            }
        }

        /// <summary>
        /// Metoda obsługująca zmianę tekstu w głównym oknie programu.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public virtual void Print_WPF(MainWindow GUI)
        {
            GUI.Label_Ilość_Paliwa.Content = Obecna_Ilość_Paliwa.ToString("0.00");
        }

        /// <summary>
        /// Metoda odpowiadająca za uzupełnienie paliwa samochodu.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public void Tankowanie(MainWindow GUI)
        {
            if (!Czy_Silnik_Jest_Włączony && Prędkość == 0)
            {
                Obecna_Ilość_Paliwa = Max_Ilość_Paliwa;
            }
            else
            {
                GUI.Log("Błąd: Tankowanie, musisz się zatrzymać i wyłączyć silnik.", Wyświetl_Czas: true);
            }
        }

        /// <summary>
        /// Metoda odpowiadająca za uzupełnianie oleju samochodu.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public void Uzupełnienie_Oleju(MainWindow GUI)
        {
            if (!Czy_Silnik_Jest_Włączony && Prędkość == 0)
            {
                Obecna_Ilość_Oleju = Max_Ilość_Oleju;
            }
            else
            {
                GUI.Log("Błąd: Uzupełnianie Oleju, musisz się zatrzymać i wyłączyć silnik.", Wyświetl_Czas: true);
            }
        }

    }
}
