using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using WpfApp3.Klasy;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Timers;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Globalization;

namespace WpfApp3
{

    /// <summary>
    /// Logika głównego okna.
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Samochód> Samochody = new ObservableCollection<Samochód>();

        private int Index_Obecny_Samochód = 0;
        private int Index_DataGrid;
        private int Index_Max_Local = 1;
        public bool Czy_Przycisk_Przyśpieszanie_Jest_Wciśnięty = false;
        public bool Czy_Przycisk_Hamowanie_Jest_Wciśnięty = false;
        public bool Pauza = true;

        private string Ścieżka_Do_Źródeł_Danych = "Źródła Danych"; // ../../../../Źródła Danych

        /// <summary>
        /// Główny kod projektu.
        /// </summary>
        public MainWindow()
        {
            //this.SizeToContent = SizeToContent.WidthAndHeight; // Auto resize, działa przy zmianie zakładek

            // Dodawanie elementów do listy
            Samochody.Add(new Klasy.Osobówka(Index_Max_Local.ToString(), "Lokalne", "Marka 1", "Model 1", "Czarny", new Napęd_Spalinowy(0.8f, 100f, 0.0025f), new Ręczna_Skrzynia_Biegów()));
            Index_Max_Local++;
            Samochody.Add(new Klasy.Osobówka(Index_Max_Local.ToString(), "Lokalne", "Marka 2", "Model 2", "Czarny", new Napęd_Spalinowy(0.8f, 100f, 0.0025f), new Ręczna_Skrzynia_Biegów()));
            Index_Max_Local++;
            Samochody.Add(new Klasy.Osobówka(Index_Max_Local.ToString(), "Lokalne", "Marka 3", "Model 3", "Czarny", new Napęd_Spalinowy(0.8f, 100f, 0.0025f), new Ręczna_Skrzynia_Biegów()));
            Index_Max_Local++;
            Samochody.Add(new Klasy.Osobówka(Index_Max_Local.ToString(), "Lokalne", "Marka 4", "Model 4", "Czarny", new Napęd_Spalinowy(0.8f, 100f, 0.0025f), new Ręczna_Skrzynia_Biegów()));
            Index_Max_Local++;
            Samochody.Add(new Klasy.Osobówka(Index_Max_Local.ToString(), "Lokalne", "Marka 5", "Model 5", "Czarny", new Napęd_Spalinowy(0.8f, 100f, 0.0025f), new Ręczna_Skrzynia_Biegów()));
            Index_Max_Local++;

            // GUI
            InitializeComponent();
            Samochody[Index_Obecny_Samochód].Print_WPF(this);

            //! Pobranie danych z bazy danych i plików
                // WIP Usunięte aż do uzupełniania źródeł danych
            txt_Odczyt();
            DB_SQLite_Odczyt();
            csv_Import();

            // DataGrid v3
            DataGrid_Lista_Samochodów.ItemsSource = Samochody;
            DataGrid_Lista_Samochodów_1.Binding = new Binding("Model");
            DataGrid_Lista_Samochodów_2.Binding = new Binding("Marka");
            DataGrid_Lista_Samochodów_3.Binding = new Binding("Kolor");
            DataGrid_Lista_Samochodów_4.Binding = new Binding("Napęd.Typ");
            DataGrid_Lista_Samochodów_5.Binding = new Binding("Biegi.Typ");
            DataGrid_Lista_Samochodów_6.Binding = new Binding("Typ");
            DataGrid_Lista_Samochodów_7.Binding = new Binding("ID");
            DataGrid_Lista_Samochodów_8.Binding = new Binding("Źródło");

            // Uruchomienie symulacji
            DispatcherTimer Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(0.1f);
            Timer.Tick += Timer_Funkcja;
            Timer.Start();
        }

        /// <summary>
        /// Metoda która wywołuje metodę "Symulacja" z klasy "Samochód".
        /// Jako parametr przekazuje główne okno.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        public void Timer_Funkcja(object sender, EventArgs e)
        {
            Samochody[Index_Obecny_Samochód].Symulacja(this);
        }

