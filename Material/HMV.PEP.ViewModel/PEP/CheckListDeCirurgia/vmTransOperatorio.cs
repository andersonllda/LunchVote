using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using HMV.Core.Domain.Model;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using HMV.Core.Domain.Model.PEP.CheckListCirurgia;
using HMV.Core.Framework.Exception;

namespace HMV.PEP.ViewModel.PEP.CheckListDeCirurgia
{
    public class vmTransOperatorio : ViewModelBase
    {
        #region ----- Construtor -----
        public vmTransOperatorio(vmCheckList pvmCheckList)
        {
            this._vmchecklist = pvmCheckList;
            this._checklist = pvmCheckList.CheckListdto.CheckList;

            if (this._checklist.TransOperatorio.IsNull())
            {
                this._TransOperatorio = new wrpTransOperatorio(pvmCheckList.Usuario);
            }
            else
                this._TransOperatorio = this._checklist.TransOperatorio;

            if (this._checklist.Sondagem.IsNull())
                this._checklist.Sondagem = new wrpSondagem();

            if (this._TransOperatorio.PAM.IsNull())
                this._TransOperatorio.PAM = new wrpPAM();
            if (this._TransOperatorio.CoxinsDeProtecao.IsNull())
                this._TransOperatorio.CoxinsDeProtecao = new wrpCoxinsDeProtecao();
            if (this._TransOperatorio.MantaTermica.IsNull())
                this._TransOperatorio.MantaTermica = new wrpMantaTermica();
            if (this._TransOperatorio.Drenos.IsNull())
                this._TransOperatorio.Drenos = new wrpDrenos();
            if (this._TransOperatorio.ColchaoTermico.IsNull())
                this._TransOperatorio.ColchaoTermico = new wrpColchaoTermico();
            if (this._TransOperatorio.BotaRetornoVenoso.IsNull())
                this._TransOperatorio.BotaRetornoVenoso = new wrpBotaRetornoVenoso();
            if (this._TransOperatorio.GarrotePneumatico.IsNull())
                this._TransOperatorio.GarrotePneumatico = new GarrotePneumatico();
            
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private vmCheckList _vmchecklist;
        private wrpCheckListCirurgia _checklist;
        private wrpTransOperatorio _TransOperatorio;
        //private vmSondagem ;
        //private DateTime? _DataSala;
        //private DateTime? _HoraSala;
        #endregion

        #region ----- Propriedades Públicas -----
        public wrpCheckListCirurgia CheckList
        {
            get { return this._checklist; }
            set
            {
                this._checklist = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<TimeOut>(x => x.CheckList));
            }
        }

