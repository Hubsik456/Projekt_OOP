using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp3.Klasy;

namespace WpfApp3.Interfejsy
{
    internal interface I_Napęd
    {
        public string Typ { set; get; }
        public float Prędkość { set; get; }
        public float Max_Ilość_Paliwa { set; get; }
        public float Obecna_Ilość_Paliwa { set; get; }
        public float Max_Ilość_Oleju { set; get; }
        public float Obecna_Ilość_Oleju { set; get; }
        public float RPM { set; get; }
    }
}
