using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using DevExpress.Xpf.Core;
using HMV.Core.Interfaces;
using System.Configuration;
using StructureMap.Pipeline;
using HMV.Core.Domain.Model;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmCIDListaProblemaPrescricao : ViewModelBase
    {

          #region Construtor
        public vmCIDListaProblemaPrescricao(wrpAtendimento pAtendimento, wrpUsuarios pUsuario)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            this.CID = this._atendimento.Cid;

            this._problemasPaciente = (from x in this._atendimento.Paciente.ProblemasPaciente
                                       where x.Status.Equals(StatusAlergiaProblema.Ativo) && x.CID.IsNotNull()
                                       select new ListaProblemaSumarioDTO()
                                       {
                                           CID = x.CID.Id,
                                           DataInicio = x.DataInicio,
                                           Descricao = x.CID.Descricao,
                                           ID = x.ID
                                       })
                .ToList()
                .ToObservableCollection();
            
            this.Commands.AddCommand(enumCommand.CommandSalvar, IncluirNaListaProblemas);
        }
        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento { get; set; }
        private wrpUsuarios _usuario { get; set; }
        private ObservableCollection<ListaProblemaSumarioDTO> _problemasPaciente;
        private wrpCid _cid { get; set; }
        private bool _salvou;
        
        
        #endregion

        #region Propriedades Públicas
        public ObservableCollection<ListaProblemaSumarioDTO> ProblemasPaciente
        {
            get
            {
                return _problemasPaciente;
            }

        }

        public wrpCid CID
        {
            get
            {
                return _cid;            
            }
            set
            {
                _cid = value;
                this.OnPropertyChanged("CID");
            }        
        }
       
        #endregion

        #region Eventos
        public event EventHandler GoShow;
        public event Action GoClose;
        #endregion

        #region Métodos Privados
        private void IncluirNaListaProblemas(object parameter)
        {
            if (this.CID == null)
            {
                DXMessageBox.Show("Favor informe o CID principal do Atendimento");
                return;
            }

            ICidService srv = ObjectFactory.GetInstance<ICidService>();            
            srv.verificaSeOCIDJaEstaNaListaDeProblemas(this.CID.DomainObject, this._atendimento.DomainObject, this._usuario.DomainObject);
            this._atendimento.Cid = this.CID;

            this._atendimento.Save();
            DXMessageBox.Show("CID Principal Confirmado!");
            _salvou = true;

            logAcesso();
            GoClose();
        }

        private void logAcesso()
        {
            ISistemaService srvsis = ObjectFactory.GetInstance<ISistemaService>();
            Sistemas sis = srvsis.FiltraPorId(int.Parse(ConfigurationManager.AppSettings["Sistema"].ToString()));
            ExplicitArguments args = new ExplicitArguments();
            args.SetArg("sistema", sis.ID);
            IAcessoSistemaLogService srvlog = ObjectFactory.GetInstance<IAcessoSistemaLogService>(args);

            SistemasLog log = new SistemasLog
            {
                Sistema = sis
              ,
                Acao = Acao.Inserir
              ,
                Usuarios = this._usuario.DomainObject
              ,
                Data = DateTime.Now
              ,
                Tabela = "LOG_CID_ATENDIMENTO_LISTA_PROBLEMA"
              ,
                Dispositivo = Environment.MachineName
              ,
                Chave = this._atendimento.ID
            };

            srvlog.Gravar(log);
        }

       
        #endregion

        #region Métodos Públicos
        public bool CancelaGrava()
        {
            if (!_salvou)
                DXMessageBox.Show("Confirme ou informe o CID principal do Atendimento!");            

            return _salvou;     
        }
        #endregion
    }
}
