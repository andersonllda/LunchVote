using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers;
using System.Windows.Controls;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Framework.ViewModelBaseClasses;
using System.Collections.Generic;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmMedicos : ViewModelBase
    {
        #region Contrutor
        public vmMedicos(wrpSumarioAlta pSumarioAlta)
        {
            this.AddPrestadorCommand = new AddPrestadorCommand(this);
            this.RemovePrestadorCommand = new RemovePrestadorCommand(this);
            if (pSumarioAlta.DomainObject.Prestadores == null)
                pSumarioAlta.Prestadores = new wrpPrestadorCollection(new List<Prestador>());

            this.Prestadores = pSumarioAlta.Prestadores;            
        }
        #endregion

        #region Propriedades Publicas
        public wrpPrestadorCollection Prestadores { get; set; }

        public wrpPrestador PrestadorSelecionado
        {
            get
            {
                return _prestadorselecionado;
            }
            set
            {
                _prestadorselecionado = value;                
                this.OnPropertyChanged("PrestadorSelecionado");
            }
        }
        #endregion

        #region Commands
        public ICommand AddPrestadorCommand { get; set; }
        public ICommand RemovePrestadorCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        wrpPrestador _prestadorselecionado { get; set; }
        #endregion
    }
}
