using System;
using System.Linq;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository.SumarioAvaliacaoMedica;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmResumoDoProntuarioAmbulatorial : ViewModelBase
    {
        #region Contrutor
        public vmResumoDoProntuarioAmbulatorial(Paciente paciente)
        {
            this._paciente = new wrpPaciente(paciente);
            IRepositorioDeSumariosAvaliacaoMedica rep = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedica>();
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedicaCollection(rep.OndeTipoSumarioAmbulatorios().OndePacienteIgual(paciente).List());
        }

        public vmResumoDoProntuarioAmbulatorial(Paciente paciente, int atend)
        {
            this._paciente = new wrpPaciente(paciente);
            
            IRepositorioDeSumariosAvaliacaoMedica rep = ObjectFactory.GetInstance<IRepositorioDeSumariosAvaliacaoMedica>();
            
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedicaCollection(rep.OndeTipoSumarioAmbulatorios().OndePacienteIgual(paciente).List());

            this._sumarioavaliacaoselecionado = (atend > 0) ? this._sumarioavaliacaomedica.Where(x => x.SigaAtendimento != null && x.SigaAtendimento.ID.Equals(atend)).FirstOrDefault() : null;
        }
        #endregion

        #region Propriedades Públicas
        public wrpSumarioAvaliacaoMedicaCollection ListaSumarioDeAvaliacaoMedica
        {
            get
            {
                return this._sumarioavaliacaomedica;
            }
        }

        public wrpSumarioAvaliacaoMedica SumarioDeAvalicaoSelecionado
        {
            get 
            { 
                return _sumarioavaliacaoselecionado; 
            }
            set
            {
                this._sumarioavaliacaoselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmResumoDoProntuario>(x => x.SumarioDeAvalicaoSelecionado));
            }
        }

        public ResumoDoProntuarioFiltros ItemFiltroSelecionado
        {
            get
            {
                return this._itemFiltroSelecionado;
            }
            set
            {
                this._itemFiltroSelecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmResumoDoProntuario>(x => x.ItemFiltroSelecionado));
            }
        }

        public wrpSumarioAvaliacaoMedicaCollection SumarioavaliacaoMedicaPorPeriodo
        {
            get
            {
                return this._sumarioavaliacaomedicaPorPeriodo;
            }
        }





        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento { get; set; }
        private wrpPaciente _paciente { get; set; }
        private wrpSumarioAvaliacaoMedicaCollection _sumarioavaliacaomedica { get; set; }
        private wrpSumarioAvaliacaoMedica _sumarioavaliacaoselecionado { get; set; }
        private ResumoDoProntuarioFiltros _itemFiltroSelecionado { get; set; }

        private wrpSumarioAvaliacaoMedicaCollection _sumarioavaliacaomedicaPorPeriodo { get; set; }
        #endregion

        #region Métodos Publicos
        /// <summary>
        /// Filtro para o resumo do prontuário...
        /// </summary>
        /// <param name="DataInicio">Informe uma data de início para a pesquisa</param>
        /// <param name="DataFim">Informe uma data final para a pesquisa</param>
        /// <param name="_usuario">Informe para filtrar somente os sumarios do usuario logado.</param>
        public void Filtros(DateTime? DataInicio, DateTime? DataFim, wrpUsuarios _usuario)
        {
            if ((!DataInicio.IsNull()) && (!DataFim.IsNull()))
            {
                if (this._itemFiltroSelecionado.Equals(ResumoDoProntuarioFiltros.Todos))
                {
                    _sumarioavaliacaomedicaPorPeriodo = new wrpSumarioAvaliacaoMedicaCollection((from todos in ListaSumarioDeAvaliacaoMedica
                                                                                                 where todos.DataProntuario >= DataInicio
                                                                                                 && todos.DataProntuario <= DataFim
                                                                                                 select todos).ToList().OrderByDescending(x => x.DataProntuario).Select(p => p.DomainObject).ToList());
                }
                else if (this._itemFiltroSelecionado.Equals(ResumoDoProntuarioFiltros.MeusRegistros))
                {
                    _sumarioavaliacaomedicaPorPeriodo = new wrpSumarioAvaliacaoMedicaCollection((from todos in ListaSumarioDeAvaliacaoMedica
                                                                                                 where todos.DataProntuario >= DataInicio
                                                                                                 &&    todos.DataProntuario <= DataFim
                                                                                                 &&    todos.Usuario.Usuario.cd_usuario == _usuario.cd_usuario
                                                                                                 select todos).ToList().OrderByDescending(x => x.DataProntuario).Select(p => p.DomainObject).ToList());
                }
            }
            else
            {
                if (this._itemFiltroSelecionado.Equals(ResumoDoProntuarioFiltros.Todos))
                {
                    _sumarioavaliacaomedicaPorPeriodo = new wrpSumarioAvaliacaoMedicaCollection((from todos in ListaSumarioDeAvaliacaoMedica
                                                                                                 select todos).ToList().OrderByDescending(x => x.DataProntuario).Select(p => p.DomainObject).ToList());
                }
                else if (this._itemFiltroSelecionado.Equals(ResumoDoProntuarioFiltros.MeusRegistros))
                {
                    _sumarioavaliacaomedicaPorPeriodo = new wrpSumarioAvaliacaoMedicaCollection((from todos in ListaSumarioDeAvaliacaoMedica
                                                                                                 where todos.Usuario.Usuario.cd_usuario == _usuario.cd_usuario
                                                                                                 select todos).ToList().OrderByDescending(x => x.DataProntuario).Select(p => p.DomainObject).ToList());
                }
            }
            
        }
        #endregion
    }
}
