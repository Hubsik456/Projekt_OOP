using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Interfejsy;

namespace WpfApp3.Klasy
{
    internal abstract class Samochód : I_Samochód
    {
        // Pola
        private string marka;
        private string model;
        private string kolor;
        private Rodzaj_Napędu napęd;
        private Skrzynia_Biegów biegi;
        private string id;
        private string źródło;
        private string typ = "N/A";

        // Właściwosci
        public string Marka
        {
            set { marka = value; }
            get { return marka; }
        }
        public string Model
        {
            set { model = value; }
            get { return model; }
        }
        public string Kolor
        {
            set { kolor = value; }
            get { return kolor; }
        }
        public Rodzaj_Napędu Napęd
        {
            set { napęd = value; }
            get { return napęd; }
        }
        public Skrzynia_Biegów Biegi
        {
            set { biegi = value; }
            get { return biegi; }
        }
        public string ID
        {
            set { id = value; }
            get { return  id; }
        }
        public string Źródło
        {
            set { źródło = value; }
            get { return źródło; }
        }
        public string Typ
        {
            set { typ = value; }
            get { return typ; }
        }

        internal Rodzaj_Napędu Rodzaj_Napędu
        {
            get => default;
            set
            {
            }
        }

        internal Skrzynia_Biegów Skrzynia_Biegów
        {
            get => default;
            set
            {
            }
        }

        // Konstruktory
        public Samochód(string _ID, string _Źródło, string _Marka, string _Model, string _Kolor, Rodzaj_Napędu _Napęd, Skrzynia_Biegów _Skrzynia_Biegów)
        {
            ID = _ID;
            Źródło = _Źródło;
            Marka = _Marka;
            Model = _Model;
            Kolor = _Kolor;
            Napęd = _Napęd;
            Biegi = _Skrzynia_Biegów;
        }

        // Metody

        /// <summary>
        /// Metoda zmieniająca wartości wyświeltane w głównym oknie programu.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public virtual void Print_WPF(MainWindow GUI)
        {
            GUI.Label_Typ.Content = Typ;
            GUI.Label_Model.Content = Model;
            GUI.Label_Marka.Content = Marka;
            GUI.Label_Kolor.Content = Kolor;
            GUI.Label_Rodzaj_Napędu.Content = Napęd.Typ;
            GUI.Label_Skrzynia_Biegów.Content = Biegi.Typ;

            GUI.Label_Prędkość.Content = Napęd.Prędkość.ToString("0.00");
            GUI.Label_RPM.Content = Napęd.RPM.ToString("0.00");
            GUI.Label_Bieg.Content = Biegi.Obecny_Bieg;
            //GUI.Label_Ilość_Paliwa.Content = Napęd.Obecna_Ilość_Paliwa.ToString("0.00");
            Napęd.Print_WPF(GUI);
            GUI.Label_Ilość_Oleju.Content = Napęd.Obecna_Ilość_Oleju.ToString("0.00");
        }

        /// <summary>
        /// Główna metoda odpowiadająca za pracę programu. Ta metoda wykonuje symulacje opartą na właściwościach danego samochodu.
        /// </summary>
        /// <param name="GUI">Główne okno programu.</param>
        public void Symulacja(MainWindow GUI)
        {
            if (!GUI.Pauza) // Jeśli pauza jest wyłączona
            {
                // Zmiany prędkości / RPM
                if (GUI.Czy_Przycisk_Przyśpieszanie_Jest_Wciśnięty && Napęd.Czy_Silnik_Jest_Włączony && Napęd.Obecna_Ilość_Paliwa > 0 && Napęd.Obecna_Ilość_Oleju > 0) // Przyśpieszanie
                {
                    Przyśpieszanie();
                }
                else if (GUI.Czy_Przycisk_Hamowanie_Jest_Wciśnięty && Napęd.Czy_Silnik_Jest_Włączony) // Hamowanie
                {
                    Hamowanie();
                }
                else // Hamowanie Silnikiem
                {
                    Hamowanie_Silnikiem();
                }

                if (Napęd.Czy_Silnik_Jest_Włączony)
                {
                    Napęd.Obecna_Ilość_Oleju -= 0.002f;
                    Napęd.Obecna_Ilość_Paliwa -= Napęd.Współczynnik_C;

                    if (Napęd.Obecna_Ilość_Oleju < 0)
                    {
                        Napęd.Obecna_Ilość_Oleju = 0;
                    }

                    if (Napęd.Obecna_Ilość_Paliwa < 0)
                    {
                        Napęd.Obecna_Ilość_Paliwa = 0;
                    }
                }
            }

            Print_WPF(GUI);
            //GUI.Log($"Ilość paliwa: {Napęd.Obecna_Ilość_Paliwa} / {Napęd.Max_Ilość_Paliwa}");
        }

        /// <summary>
        /// Metod pomocniczna do metody "Symulacja" z klasy "Samochód". Odpowiada za przyśpieszanie samochodu.
        /// </summary>
        public void Przyśpieszanie()
        {
            if (Biegi.Obecny_Bieg == 0) // Na biegu 0 nie da się przyśpieszać więc...
            {
                Hamowanie_Silnikiem();
            }
            else
            {
                Napęd.Prędkość += Napęd.Współczynnik_A;
                Napęd.RPM += Napęd.Współczynnik_B;

                if (Napęd.Prędkość > (Biegi.Obecny_Bieg * 50 * Napęd.Współczynnik_A)) // Limit prędkości, zależny od biegu
                {
                    Napęd.Prędkość = Biegi.Obecny_Bieg * 50 * Napęd.Współczynnik_A;
                }

                if (Napęd.RPM > 60 * (Napęd.Współczynnik_B)) // Limit RPM, nie zależny od biegu
                {
                    Napęd.RPM = 60 * Napęd.Współczynnik_B;
                }
            }
        }

        /// <summary>
        /// Metod pomocniczna do metody "Symulacja" z klasy "Samochód". Odpowiada za hamowanie samochodu.
        /// </summary>
        public virtual void Hamowanie()
        {
            Napęd.Prędkość -= 3f;
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
        public virtual void Hamowanie_Silnikiem()
        {
            Napęd.Prędkość -= 0.3f;
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
