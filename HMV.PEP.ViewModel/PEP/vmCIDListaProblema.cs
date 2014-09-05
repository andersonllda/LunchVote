using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using StructureMap;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmCIDListaProblema : ViewModelBase
    {
        #region Construtor
        public vmCIDListaProblema(wrpAtendimento pAtendimento, wrpUsuarios pUsuario)
        {
            this._atendimento = pAtendimento;
            this._usuario = pUsuario;
            this._sigaprofissional = this._usuario.Profissional;
            this._sigaproblemalogcollection = this._atendimento.SigaProblemaLog;

            AtualizaCIDsMV();
            CidsDiferentes();

            this.Commands.AddCommand(enumCommand.CommandIncluir, ShowIncluir);
            this.Commands.AddCommand(enumCommand.CommandSalvar, IncluirNaListaProblemas);
            this.Commands.AddCommand(enumCommand.CommandCancelar, CancelarLOG);
        }
        #endregion

        #region Propriedades Privadas
        private wrpAtendimento _atendimento { get; set; }
        private wrpUsuarios _usuario { get; set; }
        private wrpSiga_Profissional _sigaprofissional { get; set; }
        private ObservableCollection<CidDiferentes> _cidsmvdivergentes { get; set; }
        private CidDiferentes _cidselecionado { get; set; }
        private wrpSigaProblemaLogCollection _sigaproblemalogcollection { get; set; }
        private bool _salvou { get; set; }
        #endregion

        #region Propriedades Públicas
        public ObservableCollection<CidDiferentes> CidMVCollection
        {
            get
            {
                return this._cidsmvdivergentes;
            }
        }

        public CidDiferentes CidSelecionado
        {
            get
            {
                return this._cidselecionado;
            }
            set
            {
                this._cidselecionado = value;
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.CidSelecionado));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.CidMVCollection));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.SigaCancelamentoLogCollection));
            }
        }

        public wrpSigaProblemaLogCollection SigaCancelamentoLogCollection
        {
            get
            {
                return new wrpSigaProblemaLogCollection(this._atendimento.SigaProblemaLog.Where(x => x.Cid.IsNull()).Select(x => x.DomainObject).ToList());
            }
        }

        public ObservableCollection<CidDiferentes> Divergentes { get { return this._cidsmvdivergentes; } }

        public bool ListaDeProblemasMaiorQueZero { get { return _atendimento.Paciente.ProblemasPaciente.Count > 0; } }
        #endregion

        #region Eventos
        public event EventHandler GoShow;
        public event Action GoClose;
        #endregion

        #region Métodos Privados
        private void CidsDiferentes()
        {
            try
            {
                //this._cidsmv = this._atendimento.CidsMV;
                var _listaproblemaspaciente = (from x in this._atendimento.Paciente.ProblemasPaciente
                                               where x.CID != null
                                               select x);


                //Filtra itens de CIDmv que nao tem em ListaDeProblemas
                var difLP = from _itemmv in this._atendimento.CidsMV.Where(x => x.Cid != null)
                            where !(from _listaProblemas in _listaproblemaspaciente
                                    where _listaProblemas.CID != null && _listaProblemas.Status.Equals(StatusAlergiaProblema.Ativo)
                                    select _listaProblemas.CID.CidMV.Id.Replace(".", "")).Contains(_itemmv.Id.Replace(".", ""))
                            select _itemmv;

                //Filtra itens do resultado da query acima que os CIDs nao estao contidos no log de cancelamento                
                var difCancelados = from _item in difLP
                                    where !(from _itemCancelado in this._sigaproblemalogcollection
                                            where _itemCancelado.Cid != null
                                            select _itemCancelado.Cid.CidMV.Id.Replace(".", "")).Contains(_item.Id.Replace(".", ""))
                                    select _item;

                this._cidsmvdivergentes = new ObservableCollection<CidDiferentes>();

                if (!difCancelados.IsNull())
                {
                    foreach (var _itemmv in difCancelados)
                    {
                        this._cidsmvdivergentes.Add(new CidDiferentes
                        {
                            Ativo = _itemmv.Ativo,
                            CamposRad = _itemmv.CamposRad,
                            Cat = _itemmv.Cat,
                            Cid = _itemmv.Cid,
                            Classificacao = _itemmv.Classificacao,
                            Descricao = _itemmv.Descricao,
                            DescricaoAux = _itemmv.DescricaoAux,
                            DescricaoCompleta = _itemmv.DescricaoCompleta,
                            Estadio = _itemmv.Estadio,
                            Id = _itemmv.Id,
                            OPC = _itemmv.OPC,
                            RepeteRad = _itemmv.RepeteRad,
                            Restrsexo = _itemmv.Restrsexo,
                            Sexo = _itemmv.Sexo,
                            SgruCid = _itemmv.SgruCid,
                            Subcat = _itemmv.Subcat,
                            Incluir = false,
                            NaoIncluir = false
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void AtualizaCIDsMV()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            rep.Refresh(this._atendimento.DomainObject);

            var CIDsMV = this._atendimento.CidsMV;
            var ListaProblemaAtivos = this._atendimento.Paciente.ProblemasPaciente.Where(x => x.Status.Equals(StatusAlergiaProblema.Ativo) && x.CID != null).ToList();

            var qry = (from _todoslp in ListaProblemaAtivos
                       where !(from _todosmv in CIDsMV
                               where _todosmv.Cid != null
                               select _todosmv.Cid.Id.Replace(".", "")).Contains(_todoslp.CID.Id.Replace(".", ""))
                          && (_todoslp.CID != null) && (_todoslp.DataInicio >= this._atendimento.DataAtendimento)
                       select _todoslp).ToList();

            if (qry.Count > 0)
            {
                foreach (var item in qry)
                {
                    if (this._atendimento.CidsMV.Count(x => x.Id == item.CID.CidMV.Id) == 0) //Impede que seja adicionado 2 vezes o mesmo cid na cidMV para o mesmo atendimento.
                        this._atendimento.CidsMV.Add(new wrpCidMV
                                       {
                                           Id = item.CID.CidMV.Id,
                                           Cid = item.CID,
                                           Descricao = item.Descricao
                                       });
                }
                this._atendimento.Save();
            }
        }

        private void ShowIncluir(object parameter)
        {
            if (GoShow != null)
                GoShow(this, null);
        }

        private void IncluirNaListaProblemas(object parameter)
        {
            var _incluir = this.CidMVCollection.Where(x => x.Incluir.Equals(true)).ToList();
            var _naoincluir = this.CidMVCollection.Where(x => x.NaoIncluir.Equals(true)).ToList();
            var _prob = this._atendimento.Paciente.ProblemasPaciente;

            if ((_naoincluir.Count == 0) && (_incluir.Count == 0))
            {
                DXMessageBox.Show("É necessário que a lista de problemas contenha pelo menos um CID. Favor incluir o CID apresentado ou novo CID", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                foreach (var item in _incluir)
                {
                    this._atendimento.Paciente.DomainObject.AddProblemasPaciente(new ProblemasPaciente()
                    {
                        CID = item.Cid.DomainObject,
                        Descricao = null,
                        DataInicio = DateTime.Now,
                        DataFim = null,
                        Status = StatusAlergiaProblema.Ativo,
                        Profissional = this._sigaprofissional.DomainObject,
                        Usuario = this._usuario.DomainObject,
                        Paciente = this._atendimento.Paciente.DomainObject,
                        Comentario = null
                    });
                }
                this._atendimento.Save();
                NaoIncluirLOG();
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.CidMVCollection));
            }
        }

        private void NaoIncluirLOG()
        {
            var _naoincluir = this.CidMVCollection.Where(x => x.NaoIncluir.Equals(true)).ToList();
            this._sigaproblemalogcollection = this._atendimento.SigaProblemaLog;

            foreach (var item in _naoincluir)
            {
                this._sigaproblemalogcollection.Add(new wrpSigaProblemaLog()
                {
                    Usuario = this._usuario,
                    Atendimento = this._atendimento,
                    Data = DateTime.Now,
                    Cid = item.Cid
                });
                this._atendimento.DomainObject.SigaProblemaLog.Add
                    (new SigaProblemaLog
                    {
                        Usuario = this._usuario.DomainObject,
                        Atendimento = this._atendimento.DomainObject,
                        Data = DateTime.Now,
                        Cid = item.Cid.DomainObject
                    });
            }            
            this._atendimento.Save();
            CidsDiferentes();
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.SigaCancelamentoLogCollection));
            this._salvou = true;
            this.GoClose();
        }

        private bool DeveFecharTela()
        {
            var _prob = this._atendimento.Paciente.ProblemasPaciente;
            if (_prob.Count == 0)
            {
                DXMessageBox.Show("É necessário que a lista de problemas contenha pelo menos um CID. Favor incluir o CID apresentado ou novo CID", "ATENÇÃO: ", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }
        #endregion

        #region Métodos Públicos
        public void InsereCidNovo(Cid pCID)
        {
            if (this._cidsmvdivergentes.Count(x => x.Id == pCID.Id) == 0)
            {
                this._cidsmvdivergentes.Add(new CidDiferentes()
                                                {
                                                    Ativo = SimNao.Sim,
                                                    Cid = new wrpCid(pCID),
                                                    DescricaoAux = pCID.CidMV.DescricaoAux,
                                                    DescricaoCompleta = pCID.CidMV.DescricaoCompleta,
                                                    Id = pCID.Id,
                                                    Descricao = pCID.Descricao,
                                                    Sexo = pCID.Sexo,
                                                    OPC = pCID.OPC,
                                                    Subcat = pCID.SubCategoria.Descricao,
                                                    Incluir = true
                                                });
            }
            this.OnPropertyChanged(ExpressionEx.PropertyName<vmCIDListaProblema>(x => x.CidMVCollection));
        }

        public void CancelarLOG(object parameter)
        {
            if (DeveFecharTela())
                this.GoClose();
        }

        public bool CancelaGrava()
        {
            if (DeveFecharTela())
            {
                if (_salvou)
                    return true;
                else
                {
                    this._sigaproblemalogcollection = this._atendimento.SigaProblemaLog;
                    this._sigaproblemalogcollection.Add(new wrpSigaProblemaLog()
                    {
                        Usuario = this._usuario,
                        Atendimento = this._atendimento,
                        Data = DateTime.Now
                    });
                    this._atendimento.Save();
                    return true;
                }
            }
            else
                return false;
        }
        #endregion
    }

    #region SubClasses
    public class CidDiferentes : wrpCidMV
    {
        public bool Incluir
        {
            get { return this._incluir; }
            set
            {
                this._incluir = value;
                this._naoincluir = false;
                this.OnPropertyChanged(ExpressionEx.PropertyName<CidDiferentes>(x => x.Incluir));
            }
        }

        public bool NaoIncluir
        {
            get { return this._naoincluir; }
            set
            {
                this._naoincluir = value;
                this._incluir = false;
                this.OnPropertyChanged(ExpressionEx.PropertyName<CidDiferentes>(x => x.NaoIncluir));
            }
        }

        private bool _incluir { get; set; }
        private bool _naoincluir { get; set; }
    }
    #endregion
}
