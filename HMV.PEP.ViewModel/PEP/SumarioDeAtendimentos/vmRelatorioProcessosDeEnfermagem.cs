using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem;
using StructureMap;
using System.Collections.Generic;
using HMV.Core.Domain.Enum;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Model;
using System.Linq;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Repository.PEP.CentroObstetrico;
using HMV.Core.Wrappers.CollectionWrappers.PEP.CentroObstetrico;
using HMV.Core.Domain.Repository.PEP.ProcessoDeEnfermagem.AdmissaoAssistencialEndoscopia;
using HMV.Core.Wrappers.CollectionWrappers.PEP.AdmissaoAssistencialCTI;
using HMV.Core.Wrappers.CollectionWrappers.PEP.ProcessosEnfermagem.AdmissaoAssistencialDeEndoscopia;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using System.Windows.Media.Imaging;
using System;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAtendimentos
{
    public class vmRelatorioProcessosDeEnfermagem : ViewModelBase
    {
        #region ----- Construtor -----
        public vmRelatorioProcessosDeEnfermagem(wrpAtendimento pAtendimento)
        {
            this._atendimento = pAtendimento;

            IRepositorioDeAdmissaoAssistencial rep = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencial>();
            var ret = rep.OndeIdAtendimentoIgual(this._atendimento.ID).List().Where(x => x.Status == Status.Ativo && x.DataConclusao.HasValue).OrderByDescending(x => x.DataConclusao).ToList();
            if (ret.HasItems())
            {
                this._listaadmissaoassistencial = new wrpAdmissaoAssistencialCollection(ret);
                this._podeabrir = true;
            }

            IRepositorioDeAdmissaoAssistencialCO repCO = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCO>();
            var retCO = repCO.OndeCodigoAtendimentoIgual(_atendimento.DomainObject).List().Where(x => !x.DataExclusao.HasValue && x.DataEncerramento.HasValue).OrderByDescending(x => x.DataEncerramento).ToList();
            if (retCO.HasItems())
            {
                this._listaadmissaoassistencialCO = new wrpAdmissaoAssistencialCOCollection(retCO);
                this._podeabrir = true;
            }

            IRepositorioDeAdmissaoAssistencialCTINEO repCTINEO = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialCTINEO>();
            var retCTI = repCTINEO.OndeIdAtendimentoIgual(_atendimento.DomainObject.ID).List().Where(x => !x.DataExclusao.HasValue && x.DataEncerramento.HasValue).OrderByDescending(x => x.DataEncerramento).ToList();
            if (retCTI.HasItems())
            {
                this._listaadmissaoassistencialCTINEO = new wrpAdmissaoAssistencialCTINEOCollection(retCTI);
                this._podeabrir = true;
            }

            if (this._atendimento.AdmissaoAssistencialEndoscopia.HasItems())
            {
                this._listaadmissaoassistencialEndoscopia = new wrpAdmissaoAssistencialEndoscopiaCollection(this._atendimento.DomainObject.AdmissaoAssistencialEndoscopia.Where(x => x.DataExclusao.IsNull()).ToList());
                this._podeabrir = true;
            }

            IRepositorioDeAdmissaoAssistencialUrodinamica repAdm = ObjectFactory.GetInstance<IRepositorioDeAdmissaoAssistencialUrodinamica>();
            var retU = repAdm.OndeIdAtendimentoIgual(_atendimento.ID).List();
            if (retU.HasItems())
            {
                this._listaadmissaoassistencialUrodinamica = new wrpAdmissaoAssistencialUrodinamicaCollection(retU.Where(x => x.DataExclusao.IsNull()).ToList());
                this._podeabrir = true;
            }            

            this._checklistcollection = (from T in this._atendimento.DomainObject.DescricaoCirurgica
                                         select new HMV.PEP.ViewModel.PEP.CheckListDeCirurgia.CheckListDTO
                                         {
                                             CheckList = this.getCheckList(T),
                                             Cirurgia = this.getCirurgia(T),
                                             Prestador = this.getPrestador(T),
                                             DataAviso = T.DataAviso,
                                             AvisoCirurgia = new wrpAvisoCirurgia(T),
                                             Img = this.getImagemCheckList(T),
                                             UsuarioEncerramento = this.getUsuarioEncerramento(T)
                                         }).ToList().Where(x => x.UsuarioEncerramento.IsNotNull()).ToList();

            this._checklistUDIcollection = (from T in this._atendimento.DomainObject.DescricaoCirurgica
                                         select new HMV.PEP.ViewModel.PEP.CheckListDeUDI.CheckListDTO
                                         {
                                             CheckList = this.getCheckListUDI(T),
                                             Cirurgia = this.getCirurgiaUDI(T),
                                             Prestador = this.getPrestadorUDI(T),
                                             DataAviso = T.DataAviso,
                                             AvisoCirurgia = new wrpAvisoCirurgia(T),
                                             Img = this.getImagemCheckListUDI(T),
                                             UsuarioEncerramento = this.getUsuarioEncerramentoUDI(T)
                                         }).ToList().Where(x => x.UsuarioEncerramento.IsNotNull()).ToList();


            if (this._checklistcollection.HasItems() || this._checklistUDIcollection.HasItems())
                this._podeabrir = true;

            IRepositorioDePerguntasPaciente reppl = ObjectFactory.GetInstance<IRepositorioDePerguntasPaciente>();
            var retpl = reppl.OndeIdAtendimentoIgual(this._atendimento.ID).List();
            if (retpl.HasItems())
            {
                this._perguntaspaciente = new wrpPerguntasPacienteCollection(retpl.Where(x => x.DataImpressao.IsNotNull()).OrderByDescending(x => x.DataImpressao).ToList());
                if (this._perguntaspaciente.HasItems())
                    this._podeabrir = true;
            }
            
           this._avaliacoesderisco = new wrpAvaliacaoRiscoCollection(this._atendimento.DomainObject.AvaliacaoRisco.Where(x => x.DataExclusao == null).OrderByDescending(x => x.DataInclusao).ToList());
            if (_avaliacoesderisco.HasItems())
                this._podeabrir = true;
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpAtendimento _atendimento;
        private wrpAdmissaoAssistencialCollection _listaadmissaoassistencial;
        private wrpAdmissaoAssistencialCOCollection _listaadmissaoassistencialCO;
        private wrpAdmissaoAssistencialCTINEOCollection _listaadmissaoassistencialCTINEO;
        private wrpAdmissaoAssistencialEndoscopiaCollection _listaadmissaoassistencialEndoscopia;
        private wrpAdmissaoAssistencialUrodinamicaCollection _listaadmissaoassistencialUrodinamica;
        private IList<HMV.PEP.ViewModel.PEP.CheckListDeCirurgia.CheckListDTO> _checklistcollection;
        private IList<HMV.PEP.ViewModel.PEP.CheckListDeUDI.CheckListDTO> _checklistUDIcollection;
        private wrpPerguntasPacienteCollection _perguntaspaciente;
        private wrpAvaliacaoRiscoCollection _avaliacoesderisco;
        private bool _podeabrir;
        #endregion

        #region ----- Propriedades Públicas -----
        public bool PodeAbrir
        {
            get
            {
                return this._podeabrir;
            }
        }
        public wrpAtendimento Atendimento
        {
            get
            {
                return _atendimento;
            }
        }
        public wrpAdmissaoAssistencialCollection ListaDeAdmissaoAssistencial
        {
            get
            {
                return this._listaadmissaoassistencial;
            }
        }

        public wrpAdmissaoAssistencialCOCollection ListaDeAdmissaoAssistencialCO
        {
            get
            {
                return this._listaadmissaoassistencialCO;
            }
        }

        public wrpAdmissaoAssistencialCTINEOCollection ListaDeAdmissaoAssistencialCTINEO
        {
            get
            {
                return this._listaadmissaoassistencialCTINEO;
            }
        }

        public wrpAdmissaoAssistencialEndoscopiaCollection ListaDeAdmissaoAssistencialEndoscopia
        {
            get
            {
                return this._listaadmissaoassistencialEndoscopia;
            }
        }

        public wrpAdmissaoAssistencialUrodinamicaCollection ListaDeAdmissaoAssistencialUrodinamica
        {
            get
            {
                return this._listaadmissaoassistencialUrodinamica;
            }
        }

        public wrpAvaliacaoRiscoCollection AvaliacoesDeRiscos
        {
            get
            {
                return this._avaliacoesderisco;
            }
        }

        public IList<HMV.PEP.ViewModel.PEP.CheckListDeCirurgia.CheckListDTO> CheckListCollection
        {
            get
            {
                return this._checklistcollection;
            }
        }

        public IList<HMV.PEP.ViewModel.PEP.CheckListDeUDI.CheckListDTO> CheckListUDICollection
        {
            get
            {
                return this._checklistUDIcollection;
            }
        }

        public wrpPerguntasPacienteCollection ListaPerguntasPaciente
        {
            get { return this._perguntaspaciente; }
        }
        #endregion

        #region ----- Métodos Privados -----
        private wrpUsuarios getUsuarioEncerramento(AvisoCirurgia T)
        {            
            if (T.CheckList.IsNull() || T.CheckList.UsuarioEncerramento.IsNull())
                return null;
            return new wrpUsuarios(T.CheckList.UsuarioEncerramento);
        }

        private wrpCheckListCirurgia getCheckList(AvisoCirurgia T)
        {
            if (T.CheckList.IsNull())
                return null;
            return new wrpCheckListCirurgia(T.CheckList);
        }

        private wrpPrestador getPrestador(AvisoCirurgia T)
        {
            if (T.CheckList.IsNotNull() && T.CheckList.Prestador.IsNotNull())
                return new wrpPrestador(T.CheckList.Prestador);

            if (T.EquipesMedicas.Count(xx => xx.Principal.Equals(SimNao.Sim)) > 0)
                return new wrpPrestador(T.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador);
            return new wrpPrestador(T.EquipesMedicas.Where(x => x.AtividadeMedica.OrdemApresentacao.Equals(1)).FirstOrDefault().Prestador);
        }

        private wrpCirurgia getCirurgia(AvisoCirurgia T)
        {
            if (T.CheckList.IsNotNull() && T.CheckList.Cirurgia.IsNotNull())
                return new wrpCirurgia(T.CheckList.Cirurgia);
            return new wrpCirurgia(T.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia);
        }

        private BitmapImage getImagemCheckList(AvisoCirurgia T)
        {
            if (T.CheckList == null)
                return null;
            return new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/CheckList.png", UriKind.Relative));
        }


        private wrpUsuarios getUsuarioEncerramentoUDI(AvisoCirurgia T)
        {
            if (T.CheckListUDI.IsNull())
                return null;
            return new wrpUsuarios(T.CheckListUDI.Usuario);
        }

        private wrpCheckListUDI getCheckListUDI(AvisoCirurgia T)
        {
            if (T.CheckListUDI.IsNull())
                return null;
            return new wrpCheckListUDI(T.CheckListUDI);
        }

        private wrpPrestador getPrestadorUDI(AvisoCirurgia T)
        {
            if (T.CheckListUDI != null)
                return new wrpPrestador(T.CheckListUDI.Prestador);

            if (T.EquipesMedicas.Count(xx => xx.Principal.Equals(SimNao.Sim)) > 0)
                return new wrpPrestador(T.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador);
            return new wrpPrestador(T.EquipesMedicas.Where(x => x.AtividadeMedica.OrdemApresentacao.Equals(1)).FirstOrDefault().Prestador);
        }

        private wrpCirurgia getCirurgiaUDI(AvisoCirurgia T)
        {
            if (T.CheckListUDI != null)
                return new wrpCirurgia(T.CheckListUDI.Cirurgia);

            return new wrpCirurgia(T.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia);
        }

        private BitmapImage getImagemCheckListUDI(AvisoCirurgia T)
        {
            if (T.CheckListUDI == null)
                return null;

            return new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/CheckList.png", UriKind.Relative));
        }

        #endregion

        #region ----- Métodos Públicos -----

        #endregion

        #region ----- Commands -----

        #endregion
    }
}