        public wrpTransOperatorio TransOperatorio
        {
            get { return this._TransOperatorio; }
            set
            {
                this._TransOperatorio = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.TransOperatorio));
            }
        }

        public vmSondagem vmSondagem
        {
            get { return this._vmchecklist.vmSondagem; }
            set
            {
                this._vmchecklist.vmSondagem = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.vmSondagem));
            }
        }

        public bool HabilitaSondagem
        {
            get { return this._vmchecklist.vmSondagem.IsNotNull() && (this._vmchecklist.vmSondagem.Sondagem.IsNull() || this._vmchecklist.vmSondagem.Sondagem.DataEncerramento.IsNull()); }
        }

        public string ToolTipSondagem
        {
            get { return (this.HabilitaSondagem) ? string.Empty : "Sondagem vesical já foi preenchida pelo usuário: " + this._vmchecklist.vmSondagem.Sondagem.Usuario.Nome; }
        }

        public bool boolPAMSim
        {
            get { return this._TransOperatorio.PAM.IsNotNull() && !this._TransOperatorio.PAM.Pam.IsNull() && this._TransOperatorio.PAM.Pam.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.PAM.Pam = SimNao.Sim;
                else
                    this._TransOperatorio.PAM.Pam = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPAMNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPAMSim));
            }
        }

        public bool boolPAMNao
        {
            get { return this._TransOperatorio.PAM.IsNotNull() && !this._TransOperatorio.PAM.Pam.IsNull() && this._TransOperatorio.PAM.Pam.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.PAM.Pam = SimNao.Nao;
                    PAMLocal = string.Empty;
                }
                else
                    this._TransOperatorio.PAM.Pam = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPAMSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPAMNao));
            }
        }

        public string PAMLocal
        {
            get {
                return this._TransOperatorio.PAM.Local;
            }
            set {
                this._TransOperatorio.PAM.Local = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.PAMLocal));
            }
        }

        public bool boolCoxinsSim
        {
            get { return this._TransOperatorio.CoxinsDeProtecao.IsNotNull() && this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao = SimNao.Sim;
                else
                {
                    this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao = null;
                    CoxinsDeProtecaoLocal = string.Empty;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolCoxinsNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolCoxinsSim));
            }
        }

        public bool boolCoxinsNao
        {
            get { return this._TransOperatorio.CoxinsDeProtecao.IsNotNull() && this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao = SimNao.Nao;
                    CoxinsDeProtecaoLocal = string.Empty;
                }
                else
                    this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolCoxinsSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolCoxinsNao));
            }
        }

        public string CoxinsDeProtecaoLocal
        {
            get
            {
                return this._TransOperatorio.CoxinsDeProtecao.Local;
            }
            set
            {
                this._TransOperatorio.CoxinsDeProtecao.Local = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.CoxinsDeProtecaoLocal));
            }
        }

        public bool boolDegermacaoSim
        {
            get { return this._TransOperatorio.Degermacao.IsNotNull() && this._TransOperatorio.Degermacao.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.Degermacao = SimNao.Sim;
                else
                    this._TransOperatorio.Degermacao = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDegermacaoNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDegermacaoSim));
            }
        }

        public bool boolDegermacaoNao
        {
            get { return this._TransOperatorio.Degermacao.IsNotNull() && this._TransOperatorio.Degermacao.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.Degermacao = SimNao.Nao;
                    this.boolIododegermante = false;
                    this.boolClorohexidine = false;
                }
                else
                    this._TransOperatorio.Degermacao = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDegermacaoSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDegermacaoNao));
            }
        }

        public bool boolIododegermante
        {
            get { return this._TransOperatorio.Iododegermante.IsNotNull() && this._TransOperatorio.Iododegermante.Equals(SimNao.Sim); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.Iododegermante = SimNao.Sim;
                    boolClorohexidine = false;
                }
                else
                    this._TransOperatorio.Iododegermante = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolIododegermante));
            }
        }

        public bool boolClorohexidine
        {
            get { return this._TransOperatorio.Clorohexidine.IsNotNull() && this._TransOperatorio.Clorohexidine.Equals(SimNao.Sim); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.Clorohexidine = SimNao.Sim;
                    boolIododegermante = false;
                }
                else
                    this._TransOperatorio.Clorohexidine = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolClorohexidine));
            }
        }

        public bool boolMantaSim
        {
            get { return this._TransOperatorio.MantaTermica.IsNotNull() && this._TransOperatorio.MantaTermica.Manta.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.MantaTermica.Manta = SimNao.Sim;
                else
                {
                    this._TransOperatorio.MantaTermica.Manta = null;
                    MantaTermicaTemperatura = string.Empty;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMantaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMantaSim));
            }
        }

        public bool boolMantaNao
        {
            get { return this._TransOperatorio.MantaTermica.IsNotNull() && this._TransOperatorio.MantaTermica.Manta.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.MantaTermica.Manta = SimNao.Nao;
                    MantaTermicaTemperatura = string.Empty;
                }
                else
                    this._TransOperatorio.MantaTermica.Manta = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMantaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMantaNao));
            }
        }

        public string MantaTermicaTemperatura
        {
            get { return this._TransOperatorio.MantaTermica.Temperatura; }
            set
            {
                this._TransOperatorio.MantaTermica.Temperatura = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.MantaTermicaTemperatura));
            }
        }

        public bool boolDrenoSim
        {
            get { return this._TransOperatorio.Drenos.IsNotNull() && this._TransOperatorio.Drenos.Dreno.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.Drenos.Dreno = SimNao.Sim;
                else
                {
                    this._TransOperatorio.Drenos.Dreno = null;
                    DrenosTipo = string.Empty;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDrenoNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDrenoSim));
            }
        }

        public bool boolDrenoNao
        {
            get { return this._TransOperatorio.Drenos.IsNotNull() && this._TransOperatorio.Drenos.Dreno.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.Drenos.Dreno = SimNao.Nao;
                    DrenosTipo = string.Empty;
                }
                else
                    this._TransOperatorio.Drenos.Dreno = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDrenoSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolDrenoNao));
            }
        }

        public string DrenosTipo
        {
            get { return this._TransOperatorio.Drenos.Tipo; }
            set
            {
                this._TransOperatorio.Drenos.Tipo = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.DrenosTipo));
            }
        }

        public bool boolColchaoSim
        {
            get { return this._TransOperatorio.ColchaoTermico.IsNotNull() && this._TransOperatorio.ColchaoTermico.Colchao.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.ColchaoTermico.Colchao = SimNao.Sim;
                else
                {
                    this._TransOperatorio.ColchaoTermico.Colchao = null;
                    ColchaoTermicoTemperatura = string.Empty;
                } 
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolColchaoNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolColchaoSim));
            }
        }

        public bool boolColchaoNao
        {
            get { return this._TransOperatorio.ColchaoTermico.IsNotNull() && this._TransOperatorio.ColchaoTermico.Colchao.IsNotNull() && this._TransOperatorio.ColchaoTermico.Colchao.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.ColchaoTermico.Colchao = SimNao.Nao;
                    ColchaoTermicoTemperatura = string.Empty;    
                }
                else
                    this._TransOperatorio.ColchaoTermico.Colchao = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolColchaoSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolColchaoNao));
            }
        }

        public string ColchaoTermicoTemperatura
        {
            get { return this._TransOperatorio.ColchaoTermico.Temperatura; }
            set
            {
                this._TransOperatorio.ColchaoTermico.Temperatura = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.ColchaoTermicoTemperatura));
            }
        }

        public bool boolBotaSim
        {
            get { return this._TransOperatorio.BotaRetornoVenoso.IsNotNull() && this._TransOperatorio.BotaRetornoVenoso.Bota.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.BotaRetornoVenoso.Bota = SimNao.Sim;
                else
                {
                    this._TransOperatorio.BotaRetornoVenoso.Bota = null;
                    BotaRetornoVenosoTipo = string.Empty;
                } 
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolBotaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolBotaSim));
            }
        }

        public bool boolBotaNao
        {
            get { return this._TransOperatorio.BotaRetornoVenoso.IsNotNull() && this._TransOperatorio.BotaRetornoVenoso.Bota.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.BotaRetornoVenoso.Bota = SimNao.Nao;
                    BotaRetornoVenosoTipo = string.Empty;
                }
                else
                    this._TransOperatorio.BotaRetornoVenoso.Bota = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolBotaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolBotaNao));
            }
        }

        public string BotaRetornoVenosoTipo
        {
            get { return this._TransOperatorio.BotaRetornoVenoso.Tipo; }
            set
            {
                this._TransOperatorio.BotaRetornoVenoso.Tipo = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.BotaRetornoVenosoTipo));
            }
        }

        public bool boolMeiasSim
        {
            get { return this._TransOperatorio.MeiasElasticas.IsNotNull() && this._TransOperatorio.MeiasElasticas.Value.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.MeiasElasticas = SimNao.Sim;
                else
                    this._TransOperatorio.MeiasElasticas = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMeiasNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMeiasSim));
            }
        }

        public bool boolMeiasNao
        {
            get { return this._TransOperatorio.MeiasElasticas.IsNotNull() && this._TransOperatorio.MeiasElasticas.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._TransOperatorio.MeiasElasticas = SimNao.Nao;
                else
                    this._TransOperatorio.MeiasElasticas = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMeiasSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolMeiasNao));
            }
        }

        public bool boolTransfusaoSim
        {
            get { return this._TransOperatorio.TransfusaoSanguinea.IsNotNull() && this._TransOperatorio.TransfusaoSanguinea.Value.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.TransfusaoSanguinea = SimNao.Sim;
                else
                {
                    this._TransOperatorio.TransfusaoSanguinea = null;
                    TransfusaoSanguineaObservacao = string.Empty;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTransfusaoNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTransfusaoSim));
            }
        }

        public bool boolTransfusaoNao
        {
            get { return this._TransOperatorio.TransfusaoSanguinea.IsNotNull() && this._TransOperatorio.TransfusaoSanguinea.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.TransfusaoSanguinea = SimNao.Nao;
                    TransfusaoSanguineaObservacao = string.Empty;    
                }
                else
                    this._TransOperatorio.TransfusaoSanguinea = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTransfusaoSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTransfusaoNao));
            }
        }

        public string TransfusaoSanguineaObservacao
        {
            get { return this._TransOperatorio.TransfusaoSanguineaObservacao; }
            set
            {
                this._TransOperatorio.TransfusaoSanguineaObservacao = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.TransfusaoSanguineaObservacao));
            }
        }

        public bool boolTricotomiaSim
        {
            get { return this._TransOperatorio.Tricotomia.IsNotNull() && this._TransOperatorio.Tricotomia.Value.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.Tricotomia = SimNao.Sim;
                else
                    this._TransOperatorio.Tricotomia = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTricotomiaNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTricotomiaSim));
            }
        }

        public bool boolTricotomiaNao
        {
            get { return this._TransOperatorio.Tricotomia.IsNotNull() && this._TransOperatorio.Tricotomia.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._TransOperatorio.Tricotomia = SimNao.Nao;
                else
                    this._TransOperatorio.Tricotomia = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTricotomiaSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolTricotomiaNao));
            }
        }

        public bool boolPlacaEletroSim
        {
            get { return this._TransOperatorio.PlacaEletrocauterio.IsNotNull() && this._TransOperatorio.PlacaEletrocauterio.Value.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.PlacaEletrocauterio = SimNao.Sim;
                else
                    this._TransOperatorio.PlacaEletrocauterio = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPlacaEletroNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPlacaEletroSim));
            }
        }

        public bool boolPlacaEletroNao
        {
            get { return this._TransOperatorio.PlacaEletrocauterio.IsNotNull() && this._TransOperatorio.PlacaEletrocauterio.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                    this._TransOperatorio.PlacaEletrocauterio = SimNao.Nao;
                else
                    this._TransOperatorio.PlacaEletrocauterio = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPlacaEletroSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolPlacaEletroNao));
            }
        }

        public bool boolGarroteSim
        {
            get { return this._TransOperatorio.GarrotePneumatico.IsNotNull() && this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.IsNotNull() && this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.Value.Equals(SimNao.Sim); }
            set
            {
                if (value)
                    this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico = SimNao.Sim;
                else
                {
                    GarrotePneumaticoTempo = string.Empty;
                    this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico = null;                
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolGarroteNao));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolGarroteSim));
            }
        }

        public bool boolGarroteNao
        {
            get { return this._TransOperatorio.GarrotePneumatico.IsNotNull() && this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.IsNotNull() && this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.Value.Equals(SimNao.Nao); }
            set
            {
                if (value)
                {
                    this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico = SimNao.Nao;
                    GarrotePneumaticoTempo = string.Empty;
                }
                else
                    this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico = null;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolGarroteSim));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.boolGarroteNao));
            }
        }

        public string GarrotePneumaticoTempo
        {
            get {
                return this._TransOperatorio.GarrotePneumatico.Tempo;                
            }
            set {
                this._TransOperatorio.GarrotePneumatico.Tempo = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmTransOperatorio>(x => x.GarrotePneumaticoTempo));
            }
        }

        //public DateTime? DataSala
        //{
        //    get { return this._TransOperatorio.DataSala.HasValue ? this._TransOperatorio.DataSala.Value.Date : this._TransOperatorio.DataSala; }
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            this._DataSala = value.Value.Date;
        //            if (this._HoraSala.IsNotNull())
        //                this._TransOperatorio.DataSala = DateTime.Parse(this._DataSala.Value.ToString("dd/MM/yyyy") + " " + this._HoraSala.Value.TimeOfDay);
        //            else
        //                this._TransOperatorio.DataSala = this._DataSala;
        //        }
        //        else
        //            this._TransOperatorio.DataSala = null;
        //    }
        //}

        //public DateTime? HoraSala
        //{
        //    get { return this._TransOperatorio.DataSala.HasValue ? this._TransOperatorio.DataSala.Value : this._TransOperatorio.DataSala; }
        //    set
        //    {
        //        if (value.HasValue)
        //        {
        //            this._HoraSala = value.Value;
        //            if (this._DataSala.IsNotNull())
        //                this._TransOperatorio.DataSala = DateTime.Parse(this._DataSala.Value.ToString("dd/MM/yyyy") + " " + this._HoraSala.Value.TimeOfDay);
        //            else
        //                this._TransOperatorio.DataSala = this._HoraSala;
        //        }
        //        else
        //            this._TransOperatorio.DataSala = null;
        //    }
        //}
        #endregion

        public override bool IsValid
        {
            get
            {
                IList<string> erros = new List<string>();

                if (this._TransOperatorio.Sala.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Sala' é obrigatório!");

                if (this._TransOperatorio.NivelDeConsciencia.Lucido.Equals(SimNao.Nao) && this._TransOperatorio.NivelDeConsciencia.Entubado.Equals(SimNao.Nao) && this._TransOperatorio.NivelDeConsciencia.Sonolento.Equals(SimNao.Nao) && this._TransOperatorio.NivelDeConsciencia.Outros.Equals(SimNao.Nao))
                    erros.Add("Campo 'Nível de Consciência' é obrigatório!");
                else if (this._TransOperatorio.NivelDeConsciencia.Outros.Equals(SimNao.Sim) && this._TransOperatorio.NivelDeConsciencia.OutrosObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Outro' Nível de Consciência.");

                if (this._TransOperatorio.EntradaSalaCirurgica.Maca.Equals(SimNao.Nao) && this._TransOperatorio.EntradaSalaCirurgica.Cama.Equals(SimNao.Nao) && this._TransOperatorio.EntradaSalaCirurgica.Deambulando.Equals(SimNao.Nao) && this._TransOperatorio.EntradaSalaCirurgica.Outros.Equals(SimNao.Nao))
                    erros.Add("Campo 'Entrada em Sala Cirúrgica' é obrigatório!");
                else if (this._TransOperatorio.EntradaSalaCirurgica.Outros.Equals(SimNao.Sim) && this._TransOperatorio.EntradaSalaCirurgica.OutrosObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Outro' Entrada em Sala Cirúrgica.");

                if (this._TransOperatorio.AcessoVenosoPeriferico.NA.Equals(SimNao.Nao) && this._TransOperatorio.AcessoVenosoPeriferico.MSD.Equals(SimNao.Nao) && this._TransOperatorio.AcessoVenosoPeriferico.MSE.Equals(SimNao.Nao) && this._TransOperatorio.AcessoVenosoPeriferico.MI.Equals(SimNao.Nao))
                    erros.Add("Campo 'Acesso Venoso Periférico' é obrigatório!");
                else if (this._TransOperatorio.AcessoVenosoPeriferico.NA.Equals(SimNao.Nao) && this._TransOperatorio.AcessoVenosoPeriferico.Insyte.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Dispositivo IV n°' no Acesso Venoso Periférico.");

                if (this._TransOperatorio.AcessoCentral.NA.Equals(SimNao.Nao) && this._TransOperatorio.AcessoCentral.Monolumen.Equals(SimNao.Nao) && this._TransOperatorio.AcessoCentral.Duplolumen.Equals(SimNao.Nao) && this._TransOperatorio.AcessoCentral.PIC.Equals(SimNao.Nao))
                    erros.Add("Campo 'Acesso Central' é obrigatório!");
                else if (this._TransOperatorio.AcessoCentral.NA.Equals(SimNao.Nao) && this._TransOperatorio.AcessoCentral.Local.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Local' no Acesso Central.");

                if (this._TransOperatorio.PAM.IsNull() || this._TransOperatorio.PAM.Pam.IsNull())
                    erros.Add("Campo 'PAM' é obrigatório!");
                else if (this._TransOperatorio.PAM.Pam.Value.Equals(SimNao.Sim) && this._TransOperatorio.PAM.Local.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Local' no PAM.");


                if (this._TransOperatorio.Anestesia.Geral.Equals(SimNao.Nao) && this._TransOperatorio.Anestesia.BloqueioPeridural.Equals(SimNao.Nao)
                    && this._TransOperatorio.Anestesia.Raqui.Equals(SimNao.Nao) && this._TransOperatorio.Anestesia.Sedacao.Equals(SimNao.Nao)
                    && this._TransOperatorio.Anestesia.Local.Equals(SimNao.Nao) && this._TransOperatorio.Anestesia.Outros.Equals(SimNao.Nao))
                    erros.Add("É obrigatório informar uma 'Anestesia'");
                
                if (this._TransOperatorio.Anestesia.Outros.Equals(SimNao.Sim) && this._TransOperatorio.Anestesia.OutrosObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o campo Observação Outro da 'Anestesia'");

                if (this._TransOperatorio.Anestesia.TuboEndotraqueal.Equals(SimNao.Sim) && this._TransOperatorio.Anestesia.TuboEndotraquealNumero.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o campo Tubo Endotraqueal n° da 'Anestesia'");

                if (this._TransOperatorio.Anestesia.MascaraLaringea.Equals(SimNao.Sim) && this._TransOperatorio.Anestesia.MascaraLaringeaNumero.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o campo Máscara Laríngea n° da 'Anestesia'");

                if (this._TransOperatorio.Anestesia.Geral == SimNao.Sim && this._TransOperatorio.Anestesia.TuboEndotraqueal.Equals(SimNao.Nao) &&
                    this._TransOperatorio.Anestesia.MascaraLaringea.Equals(SimNao.Nao) )
                    erros.Add("Informe o campo Tubo Endotraqueal ou Máscara Laríngea da 'Anestesia'");

                if (this._TransOperatorio.PosicaoCirurgicaTipo.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Posição Cirúrgica' é obrigatório!");

                if (this._TransOperatorio.CoxinsDeProtecao.IsNull() || this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.IsNull())
                    erros.Add("Campo 'Coxins de Proteção' é obrigatório!");
                else if (this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.IsNotNull() && this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.Value.Equals(SimNao.Sim) && this._TransOperatorio.CoxinsDeProtecao.Local.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Local' no Coxins de Proteção.");

                if (this._TransOperatorio.Degermacao.IsNull())
                    erros.Add("Campo 'Degermação' é obrigatório!");
                else if (this._TransOperatorio.Degermacao.Value.Equals(SimNao.Sim))
                    if (this._TransOperatorio.Clorohexidine.IsNull() && this._TransOperatorio.Iododegermante.IsNull())// && this._TransOperatorio.Clorohexidine.Equals(SimNao.Nao) && this._TransOperatorio.Iododegermante.Equals(SimNao.Nao))
                        erros.Add("Marque 'Clorohexidine degermante' ou 'Iodo degermante'!");

                if (this._TransOperatorio.Assepsia.Alcool70 == SimNao.Nao && this._TransOperatorio.Assepsia.CloroAlcoolico == SimNao.Nao &&
                    this._TransOperatorio.Assepsia.CloroDegermante == SimNao.Nao && this._TransOperatorio.Assepsia.CloroTopico == SimNao.Nao &&
                    this._TransOperatorio.Assepsia.IodoAlcoolico == SimNao.Nao && this._TransOperatorio.Assepsia.IodoTopico == SimNao.Nao &&
                    this._TransOperatorio.Assepsia.SoroFisiologico == SimNao.Nao)
                {
                    erros.Add("Pelo menos um item da Assepsia deve ser marcado.");
                }

                if (this.vmSondagem.Sondagem.VesicalAlivio.IsNull())
                    erros.Add("Campo 'Sondagem Vesical de Alívio' é obrigatório!");

                if (this.vmSondagem.Sondagem.VesicalDemora.IsNull())
                    erros.Add("Campo 'Sondagem Vesical de Demora' é obrigatório!");
                
                //else if (this.vmSondagem.Sondagem.VesicalAlivio.Value.Equals(SimNao.Sim) && this.vmSondagem.Sondagem.VesicalAlivioVolume.IsEmptyOrWhiteSpace())
                //    erros.Add("Campo 'Volume' da Sondagem Vesical de Alívio é obrigatório!");
                
                //foreach (string item in this.vmSondagem.Valida())
                //    erros.Add(item);

                //if (this._TransOperatorio.CoxinsDeProtecao.IsNull() || this._TransOperatorio.CoxinsDeProtecao.CoxinsProtecao.IsNull())
                //    erros.Add("Campo 'Coxins de Proteção' é obrigatório!");

                if (this._TransOperatorio.ColchaoTermico.IsNull() ||     this._TransOperatorio.ColchaoTermico.Colchao.IsNull())
                    erros.Add("Campo 'Colchão Térmico' é obrigatório!");
                else if (this._TransOperatorio.ColchaoTermico.Colchao.IsNotNull() && this._TransOperatorio.ColchaoTermico.Colchao.Value.Equals(SimNao.Sim) && this._TransOperatorio.ColchaoTermico.Temperatura.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a 'Temperatura' do Colchão Térmico.");

                if (this._TransOperatorio.MantaTermica.IsNull() || this._TransOperatorio.MantaTermica.Manta.IsNull())
                    erros.Add("Campo 'Manta Térmica' é obrigatório!");
                else if (this._TransOperatorio.MantaTermica.Manta.IsNotNull() && this._TransOperatorio.MantaTermica.Manta.Value.Equals(SimNao.Sim) && this._TransOperatorio.MantaTermica.Temperatura.IsEmptyOrWhiteSpace())
                    erros.Add("Informe a 'Temperatura da Manta Térmica.");

                if (this._TransOperatorio.BotaRetornoVenoso.IsNull() || this._TransOperatorio.BotaRetornoVenoso.Bota.IsNull())
                    erros.Add("Campo 'Bota de Retorno Venoso' é obrigatório!");
                else if (this._TransOperatorio.BotaRetornoVenoso.Bota.IsNotNull() && this._TransOperatorio.BotaRetornoVenoso.Bota.Value.Equals(SimNao.Sim) && (this._TransOperatorio.BotaRetornoVenoso.Tipo.IsEmptyOrWhiteSpace()))
                    erros.Add("Informe 'Tipo' de Bota de Retorno Venoso.");

                if (this._TransOperatorio.MeiasElasticas.IsNull())
                    erros.Add("Campo 'Meias Elásticas' é obrigatório!");

                if (this._TransOperatorio.Drenos.IsNull() || this._TransOperatorio.Drenos.Dreno.IsNull())
                    erros.Add("Campo 'Dreno' é obrigatório!");
                else if (this._TransOperatorio.Drenos.Dreno.Value.Equals(SimNao.Sim) && (this._TransOperatorio.Drenos.Tipo.IsEmptyOrWhiteSpace()))
                    erros.Add("Informe 'Tipo' de Dreno.");

                if (this._TransOperatorio.TransfusaoSanguinea.IsNull())
                    erros.Add("Campo 'Transfusão Sanguínea' é obrigatório!");
                else if (this._TransOperatorio.TransfusaoSanguinea.Value.Equals(SimNao.Sim) && this._TransOperatorio.TransfusaoSanguineaObservacao.IsEmptyOrWhiteSpace())
                    erros.Add("Informe 'Observação' na Transfusão Sanguínea.");

                if (this._TransOperatorio.GarrotePneumatico.IsNull() || this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.IsNull() )
                    erros.Add("Campo 'Garrote Pneumático' é obrigatório!");
                else if (this._TransOperatorio.GarrotePneumatico.Garrote_Pneumatico.Equals(SimNao.Sim) && this._TransOperatorio.GarrotePneumatico.Tempo.IsEmptyOrWhiteSpace())
                    erros.Add("Informe o 'Tempo' no Garrote Pneumático.");

                if (this._TransOperatorio.DestinoPaciente.IsEmptyOrWhiteSpace())
                    erros.Add("Campo 'Destino do Paciente' é obrigatório!");

                if (this._TransOperatorio.Tricotomia.IsNull())
                    erros.Add("Parte 2 - 'Tricotomia' é obrigatório!");
                if (this._TransOperatorio.PlacaEletrocauterio.IsNull())
                    erros.Add("Parte 2 - 'Placa de Eletrocautério' é obrigatório!");

                if (this._TransOperatorio.Tricotomia.IsNotNull() && this._TransOperatorio.Tricotomia.Value == SimNao.Sim && this._TransOperatorio.DomainObject.CorpoHumanoImg.IsNull())
                    erros.Add("Parte 2 - 'Tricotomia' marque a imagem");

                if (this._TransOperatorio.PlacaEletrocauterio.IsNotNull() && this._TransOperatorio.PlacaEletrocauterio.Value == SimNao.Sim && this._TransOperatorio.DomainObject.CorpoHumanoImg.IsNull())
                    erros.Add("Parte 2 - 'Placa de Eletrocautério' marque a imagem");

                if (erros.Count > 0)
                    throw new BusinessMsgException(erros, MessageImage.Error);

                this._TransOperatorio.DataEncerramento = DateTime.Now;
                this._vmchecklist.CheckListdto.CheckList.TransOperatorio = this._TransOperatorio;
                return true;
            }
        }
    }
}
