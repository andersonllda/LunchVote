using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.ViewModelBaseClasses;
using StructureMap;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Wrappers.CollectionWrappers;
using System.Collections.Generic;
using System;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Exception;

namespace HMV.PEP.ViewModel.BoletimEmergencia
{
    public class vmBoletimEmergencia : ViewModelBase
    {
        #region Contrutor
        public vmBoletimEmergencia(Atendimento pAtendimento, Usuarios pUsuario, bool pIsPAME)
        {
            this._pame = pIsPAME;
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(pAtendimento);

            this._atendimento = new wrpAtendimento(pAtendimento);
            this._atendimentoprincipal = pAtendimento;
            this._usuarios = pUsuario;

            if (pAtendimento.BoletinsDeEmergencia != null)
            {
                if (pAtendimento.BoletinsDeEmergencia.Count == 0)
                {
                    pAtendimento.BoletinsDeEmergencia.Add(new BoletimDeEmergencia(pAtendimento));
                    this._boletim = new wrpBoletimDeEmergencia(pAtendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
                    this._boletim.Save();
                }
                else
                    this._boletim = new wrpBoletimDeEmergencia(pAtendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
            }
            else
            {
                pAtendimento.BoletinsDeEmergencia.Add(new BoletimDeEmergencia(pAtendimento));
                this._boletim = new wrpBoletimDeEmergencia(pAtendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last());
                this._boletim.Save();
            }


            IRepositorioDeBoletimDeEmergencia repBoletin = ObjectFactory.GetInstance<IRepositorioDeBoletimDeEmergencia>();
            repBoletin.Refresh(this._boletim.DomainObject);

            if (pUsuario.Prestador.Conselho.isMedico() && this._boletim.DataHoraInicioAtendimento == null)
                this._boletim.DataHoraInicioAtendimento = DateTime.Now;

            this._vmsinaisvitais = new vmSinaisVitais(this._boletim, this._usuarios, this);
            this._vmclassificacao = new vmClassificacaoRisco(this._boletim, this._usuarios, this);
            this._vmcids = new vmCIDsAtendimento(this);
            this._vmitensregistro = new vmItensRegistro(this);
            this._tabselecionada = "ItensRegistro";

            if (this._boletim.PAME.IsNull())
                this._boletim.PAME = new wrpPAME();
            this.OnPropertyChanged("BoletimFechadoEPAME");
        }
        #endregion

        #region Propriedades Publicas
        public int? AtendimentoId
        {
            get
            {
                if (this._atendimento != null)
                    return _atendimento.ID;

                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    if (value > 0)
                    {
                        IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                        Atendimento ate = rep.OndeCodigoAtendimentoIgual(value.Value).List().SingleOrDefault();
                        if (ate != null)
                        {

                            if (ate.Paciente == this._atendimentoprincipal.Paciente && ate.TipoDeAtendimento == TipoAtendimento.Urgencia)
                            {
                                this._atendimento = new wrpAtendimento(ate);
                                if (ate.BoletinsDeEmergencia != null)
                                {
                                    if (ate.BoletinsDeEmergencia.Count == 0)
                                    {
                                        ate.BoletinsDeEmergencia.Add(new BoletimDeEmergencia(ate));
                                    }
                                    this._boletim = new wrpBoletimDeEmergencia(ate.BoletinsDeEmergencia.Max());
                                }
                                else
                                {
                                    ate.BoletinsDeEmergencia.Add(new BoletimDeEmergencia(ate));
                                    this._boletim = new wrpBoletimDeEmergencia(ate.BoletinsDeEmergencia.Max());
                                }
                            }
                            else
                            {
                                this._atendimento = null;
                                this._boletim = null;
                            }
                        }
                        else
                        {
                            this._atendimento = null;
                            this._boletim = null;
                        }
                    }
                }
                else
                {
                    this._atendimento = null;
                }
                this.OnPropertyChanged("AtendimentoId");
                this.OnPropertyChanged("Paciente");
                this.OnPropertyChanged("vmSinaisVitais");
                this.OnPropertyChanged("vmClassificacao");
                this.OnPropertyChanged("vmCidDiagnostico");
                this.OnPropertyChanged("vmItensDeRegistro");
            }
        }

        public wrpPaciente Paciente
        {
            get
            {
                if (this._atendimento != null)
                    return this._atendimento.Paciente;
                return null;
            }
        }

        public vmSinaisVitais vmSinaisVitais
        {
            get
            {
                return this._vmsinaisvitais;
            }
        }

        public vmClassificacaoRisco vmClassificacao
        {
            get
            {
                return this._vmclassificacao;
            }
        }

        public vmCIDsAtendimento vmCidDiagnostico
        {
            get
            {
                return this._vmcids;
            }
        }

        public vmItensRegistro vmItensDeRegistro
        {
            get
            {
                return this._vmitensregistro;
            }
        }

        public bool BoletimFechado
        {
            get
            {
                if (this._boletim.DataAlta != null)
                    return true;
                return false;
            }
        }

        public bool BoletimFechadoEPAME
        {
            get
            {
                return !BoletimFechado && _pame;
            }
        }

        public Usuarios Usuarios
        {
            get
            {
                return this._usuarios;
            }
        }

        public wrpDocumentosCollection Documentos
        {
            get
            {
                return this._atendimento.Documentos;
            }
        }

        public wrpBoletimDeEmergenciaCollection BoletinsDeEmergencias
        {
            get
            {
                return this._atendimento.BoletinsDeEmergencia;
            }
        }

        public wrpBoletimDeEmergencia BoletimEmergencia
        {
            get
            {
                return this._boletim;
            }
        }

        public string TabSelecionada
        {
            get
            {
                return this._tabselecionada;
            }
            set
            {
                this._tabselecionada = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmBoletimEmergencia>(x => x.TabSelecionada));
            }
        }

