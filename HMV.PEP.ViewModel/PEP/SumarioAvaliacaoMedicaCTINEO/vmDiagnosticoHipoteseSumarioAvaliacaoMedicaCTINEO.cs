using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaCTINEO;
using HMV.PEP.Interfaces;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO
{
    public class vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO : ViewModelBase
    {
        #region ----- Construtor -----
        public vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO(wrpSumarioAvaliacaoMedicaCTINEO pSumarioAvaliacaoMedicaCTINEO)
        {
            this._sumarioavaliacaomedicactineo = pSumarioAvaliacaoMedicaCTINEO;

            if (this._sumarioavaliacaomedicactineo.Atendimento.Cid.IsNotNull())
                this._cidid = this._sumarioavaliacaomedicactineo.Atendimento.Cid.Id;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaCTINEO _sumarioavaliacaomedicactineo;
        private wrpSumarioAvaliacaoMedicaCTINEODiagnostico _sumarioavaliacaomedicactineodiagnostico;
        private wrpSumarioAvaliacaoMedicaCTINEOHipotese _sumarioavaliacaomedicahipotese;
        private string _cidid;
        #endregion

        #region ----- Propriedades Públicas -----      
        public Atendimento Atendimento
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.Atendimento.DomainObject;
            }
        }
        public Paciente Paciente
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.Paciente.DomainObject;
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
                    this._sumarioavaliacaomedicactineo.Atendimento.Cid = new wrpCid(cid);
                    this._cidid = value;
                }
                else
                {                    
                    this._sumarioavaliacaomedicactineo.Atendimento.DomainObject.Cid = null;
                    this._sumarioavaliacaomedicactineo.Atendimento.Cid = null;
                    this._cidid = string.Empty;
                }

                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.CID);
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.CidId);
            }
        }

        public wrpCid CID
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.Atendimento.Cid;
            }
            set
            {
                this._sumarioavaliacaomedicactineo.Atendimento.DomainObject.Cid = value.DomainObject;
                this._sumarioavaliacaomedicactineo.Atendimento.Cid = value;
                if (this._sumarioavaliacaomedicactineo.Atendimento.Cid.IsNotNull())
                    this._cidid = this._sumarioavaliacaomedicactineo.Atendimento.Cid.Id;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.CidId);
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.CID);
            }
        }
        public wrpSumarioAvaliacaoMedicaCTINEODiagnosticoCollection SumarioAvaliacaoMedicaCTINEODiagnosticoCollection
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEODiagnostico;
            }
        }

        public wrpSumarioAvaliacaoMedicaCTINEODiagnostico SumarioAvaliacaoMedicaCTINEODiagnostico
        {
            get
            {
                return this._sumarioavaliacaomedicactineodiagnostico;
            }
            set
            {
                this._sumarioavaliacaomedicactineodiagnostico = value;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.SumarioAvaliacaoMedicaCTINEODiagnostico);
            }
        }

        public wrpSumarioAvaliacaoMedicaCTINEOHipoteseCollection SumarioAvaliacaoMedicaCTINEOHipoteseCollection
        {
            get
            {
                return this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOHipotese;
            }
        }

        public wrpSumarioAvaliacaoMedicaCTINEOHipotese SumarioAvaliacaoMedicaCTINEOHipotese
        {
            get
            {
                return this._sumarioavaliacaomedicahipotese;
            }
            set
            {
                this._sumarioavaliacaomedicahipotese = value;
                this.OnPropertyChanged<vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO>(x => x.SumarioAvaliacaoMedicaCTINEOHipotese);
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
                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEODiagnostico.Count(x => x.CID10.Id == pCid.Id) == 0)
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEODiagnostico.Add(new wrpSumarioAvaliacaoMedicaCTINEODiagnostico(this._sumarioavaliacaomedicactineo) { CID10 = new wrpCid(pCid), Complemento = pComplemento });

                ICidService rep = ObjectFactory.GetInstance<ICidService>();
                rep.verificaSeOCIDJaEstaNaListaDeProblemas(pCid, this._sumarioavaliacaomedicactineo.Atendimento.DomainObject, this._sumarioavaliacaomedicactineo.Usuario.DomainObject);

                return true;
            }
            else
                DXMessageBox.Show("Informe o CID!", "Atenção.", MessageBoxButton.OK, MessageBoxImage.Warning);

            return false;
        }

        public void RemoveDiagnostico()
        {
            if (this._sumarioavaliacaomedicactineodiagnostico.IsNotNull())
            {
                if (DXMessageBox.Show("Deseja realmente Excluir o CID do Diagnóstico?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEODiagnostico.Remove(this._sumarioavaliacaomedicactineodiagnostico);

                    wrpProblemasPaciente _problema = this._sumarioavaliacaomedicactineo.Atendimento.Paciente.ProblemasPaciente.Where(x => x.CID.IsNotNull() && x.CID.Id.Equals(this._sumarioavaliacaomedicactineodiagnostico.CID10.Id) && x.Atendimento.IsNotNull() && x.Atendimento.ID.Equals(this._sumarioavaliacaomedicactineo.Atendimento.ID)).FirstOrDefault();
                    if (_problema != null)
                        _problema.Status = StatusAlergiaProblema.Excluído;

                }
            }
        }

        public bool AddHipotese(string pHipotese)
        {
            if (pHipotese.IsNotEmptyOrWhiteSpace())
            {
                if (this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOHipotese.Count(x => x.Hipotese == pHipotese) == 0)
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOHipotese.Add(new wrpSumarioAvaliacaoMedicaCTINEOHipotese(this._sumarioavaliacaomedicactineo) { Hipotese = pHipotese });


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
                    this._sumarioavaliacaomedicactineo.SumarioAvaliacaoMedicaCTINEOHipotese.Remove(this._sumarioavaliacaomedicahipotese);
        }   
        #endregion

        #region ----- Commands -----

        #endregion
    }
}
