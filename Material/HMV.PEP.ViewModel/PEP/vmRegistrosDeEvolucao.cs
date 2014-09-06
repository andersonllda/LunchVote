using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows.Input;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmRegistrosDeEvolucao : ViewModelBase
    {
        #region Contrutor
        public vmRegistrosDeEvolucao(Paciente pPaciente, Siga_Profissional pProfissional, Atendimento pAtendimento)
        {

            this._profissional = new wrpSiga_Profissional(pProfissional);

            IRepositorioSubClinica rep = ObjectFactory.GetInstance<IRepositorioSubClinica>();
            this._subclinica = new wrpSubClinica(rep.OndeIDIgual(Convert.ToInt32(ConfigurationManager.AppSettings["SubClinicaDefault"])).Single());
            this._paciente = new wrpPaciente(pPaciente);

            this.SaveRegistrosDeEvolucaoCommand = new SaveRegistrosDeEvolucaoCommand(this);

            this.PodeIncluir = false;
            if (pAtendimento != null)
                if (pAtendimento.TipoDeAtendimento == TipoAtendimento.Ambulatorio)
                    this.PodeIncluir = true;
        }
        #endregion

        #region Propriedades Publicas

        public string Evolucao
        {
            get
            {
                return this._registrosdeevolucao.Evolucao;
            }
            set
            {
                this._registrosdeevolucao.Evolucao = value;
                this.OnPropertyChanged("Evolucao");
                this.OnPropertyChanged("ItemSelecionado");
            }
        }

        public wrpRegistrosDeEvolucaoCollection RegistrosDeEvolucao
        {
            get
            {
                return this._paciente.RegistrosDeEvolucao;
            }
        }

        public wrpPaciente Paciente { get { return _paciente; } }

        public RegistrosEvolucao ItemSelecionado
        {
            get
            {
                return this._itemselecionado;
            }
            set
            {
                this._itemselecionado = value;
                this.OnPropertyChanged("ItemSelecionado");
                this.OnPropertyChanged("RegistrosDeEvolucao2");
            }
        }

        public List<wrpRegistrosDeEvolucao> RegistrosDeEvolucao2
        {
            get
            {
                if (this._itemselecionado == RegistrosEvolucao.RegistrosCentro)
                {
                    return (from x in this.RegistrosDeEvolucao.ToList()
                            where (x.Centro.ID.Equals(this._subclinica.ID))
                            select x).ToList();
                }
                else if (this._itemselecionado == RegistrosEvolucao.MeusRegistros)
                {
                    return (from x in this.RegistrosDeEvolucao.ToList()
                            where (x.Profissional.cod_profissional.Equals(this._profissional.cod_profissional))
                            select x).ToList();
                }
                else
                {
                    return this.RegistrosDeEvolucao.ToList();
                }
            }
        }

        public bool PodeIncluir { get; set; }

        public bool PodeImprimir { get { return (RegistrosDeEvolucao2.Count > 0 ? true : false); } }
        #endregion

        #region Commands

        public ICommand SaveRegistrosDeEvolucaoCommand { get; set; }

        #endregion

        #region Metodos

        public void NovoRegistro()
        {
            this._registrosdeevolucao = new wrpRegistrosDeEvolucao(new Core.Domain.Model.PEP.RegistrosDeEvolucao(this._paciente.DomainObject, this._subclinica.DomainObject, this._profissional.DomainObject));
        }


        public void Save()
        {
            IRepositorioDePacientes rep = ObjectFactory.GetInstance<IRepositorioDePacientes>();
            this._paciente.RegistrosDeEvolucao.Add(this._registrosdeevolucao);
            rep.Save(this._paciente.DomainObject);
        }

        public void AtualizaListaEvolucao()
        {
            this.OnPropertyChanged("RegistrosDeEvolucao2");
        }

        #endregion

        #region Propriedades Privadas
        // private wrpRegistrosDeEvolucaoCollection _registrosdeevolucoes { get; set; }
        private wrpRegistrosDeEvolucao _registrosdeevolucao { get; set; }
        private wrpSiga_Profissional _profissional { get; set; }
        private wrpSubClinica _subclinica { get; set; }
        private wrpPaciente _paciente { get; set; }
        private RegistrosEvolucao _itemselecionado { get; set; }

        #endregion


    }
}
