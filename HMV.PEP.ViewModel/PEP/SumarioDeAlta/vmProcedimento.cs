using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Framework.ViewModelBaseClasses;
using System.Collections.Generic;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Core;
using System.Windows;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Linq;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmProcedimento : ViewModelBase
    {
        #region Contrutor
        public vmProcedimento(wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios)
        {
            this.SumarioAlta = pSumarioAlta;
            this._usuarios = pUsuarios;
            this.AddProcedimentoAltaCommand = new AddProcedimentoAltaCommand(this);
            this.RemoveProcedimentoAltaCommand = new RemoveProcedimentoAltaCommand(this);

            if (this.SumarioAlta.DomainObject.ProcedimentosAlta == null)
                this.SumarioAlta.ProcedimentosAlta = new wrpProcedimentoAltaCollection(new List<ProcedimentoAlta>());

            this.ProcedimentosAlta = this.SumarioAlta.ProcedimentosAlta;
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }
        public wrpProcedimentoAltaCollection ProcedimentosAlta { get; set; }

        public Cirurgia cirurgiaSelecionada
        {
            get
            {
                return this._cirurgiaSelecionada;
            }
            set
            {
                this._cirurgiaSelecionada = value;
                this.OnPropertyChanged("isPodeEditar");
                this.OnPropertyChanged("cirurgiaSelecionada");
            }
        }

        public wrpProcedimentoAlta procedimentoAltaSelecionado
        {
            get
            {
                return _procedimentoAltaSelecionado;
            }
            set
            {
                _procedimentoAltaSelecionado = value;
                this.OnPropertyChanged("isPodeEditar");
                this.OnPropertyChanged("ProcedimentosAlta");
                this.OnPropertyChanged("procedimentoAltaSelecionado");
            }
        }

        public SimNao NaoSeAplica
        {
            get
            {
                return this.SumarioAlta.SemProcedimentoInvasivo;
            }
            set
            {
                if (value == SimNao.Sim)
                    if (this.SumarioAlta.ProcedimentosAlta == null || this.SumarioAlta.ProcedimentosAlta.Count == 0)
                        this.SumarioAlta.SemProcedimentoInvasivo = value;
                    else
                    {
                        this.SumarioAlta.SemProcedimentoInvasivo = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver procedimentos cadastrados.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    this.SumarioAlta.SemProcedimentoInvasivo = value;

                this.OnPropertyChanged("NaoSeAplica");
            }
        }

        public IList<Cirurgia> ProcedimentosAltaItens
        {
            get
            {
                IRepositorioDeCirurgiasSumarioAlta rep = ObjectFactory.GetInstance<IRepositorioDeCirurgiasSumarioAlta>();
                //rep.OndeOProcedimentoEhPermitidoNoSumarioDeAlta();
                return rep.List().OrderBy(x => x.ds_cirurgia).ToList();
            }
        }

        public bool isPodeEditar
        {
            get { return _procedimentoAltaSelecionado != null; }
        }

        public bool FOSim
        {
            get
            {
                return (this.SumarioAlta.SemInfeccaoFO == SimNaoNA.Sim);
            }
            set
            {
                if (value)
                    this.SumarioAlta.SemInfeccaoFO = SimNaoNA.Sim;
                else
                    this.SumarioAlta.SemInfeccaoFO = null;

                OnPropertyChanged<vmProcedimento>(x => x.FOSim);
                OnPropertyChanged<vmProcedimento>(x => x.FONao);
                OnPropertyChanged<vmProcedimento>(x => x.FONaoSeAplica);
            }
        }

        public bool FONao
        {
            get
            {
                return (this.SumarioAlta.SemInfeccaoFO == SimNaoNA.Nao);
            }
            set
            {
                if (value)
                    this.SumarioAlta.SemInfeccaoFO = SimNaoNA.Nao;
                else
                    this.SumarioAlta.SemInfeccaoFO = null;

                OnPropertyChanged<vmProcedimento>(x => x.FOSim);
                OnPropertyChanged<vmProcedimento>(x => x.FONao);
                OnPropertyChanged<vmProcedimento>(x => x.FONaoSeAplica);
            }
        }

        public bool FONaoSeAplica
        {
            get
            {
                return (this.SumarioAlta.SemInfeccaoFO == SimNaoNA.NA);
            }
            set
            {
                if (value)
                    this.SumarioAlta.SemInfeccaoFO = SimNaoNA.NA;
                else
                    this.SumarioAlta.SemInfeccaoFO = null;

                OnPropertyChanged<vmProcedimento>(x => x.FOSim);
                OnPropertyChanged<vmProcedimento>(x => x.FONao);
                OnPropertyChanged<vmProcedimento>(x => x.FONaoSeAplica);
            }
        }

        public bool TemProcedimento
        {
            get
            {
                return ProcedimentosAlta.HasItems();
            }
        }

        #endregion

        #region Commands
        public ICommand AddProcedimentoAltaCommand { get; set; }
        public ICommand RemoveProcedimentoAltaCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        wrpProcedimentoAlta _procedimentoAltaSelecionado { get; set; }
        Cirurgia _cirurgiaSelecionada { get; set; }
        Usuarios _usuarios { get; set; }
        #endregion

        #region Metodos
        public void Refresh()
        {
            OnPropertyChanged<vmProcedimento>(x => x.TemProcedimento);
            if (!ProcedimentosAlta.HasItems())
                this.SumarioAlta.SemInfeccaoFO = null;
        }
        public void SetaProcedimento()
        {
            //if (_procedimentoAltaSelecionado != null)                        
            this.procedimentoAltaSelecionado = new wrpProcedimentoAlta(this.cirurgiaSelecionada, this._usuarios);
            this.procedimentoAltaSelecionado.Atendimento = this.SumarioAlta.Atendimento;
            //this.AddProcedimentoAltaCommand.Execute(null);            
        }

        #endregion
    }
}
