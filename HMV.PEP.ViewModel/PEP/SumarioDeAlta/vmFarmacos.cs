using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Consult;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.Extensions;
using System;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.ViewModel.SumarioDeAlta
{
    public class vmFarmacos : ViewModelBase
    {
        #region Contrutor
        public vmFarmacos(wrpSumarioAlta pSumarioAlta, Usuarios pUsuarios)
        {
            this.SumarioAlta = pSumarioAlta;
            this._usuarios = pUsuarios;
            this.AddFarmacoCommand = new AddFarmacoCommand(this);
            this.RemoveFarmacoCommand = new RemoveFarmacoCommand(this);

            this.Farmacos = this.SumarioAlta.Farmacos;

            //Adiciona todos farmacos na entrada...
            var aff = this.FarmacoItens; //apenas para dar o get e associar a variavel _produtosselecionados pq tenho que fazer isso rapido e nao da tempo de arrumar codigo dos outros...
            this.MarcaTodosProdutos();
            this.SetaFarmaco();
        }
        #endregion

        #region Propriedades Publicas
        public wrpSumarioAlta SumarioAlta { get; set; }

        private wrpFarmacoCollection _Farmacos { get; set; }
        public wrpFarmacoCollection Farmacos { get { return _Farmacos; } set { _Farmacos = value; OnPropertyChanged("Farmacos"); } }
        public wrpFarmaco FarmacoSelecionado
        {
            get { return this._farmacoselecionado; }
            set
            {
                this._farmacoselecionado = value;
                this.OnPropertyChanged("FarmacoSelecionado");
            }
        }

        public string Observacao
        {
            get { return this.SumarioAlta.FarmacoObservacao; }
            set
            {
                this.SumarioAlta.FarmacoObservacao = value;
                this.OnPropertyChanged("Observacao");
            }
        }

        public SimNao SemFarmaco
        {
            get
            {
                return this.SumarioAlta.SemFarmaco;
            }
            set
            {
                if (value == SimNao.Sim)
                    if (this.SumarioAlta.Farmacos == null || this.SumarioAlta.Farmacos.Count == 0)
                        this.SumarioAlta.SemFarmaco = value;
                    else
                    {
                        this.SumarioAlta.SemFarmaco = SimNao.Nao;
                        DXMessageBox.Show("Não é possível marcar esta opção enquanto houver fármacos cadastrados.", "Atenção!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                else
                    this.SumarioAlta.SemFarmaco = value;

                this.OnPropertyChanged("SemFarmaco");
            }
        }

        public wrpProdutoCollection FarmacoItens
        {
            get
            {               
                List<Produto> Lista = new List<Produto>();
                IProdutoConsult consult = ObjectFactory.GetInstance<IProdutoConsult>();
                var T = new wrpProdutoCollection(consult.carregaProdutoFarmaco(this.SumarioAlta.Atendimento.DomainObject).OrderBy(x => x.Descricao).ToList());
                foreach (var item in T)
                {
                    if (Farmacos.Count(x => x.Produto.Id == item.Id) == 0)                      
                            Lista.Add(item.DomainObject);
                }

                var repA = ObjectFactory.GetInstance<HMV.Core.Domain.Repository.IRepositorioDeAtendimento>();
                var ret = repA.OndeCodigoPacienteIgual(this.SumarioAlta.Atendimento.Paciente.ID).List();
                if (ret.IsNotNull())
                {
                    var atendmentosanteriores = ret.Where(x => x.DataAlta >= DateTime.Now.AddDays(-1)).ToList();
                    if (atendmentosanteriores.HasItems())
                        if (atendmentosanteriores.Count(x => x.AtendimentoPai.IsNotNull()) > 0)
                        {
                            Lista = new List<Produto>();
                            IProdutoConsult consulta = ObjectFactory.GetInstance<IProdutoConsult>();
                            var Ta = new wrpProdutoCollection(consulta.carregaProdutoFarmaco(atendmentosanteriores.LastOrDefault()).OrderBy(x => x.Descricao).ToList());
                            foreach (var item in Ta)
                            {
                                if (Farmacos.Count(x => x.Produto.Id == item.Id) == 0)                                    
                                        Lista.Add(item.DomainObject);
                            }
                        }
                }

                return this._produtosselecionados = new wrpProdutoCollection(Lista);
            }
        }

        public bool HabilitaIncluir
        {
            get
            {
                return this.FarmacoItens.Count == 0 ? false : true;
            }
        }
        #endregion

        #region Commands
        public ICommand AddFarmacoCommand { get; set; }
        public ICommand RemoveFarmacoCommand { get; set; }
        #endregion

        #region Propriedades Privadas
        wrpFarmaco _farmacoselecionado { get; set; }
        private wrpProdutoCollection _produtosselecionados { get; set; }
        Usuarios _usuarios { get; set; }
        #endregion

        #region Metodos
        public bool HabilitarBotaoSelecionar()
        {
            return _produtosselecionados.Count(x => x.Selecionado == true) > 0;
        }

        public void SetaFarmaco()
        {
            foreach (var item in _produtosselecionados)
            {
                if (item.Selecionado)
                {
                    this.FarmacoSelecionado = new wrpFarmaco(item.DomainObject, this._usuarios);
                    this.FarmacoSelecionado.Atendimento = this.SumarioAlta.Atendimento;
                    this.AddFarmacoCommand.Execute(null);
                }
            }
            DesMarcaTodos();
            this.OnPropertyChanged("HabilitaIncluir");
        }

        public void MarcaTodos()
        {
            foreach (var item in this.Farmacos)
            {
                item.Selecionado = true;
            }
            OnPropertyChanged("Farmacos");
        }

        public void DesMarcaTodos()
        {
            foreach (var item in this.Farmacos)
            {
                item.Selecionado = false;
            }
            OnPropertyChanged("Farmacos");
        }

        public void MarcaTodosProdutos()
        {
            foreach (var item in _produtosselecionados)
            {
                item.Selecionado = true;
            }
        }

        public void DesmarcaTodosProdutos()
        {
            foreach (var item in _produtosselecionados)
            {
                item.Selecionado = false;
            }
        }

        public void Refresh()
        {
            this.OnPropertyChanged("HabilitaIncluir");
        }
        #endregion
    }
}
