using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.Interfaces;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO
{
    public class vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO : ViewModelBase
    {
        #region ----- Construtor -----
        public vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO(wrpSumarioAvaliacaoMedicaCO pSumarioAvaliacaoMedicaCO)
        {
            this._sumarioavaliacaomedicaco = pSumarioAvaliacaoMedicaCO;

            if (this._sumarioavaliacaomedicaco.Atendimento.Cid.IsNotNull())
                this._cidid = this._sumarioavaliacaomedicaco.Atendimento.Cid.Id;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaCO _sumarioavaliacaomedicaco;
        private wrpSumarioAvaliacaoMedicaCODiagnostico _sumarioavaliacaomedicacodiagnostico;
        private wrpSumarioAvaliacaoMedicaCOHipotese _sumarioavaliacaomedicahipotese;
        private string _cidid;
        #endregion

        #region ----- Propriedades Públicas -----
        //public wrpSumarioAvaliacaoMedicaCO SumarioAvaliacaoMedicaCO
        //{
        //    get
        //    {
        //        return this._sumarioavaliacaomedicaco;
        //    }
        //}
        public Atendimento Atendimento
        {
            get
            {
                return this._sumarioavaliacaomedicaco.Atendimento.DomainObject;
            }
        }
        public Paciente Paciente
        {
            get
            {
                return this._sumarioavaliacaomedicaco.Paciente.DomainObject;
            }
        }

        public string CidId
        {
            get { return _cidid; }
            set
            {
                IRepositorioDeCid serv = ObjectFactory.GetInstance<IRepositorioDeCid>();
                Cid cid = serv.OndeCid10Igual(value).Single();

                if (cid.IsNotNull())
                {
                    this._sumarioavaliacaomedicaco.Atendimento.Cid = new wrpCid(cid);
                    this._cidid = value;
                }
                else
                {                    
                    this._sumarioavaliacaomedicaco.Atendimento.DomainObject.Cid = null;
                    this._sumarioavaliacaomedicaco.Atendimento.Cid = null;
                    this._cidid = string.Empty;
                }

                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.CID);
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.CidId);
            }
        }

        public wrpCid CID
        {
            get
            {
                return this._sumarioavaliacaomedicaco.Atendimento.Cid;
            }
            set
            {
                this._sumarioavaliacaomedicaco.Atendimento.DomainObject.Cid = value.DomainObject;
                this._sumarioavaliacaomedicaco.Atendimento.Cid = value;
                if (this._sumarioavaliacaomedicaco.Atendimento.Cid.IsNotNull())
                    this._cidid = this._sumarioavaliacaomedicaco.Atendimento.Cid.Id;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.CidId);
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.CID);
            }
        }
        public wrpSumarioAvaliacaoMedicaCODiagnosticoCollection SumarioAvaliacaoMedicaCODiagnosticoCollection
        {
            get
            {
                return this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCODiagnostico;
            }
        }

        public wrpSumarioAvaliacaoMedicaCODiagnostico SumarioAvaliacaoMedicaCODiagnostico
        {
            get
            {
                return this._sumarioavaliacaomedicacodiagnostico;
            }
            set
            {
                this._sumarioavaliacaomedicacodiagnostico = value;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.SumarioAvaliacaoMedicaCODiagnostico);
            }
        }

        public wrpSumarioAvaliacaoMedicaCOHipoteseCollection SumarioAvaliacaoMedicaCOHipoteseCollection
        {
            get
            {
                return this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOHipotese;
            }
        }

        public wrpSumarioAvaliacaoMedicaCOHipotese SumarioAvaliacaoMedicaCOHipotese
        {
            get
            {
                return this._sumarioavaliacaomedicahipotese;
            }
            set
            {
                this._sumarioavaliacaomedicahipotese = value;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO>(x => x.SumarioAvaliacaoMedicaCOHipotese);
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public bool AddDiagnostico(Cid pCid, string pComplemento)
        {
            if (pCid.IsNotNull())
            {
                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCODiagnostico.Count(x => x.CID10.Id == pCid.Id) == 0)
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCODiagnostico.Add(new wrpSumarioAvaliacaoMedicaCODiagnostico(this._sumarioavaliacaomedicaco) { CID10 = new wrpCid(pCid), Complemento = pComplemento });

                ICidService rep = ObjectFactory.GetInstance<ICidService>();
                rep.verificaSeOCIDJaEstaNaListaDeProblemas(pCid, this._sumarioavaliacaomedicaco.Atendimento.DomainObject, this._sumarioavaliacaomedicaco.Usuario.DomainObject);

                return true;
            }
            else
                DXMessageBox.Show("Informe o CID!", "Atenção.", MessageBoxButton.OK, MessageBoxImage.Warning);

            return false;
        }

        public void RemoveDiagnostico()
        {
            if (this._sumarioavaliacaomedicacodiagnostico.IsNotNull())
            {
                if (DXMessageBox.Show("Deseja realmente Excluir o CID do Diagnóstico?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCODiagnostico.Remove(this._sumarioavaliacaomedicacodiagnostico);

                    wrpProblemasPaciente _problema = this._sumarioavaliacaomedicaco.Atendimento.Paciente.ProblemasPaciente.Where(x => x.CID.IsNotNull() && x.CID.Id.Equals(this._sumarioavaliacaomedicacodiagnostico.CID10.Id) && x.Atendimento.IsNotNull() && x.Atendimento.ID.Equals(this._sumarioavaliacaomedicaco.Atendimento.ID)).FirstOrDefault();
                    if (_problema != null)
                        _problema.Status = StatusAlergiaProblema.Excluído;
                }
            }
        }

        public bool AddHipotese(string pHipotese)
        {
            if (pHipotese.IsNotEmptyOrWhiteSpace())
            {
                if (this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOHipotese.Count(x => x.Hipotese == pHipotese) == 0)
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOHipotese.Add(new wrpSumarioAvaliacaoMedicaCOHipotese(this._sumarioavaliacaomedicaco) { Hipotese = pHipotese });
                return true;
            }
            else
                DXMessageBox.Show("Informe a Hipótese!", "Atenção.", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        public void RemoveHipotese()
        {
            if (this._sumarioavaliacaomedicahipotese.IsNotNull())
                if (DXMessageBox.Show("Deseja realmente Excluir a hipótese diagnostica?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    this._sumarioavaliacaomedicaco.SumarioAvaliacaoMedicaCOHipotese.Remove(this._sumarioavaliacaomedicahipotese);
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
