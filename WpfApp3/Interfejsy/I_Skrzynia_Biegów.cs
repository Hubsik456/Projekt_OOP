using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Klasy;

namespace WpfApp3.Interfejsy
{
    internal interface I_Skrzynia_Biegów
    {
        public string Typ { set; get; }
        public int Obecny_Bieg { set; get; }
    }
}
