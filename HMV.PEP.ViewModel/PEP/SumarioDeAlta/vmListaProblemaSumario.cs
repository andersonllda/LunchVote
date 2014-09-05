using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Collections.ObjectModel;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Repository;

namespace HMV.PEP.ViewModel.PEP.SumarioDeAlta
{
    public class vmListaProblemaSumario : ViewModelBase
    {
        #region Contrutor
        public vmListaProblemaSumario(wrpAtendimento pAtendimento)
        {
            this._atendimento = pAtendimento;
            this._paciente = pAtendimento.Paciente;

            this._problemasPaciente = (from x in _paciente.ProblemasPaciente
                                       where x.Status.Equals(StatusAlergiaProblema.Ativo) && x.DataFim.IsNull() && x.CID.IsNotNull()
                                       select new ListaProblemaSumarioDTO()
                                       {
                                           CID = x.CID.Id,
                                           DataInicio = x.DataInicio,
                                           Descricao = x.CID.Descricao,
                                           ID = x.ID,
                                           Manter = x.Manter.HasValue && x.Manter.Equals(SimNao.Sim) ? true : false,
                                           Resolvido = false
                                       })
                .ToList()
                .ToObservableCollection();
        }
        #endregion

        #region Propriedades Privadas
        private wrpPaciente _paciente;
        private wrpAtendimento _atendimento;
        private ListaProblemaSumarioDTO _problemaSelecionado;
        private ObservableCollection<ListaProblemaSumarioDTO> _problemasPaciente;
        #endregion

        #region Propriedades Publicas
        public wrpAtendimento Atendimento { get { return _atendimento; } }

        public ListaProblemaSumarioDTO ProblemaSelecionado
        {
            get
            {
                return _problemaSelecionado;
            }
            set
            {
                _problemaSelecionado = value;
                this.OnPropertyChanged("ProblemaSelecionado");
            }
        }


        public ObservableCollection<ListaProblemaSumarioDTO> ProblemasPaciente
        {
            get
            {
                return _problemasPaciente;
            }

        }
        #endregion

        #region Commands
        protected override void CommandIncluir(object param)
        {
            base.CommandIncluir(param);
        }

        #endregion

        #region Métodos Publicos
        public void Refresh(ProblemasPaciente pProblema)
        {
            this.ProblemasPaciente.Add(new ListaProblemaSumarioDTO()
            {
                CID = pProblema.CID.Id,
                DataInicio = pProblema.DataInicio,
                Descricao = pProblema.CID.Descricao,
                ID = pProblema.ID,
                Manter = false,
                Resolvido = false
            });

            this.OnPropertyChanged("ProblemasPaciente");
        }

        public bool ValidaCIDs()
        {
            if (this.ProblemasPaciente.IsNotNull())
                foreach (ListaProblemaSumarioDTO cids in this.ProblemasPaciente)
                    if (!cids.Resolvido && !cids.Manter)
                    {
                        DevExpress.Xpf.Core.DXMessageBox.Show("Na aba ALTA MÉDICA do Sumário de Alta, informe se o CID/Problema continua ativo ou já foi resolvido.");
                        return false;
                    }
            return true;
        }

        public void Save()
        {
            if (this.ProblemasPaciente.IsNotNull())
            {
                bool alterou = false;
             
                this._paciente.ProblemasPaciente = new wrpProblemasPacienteCollection(this._paciente.DomainObject.ProblemasPaciente.ToList());

                foreach (ListaProblemaSumarioDTO cids in this.ProblemasPaciente)
                    if (cids.Resolvido || cids.Manter)
                    {
                        IPacienteService pac = ObjectFactory.GetInstance<IPacienteService>();
                        wrpProblemasPaciente problema = this._paciente.ProblemasPaciente.Where(x => x.ID.Equals(cids.ID)).Single();
                        if (cids.Resolvido)
                        {
                            problema.DataFim = DateTime.Now;
                            problema.Status = StatusAlergiaProblema.Inativo;
                        }
                        if (cids.Manter)
                            problema.Manter = SimNao.Sim;

                        pac.Salvar(this._paciente.DomainObject);

                        alterou = true;
                    }

                if (alterou)
                {
                    this._problemasPaciente = this._problemasPaciente.Where(x => x.Resolvido.Equals(false)).ToList().ToObservableCollection();
                    this.OnPropertyChanged("ProblemasPaciente");
                }
            }
        }
        #endregion
    }
}
