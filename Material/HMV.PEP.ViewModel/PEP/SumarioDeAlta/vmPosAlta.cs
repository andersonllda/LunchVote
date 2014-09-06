using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.Consult;
using HMV.PEP.DTO;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Wrappers;
using StructureMap;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmPosAlta : ViewModelBase
    {
        #region Contrutor
        //Utima prescricao
        public vmPosAlta(IList<MedicamentoPosAltaDTO> pMedicamentosPosAlta, wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios)
        {
            this._sumarioalta = pSumarioAlta;
            this._usuarios = pUsuarios;
            this.ItensUltimaPrescricao = pMedicamentosPosAlta;
            this.Inicializa();
        }
        //Pos, Cad e Sel
        public vmPosAlta(wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios)
        {
            this._sumarioalta = pSumarioAlta;
            if (this._sumarioalta.DomainObject.PlanoPosAlta == null)
                this._sumarioalta.DomainObject.PlanoPosAlta = new List<PlanoPosAlta>();
            this._usuarios = pUsuarios;
            this.Inicializa();
        }

        private void Inicializa()
        {
            this.AddPlanoPosAltaCommand = new AddPlanoPosAltaCommand(this);
            this.RemovePlanoPosAltaCommand = new RemovePlanoPosAltaCommand(this);
        }
        #endregion

        #region Propriedades Publicas

        public bool IsCadastroOutros { get { if (_tipomedicamento == TipoMedicamentoPosAlta.Outros) return true; return false; } }
        public bool IsCadastroPrescricao { get { if (_tipomedicamento == TipoMedicamentoPosAlta.Prescrito || _tipomedicamento == TipoMedicamentoPosAlta.Possivel) return true; return false; } }

        public string TituloCadastro
        {
            get
            {
                if (this._tipomedicamento == TipoMedicamentoPosAlta.Prescrito)
                    return "Cadastro de Medicamentos Prescritos";
                if (this._tipomedicamento == TipoMedicamentoPosAlta.Possivel)
                    return "Cadastro da Lista do Hospital";

                return "Cadastro de Outros Medicamentos";
            }
        }

        public IList<MedicamentoPosAltaDTO> ItensUltimaPrescricao { get; set; }

        public IList<MedicamentoPosAltaDTO> MedicamentosItens
        {
            get
            {
                IPosAltaConsult iConsult = ObjectFactory.GetInstance<IPosAltaConsult>();
                if (this._tipomedicamento == TipoMedicamentoPosAlta.Possivel)
                    return iConsult.carregaMedicametos().OrderBy(x => x.DescricaoProduto).ToList();
                else if (this._tipomedicamento == TipoMedicamentoPosAlta.Prescrito)
                    return iConsult.carregaMedicametosPrescritos(this._sumarioalta.Atendimento.DomainObject).OrderBy(x => x.DescricaoProduto).ToList();
                return null;
            }
        }

        public TipoMedicamentoPosAlta? TipoMedicamento
        {
            get
            {
                return this._tipomedicamento;
            }
            set
            {
                this._tipomedicamento = value;
                if (_tipomedicamento == TipoMedicamentoPosAlta.Outros)
                    this._planoposaltaselecionado = new wrpPlanoPosAlta(this._usuarios, this._sumarioalta.Atendimento.DomainObject);
                this.OnPropertyChanged("TipoMedicamento");
            }
        }

        public MedicamentoPosAltaDTO MedicamentoSelecionado
        {
            get
            {
                return _medicamentoselecionado;
            }
            set
            {
                _medicamentoselecionado = value;
                this.OnPropertyChanged("MedicamentoSelecionado");
            }
        }

        public wrpPlanoPosAltaCollection PlanosPosAlta
        {
            get
            {
                return _sumarioalta.PlanoPosAlta;
            }
            set
            {
                this._sumarioalta.PlanoPosAlta = value;
                this.OnPropertyChanged("SumarioAlta");
            }
        }

        public wrpPlanoPosAlta PlanoPosAltaSelecionado
        {
            get
            {
                return this._planoposaltaselecionado;
            }
            set
            {
                if (value != null)
                    this._planoposaltaselecionado = value;
                this.OnPropertyChanged("PlanoPosAltaSelecionado");
                this.OnPropertyChanged("HabilitaAlterar");
            }
        }

        public SimNao NaoSeAplica
        {
            get
            {
                return this._sumarioalta.SemMedPosAlta;
            }
            set
            {
                if (value == SimNao.Sim)
                    if (this._sumarioalta.PlanoPosAlta == null || this._sumarioalta.PlanoPosAlta.Count == 0)
                        this._sumarioalta.SemMedPosAlta = value;
                    else
                    {
                        this._sumarioalta.SemMedPosAlta = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver medicamentos cadastrados.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    this._sumarioalta.SemMedPosAlta = value;

                this.OnPropertyChanged("NaoSeAplica");
            }
        }

        public bool HabilitaAlterar
        {
            get { return (this.PlanosPosAlta.Count > 0); }
        }

        #endregion

        #region Commands
        public ICommand AddPlanoPosAltaCommand { get; set; }
        public ICommand RemovePlanoPosAltaCommand { get; set; }
        #endregion

        #region Metodos
        public void SetaPlanoPosAltaSelecionado()
        {
            if (_tipomedicamento == TipoMedicamentoPosAlta.Outros)
            {
                this.PlanoPosAltaSelecionado = null;
                this.PlanoPosAltaSelecionado = new wrpPlanoPosAlta(_usuarios, _sumarioalta.Atendimento.DomainObject);
            }
            else
                this._planoposaltaselecionado = null;
        }

        public void SetaPlanoPosAlta()
        {
            wrpPlanoPosAlta posalta = new wrpPlanoPosAlta(this._usuarios, this._sumarioalta.Atendimento.DomainObject);
            posalta.Frequencia = this._medicamentoselecionado.Frequencia;
            posalta.Via = this._medicamentoselecionado.Via;
            if (this._medicamentoselecionado.IdTipoPrescricao != null)
            {
                IRepositorioDeTipoPrescricaoMedica rep = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>();
                posalta.TipoPrescricaoMedica = new wrpTipoPrescricaoMedica(rep.OndeIdIgual(this._medicamentoselecionado.IdTipoPrescricao.Value).Single());
            }
            else
            {
                IRepositorioDeProduto rep = ObjectFactory.GetInstance<IRepositorioDeProduto>();
                posalta.Produto = new wrpProduto(rep.OndeIdIgual(this._medicamentoselecionado.IdProduto.Value).Single());
            }
            this._planoposaltaselecionado = posalta;
        }

        public void AdicionaMedicamentosDTOPlanosPosAlta()
        {
            if (_sumarioalta == null || this.ItensUltimaPrescricao == null) return;

            foreach (var item in this.ItensUltimaPrescricao)
            {
                if (item.Marcado)
                {
                    wrpPlanoPosAlta posalta = new wrpPlanoPosAlta(this._usuarios, this._sumarioalta.Atendimento.DomainObject);
                    posalta.Frequencia = item.Frequencia;
                    posalta.Via = item.Via;
                    posalta.Dose = item.DoseComUnidade.IsNotEmptyOrWhiteSpace() ? item.DoseComUnidade.ToString() : string.Empty;
                    if (item.IdTipoPrescricao != null)
                    {
                        IRepositorioDeTipoPrescricaoMedica rep = ObjectFactory.GetInstance<IRepositorioDeTipoPrescricaoMedica>();
                        posalta.TipoPrescricaoMedica = new wrpTipoPrescricaoMedica(rep.OndeIdIgual(item.IdTipoPrescricao.Value).Single());
                    }
                    else
                    {
                        IRepositorioDeProduto rep = ObjectFactory.GetInstance<IRepositorioDeProduto>();
                        posalta.Produto = new wrpProduto(rep.OndeIdIgual(item.IdProduto.Value).Single());
                    }
                    this._tipomedicamento = TipoMedicamentoPosAlta.Outros;
                    this._planoposaltaselecionado = posalta;
                    this.AddPlanoPosAltaCommand.Execute(null);
                    this._tipomedicamento = null;
                }
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpSumarioAlta _sumarioalta { get; set; }
        private Usuarios _usuarios { get; set; }
        private wrpPlanoPosAlta _planoposaltaselecionado { get; set; }
        private TipoMedicamentoPosAlta? _tipomedicamento { get; set; }
        private MedicamentoPosAltaDTO _medicamentoselecionado { get; set; }
        #endregion

  
    }
}