        /// <summary>
        /// Metoda odczytująca dane z wszystkie dane z plików .txt znajdujących się w określonej lokalizacji i "tworząca" nowe obiekty o odpowiednich danych. 
        /// </summary>
        private void txt_Odczyt()
        {
            try
            {
                DirectoryInfo Folder = new DirectoryInfo($"{Ścieżka_Do_Źródeł_Danych}");
                foreach (var Plik in Folder.EnumerateFiles("*.txt"))
                {
                    string Zawartość_Pliku_Raw = File.ReadAllText($"{Ścieżka_Do_Źródeł_Danych}/{Plik.Name}");

                    string[] Zawartość_Pliku = Zawartość_Pliku_Raw.Split("\n");
                    string Typ = "";
                    string Model = "";
                    string Marka = "";
                    string Kolor = "";
                    string Napęd = "";
                    string Biegi = "";
                    string Współczynnik_A = "";
                    string Współczynnik_B = "";
                    string Współczynnik_C = "";

                    for (var x = 0; x < Zawartość_Pliku.Length; x++)
                    {
                        if (Zawartość_Pliku[x].StartsWith("Typ: "))
                        {
                            Typ = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Typ = Typ.Substring(0, Typ.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Model: "))
                        {
                            Model = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Model = Model.Substring(0, Model.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Marka: "))
                        {
                            Marka = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Marka = Marka.Substring(0, Marka.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Kolor: "))
                        {
                            Kolor = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Kolor = Kolor.Substring(0, Kolor.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Rodzaj Napędu: "))
                        {
                            Napęd = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Napęd = Napęd.Substring(0, Napęd.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Skrzynia Biegów: "))
                        {
                            Biegi = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Biegi = Biegi.Substring(0, Biegi.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Współczynnik A: "))
                        {
                            Współczynnik_A = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Współczynnik_A = Współczynnik_A.Substring(0, Współczynnik_A.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Współczynnik B: "))
                        {
                            Współczynnik_B = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Współczynnik_B = Współczynnik_B.Substring(0, Współczynnik_B.IndexOf(";"));
                        }

                        if (Zawartość_Pliku[x].StartsWith("Współczynnik C: "))
                        {
                            Współczynnik_C = Zawartość_Pliku[x].Substring(Zawartość_Pliku[x].IndexOf(": ") + 2);
                            Współczynnik_C = Współczynnik_C.Substring(0, Współczynnik_C.IndexOf(";"));
                        }
                    }
                    //Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, ".txt", Plik.Name, float.Parse(Współczynnik_A, Format_Liczb), float.Parse(Współczynnik_B, CultureInfo.InvariantCulture.NumberFormat), float.Parse(Współczynnik_C, CultureInfo.InvariantCulture.NumberFormat));
                    //Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, ".txt", Plik.Name, float.Parse(Współczynnik_A, Format_Liczb), float.Parse(Współczynnik_B, Format_Liczb), float.Parse(Współczynnik_C, Format_Liczb));
                    Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, ".txt", Plik.Name, float.Parse(Współczynnik_A), float.Parse(Współczynnik_B), float.Parse(Współczynnik_C));
                }
            }
            catch (Exception Błąd)
            {
                //Log($"BŁAD: Błąd podczas odczytu z pliku.\nKod błędu: {Błąd.Message}");
                MessageBox.Show($"Błąd podczas odczytywania danych z plików .txt.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Obsługa Bazy Danych
        /// <summary>
        /// Metoda odczytująca dane z bazy danych (SQLite) znajdującej się w określonej lokalizacji i "tworząca" nowe obiekty o odpowiednich danych.
        /// </summary>
        private void DB_SQLite_Odczyt()
        {
            try
            {
                // Wczytanie danych z bazy danych
                using (var connection = new SqliteConnection($"Data Source={Ścieżka_Do_Źródeł_Danych}/Projekt_v1.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                        SELECT Samochody.Id, Samochody.Typ, Samochody.Model, Samochody.Marka, Samochody.Kolor, Samochody.Rodzaj_Napedu, Samochody.Skrzynia_Biegow, Samochody.Wspolczynnik_A, Samochody.Wspolczynnik_B, Samochody.Wspolczynnik_C
                        FROM Samochody
                    ";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Przypisanie zmiennym danych zwracanych przez zapytanie SQL
                            var ID = reader.GetString(0);
                            var Typ = reader.GetString(1);
                            var Model = reader.GetString(2);
                            var Marka = reader.GetString(3);
                            var Kolor = reader.GetString(4);
                            var Napęd = reader.GetString(5);
                            var Biegi = reader.GetString(6);
                            var Współczynnik_A = reader.GetString(7).Replace(".", ",");
                            var Współczynnik_B = reader.GetString(8).Replace(".", ",");
                            var Współczynnik_C = reader.GetString(9).Replace(".", ",");

                            // Utworzenie obiektu odpowiadającemu danym z rekordu
                            Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, "DB", ID, float.Parse(Współczynnik_A), float.Parse(Współczynnik_B), float.Parse(Współczynnik_C));
                        }
                    }
                }
            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nie można nawiązać połączenia z bazą danych.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        /// <summary>
        /// Metoda dodająca dane do bazy danych (SQLite) znajdującej się w określonej lokalizacji.
        /// Dane są dodawane na podstawie wartości pól tekstowych i list rozwijanych w zakładce "Wybór Samochodu".
        /// </summary>
        private void DB_SQLite_Dodanie()
        {
            try
            {
                // Dodanie nowego rekordu do bazy danych
                using (var connection = new SqliteConnection($"Data Source={Ścieżka_Do_Źródeł_Danych}/Projekt_v1.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO Samochody (Typ, Model, Marka, Kolor, Rodzaj_Napedu, Skrzynia_Biegow, Wspolczynnik_A, Wspolczynnik_B, Wspolczynnik_C)
                            VALUES ($Typ, $Model, $Marka, $Kolor, $Naped, $Biegi, $A, $B, $C)
                        ";

                    //command.Parameters.AddWithValue("$ID", );
                    command.Parameters.AddWithValue("$Typ", ComboBox_Typ.Text);
                    command.Parameters.AddWithValue("$Model", TextBox_Model.Text);
                    command.Parameters.AddWithValue("$Marka", TextBox_Marka.Text);
                    command.Parameters.AddWithValue("$Kolor", TextBox_Kolor.Text);
                    command.Parameters.AddWithValue("$Naped", ComboBox_Rodzaj_Napędu.Text);
                    command.Parameters.AddWithValue("$Biegi", ComboBox_Skrzynia_Biegów.Text);
                    command.Parameters.AddWithValue("$A", TextBox_Współczynnik_A.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("$B", TextBox_Współczynnik_B.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("$C", TextBox_Współczynnik_C.Text.Replace(",", "."));

                    MessageBox.Show($"{command.CommandText.ToString()}");

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nie udało się dodać rekordu.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Metoda usuwające dane z bazy danych (SQLite) znajdującej się w określonej lokalizacji.
        /// Dane są usuwane na podstawie zaznaczonego wiersza w siatce, która znajduje się w zakładce "Wybór Samochodu".
        /// </summary>
        private void DB_SQLite_Usuwanie()
        {
            try
            {
                // Dodanie nowego rekordu do bazy danych
                using (var connection = new SqliteConnection($"Data Source={Ścieżka_Do_Źródeł_Danych}/Projekt_v1.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            DELETE FROM Samochody WHERE ID = $Usuwane_ID
                        ";

                    //command.Parameters.AddWithValue("$ID", );
                    command.Parameters.AddWithValue("$Usuwane_ID", Samochody[Index_DataGrid].ID.ToString());

                    MessageBox.Show($"{command.CommandText.ToString()}");

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nie udało się usunąć rekordu.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Metoda modyfikująca dane z bazy danych (SQLite).
        /// Dane są modyfikowane na podstawie zaznaczonego wiersza w siatce oraz wartości pól tekstowych i list rozwijanych, znajdujących się w zakładce "Wybór Samochodu"
        /// </summary>
        private void DB_SQLite_Modyfikowanie()
        {
            try
            {
                // Dodanie nowego rekordu do bazy danych
                using (var connection = new SqliteConnection($"Data Source={Ścieżka_Do_Źródeł_Danych}/Projekt_v1.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            UPDATE Samochody
                            SET
                                Typ = $Nowe_Typ,
                                Marka = $Nowe_Marka,
                                Model = $Nowe_Model,
                                Kolor = $Nowe_Kolor,
                                Rodzaj_Napedu = $Nowe_Naped,
                                Skrzynia_Biegow = $Nowe_Biegi,
                                Wspolczynnik_A = $Nowe_A,
                                Wspolczynnik_B = $Nowe_B,
                                Wspolczynnik_C = $Nowe_C
                            WHERE ID = $ID
                        ";

                    command.Parameters.AddWithValue("$ID", Samochody[Index_DataGrid].ID.ToString());
                    command.Parameters.AddWithValue("$Nowe_Typ", ComboBox_Typ.Text);
                    command.Parameters.AddWithValue("$Nowe_Model", TextBox_Model.Text);
                    command.Parameters.AddWithValue("$Nowe_Marka", TextBox_Marka.Text);
                    command.Parameters.AddWithValue("$Nowe_Kolor", TextBox_Kolor.Text);
                    command.Parameters.AddWithValue("$Nowe_Naped", ComboBox_Rodzaj_Napędu.Text);
                    command.Parameters.AddWithValue("$Nowe_Biegi", ComboBox_Skrzynia_Biegów.Text);
                    command.Parameters.AddWithValue("$Nowe_A", TextBox_Współczynnik_A.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("$Nowe_B", TextBox_Współczynnik_B.Text.Replace(",", "."));
                    command.Parameters.AddWithValue("$Nowe_C", TextBox_Współczynnik_C.Text.Replace(",", "."));

                    MessageBox.Show($"{command.CommandText.ToString()}");

                    command.ExecuteNonQuery();
                }
            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nie udało się zmodyfikować rekordu.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // Obsługa plików .csv
        /// <summary>
        /// Metoda która eksportuje wszystkie samochody (te widoczne w DataGrid w zakładce "Wybór Samochodu") do pliki Dane.csv.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void csv_Eksport(object sender, EventArgs e)
        {
            string Dane = "";
            for (int x = 0; x < Samochody.Count; x++)
            {
                Dane += $"{Samochody[x].Typ};{Samochody[x].Marka};{Samochody[x].Model};{Samochody[x].Kolor};{Samochody[x].Napęd.Typ};{Samochody[x].Biegi.Typ};{Samochody[x].Napęd.Współczynnik_A};{Samochody[x].Napęd.Współczynnik_B};{Samochody[x].Napęd.Współczynnik_C}\n";
            }

            try
            {
                File.WriteAllText($"{Ścieżka_Do_Źródeł_Danych}/Dane.csv", Dane);
                MessageBox.Show("Zapisano dane do pliku Dane.csv.", "Zapisano Dane", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nieudany eksport do pliku Dane.csv\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Metoda która importuje samochody zapisane w pliku Dane.csv
        /// </summary>
        private void csv_Import()
        {
            if (!File.Exists($"{Ścieżka_Do_Źródeł_Danych}/Dane.csv"))
            {
                return;
            }

            try
            {
                foreach (string Linijka in File.ReadLines($"{Ścieżka_Do_Źródeł_Danych}/Dane.csv"))
                {
                    string[] Dane = Linijka.Split(";");
                    //Log(Dane[0]);

                    //Log(Linijka);

                    Tworzenie_Samochodu(Dane[0], Dane[1], Dane[2], Dane[3], Dane[4], Dane[5], ".csv", "Dane.csv", float.Parse(Dane[6]), float.Parse(Dane[7]), float.Parse(Dane[8]));
                }

            }
            catch (Exception Błąd)
            {
                MessageBox.Show($"Nieudane wczytywanie danych z pliku Dane.csv\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Metoda która odpowiada za "tworzenie" nowego samochodu na podstawie zbioru wartości, poprzez sprawdzenie podanych wartości a następnie wywołanie odpowiedniego konstruktora z odpowiednimi wartościami a następnie dodaje utworzony samochód do kolekcji.
        /// </summary>
        /// <param name="Typ_Samochodu">Osobówka | Dostawczak | Tir</param>
        /// <param name="Marka">Marka nowego samochodu.</param>
        /// <param name="Model">Model nowego modelu.</param>
        /// <param name="Kolor">Kolor nowego samochodu.</param>
        /// <param name="Napęd">Spalinowy | Elektryczny</param>
        /// <param name="Biegi">Musi przyjmować wartość "Ręczna"</param>
        /// <param name="Źródło">Zawiera informacje o tym z jakiego rodzaju źródła pochodzi dany samochód. Local: Zmiany tymczasowe, nie zachowywane pomiędzy restartami aplikajci, .txt: Dane pochodzące z plików tektowych, DB: Dane pochodzące z bazy danych SQLite</param>
        /// <param name="Index">Zawiera informacje o tym z jakiego dokładnie źródła pochodzi dany samochód. Nazwa pliku lub ID danego rekordu z bazy danych SQLite.</param>
        /// <param name="Współczynnik_A">Współczynnik A nowego samochodu, odpowiada za przyśpieszanie.</param>
        /// <param name="Współczynnik_B">Współczynnik B nowego samochodu, odpowiada za RPM.</param>
        /// <param name="Współczynnik_C">Współczynnik C nowego samochodu, odpowiada za tempo zużywania paliwa.</param>
        private void Tworzenie_Samochodu(string Typ_Samochodu, string Marka, string Model, string Kolor, string Napęd, string Biegi, string Źródło, string Index, float Współczynnik_A, float Współczynnik_B, float Współczynnik_C)
        {
            // TODO: Zrobić tu walidację, wypisać jakiś błąd w zależności z czego jest błąd (Lokalne, plik, db)
            
            // Sprawdzanie czy dany element już się znajduje w liście
            for (int x = 0; x < Samochody.Count; x++)
            {
                if ((Samochody[x].ID == Index) && (Samochody[x].Źródło == Źródło) && (Samochody[x].Źródło != ".csv"))
                {
                    Log($"Uwaga: Samochód o ID {Index} ({Źródło}) już znajduje się na liście", Wyświetl_Czas: true);
                    return;
                }
            }

            // Tworzenie nowych samochodów
            if (Typ_Samochodu == "Osobówka")
            {
                if (Napęd == "Spalinowy")
                {
                    Samochody.Add(new Klasy.Osobówka(Index, Źródło, Marka, Model, Kolor, new Napęd_Spalinowy(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                }

                if (Napęd == "Elektryczny")
                {
                    Samochody.Add(new Klasy.Osobówka(Index, Źródło, Marka, Model, Kolor, new Napęd_Elektryczny(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                    
                }
            }
            else if (Typ_Samochodu == "Dostawczak")
            {
                if (Napęd == "Spalinowy")
                {
                    Samochody.Add(new Klasy.Dostawczak(Index, Źródło, Marka, Model, Kolor, new Napęd_Spalinowy(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                }

                if (Napęd == "Elektryczny")
                {
                    Samochody.Add(new Klasy.Dostawczak(Index, Źródło, Marka, Model, Kolor, new Napęd_Elektryczny(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                }
            }
            else if (Typ_Samochodu == "Tir")
            {
                if (Napęd == "Spalinowy")
                {
                    Samochody.Add(new Klasy.Tir(Index, Źródło, Marka, Model, Kolor, new Napęd_Spalinowy(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                }

                if (Napęd == "Elektryczny")
                {
                    Samochody.Add(new Klasy.Tir(Index, Źródło, Marka, Model, Kolor, new Napęd_Elektryczny(Współczynnik_A, Współczynnik_B, Współczynnik_C), new Ręczna_Skrzynia_Biegów()));
                }
            }
        }

        /// <summary>
        /// Dopisuje tekst w wielolinijkowym polu tekstowym.
        /// </summary>
        /// <param name="Text">Wymagane. Tekst który zostanie wyświetlony.</param>
        /// <param name="Nowa_Linia">Opcjonalne. Określa czy na końcu tekstu zostanie doane przejście do nowej linii ("\n"). false | true</param>
        /// <param name="Wyczyść"Opcjonalne. Określa czy należy skasować cały obecny tekst. false | true></param>
        /// <param name="Wyświetl_Czas">Opcjonalne. Określa czy ma zostać wyświetlona dokłana godzina kiedy został dopisany tekst. false | true</param>
        public void Log(string Text = "", bool Nowa_Linia = true, bool Wyczyść = false, bool Wyświetl_Czas = false)
        {
            if (Wyczyść)
            {
                TextBlock_Log.Text = "";
            }

            if (Nowa_Linia)
            {
                if (Wyświetl_Czas)
                {
                    TextBlock_Log.Text += $"{DateTime.Now.ToLongTimeString()} {Text}\n";
                }
                else
                {
                    TextBlock_Log.Text += $"{Text}\n";
                }
                
            }
            else
            {
                if (Wyświetl_Czas)
                {
                    TextBlock_Log.Text += $"{DateTime.Now.ToLongTimeString()} {Text}";
                }
                else
                {
                    TextBlock_Log.Text += $"{Text}";
                }
            }
        }
        
        /// <summary>
        /// Metoda która wczytuje wszystkie dane z plików tekstowych i bazy danych, poprzez wywołanie odpowiednich metod.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void Załaduj_Dane(object sender, EventArgs e)
        {
            MessageBox.Show("Wczytano dane z plików txt, csv i bazy danych", "Wczytano Dane", MessageBoxButton.OK, MessageBoxImage.Information);
            txt_Odczyt();
            DB_SQLite_Odczyt();
            csv_Import();
        }
        
        /// <summary>
        /// Metoda która wczytuje wszystkie dane z plików tekstowych i bazy danych, poprzez wywołanie odpowiednich metod.
        /// </summary>
        private void Załaduj_Dane()
        {
            MessageBox.Show("Wczytano dane z plików txt, csv i bazy danych", "Wczytano Dane", MessageBoxButton.OK, MessageBoxImage.Information);
            txt_Odczyt();
            DB_SQLite_Odczyt();
            csv_Import();
        }

        /// <summary>
        /// Metoda która wywołuje metodę "Silnik" dla właściwości "Napęd" z klasy "Samochód".
        /// Metoda przekazuje główne okno programu.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        public void GUI_Silnik(object sender, EventArgs e)
        {
            Samochody[Index_Obecny_Samochód].Napęd.Silnik(this);
        }

        /// <summary>
        /// Metoda wywołująca metodę "Zmiana_Biegu" dla właściwości "Biegi" z klasy "Samochód".
        /// Metoda przekazuje główne okno programu.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Biegi(object sender, RoutedEventArgs e)
        {
            string Button_Text = (sender as Button).Content.ToString();
            Samochody[Index_Obecny_Samochód].Biegi.Zmiana_Biegu(this, System.Convert.ToInt32(Button_Text));
        }

        /// <summary>
        /// Metoda która wywłuje metodę "Log" tak aby usunąć całą istniejącą zawartość wielolinijkowego pola tekstowego.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Log_Wyczyść(object sender, RoutedEventArgs e)
        {
            Log(Nowa_Linia: false, Wyczyść: true);
        }

        /// <summary>
        /// TODO: To chyba jest do usunięcia
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void WIP_Data_Grid_Index(object sender, RoutedEventArgs e)
        {
            var Index = DataGrid_Lista_Samochodów.Items.IndexOf(DataGrid_Lista_Samochodów.CurrentItem);
            Log($"WIP Index: {Index}");
        }
    
        /// <summary>
        /// Metoda zmieniająca ustawia zmienną odpowiedzialną za to czy przycisk przyśpieszanie/hamowanie jest wciśnięty, na wartość true.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Przytrzymanie_Przycisku(object sender, RoutedEventArgs e)
        {
            string Text_Przycisku = (sender as Button).Content.ToString();

            if (Text_Przycisku == "Przyśpiesz")
            {
                Czy_Przycisk_Przyśpieszanie_Jest_Wciśnięty = true;
            }
            else if (Text_Przycisku == "Hamuj")
            {
                Czy_Przycisk_Hamowanie_Jest_Wciśnięty = true;
            }
            else
            {
                Log("Błąd: Zły przycisk.");
            }
        }

        /// <summary>
        /// Metoda zmieniająca ustawia zmienną odpowiedzialną za to czy przycisk przyśpieszanie/hamowanie jest wciśnięty, na wartość true.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Puszczenie_Przycisku(object sender, RoutedEventArgs e)
        {
            string Text_Przycisku = (sender as Button).Content.ToString();

            if (Text_Przycisku == "Przyśpiesz")
            {
                Czy_Przycisk_Przyśpieszanie_Jest_Wciśnięty = false;
            }
            else if (Text_Przycisku == "Hamuj")
            {
                Czy_Przycisk_Hamowanie_Jest_Wciśnięty = false;
            }
            else
            {
                Log("Błąd: Zły przycisk.");
            }
        }

        /// <summary>
        /// Metoda wywołująca metodę "Tankowanie" z klasy "Samochód".
        /// Metoda przekazuje główne okno programu.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Tankowanie(object sender, RoutedEventArgs e)
        {
            Samochody[Index_Obecny_Samochód].Napęd.Tankowanie(this);
        }

        /// <summary>
        /// Metoda wywołująca metodę "Uzupełnienie_Oleju" z klasy "Samochód".
        /// Metoda przekazuje główne okno programu.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void GUI_Uzupełnienie_Oleju(object sender, RoutedEventArgs e)
        {
            Samochody[Index_Obecny_Samochód].Napęd.Uzupełnienie_Oleju(this);
        }
    
        /// <summary>
        /// Metoda odpowiadająca za zmianę indeksu kiedy zostanie wybrany dany wiersz w siatce znajdującej się w zakładce "Wybór Samochodu". Po wybraniu samochodu zmienia wartości w polach tekstowych i listach rozwijanych na wartości odpowiadające zaznaczonemu wierszowi.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void DataGrid_Wybór_Z_Listy(object sender, RoutedEventArgs e)
        {
            var Tabelka = sender as DataGrid;
            
            if (Tabelka == null)
            {
                return;
            }

            var Index = Tabelka.SelectedIndex;

            if (Index == Index_DataGrid) // Jeśli kliknięto na wiersz z obecnie wybranym samochodem
            {
                return;
            }

            Index_DataGrid = Index;

            // Debug
            //TextBox_WIP.Text = $"Zmiana Auta: \n{Index}";
            
            //MessageBox.Show($"Index: {Index}");
            if (Index == -1) // z automatu po usunięciu wiersza, bez tego jest błąd
            {
                DataGrid_Lista_Samochodów.SelectedIndex = 0;
                return;
            }

            TextBox_Nazwa_Pliku_ID.Text = "";
            TextBox_Marka.Text = Samochody[Index].Marka;
            TextBox_Model.Text = Samochody[Index].Model;
            TextBox_Kolor.Text = Samochody[Index].Kolor;
            ComboBox_Rodzaj_Napędu.Text = Samochody[Index].Napęd.Typ;
            ComboBox_Skrzynia_Biegów.Text = Samochody[Index].Biegi.Typ;
            TextBox_Współczynnik_A.Text = Samochody[Index].Napęd.Współczynnik_A.ToString(); // .Replace(",", ".")
            TextBox_Współczynnik_B.Text = Samochody[Index].Napęd.Współczynnik_B.ToString();
            TextBox_Współczynnik_C.Text = Samochody[Index].Napęd.Współczynnik_C.ToString();

            switch (Samochody[Index].Typ)
            {
                case ("Osobówka"):
                    {
                        ComboBox_Typ.SelectedIndex = 0;
                        break;
                    }
                case ("Dostawczak"):
                    {
                        ComboBox_Typ.SelectedIndex = 1;
                        break;
                    }
                case ("Tir"):
                    {
                        ComboBox_Typ.SelectedIndex = 2;
                        break;
                    }
            }

            if (Samochody[Index].Napęd.Typ == "Spalinowy")
            {
                ComboBox_Rodzaj_Napędu.SelectedIndex = 0;
            }
            else if (Samochody[Index].Napęd.Typ == "Elektryczny")
            {
                ComboBox_Rodzaj_Napędu.SelectedIndex = 1;
            }

            if (Samochody[Index].Napęd.Typ == "Ręczna")
            {
                ComboBox_Rodzaj_Napędu.SelectedIndex = 0;
            }
            else if (Samochody[Index].Napęd.Typ == "Automatyczna")
            {
                ComboBox_Rodzaj_Napędu.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Metoda odpowiadająca za zmianę samochodu który jest wykorzystywany w symulacji.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void DataGrid_Wybranie_Samochodu(object sender, EventArgs e)
        {
            // Zatrzymanie symulacji
            Pauza = true;

            // Zmiana wartości zmiennych samochodu z którego się przełączmy
            Samochody[Index_Obecny_Samochód].Napęd.Czy_Silnik_Jest_Włączony = false;
            Samochody[Index_Obecny_Samochód].Napęd.RPM = 0f;
            Samochody[Index_Obecny_Samochód].Napęd.Prędkość = 0f;
            Samochody[Index_Obecny_Samochód].Napęd.Obecna_Ilość_Paliwa = Samochody[Index_Obecny_Samochód].Napęd.Max_Ilość_Paliwa;
            Samochody[Index_Obecny_Samochód].Napęd.Obecna_Ilość_Oleju = Samochody[Index_Obecny_Samochód].Napęd.Max_Ilość_Oleju;
            Samochody[Index_Obecny_Samochód].Biegi.Obecny_Bieg = 0;

            // Zmiana Indexu
            int Index_Poprzedniego_Samochodu = Index_Obecny_Samochód;
            Index_Obecny_Samochód = Index_DataGrid;

            // Aktualizacja GUI
            Checkbox_Silnik.IsChecked = false;
            Samochody[Index_Obecny_Samochód].Print_WPF(this);

            Log("Zmiana Samochodu", Wyświetl_Czas: true);
        }
        
        /// <summary>
        /// Metoda odpowiadająca za usunięcie zaznaczonego samochodu z siatki oraz z pliku lub bazy danych.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void DataGrid_Usunięcie_Samochodu(object sender, EventArgs e)
        {
            if (Samochody.Count == 1)
            {
                MessageBox.Show("Nie można usunąć ostatniego samochodu.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Index_Obecny_Samochód == Index_DataGrid)
            {
                //Samochody.RemoveAt(Index_DataGrid);
                MessageBox.Show("Nie można usunąć samochodu który jest obecnie wykorzystywany w symulacji.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Samochody[Index_DataGrid].Źródło == "Localne")
            {
                Samochody.RemoveAt(Index_DataGrid);
            }
            else if (Samochody[Index_DataGrid].Źródło == ".txt")
            {
                try
                {
                    File.Delete($"{Ścieżka_Do_Źródeł_Danych}/{Samochody[Index_DataGrid].ID}");
                    Samochody.RemoveAt(Index_DataGrid);
                    MessageBox.Show($"Usunięto plik {Samochody[Index_DataGrid].ID}", "Usunięto Plik", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception Błąd)
                {
                    MessageBox.Show($"Nie można usunać pliku {Samochody[Index_DataGrid].ID}.\n{Błąd.Message}");
                }
            }
            else if (Samochody[Index_DataGrid].Źródło == "DB")
            {
                try
                {
                    DB_SQLite_Usuwanie();
                    Samochody.RemoveAt(Index_DataGrid);
                    MessageBox.Show($"Usunięto Rekord o ID: {Samochody[Index_DataGrid].ID}", "Usunięto Rekord", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception Błąd)
                {
                    MessageBox.Show($"Nie można usunać rekordu {Samochody[Index_DataGrid].ID}.\n{Błąd.Message}");
                }
            }
            else if (Samochody[Index_DataGrid].Źródło == ".csv")
            {
                try
                {
                    Samochody.RemoveAt(Index_DataGrid);
                }
                catch (Exception Błąd)
                {
                    MessageBox.Show($"Nie można samochodu z zaznaczonego wiersza.\n{Błąd.Message}");
                }
            }
        }

        /// <summary>
        /// Metoda odpowiadająca za modyfikowanie zaznaczonego samochodu z siatki oraz w pliku lub bazie danych na podstawie danych z pól tekstowych oraz list rozwijanych.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void DataGrid_Modyfikowanie_Samochodu(object sender, EventArgs e)
        {
            // Walidacja
            if (!GUI_Walidacja())
            {
                return;
            }

            // Zebranie danych z pól tekstowych i selectów
            string Typ = ComboBox_Typ.Text;
            string Marka = TextBox_Marka.Text;
            string Model = TextBox_Model.Text;
            string Kolor = TextBox_Kolor.Text;
            string Napęd = ComboBox_Rodzaj_Napędu.Text;
            string Biegi = ComboBox_Skrzynia_Biegów.Text;
            float Współczynnik_A = float.Parse(TextBox_Współczynnik_A.Text);
            float Współczynnik_B = float.Parse(TextBox_Współczynnik_B.Text);
            float Współczynnik_C = float.Parse(TextBox_Współczynnik_C.Text);

            // Sprawdza czy chcemy edytować samochód który znajduje się teraz w symulacji.
            if (Index_Obecny_Samochód != Index_DataGrid)
            {
                string ID = Samochody[Index_DataGrid].ID;
                string Źródło = Samochody[Index_DataGrid].Źródło;

                if (Samochody[Index_DataGrid].Źródło == "Lokalne")
                {
                    // Sprawdzanie czy zmieniany jest typ samochodu, napędu lub skrzyni biegów. Wtedy trzeba utworzyć obiekt od nowa.
                    if ((Samochody[Index_DataGrid].Typ != Typ) || (Samochody[Index_DataGrid].Napęd.Typ != Napęd) || (Samochody[Index_DataGrid].Biegi.Typ != Biegi))
                    {
                        Samochody.RemoveAt(Index_DataGrid);
                        Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, Źródło, ID, Współczynnik_A, Współczynnik_B, Współczynnik_C);
                        DataGrid_Lista_Samochodów.Items.Refresh();
                    }
                    else
                    {
                        Samochody[Index_DataGrid].Marka = Marka;
                        Samochody[Index_DataGrid].Model = Model;
                        Samochody[Index_DataGrid].Kolor = Kolor;
                        Samochody[Index_DataGrid].Napęd.Współczynnik_A = Współczynnik_A;
                        Samochody[Index_DataGrid].Napęd.Współczynnik_B = Współczynnik_B;
                        Samochody[Index_DataGrid].Napęd.Współczynnik_C = Współczynnik_C;
                        DataGrid_Lista_Samochodów.Items.Refresh();
                    }
                }

                else if (Samochody[Index_DataGrid].Źródło == ".txt")
                {
                    try // Tu nie ma potrzeby sprawdzania czy plik istnieje, bo jak nawet nie istnieje to zostanie utworzony
                    {
                        string Dane = "";

                        Dane += $"Typ: {Typ};\n";
                        Dane += $"Model: {Model};\n";
                        Dane += $"Marka: {Marka};\n";
                        Dane += $"Kolor: {Kolor};\n";
                        Dane += $"Rodzaj Napędu: {Napęd};\n";
                        Dane += $"Skrzynia Biegów: {Biegi};\n";
                        Dane += $"Współczynnik A: {Współczynnik_A};\n";
                        Dane += $"Współczynnik B: {Współczynnik_B};\n";
                        Dane += $"Współczynnik C: {Współczynnik_C};";

                        MessageBox.Show($"{Dane}", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Information);

                        File.WriteAllText($"{Ścieżka_Do_Źródeł_Danych}/{Samochody[Index_DataGrid].ID}", Dane); // To musi być tu, bo inaczej popsuje się Index_DataGrid
                        DataGrid_Lista_Samochodów.Items.Refresh();

                        // Sprawdzanie czy zmieniany jest typ samochodu, napędu lub skrzyni biegów. Wtedy trzeba utworzyć obiekt od nowa.
                        if ((Samochody[Index_DataGrid].Typ != Typ) || (Samochody[Index_DataGrid].Napęd.Typ != Napęd) || (Samochody[Index_DataGrid].Biegi.Typ != Biegi))
                        {
                            Samochody.RemoveAt(Index_DataGrid);
                            Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, Źródło, ID, Współczynnik_A, Współczynnik_B, Współczynnik_C);
                        }
                        else
                        {
                            Samochody[Index_DataGrid].Marka = Marka;
                            Samochody[Index_DataGrid].Model = Model;
                            Samochody[Index_DataGrid].Kolor = Kolor;
                            Samochody[Index_DataGrid].Napęd.Współczynnik_A = Współczynnik_A;
                            Samochody[Index_DataGrid].Napęd.Współczynnik_B = Współczynnik_B;
                            Samochody[Index_DataGrid].Napęd.Współczynnik_C = Współczynnik_C;
                        }
                        
                    }
                    catch (Exception Błąd)
                    {
                        MessageBox.Show($"Błąd podczas edycji danych w pliku .txt.\n{Błąd.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                else if (Samochody[Index_DataGrid].Źródło == "DB")
                {
                    DB_SQLite_Modyfikowanie();
                    Samochody.RemoveAt(Index_DataGrid);
                    DB_SQLite_Odczyt();
                    DataGrid_Lista_Samochodów.Items.Refresh();
                }

                else if (Samochody[Index_DataGrid].Źródło == ".csv")
                {
                    MessageBox.Show("Nie można edytować samochodów z plików .csv", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Nie można edytować samochodu który jest obecnie wykorzystywany w symulacji.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Metoda odpowiadajaca za dodawanie nowego samochodu na podstawie danych z pól tekstowych i list rozwijanych oraz utworzenie nowego pliku/rekordu.
        /// </summary>
        /// <param name="sender">Obiekt (element GUI) który wywołał zdarzenie.</param>
        /// <param name="e">Informacje o zdarzeniu</param>
        private void DataGrid_Dodaj_Nowy(object sender, RoutedEventArgs e)
        {
            bool Sprawdzanie_Czy_Istnieje_Taki_Plik(string ID, string Źródło)
            {
                if (Źródło == ".txt")
                {
                    ID = ID + ".txt";
                }
                
                for (int x = 0; x < Samochody.Count; x++)
                {
                    if ((Samochody[x].ID == ID) && (Samochody[x].Źródło == Źródło))
                    {
                        MessageBox.Show("Istnieje już plik/rekord o takiej nazwie/ID.", "Błąd", MessageBoxButton.OK);
                        return true;
                    }
                }

                return false;
            }

            // Walidacja
            if (!GUI_Walidacja(true))
            {
                return;
            }

            string Tryb = (sender as Button).Content.ToString();

            string Nazwa_Pliku = TextBox_Nazwa_Pliku_ID.Text;
            string Typ = ComboBox_Typ.Text;
            string Marka = TextBox_Marka.Text;
            string Model = TextBox_Model.Text;
            string Kolor = TextBox_Kolor.Text;
            string Napęd = ComboBox_Rodzaj_Napędu.Text;
            string Biegi = ComboBox_Skrzynia_Biegów.Text;
            float Współczynnik_A = float.Parse(TextBox_Współczynnik_A.Text);
            float Współczynnik_B = float.Parse(TextBox_Współczynnik_B.Text);
            float Współczynnik_C = float.Parse(TextBox_Współczynnik_C.Text);

            switch (Tryb)
            {
                case "Zapisz Jako (Local)":
                    Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, ".txt", Index_Max_Local.ToString(), Współczynnik_A, Współczynnik_B, Współczynnik_C);
                    Index_Max_Local++;
                    break;
                case "Zapisz Jako (Plik)":
                    if (!Sprawdzanie_Czy_Istnieje_Taki_Plik(Nazwa_Pliku, ".txt"))
                    {
                        Tworzenie_Samochodu(Typ, Marka, Model, Kolor, Napęd, Biegi, ".txt", $"{Nazwa_Pliku}.txt", Współczynnik_A, Współczynnik_B, Współczynnik_C);
                        string Dane = "";

                        Dane += $"Typ: {Typ};\n";
                        Dane += $"Model: {Model};\n";
                        Dane += $"Marka: {Marka};\n";
                        Dane += $"Kolor: {Kolor};\n";
                        Dane += $"Rodzaj Napędu: {Napęd};\n";
                        Dane += $"Skrzynia Biegów: {Biegi};\n";
                        Dane += $"Współczynnik A: {Współczynnik_A};\n";
                        Dane += $"Współczynnik B: {Współczynnik_B};\n";
                        Dane += $"Współczynnik C: {Współczynnik_C};";

                        try
                        {
                            File.WriteAllText($"{Ścieżka_Do_Źródeł_Danych}/{Nazwa_Pliku}.txt", Dane);
                        }
                        catch (Exception Błąd)
                        {
                            MessageBox.Show($"Nie można było utworzyć pliku\n{Błąd.Message}", "Błąd");
                        }
                    }
                    break;
                case "Zapisz Jako (DB)":
                    DB_SQLite_Dodanie();
                    DB_SQLite_Odczyt();
                    break;
            }
            
            DataGrid_Lista_Samochodów.Items.Refresh();
        }

        /// <summary>
        /// Metoda sprawdzająca czy dane wprowadzone przez użytkownika są poprawne
        /// </summary>
        /// <param name="Czy_Sprawdzać_Nazwę_Pliku">Określa czy należy sprawdzić również nazwę pliku.</param>
        /// <returns>Zwraca true jeśli dane są poprawne, false jeśli są błędne</returns>
        private bool GUI_Walidacja(bool Czy_Sprawdzać_Nazwę_Pliku = false)
        {
            // Sprawdzenie 
            string Nazwa_Pliku = TextBox_Nazwa_Pliku_ID.Text;
            string Typ = ComboBox_Typ.Text;
            string Marka = TextBox_Marka.Text;
            string Model = TextBox_Model.Text;
            string Kolor = TextBox_Kolor.Text;
            string Napęd = ComboBox_Rodzaj_Napędu.Text;
            string Biegi = ComboBox_Skrzynia_Biegów.Text;
            string Komunikat = "";
            
            // Sprawdzanie poszczególnych danych
            if (Czy_Sprawdzać_Nazwę_Pliku)
            {
                if (Nazwa_Pliku.Contains('.') || Nazwa_Pliku.Contains('/'))
                {
                    Komunikat += "Podaj poprawną nazwę pliku.\n";
                }
            }

            if (!((Typ == "Osobówka") || (Typ == "Dostawczak") || (Typ == "Tir")))
            {
                Komunikat += "Podaj poprawny typ samochodu.\n";
            }

            if (Marka == "")
            {
                Komunikat += "Podaj markę.\n";
            }

            if (Model == "")
            {
                Komunikat += "Podaj model.\n";
            }

            if (Kolor == "")
            {
                Komunikat += "Podaj kolor.\n";
            }

            if (!((Napęd == "Spalinowy") || (Napęd == "Elektryczny")))
            {
                Komunikat += "Podaj poprawny rodzaj napędu.\n";
            }

            if (Biegi != "Ręczna")
            {
                Komunikat += "Podaj poprawny rodzaj skrzyni biegów.\n";
            }

            if (!float.TryParse(TextBox_Współczynnik_A.Text, out float result_A))
            {
                Komunikat += "Podaj poprawny współczynnik A.\n";
            }

            if (!float.TryParse(TextBox_Współczynnik_B.Text, out float result_B))
            {
                Komunikat += "Podaj poprawny współczynnik B.\n";
            }

            if (!float.TryParse(TextBox_Współczynnik_C.Text, out float result_C))
            {
                Komunikat += "Podaj poprawny współczynnik C.\n";
            }

            // Sprawdzenie wyniku
            if (Komunikat == "")
            {
                return true;
            }
            else
            {
                MessageBox.Show($"Podaj prawidłowe dane!\n\n{Komunikat}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}