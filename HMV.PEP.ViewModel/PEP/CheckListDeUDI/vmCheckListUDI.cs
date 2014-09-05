using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;
using DevExpress.Xpf.Core;
using System.Windows;

namespace HMV.PEP.ViewModel.PEP.CheckListDeUDI
{
    public class vmCheckListUDI : ViewModelBase
    {
        #region ----- Construtor -----
        public vmCheckListUDI(Atendimento pAtendimento, Usuarios pUsuario)
        {
            this._atendimento = new wrpAtendimento(pAtendimento);
            this._usuarios = new wrpUsuarios(pUsuario);                       

            if (this._atendimento.DomainObject.DescricaoCirurgica.Count.Equals(0))
                DXMessageBox.Show("Não existe 'Aviso Cirurgia' para este atendimento.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);

            this.CarregaListaCheckList();

            this.getBotoes();            
        }
        #endregion

        #region ----- Propriedades Privadas -----
        private wrpAtendimento _atendimento;
        IList<CheckListDTO> _checklistcollection;
        private CheckListDTO _checklistdto;
        private wrpUsuarios _usuarios;
        private vmAntesDaEntrada _vmAntesDaEntrada;        
        private vmTimeOut _vmTimeOut;
        private vmCheckOut _vmCheckOut;               
        private bool TemCheckList { get { return this._checklistdto.IsNotNull() && this._checklistdto.CheckList.IsNotNull(); } }
        #endregion

        #region ----- Propriedades Públicas -----

        public vmAntesDaEntrada vmAntesDaEntrada
        {
            get { return this._vmAntesDaEntrada; }
        }

        public vmTimeOut vmTimeOut
        {
            get { return this._vmTimeOut; }
        }

        public vmCheckOut vmCheckOut
        {
            get { return this._vmCheckOut; }
        }   

        public wrpUsuarios Usuario
        {
            get { return this._usuarios; }
        }

        public IList<CheckListDTO> CheckListCollection
        {
            get
            {
                return _checklistcollection;
            }
        }

        public CheckListDTO CheckListdto
        {
            get { return this._checklistdto; }
            set
            {
                this._checklistdto = value;
                this.getBotoes();
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckListUDI>(x => x.CheckListdto));
            }
        }

        public wrpPaciente Paciente
        {
            get { return this._atendimento.Paciente; }
        }

        public bool boolExcluir
        {
            get { return !this._checklistdto.IsNull() && !this._checklistdto.CheckList.IsNull(); }
        }

