using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Klasy;

namespace WpfApp3.Interfejsy
{
    internal interface I_Samochód
    {
        public string Marka { set; get; }
        public string Model { set; get; }
        public string Kolor { set; get; }
        public Rodzaj_Napędu Napęd { set; get; }
        public Skrzynia_Biegów Biegi { set; get; }
    }
}
