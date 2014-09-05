using System.Collections.ObjectModel;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.ObjectWrappers.PEP.ProcessosEnfermagem.AdmissaoAssistencialDeEndoscopia;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaEndoscopia;
using HMV.ProcessosEnfermagem.ViewModel;
using StructureMap;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaEndoscopia
{
    public class vmSumarioAvaliacaoMedicaMedicamentosEndoscopia : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaMedicamentosEndoscopia(wrpSumarioAvaliacaoMedicaEndoscopia pSumarioAvaliacaoMedicaEndoscopia)
        {
            this._sumarioavaliacaomedicaendoscopia = pSumarioAvaliacaoMedicaEndoscopia;
            this._paciente = pSumarioAvaliacaoMedicaEndoscopia.Paciente;
            this._usuario = pSumarioAvaliacaoMedicaEndoscopia.Usuario;
            //this._evento = pEvento;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaEndoscopia _sumarioavaliacaomedicaendoscopia;
        private ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia> _collectionitemendoscopia;
        private vmMedicamentosEmUsoEvento _vmmedicamentosemusoevento;
        //private wrpEventoSumarioAvaliacaoMedicaEndoscopia _evento;
        private wrpUsuarios _usuario;
        private wrpPaciente _paciente;
        #endregion

        #region ----- Propriedades Públicas -----
        public ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia> CollectionItemEndoscopia
        {
            get
            {
                if (this._collectionitemendoscopia.IsNull())
                {
                    IRepositorioDeItemEndoscopia rep = ObjectFactory.GetInstance<IRepositorioDeItemEndoscopia>();
                    rep.FiltraAtivos();
                    rep.FiltraMedicamentoRisco();
                    var lista = rep.List();
                    this._collectionitemendoscopia = new ObservableCollection<vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia>();
                    lista.Each(x =>
                    {
                        this._collectionitemendoscopia.Add(new vmSumarioAvaliacaoMedicaEndoscopia.ItensEndoscopia
                        {
                            ItemEndoscopia = x,
                            Nega = this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Nega :
                                   SimNao.Nao : SimNao.Nao,
                            Observacao = this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.HasItems() ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).Count() > 0 ?
                                   this._sumarioavaliacaomedicaendoscopia.ItemEndoscopia.Where(y => y.ItemEndoscopia.ID == x.ID).FirstOrDefault().Observacao :
                                   string.Empty : string.Empty
                        });
                    });
                }
                return this._collectionitemendoscopia;
            }
        }

        public vmMedicamentosEmUsoEvento vmMedicamentosEmUsoEvento
        {
            get
            {
                if (this._vmmedicamentosemusoevento.IsNull())
                {
                    wrpMedicamentoEmUsoEventoCollection _MedicamentosCollection = null;

                    IRepositorioDeEventoMedicamentosEmUso repp = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                    repp.OndeChaveIgual(this._sumarioavaliacaomedicaendoscopia.ID);
                    repp.OndeTipoEventoIgual(Core.Domain.Enum.TipoEvento.SumarioAvaliacaoMedicaEndoscopia);
                    var ret = repp.List();
                    if (ret.IsNotNull())
                        _MedicamentosCollection = new wrpMedicamentoEmUsoEventoCollection(ret);
                   
                    this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, this._paciente, this._usuario, TipoEvento.SumarioAvaliacaoMedicaEndoscopia, _MedicamentosCollection
                    , this._sumarioavaliacaomedicaendoscopia.ID, true, false, true, true,false,false,false,false,false, _sumarioavaliacaomedicaendoscopia.Atendimento);
                    
                   // this._vmmedicamentosemusoevento = new vmMedicamentosEmUsoEvento(false, this._paciente, this._usuario, new wrpEvento(this._evento.DomainObject), this._sumarioavaliacaomedicaendoscopia.ID, pMostraUltAdministracao: true,
                   //pMostraDataInicio: false, pMostraVia: false, pMostraFrequencia: false, pMostraComboVia: true, pMostraComboFrequencia: true);
                }
                return this._vmmedicamentosemusoevento;
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----

        #endregion        
    }
}
