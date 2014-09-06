using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.DTO;

namespace HMV.PEP.ViewModel.PEP
{
    public class VMTaxaOcupacao : ViewModelBase
    {
        #region Construtor
        public VMTaxaOcupacao(TaxaOcupacaoDTO Taxa, bool pMostraTitulo = false)
        {
            MostraTitulo = pMostraTitulo;
            Valor = Taxa.Ocupacao;
            _setor = Taxa.Descricao;
        }
        #endregion

        #region Propriedades Privadas
        private bool _pmostratitulo;
        private string _setor;        
        private double _valor;

        #endregion

        public bool MostraTitulo
        {
            get
            {
                return _pmostratitulo;
            }
            
            private set
            {
                this._pmostratitulo = value;
                this.OnPropertyChanged("MostraTitulo");
            }
        }


        public string Setor
        {
            get
            {
                return _setor;
            }          
        }

        public string Percentual
        {
            get
            {
                return Math.Round(Valor, 2).ToString() + " %";
            }
          
        }

        public int Cor
        {
            get
            {
                if (Valor <= 80)
                    return 3;
                else if (Valor > 80 && Valor <= 90)
                    return 2;
                else
                    return 1;
            }
        }

        public double Valor
        {
            get
            {

                return _valor;
            }
            
            set
            {
                this._valor = value;
                this.OnPropertyChanged("Valor");
                this.OnPropertyChanged("Cor");
                this.OnPropertyChanged("Percentual");
            }
        }

    }
}
