using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.Interfaces;
using HMV.PEP.ViewModel.BoletimEmergencia;
using HMV.PEP.ViewModel.Commands;
using StructureMap;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmCIDsAtendimento : ViewModelBase
    {
        #region Construtor
        public vmCIDsAtendimento(vmBoletimEmergencia pVM)
        {
            this._usuario = new wrpUsuarios (pVM.Usuarios);
            this._vmboletimemergencia = pVM;
            this._boletimdeemergencia = pVM.BoletimEmergencia;
            this.AddCIDsAtendimentoCommand = new AddCIDsAtendimentoCommand(this);
            this.RemoveCIDsAtendimentoCommand = new RemoveCIDsAtendimentoCommand(this);
        }
        #endregion

        #region Propriedades Publicas
        public override void EndEdit()
        {
            base.EndEdit();
            this.OnPropertyChanged("CidsAtendimentoGrid");
        }

        public String CidSelecionado
        {
            get
            {
                if (_cidselecionado != null)
                    return _cidselecionado.Id;
                return string.Empty;
            }
            set
            {

                ICidService serv = ObjectFactory.GetInstance<ICidService>();
                var meucid = serv.FiltraPorCid10(value);
                if (meucid != null)
                    this._cidselecionado = new wrpCid(meucid);
                else
                    this._cidselecionado = null;

                this.OnPropertyChanged("CidSelecionado");
                this.OnPropertyChanged("DescricaoCid");             
            }
        }

        public string DescricaoCid
        {
            get
            {
                if (_cidselecionado != null)
                    return _cidselecionado.Descricao;
                return string.Empty;
            }
        }

        public wrpUsuarios Usuario { get { return _usuario; } }

        public wrpCidCollection CidsAtendimento
        {
            get
            {
                return this._vmboletimemergencia.BoletimEmergencia.Atendimento.CIDs;
            }
        }

        public wrpAtendimento Atendimento
        {
            get
            {
                return this._vmboletimemergencia.BoletimEmergencia.Atendimento;
            }
            set
            {
                this._vmboletimemergencia.BoletimEmergencia.Atendimento = value;
            }
        }

        public List<wrpCid> CidsAtendimentoGrid
        {
            get
            {
                return (from x in this.CidsAtendimento.ToList()
                          select new wrpCid()
                          { CanDelete =  !this._boletimdeemergencia.DataAlta.HasValue, 
                              Descricao = x.Descricao,
                              Id = x.Id}).ToList();
            }
        }
        #endregion

        #region Propriedades Privadas
        private wrpUsuarios _usuario { get; set; }     
        public wrpCid _cidselecionado { get; set; }
        public wrpBoletimDeEmergencia _boletimdeemergencia { get; set; }
        private wrpCidCollection _cidAtendimento { get; set; }
        private vmBoletimEmergencia _vmboletimemergencia { get; set; }
        #endregion

        #region Metodos Publicos
        public void DeletaCID(object pCID)
        {
            this._vmboletimemergencia.BoletimEmergencia.Atendimento.DomainObject.RemoveCid(this.CidsAtendimento.Where(x => x.Id.Equals(((wrpCid)pCID).Id)).Single().DomainObject);

            if (Atendimento != null)
            {
                ICidService serv = ObjectFactory.GetInstance<ICidService>();
                if (pCID!=null)
                {
                    Paciente _paciente = Atendimento.Paciente.DomainObject;

                    IPacienteService pac = ObjectFactory.GetInstance<IPacienteService>();
                    ProblemasPaciente problema = _paciente.ProblemasPaciente.Where(x => x.CID.IsNotNull() && x.CID.Id.Equals(((wrpCid)pCID).Id) && x.Atendimento.IsNotNull() && x.Atendimento.ID.Equals(Atendimento.ID)).SingleOrDefault();
                    if (problema != null)
                    {
                        problema.Status = StatusAlergiaProblema.Excluído;
                        pac.Salvar(_paciente);
                    }
                }
            }


            //this.CidsAtendimento.Remove(this.CidsAtendimento.Where(x => x.Id.Equals(((wrpCid)pCID).Id)).Single());
            Editou();
            this.OnPropertyChanged("CidsAtendimentoGrid");
        }
        public void Editou()
        {
            this._vmboletimemergencia.Editou = true;
        }
        #endregion

        #region Metodos Privados - VAZIO

        #endregion

        #region Commands
        public ICommand AddCIDsAtendimentoCommand { get; set; }
        public ICommand RemoveCIDsAtendimentoCommand { get; set; }
        #endregion

    }
}
