using System;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Enum.SumarioDeAvaliacaoMedicaRN;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Types;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers.PEP.SumarioDeAvaliacaoMedicaRN;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Threading;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Model;

namespace HMV.PEP.ViewModel.PEP.SumarioAvaliacaoMedicaRN
{
    public class vmSumarioAvaliacaoMedicaRNRecemNascido : ViewModelBase
    {
        #region ----- Construtor -----
        public vmSumarioAvaliacaoMedicaRNRecemNascido(wrpSumarioAvaliacaoMedicaRN pSumarioAvaliacaoMedicaRN, vmSumarioAvaliacaoMedicaRN pVm)
        {
            this._sumarioavaliacaomedicaRN = pSumarioAvaliacaoMedicaRN;
            if (pSumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto.IsNull())
                pSumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto = new wrpSumarioAvaliacaoMedicaRNSalaParto(pSumarioAvaliacaoMedicaRN);

            this._sumarioavaliacaomedicarnSalaParto = pSumarioAvaliacaoMedicaRN.SumarioAvaliacaoMedicaRNSalaParto;

            if (pVm.Novo)
            {
                var dadosRN = pSumarioAvaliacaoMedicaRN.Atendimento.SumarioAvaliacaoMedicaRNMVRecemNascido;

                // Se nao exitir dados do rn no mv para o atendimento, verifica se o paciente tem outro atendimento com dados do rn. 
                if (dadosRN.IsNull())
                {
                    IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
                    IList<Atendimento> atendimentos = repAte.OndeCodigoPacienteIgual(pSumarioAvaliacaoMedicaRN.Atendimento.Paciente.ID).List();
                    foreach (var item in atendimentos.OrderByDescending(x => x.DataAtendimento).ToList())
                    {
                        if (item.SumarioAvaliacaoMedicaRNMVRecemNascidos.HasItems())
                        {
                            dadosRN = new wrpSumarioAvaliacaoMedicaRNMVRecemNascido(item.SumarioAvaliacaoMedicaRNMVRecemNascidos
                                .OrderByDescending(x => x.IdRecemNascido).FirstOrDefault());
                            break;
                        }
                    }
                }

                if (dadosRN.IsNotNull())
                {
                    this._sumarioavaliacaomedicarnSalaParto.Cor = dadosRN.Cor;
                    this._sumarioavaliacaomedicarnSalaParto.Comprimento = dadosRN.Comprimento;
                    this._sumarioavaliacaomedicarnSalaParto.Peso = dadosRN.Peso;
                    this._sumarioavaliacaomedicarnSalaParto.Sexo = dadosRN.Sexo;
                    this._sumarioavaliacaomedicarnSalaParto.DataNascimento = dadosRN.DataNascimento;
                    this._sumarioavaliacaomedicarnSalaParto.PerimentroCefalico = dadosRN.PerimetroCefalico;
                    this._sumarioavaliacaomedicarnSalaParto.PerimetroToracico = dadosRN.PerimetroToraxico;
                }
            }
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpSumarioAvaliacaoMedicaRNSalaParto _sumarioavaliacaomedicarnSalaParto;
        private DateTime? _dianascimento;
        private DateTime? _horanascimento;
        private ObservableCollection<APGARItens> _apgarcollection;
        private wrpSumarioAvaliacaoMedicaRN _sumarioavaliacaomedicaRN;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpSumarioAvaliacaoMedicaRNSalaParto SumarioAvaliacaoMedicaRNSalaParto
        {
            get
            {
                return this._sumarioavaliacaomedicarnSalaParto;
            }
        }

        public List<string> CarregaSexo
        {
            get
            {
                return Enum<Sexo>.GetCustomDisplay().ToList();
            }
        }
        public List<string> CarregaCor
        {
            get
            {
                return Enum<Cor>.GetCustomDisplay().ToList();
            }
        }

        public DateTime? DiaNascimento
        {
            get
            {
                if (this._dianascimento.IsNull() && this._sumarioavaliacaomedicarnSalaParto.DataNascimento.HasValue)
                    this._dianascimento = this._sumarioavaliacaomedicarnSalaParto.DataNascimento.Value;
                return this._dianascimento;
            }
            set
            {
                this._dianascimento = value;
                if (_horanascimento.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicarnSalaParto.DataNascimento = DateTime.Parse(value.Value.ToShortDateString() + " " + _horanascimento.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicarnSalaParto.DataNascimento = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.DiaNascimento);
            }
        }
        public DateTime? HoraNascimento
        {
            get
            {
                if (this._horanascimento.IsNull() && this._sumarioavaliacaomedicarnSalaParto.DataNascimento.HasValue)
                    this._horanascimento = this._sumarioavaliacaomedicarnSalaParto.DataNascimento.Value;
                return this._horanascimento;
            }
            set
            {
                this._horanascimento = value;
                if (this._dianascimento.HasValue && value.HasValue)
                    this._sumarioavaliacaomedicarnSalaParto.DataNascimento = DateTime.Parse(this._dianascimento.Value.ToShortDateString() + " " + value.Value.ToShortTimeString());
                else
                    this._sumarioavaliacaomedicarnSalaParto.DataNascimento = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.HoraNascimento);
            }
        }

        public bool ReanimacaoNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsReanimacao.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacao = SimNao.Nao;
                    this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = null;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoEntubacao = null;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoVentilacao = null;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoOxigenio = null;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoMassagem = null;
                }
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Ventilacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Oxigenio);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MassagemCardiaca);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Entubacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ReanimacaoSim);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ReanimacaoNao);
            }
        }
        public bool ReanimacaoSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsReanimacao.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacao = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacao = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ReanimacaoNao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ReanimacaoSim);
            }
        }

        public bool Entubacao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.Value == TipoReanimacaoRN.Entubacao);
                else if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoEntubacao == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoEntubacao = SimNao.Sim;
                //this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = TipoReanimacaoRN.Entubacao;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoEntubacao = SimNao.Nao;

                this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Ventilacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Oxigenio);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MassagemCardiaca);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Entubacao);
            }
        }

        public bool MassagemCardiaca
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.Value == TipoReanimacaoRN.MassagemCardiaca);
                else if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoMassagem == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoMassagem = SimNao.Sim;
                //this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = TipoReanimacaoRN.MassagemCardiaca;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoMassagem = SimNao.Nao;

                this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Ventilacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Oxigenio);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Entubacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MassagemCardiaca);
            }
        }
        public bool Oxigenio
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.Value == TipoReanimacaoRN.Oxigenio);
                else if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoOxigenio == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    //this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = TipoReanimacaoRN.Oxigenio;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoOxigenio = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoOxigenio = SimNao.Nao;

                this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Ventilacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MassagemCardiaca);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Entubacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Oxigenio);
            }
        }
        public bool Ventilacao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao.Value == TipoReanimacaoRN.Ventilacao);
                else if (this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoVentilacao == SimNao.Sim)
                    return true;
                return false;
            }
            set
            {
                if (value)
                    //this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = TipoReanimacaoRN.Ventilacao;
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoVentilacao = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsReanimacaoVentilacao = SimNao.Nao;

                this._sumarioavaliacaomedicarnSalaParto.TipoReanimacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Oxigenio);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MassagemCardiaca);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Entubacao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.Ventilacao);
            }
        }

        public bool MedicamentosNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                {
                    this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos = SimNao.Nao;
                    this._sumarioavaliacaomedicarnSalaParto.MedicamentosObservacao = string.Empty;
                }
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MedicamentosSim);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MedicamentosNao);
            }
        }
        public bool MedicamentosSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsMedicamentos = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MedicamentosSim);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.MedicamentosNao);
            }
        }

        public int? ApgarPrimeiro
        {
            get
            {
                return this._sumarioavaliacaomedicarnSalaParto.ApgarPrimeiro;
            }
            set
            {
                if (this.APGARCollection.Count(x => x.FirstMinute == true) > 0)
                {
                    if (DXMessageBox.Show("Deseja apagar os dados inseridos nas respostas do APGAR?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.APGARCollection.Each(x => x.FirstMinute = false);
                        this._sumarioavaliacaomedicarnSalaParto.ApgarPrimeiro = value;
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.APGARCollection);
                    }
                }
                else
                    this._sumarioavaliacaomedicarnSalaParto.ApgarPrimeiro = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarPrimeiro);
            }
        }
        public int? ApgarQuinto
        {
            get
            {
                return this._sumarioavaliacaomedicarnSalaParto.ApgarQuinto;
            }
            set
            {
                if (this.APGARCollection.Count(x => x.FifthMinute == true) > 0)
                {
                    if (DXMessageBox.Show("Deseja apagar os dados inseridos nas respostas do APGAR?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.APGARCollection.Each(x => x.FifthMinute = false);
                        this._sumarioavaliacaomedicarnSalaParto.ApgarQuinto = value;
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.APGARCollection);
                    }
                }
                else
                    this._sumarioavaliacaomedicarnSalaParto.ApgarQuinto = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarQuinto);
            }
        }
        public int? ApgarDessimo
        {
            get
            {
                return this._sumarioavaliacaomedicarnSalaParto.ApgarDessimo;
            }
            set
            {
                if (this.APGARCollection.Count(x => x.TenthtMinute == true) > 0)
                {
                    if (DXMessageBox.Show("Deseja apagar os dados inseridos nas respostas do APGAR?", "Atenção", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.APGARCollection.Each(x => x.TenthtMinute = false);
                        this._sumarioavaliacaomedicarnSalaParto.ApgarDessimo = value;
                        base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.APGARCollection);
                    }
                }
                else
                    this._sumarioavaliacaomedicarnSalaParto.ApgarDessimo = value;
                base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarDessimo);
            }
        }

        public bool ClassificacaoAIG
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.Classificacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.Classificacao.Value == ClassificacaoRN.AIG);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = ClassificacaoRN.AIG;
                else
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoPIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoGIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoAIG);
            }
        }
        public bool ClassificacaoGIG
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.Classificacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.Classificacao.Value == ClassificacaoRN.GIG);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = ClassificacaoRN.GIG;
                else
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoAIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoPIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoGIG);
            }
        }
        public bool ClassificacaoPIG
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.Classificacao.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.Classificacao.Value == ClassificacaoRN.PIG);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = ClassificacaoRN.PIG;
                else
                    this._sumarioavaliacaomedicarnSalaParto.Classificacao = null;

                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoAIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoGIG);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ClassificacaoPIG);
            }
        }

        public bool UrinouNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsUrinou.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsUrinou.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsUrinou = SimNao.Nao;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsUrinou = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.UrinouSim);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.UrinouNao);
            }
        }
        public bool UrinouSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsUrinou.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsUrinou.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsUrinou = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsUrinou = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.UrinouNao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.UrinouSim);
            }
        }

        public bool EvacuouNao
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsEvacuou.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsEvacuou.Value == SimNao.Nao);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsEvacuou = SimNao.Nao;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsEvacuou = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.EvacuouSim);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.EvacuouNao);
            }
        }
        public bool EvacuouSim
        {
            get
            {
                if (this._sumarioavaliacaomedicarnSalaParto.IsEvacuou.HasValue)
                    return (this._sumarioavaliacaomedicarnSalaParto.IsEvacuou.Value == SimNao.Sim);
                return false;
            }
            set
            {
                if (value)
                    this._sumarioavaliacaomedicarnSalaParto.IsEvacuou = SimNao.Sim;
                else
                    this._sumarioavaliacaomedicarnSalaParto.IsEvacuou = null;
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.EvacuouNao);
                this.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.EvacuouSim);
            }
        }

        public ObservableCollection<APGARItens> APGARCollection
        {
            get
            {
                if (this._apgarcollection.IsNull())
                {
                    this._apgarcollection = new ObservableCollection<APGARItens>();
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.FC, PontuacaoValor = (int)FRPontos.Ausente, PontuacaoDescricao = FRPontos.Ausente.GetEnumDescription(), Ordem = 1 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.FC, PontuacaoValor = (int)FRPontos.Menos100, PontuacaoDescricao = FRPontos.Menos100.GetEnumDescription(), Ordem = 2 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.FC, PontuacaoValor = (int)FRPontos.Mais100, PontuacaoDescricao = FRPontos.Mais100.GetEnumDescription(), Ordem = 3 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.ER, PontuacaoValor = (int)ERPontos.Ausente, PontuacaoDescricao = ERPontos.Ausente.GetEnumDescription(), Ordem = 4 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.ER, PontuacaoValor = (int)ERPontos.ChoroFraco, PontuacaoDescricao = ERPontos.ChoroFraco.GetEnumDescription(), Ordem = 5 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.ER, PontuacaoValor = (int)ERPontos.ChoroForte, PontuacaoDescricao = ERPontos.ChoroForte.GetEnumDescription(), Ordem = 6 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.TM, PontuacaoValor = (int)TMPontos.Flacido, PontuacaoDescricao = TMPontos.Flacido.GetEnumDescription(), Ordem = 7 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.TM, PontuacaoValor = (int)TMPontos.FlexaoExt, PontuacaoDescricao = TMPontos.FlexaoExt.GetEnumDescription(), Ordem = 8 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.TM, PontuacaoValor = (int)TMPontos.BoaFlexao, PontuacaoDescricao = TMPontos.BoaFlexao.GetEnumDescription(), Ordem = 9 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.IR, PontuacaoValor = (int)IRPontos.SemResposta, PontuacaoDescricao = IRPontos.SemResposta.GetEnumDescription(), Ordem = 10 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.IR, PontuacaoValor = (int)IRPontos.AlgumMovimento, PontuacaoDescricao = IRPontos.AlgumMovimento.GetEnumDescription(), Ordem = 11 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.IR, PontuacaoValor = (int)IRPontos.Choro, PontuacaoDescricao = IRPontos.Choro.GetEnumDescription(), Ordem = 12 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.COR, PontuacaoValor = (int)CORPontos.Palido, PontuacaoDescricao = CORPontos.Palido.GetEnumDescription(), Ordem = 13 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.COR, PontuacaoValor = (int)CORPontos.Cianose, PontuacaoDescricao = CORPontos.Cianose.GetEnumDescription(), Ordem = 14 });
                    this._apgarcollection.Add(new APGARItens(this) { ItemAPGAR = ItemAPGAR.COR, PontuacaoValor = (int)CORPontos.Rosado, PontuacaoDescricao = CORPontos.Rosado.GetEnumDescription(), Ordem = 15 });

                    #region Carrega Grid
                    if (this._sumarioavaliacaomedicaRN.SumarioAvaliacaoMedicaRNApgar.HasItems())
                    {
                        foreach (var item in this._sumarioavaliacaomedicaRN.SumarioAvaliacaoMedicaRNApgar)
                        {
                            var apgar = this._apgarcollection.Where(x => x.Ordem == item.Ordem && item.Minuto == MinutoApgarRN.Primeiro).FirstOrDefault();
                            if (apgar.IsNotNull())
                                apgar._first = true;

                            apgar = this._apgarcollection.Where(x => x.Ordem == item.Ordem && item.Minuto == MinutoApgarRN.Quinto).FirstOrDefault();
                            if (apgar.IsNotNull())
                                apgar._five = true;

                            apgar = this._apgarcollection.Where(x => x.Ordem == item.Ordem && item.Minuto == MinutoApgarRN.Dessimo).FirstOrDefault();
                            if (apgar.IsNotNull())
                                apgar._ten = true;

                            //if (apgar.IsNotNull())
                            //{
                            //    if (item.Cor.IsNotNull())
                            //    {
                            //        if (item.Minuto == MinutoApgarRN.Primeiro)
                            //            apgar._first = true;
                            //        if (item.Minuto == MinutoApgarRN.Quinto)
                            //            apgar._five = true;
                            //        if (item.Minuto == MinutoApgarRN.Dessimo)
                            //            apgar._ten = true;
                            //    }
                            //    if (item.Esforco.IsNotNull())
                            //    {
                            //        if (item.Minuto == MinutoApgarRN.Primeiro)
                            //            apgar._first = true;
                            //        if (item.Minuto == MinutoApgarRN.Quinto)
                            //            apgar._five = true;
                            //        if (item.Minuto == MinutoApgarRN.Dessimo)
                            //            apgar._ten = true;
                            //    }
                            //    if (item.FrequenciaCardiaca.IsNotNull())
                            //    {
                            //        if (item.Minuto == MinutoApgarRN.Primeiro)                                        
                            //                apgar._first = true;
                            //        if (item.Minuto == MinutoApgarRN.Quinto)                                        
                            //                apgar._five = true;
                            //        if (item.Minuto == MinutoApgarRN.Dessimo)                                        
                            //                apgar._ten = true;
                            //    }
                            //    if (item.Irritabilidade.IsNotNull())
                            //    {
                            //        if (item.Minuto == MinutoApgarRN.Primeiro)
                            //            apgar._first = true;
                            //        if (item.Minuto == MinutoApgarRN.Quinto)
                            //            apgar._five = true;
                            //        if (item.Minuto == MinutoApgarRN.Dessimo)
                            //            apgar._ten = true;
                            //    }
                            //    if (item.Tonus.IsNotNull())
                            //    {
                            //        if (item.Minuto == MinutoApgarRN.Primeiro)
                            //            apgar._first = true;
                            //        if (item.Minuto == MinutoApgarRN.Quinto)
                            //            apgar._five = true;
                            //        if (item.Minuto == MinutoApgarRN.Dessimo)
                            //            apgar._ten = true;
                            //    }
                            //}
                        }
                    }
                    #endregion
                }
                return this._apgarcollection;
            }
        }
        #endregion

        #region ----- Métodos Privados -----

        #endregion

        #region ----- Métodos Públicos -----
        public void CalcularPontos()
        {
            int first = 0;
            int five = 0;
            int ten = 0;
            this.APGARCollection.Each(x =>
            {
                if (x.FirstMinute)
                    first += x.PontuacaoValor;
                if (x.FifthMinute)
                    five += x.PontuacaoValor;
                if (x.TenthtMinute)
                    ten += x.PontuacaoValor;
            });

            this._sumarioavaliacaomedicarnSalaParto.ApgarPrimeiro = first;
            this._sumarioavaliacaomedicarnSalaParto.ApgarQuinto = five;
            this._sumarioavaliacaomedicarnSalaParto.ApgarDessimo = ten;
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarPrimeiro);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarQuinto);
            base.OnPropertyChanged<vmSumarioAvaliacaoMedicaRNRecemNascido>(x => x.ApgarDessimo);
        }
        #endregion

        #region ----- Commands -----

        #endregion

        #region Classes
        public enum ItemAPGAR
        {
            [Description("Frequência Cardíaca")]
            FC,
            [Description("Esforço Respiratório")]
            ER,
            [Description("Tônus Muscular")]
            TM,
            [Description("Irritabilidade Reflexa")]
            IR,
            [Description("Cor")]
            COR
        }
        public enum FRPontos
        {
            [Description("Ausente")]
            Ausente = 0,
            [Description("Menos de 100")]
            Menos100 = 1,
            [Description("Mais de 100")]
            Mais100 = 2
        }
        public enum ERPontos
        {
            [Description("Ausente")]
            Ausente = 0,
            [Description("Choro Fraco")]
            ChoroFraco = 1,
            [Description("Choro Forte")]
            ChoroForte = 2
        }
        public enum TMPontos
        {
            [Description("Flácido")]
            Flacido = 0,
            [Description("Flexão de Extremidades")]
            FlexaoExt = 1,
            [Description("Boa Flexão")]
            BoaFlexao = 2
        }
        public enum IRPontos
        {
            [Description("Sem Resposta")]
            SemResposta = 0,
            [Description("Algum Movimento")]
            AlgumMovimento = 1,
            [Description("Choro")]
            Choro = 2
        }
        public enum CORPontos
        {
            [Description("Pálido/Cianótico")]
            Palido = 0,
            [Description("Cianose de Extremidades")]
            Cianose = 1,
            [Description("Todo Rosado")]
            Rosado = 2
        }

        public class APGARItens : ViewModelBase
        {
            //public APGARItens() { }
            public APGARItens(vmSumarioAvaliacaoMedicaRNRecemNascido pvm)
            {
                this._vm = pvm;
            }

            public bool _first { get; set; }
            public bool _five { get; set; }
            public bool _ten { get; set; }
            private vmSumarioAvaliacaoMedicaRNRecemNascido _vm;

            public int Ordem { get; set; }

            public ItemAPGAR ItemAPGAR { get; set; }
            public string Descricao
            {
                get
                {
                    return ItemAPGAR.GetEnumDescription();
                }
            }
            public int PontuacaoValor { get; set; }
            public string PontuacaoDescricao { get; set; }
            public string Pontuacao
            {
                get
                {
                    return this.PontuacaoValor + " - " + this.PontuacaoDescricao;
                }
            }
            public bool FirstMinute
            {
                get
                {
                    return this._first;
                }
                set
                {
                    if (value)
                        this._vm.APGARCollection.Where(x => x.ItemAPGAR == this.ItemAPGAR).Each(y => y.FirstMinute = false);
                    this._first = value;
                    this._vm.CalcularPontos();
                    base.OnPropertyChanged<APGARItens>(x => x.FirstMinute);
                }
            }
            public bool FifthMinute
            {
                get
                {
                    return this._five;
                }
                set
                {
                    if (value)
                        this._vm.APGARCollection.Where(x => x.ItemAPGAR == this.ItemAPGAR).Each(y => y.FifthMinute = false);
                    this._five = value;
                    this._vm.CalcularPontos();
                    base.OnPropertyChanged<APGARItens>(x => x.FifthMinute);
                }
            }
            public bool TenthtMinute
            {
                get
                {
                    return this._ten;
                }
                set
                {
                    if (value)
                        this._vm.APGARCollection.Where(x => x.ItemAPGAR == this.ItemAPGAR).Each(y => y.TenthtMinute = false);
                    this._ten = value;
                    this._vm.CalcularPontos();
                    base.OnPropertyChanged<APGARItens>(x => x.TenthtMinute);
                }
            }

            public bool Marcado
            {
                get
                {
                    return (this._first || this._five || this._ten);
                }
            }
        }
        #endregion
    }
}