        public bool Editou
        {
            get { return this._editou; }
            set
            {
                this._editou = value;
            }
        }

        public bool boolUrgenciaClinica
        {
            get { return this._boletim.PAME.IsNotNull() && this._boletim.PAME.TipoAtendimento.IsNotNull() && this._boletim.PAME.TipoAtendimento.Equals(TipoAtendimentoPAME.UrgenciaClinica); }
            set
            {
                if (value)
                {
                    this._boletim.PAME.Local = string.Empty;
                    this._boletim.PAME.Relato = string.Empty;
                    this._boletim.PAME.Data = null;
                    this._boletim.PAME.TipoAtendimento = TipoAtendimentoPAME.UrgenciaClinica;
                    this.OnPropertyChanged(ExpressionEx.PropertyName<vmBoletimEmergencia>(x => x.BoletimEmergencia));
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmBoletimEmergencia>(x => x.boolUrgenciaClinica));
            }
        }

        public bool boolRegistroEvento
        {
            get { return this._boletim.PAME.IsNotNull() && this._boletim.PAME.TipoAtendimento.IsNotNull() && this._boletim.PAME.TipoAtendimento.Equals(TipoAtendimentoPAME.RegistroEvento); }
            set
            {
                if (value)
                    this._boletim.PAME.TipoAtendimento = TipoAtendimentoPAME.RegistroEvento;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmBoletimEmergencia>(x => x.boolRegistroEvento));
            }
        }

        public bool TemPAME
        {
            get { return this._boletim.PAME.IsNotNull() && this._boletim.PAME.TipoAtendimento.IsNotNull() && this._boletim.DataAlta.IsNotNull(); }
        }

        public bool IsPame
        {
            get { return this._pame; }
        }

        public bool IsValidPAME
        {
            get
            {
                IList<string> erros = new List<string>();

                if (this._boletim.PAME.TipoAtendimento.IsNull())
                    erros.Add("Informe o 'Tipo de Atendimento'.");

                else if (this._boletim.PAME.TipoAtendimento.Equals(TipoAtendimentoPAME.RegistroEvento))
                {
                    if (this._boletim.PAME.Local.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Local'.");
                    if (this._boletim.PAME.Data.IsNull())
                        erros.Add("Informe a 'Data'.");
                    if (this._boletim.PAME.Hora.IsNull() || this._boletim.PAME.Hora.Value.TimeOfDay.ToString().Equals("00:00:00"))
                        erros.Add("Informe a 'Hora'.");
                    if (this._boletim.PAME.Relato.IsEmptyOrWhiteSpace())
                        erros.Add("Informe o 'Relato'.");
                }

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._boletim.Save();

                return true;
            }
        }
        #endregion

        #region Metodos Privados

        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento { get; set; }
        private Atendimento _atendimentoprincipal { get; set; }
        private Usuarios _usuarios { get; set; }
        private wrpBoletimDeEmergencia _boletim { get; set; }
        private vmSinaisVitais _vmsinaisvitais { get; set; }
        private vmItensRegistro _vmitensregistro { get; set; }
        private vmClassificacaoRisco _vmclassificacao { get; set; }
        private vmCIDsAtendimento _vmcids { get; set; }
        private string _tabselecionada { get; set; }
        private bool _editou;
        private bool _pame;
        public event EventHandler EventRelatorio;
        #endregion

        #region Metodos Publicos
        public void SalvaBoletim(bool pMostraMsg = true)
        {
            this._vmitensregistro.AdicionaItem();

            if (this._boletim.Save())
            {                
                this._editou = false;
                this.OnPropertyChanged("BoletimFechado");
                this.OnPropertyChanged("BoletimFechadoEPAME");
                if (pMostraMsg)
                    DXMessageBox.Show("Boletim Salvo!", "Boletim de Emergêcia", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void NovoBoletim()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(this._atendimento.DomainObject);

            this._boletim = new wrpBoletimDeEmergencia(this._atendimento.DomainObject);
            if (this._usuarios.Prestador.Conselho.isMedico())
                this._boletim.DataHoraInicioAtendimento = DateTime.Now;
            this._atendimento.BoletinsDeEmergencia.Add(this._boletim);

            this._boletim.Save();
            this._atendimento.Save();

            rep.Refresh(this._atendimento.DomainObject);

            this._boletim = this._atendimento.BoletinsDeEmergencia.OrderBy(x => x.Id).Last();

            this._vmsinaisvitais = new vmSinaisVitais(this._boletim, this._usuarios, this);
            this._vmclassificacao = new vmClassificacaoRisco(this._boletim, this._usuarios, this);
            this._vmcids = new vmCIDsAtendimento(this);
            this._vmitensregistro = new vmItensRegistro(this);

            this.OnPropertyChanged("BoletimFechado");
            this.OnPropertyChanged("vmSinaisVitais");
            this.OnPropertyChanged("vmClassificacao");
            this.OnPropertyChanged("vmCidDiagnostico");
            this.OnPropertyChanged("vmItensDeRegistro");
            this.OnPropertyChanged("BoletimFechadoEPAME");
        }

        public void MostraRelatorio()
        {
            if (EventRelatorio != null)
                EventRelatorio(this, null);
        }
        #endregion


    }
}
