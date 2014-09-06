using System.Collections.Generic;
using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Wrappers;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Model.ControleAcesso;
using System;
using System.Linq;
using System.Configuration;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmRegistrosDeDor : ViewModelBase
    {
        #region Contrutor
        public vmRegistrosDeDor(Paciente paciente)
        {
            wrpPaciente objPaciente = new wrpPaciente(paciente);
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.OndeCodigoPacienteIgual(paciente.ID);
            this._Atendimentos = new wrpAtendimentoCollection(rep.OndeExisteRegistroDeDor().List());
            this._ListaRegistroDor = ListarRegistroDor();
            this._ListaRegistroDorEvolucao = new wrpRegistroDorCollection(null);
        }
        #endregion

        #region Propriedades Publicas

        public wrpAtendimentoCollection Atendimentos
        {
            get { return this._Atendimentos; }

        }

        public wrpAtendimento Atendimento
        {
            get { return this._Atendimentos.FirstOrDefault(); }

        }

        public wrpRegistroDorCollection ListaRegistroDor
        {
            get { return this._ListaRegistroDor; }
          
        }

        public wrpRegistroDorCollection ListaRegistroDorEvolucaoGrafico 
        {
            get
            {
                wrpRegistroDorCollection ListaRetorno = new wrpRegistroDorCollection(null);
                foreach (wrpRegistroDor item in this._ListaRegistroDorEvolucao)
                 if (item.Selecionado)
                    ListaRetorno.Add(item);

                return ListaRetorno;

            }
        }

        public wrpRegistroDorCollection ListaRegistroDorEvolucao
        {

            get
            {
                DesMarcaTodosRegistroDor();
                var lista = this._ListaRegistroDor.Where(x => x.PontoReferencia.Articulacao.DomainObject == this.ArticulacaoSeleciona.DomainObject && 
                                                         x.DataInclusao.Date >= this.DataInicial.Date && x.DataInclusao.Date <= this.DataFinal.Date).ToList();
                
                this._ListaRegistroDorEvolucao.Clear();
                foreach (wrpRegistroDor item in lista)
                    this._ListaRegistroDorEvolucao.Add(item);
                
                
                return this._ListaRegistroDorEvolucao;

            }
            set { this._ListaRegistroDorEvolucao = value; }
        }

        public wrpPontoReferenciaCollection ListaPontoReferencia
        {
            get
            {
                List<PontoReferencia> ListaPontoReferencia = new List<PontoReferencia>();
                foreach (wrpRegistroDor item in this._ListaRegistroDor.ToList())
               
                    ListaPontoReferencia.Add(item.PontoReferencia.DomainObject);
                
                return new wrpPontoReferenciaCollection(ListaPontoReferencia);
            }
        }

        public wrpArticulacao ArticulacaoSeleciona
        {
            get
            {
                if (this._ArticulacaoSeleciona == null)
                    return this.ListaArticulacao.FirstOrDefault();
                else
                    return this._ArticulacaoSeleciona;
            }
            set
            {
                this._ArticulacaoSeleciona = value;
                this.OnPropertyChanged("ArticulacaoSeleciona");
                this.OnPropertyChanged("ListaRegistroDorEvolucao");
            }
        }

        public wrpArticulacaoCollection ListaArticulacao
        {
            get
            {
                List<Articulacao> objlista = new List<Articulacao>();
                foreach (wrpPontoReferencia item in this.ListaPontoReferencia.ToList())
                    objlista.Add(item.Articulacao.DomainObject);
                return new wrpArticulacaoCollection(objlista.Distinct().ToList());
            }
        }

        public DateTime DataInicial
        {
            get
            {
                if (this._DataInicial == DateTime.MinValue)
                    return ListaRegistroDor.OrderBy(x=>x.DataInclusao).FirstOrDefault().DataInclusao;
                else
                    return this._DataInicial;
            }
            set
            {
                this._DataInicial = value;
                this.OnPropertyChanged("ListaRegistroDorEvolucao");
            }
        }

        public DateTime DataFinal
        {
            get
            {
                if (this._DataFinal == DateTime.MinValue)
                    return ListaRegistroDor.OrderBy(x=>x.DataInclusao).LastOrDefault().DataInclusao;
                else
                    return this._DataFinal;
            }
            set
            {
                this._DataFinal = value;
                this.OnPropertyChanged("ListaRegistroDorEvolucao");
            }
        }
        
        public bool HabilitaEvolucao
        {
            get
            {
                return this.ListaRegistroDor.Count == 0 ? false : true;
            }
        }

        #endregion

        #region Propriedades Privadas
        private wrpAtendimentoCollection _Atendimentos { get; set; }
        private wrpRegistroDorCollection _ListaRegistroDor;
        public wrpRegistroDorCollection _ListaRegistroDorEvolucao;

        private wrpArticulacao _ArticulacaoSeleciona { get; set; }
        private DateTime _DataInicial { get; set; }
        private DateTime _DataFinal { get; set; }

        #endregion

        #region Metodos Publicos
    
        public void MarcaTodosRegistroDor()
        {
            foreach (wrpRegistroDor item in this._ListaRegistroDor)
                item.Selecionado = true;
        }

        public void DesMarcaTodosRegistroDor()
        {
            foreach (wrpRegistroDor item in this._ListaRegistroDor)
                item.Selecionado = false;
          
        }
       
        #endregion

        #region Metodos Privados
       
        private wrpRegistroDorCollection ListarRegistroDor()
        {

            wrpRegistroDorCollection objLista = new wrpRegistroDorCollection(null);
            var listaRegistroAtendimentosSelectToList = this._Atendimentos.SelectMany(x => x.RegistroDor);

            foreach (wrpRegistroDor itemUnico in listaRegistroAtendimentosSelectToList)
                    objLista.Add(itemUnico);

            return objLista;
        }

        #region Commands
        #endregion

        #endregion

        public override string this[string columnName]
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override string Error
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

    }
}
