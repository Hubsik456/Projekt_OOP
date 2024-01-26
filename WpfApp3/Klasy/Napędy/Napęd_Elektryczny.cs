using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3.Klasy
{
    internal class Napęd_Elektryczny : Rodzaj_Napędu
    {
        public Napęd_Elektryczny(float _Współczynnik_A, float _Współczynnik_B, float _Współczynnik_C) : base(_Współczynnik_A, _Współczynnik_B, _Współczynnik_C)
        {
            Typ = "Elektryczny";
        }

        public override void Print_WPF(MainWindow GUI)
        {
            base.Print_WPF(GUI);
            GUI.Label_Paliwo.Content = "Ilość Czasu Jazdy: ";
        }
    }
}
