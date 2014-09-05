using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmEvolucaoEmergencia2 : ViewModelBase
    {
        public vmEvolucaoEmergencia2()
        {
            List<EvolucaoEmergencia2> nova = new List<EvolucaoEmergencia2>();

            nova.Add(new EvolucaoEmergencia2
            {
                Nome = "Isak Silva Silva",
                Leito = "V02"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "V03",
                Nome = "Nathan Lima"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "V04",
                Nome = "Cecilia Maria"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L05",
                Nome = "Otavia de Borba"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L06",
                Nome = "Darcy Antonio"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L07",
                Nome = "Darcy José"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "L08",
                Nome = "Mariza Mariza"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A09",
                Nome = "Adela Pires"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A10",
                Nome = "Esther Hulda"
            });
            nova.Add(new EvolucaoEmergencia2
            {
                Leito = "A11",
                Nome = "Beatriz"
            });

            evolucoes = nova;
        }

        private bool _isVermelho { get; set; }
        public List<EvolucaoEmergencia2> evolucoes { get; set; }

        public bool isVermelho
        {
            get
            {
                return _isVermelho;
            }
            set
            {
                _isVermelho = value;
            }
        }
    }

    public class EvolucaoEmergencia2
    {
        public string Nome { get; set; }
        public string Leito { get; set; }
    }
}