        public bool boolEntrada
        {
            get
            {
                if (this._atendimento.DomainObject.DescricaoCirurgica.Count.Equals(0))
                    return false;
                return !TemCheckList || (this._checklistdto.CheckList.AntesEntradaUDI.IsNull() || this._checklistdto.CheckList.AntesEntradaUDI.DataEncerramento.IsNull());
            }
        }
        public bool boolCheckout
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.CheckOutUDI.IsNotNull() && this._checklistdto.CheckList.CheckOutUDI.DataEncerramento.IsNull())
                        return true;
                return false;
            }
        }
        public bool boolTimeOut 
        {
            get
            {
                if (TemCheckList)
                    if (this._checklistdto.CheckList.TimeOutUDI.IsNotNull() && this._checklistdto.CheckList.TimeOutUDI.DataEncerramento.IsNull())                        
                        return true;
                return false;
            }
        }
               
        public bool BotoesVisiveis
        {
            get
            {
                return this._checklistdto.IsNotNull()
                    && this._checklistdto.CheckList.IsNotNull()                                       
                    && this._checklistdto.CheckList.DataEncerramento.IsNotNull();
            }
        }
        #endregion

        #region ----- Métodos Privados -----       
        private wrpUsuarios getUsuarioEncerramento(AvisoCirurgia T)
        {
            if (T.CheckListUDI.IsNull())
                return null;
            return new wrpUsuarios(T.CheckListUDI.Usuario);
        }

        private wrpCheckListUDI getCheckList(AvisoCirurgia T)
        {
            if (T.CheckListUDI.IsNull())
                return null;
            return new wrpCheckListUDI(T.CheckListUDI);
        }

        private wrpPrestador getPrestador(AvisoCirurgia T)
        {
            if (T.CheckListUDI != null)
                return new wrpPrestador(T.CheckListUDI.Prestador);

            if (T.EquipesMedicas.Count(xx => xx.Principal.Equals(SimNao.Sim)) > 0)
                return new wrpPrestador(T.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador);
            return new wrpPrestador(T.EquipesMedicas.Where(x => x.AtividadeMedica.OrdemApresentacao.Equals(1)).FirstOrDefault().Prestador);
        }

        private wrpCirurgia getCirurgia(AvisoCirurgia T)
        {
            if (T.CheckListUDI != null)
                return new wrpCirurgia(T.CheckListUDI.Cirurgia);

            return new wrpCirurgia(T.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia);
        }

        private BitmapImage getImagemCheckList(AvisoCirurgia T)
        {
            if (T.CheckListUDI == null)
                return null;

            return new BitmapImage(new Uri(@"/HMV.Core.Framework.WPF;component/Images/CheckList.png", UriKind.Relative));
        }

        private void getBotoes()
        {
            this.OnPropertyChanged<vmCheckListUDI>(x => x.boolExcluir);
            this.OnPropertyChanged<vmCheckListUDI>(x => x.boolEntrada);            
            this.OnPropertyChanged<vmCheckListUDI>(x => x.boolTimeOut);
            this.OnPropertyChanged<vmCheckListUDI>(x => x.boolCheckout);                    
            this.OnPropertyChanged<vmCheckListUDI>(x => x.BotoesVisiveis);
        }
        #endregion

        #region ----- Métodos Públicos -----
        public void IniciaVMS()
        {
            this._vmAntesDaEntrada = new vmAntesDaEntrada(this);           
            this._vmTimeOut = new vmTimeOut(this);            
            this._vmCheckOut = new vmCheckOut(this);            
        }

        public void CarregaListaCheckList()
        {
            this._checklistcollection = (from T in this._atendimento.DomainObject.DescricaoCirurgica
                                      select new CheckListDTO
                                      {
                                          CheckList = this.getCheckList(T),
                                          Cirurgia = this.getCirurgia(T),
                                          Prestador = this.getPrestador(T),
                                          DataAviso = T.DataAviso,
                                          AvisoCirurgia = new wrpAvisoCirurgia(T),
                                          Img = this.getImagemCheckList(T),
                                          UsuarioEncerramento = this.getUsuarioEncerramento(T)
                                      }).ToList();
        }

        public void Novo()
        {
            this._checklistdto.CheckList = new wrpCheckListUDI(this._usuarios)
            {
                Atendimento = this._atendimento,
                AvisoCirurgia = this._checklistdto.AvisoCirurgia,
                Cirurgia = this._checklistdto.Cirurgia,
                Paciente = this._atendimento.Paciente,
                Prestador = this._checklistdto.Prestador
            };
        }      

        public void Salvar()
        {
            this._checklistdto.CheckList.Paciente.Save();
            this._checklistdto.CheckList.Save();

            if (this._vmAntesDaEntrada.IsNotNull())
                this._vmAntesDaEntrada.SalvaAlergias();

            this._checklistdto.CheckList.AvisoCirurgia.DomainObject.Refresh();
        }

        public void SalvarFechar(ViewModelBase pmv)
        {
            if (pmv.GetType().Equals(typeof(vmAntesDaEntrada)))
                this._checklistdto.CheckList.AntesEntradaUDI = this._vmAntesDaEntrada.AntesEntradaPaciente;
            else if (pmv.GetType().Equals(typeof(vmTimeOut)))
                this._checklistdto.CheckList.TimeOutUDI = this._vmTimeOut.TimeOut;
            else if (pmv.GetType().Equals(typeof(vmCheckOut)))
            {
                this._checklistdto.CheckList.CheckOutUDI = this._vmCheckOut.CheckOut;
                this._checklistdto.CheckList.CheckOutUDI.CheckOutMaterialUDI = this._vmCheckOut.CheckOutMaterialUDILista;
            }
            
            Salvar();
        }

        public void Remover()
        {
            this._checklistdto.CheckList.DomainObject.DataCancelamento = DateTime.Now;
            Salvar();
            this._atendimento.DomainObject.DescricaoCirurgica.FirstOrDefault(x => x.Id.Equals(this._checklistdto.AvisoCirurgia.cd_aviso_cirurgia)).Refresh();
            Refresh();
        }

        public void Refresh()
        {
            CarregaListaCheckList();
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCheckListUDI>(x => x.CheckListCollection));
        }
        #endregion

        #region ----- Commands -----

        #endregion
    }

    public class CheckListDTO
    {
        public DateTime DataAviso { get; set; }
        public wrpCirurgia Cirurgia { get; set; }
        public wrpAvisoCirurgia AvisoCirurgia { get; set; }
        public wrpPrestador Prestador { get; set; }
        public BitmapImage Img { get; set; }
        public wrpCheckListUDI CheckList { get; set; }
        public wrpUsuarios UsuarioEncerramento { get; set; }
    }
}
