using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Extensions;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using System.Collections.Generic;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;

namespace HMV.PEP.ViewModel.PEP
{
    public class vmListaDeProblemas : ViewModelBase
    {
        #region Construtor
        public vmListaDeProblemas(SumarioAvaliacaoMedica pSumarioAM, Usuarios pUsuario, bool pIsCidPrincipal)
        {
            this._iscidprincipal = pIsCidPrincipal;
            this._usuarios = new wrpUsuarios(pUsuario);
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedica(pSumarioAM);

            if (this._sumarioavaliacaomedica.Diagnosticos.Count > 0)
            {
                this._listadiagnosticocids = new ObservableCollection<DiagnosticoLista>();
                foreach (var item in this._sumarioavaliacaomedica.Diagnosticos)
                {
                    DiagnosticoLista novo = new DiagnosticoLista(this)
                    {
                        Cid = item.Cid,
                        Complemento = item.Complemento
                    };

                    this._listadiagnosticocids.Add(novo);
                }
            }
        }

        public vmListaDeProblemas(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO pSumarioAM, Usuarios pUsuario, bool pIsCidPrincipal)
        {
            this._iscidprincipal = pIsCidPrincipal;
            this._usuarios = new wrpUsuarios(pUsuario);
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedica(pSumarioAM.Paciente.TipoDoPaciente) { Atendimento = new wrpAtendimento(pSumarioAM.Atendimento)
                , Paciente = new wrpPaciente(pSumarioAM.Paciente) };

            this._listadiagnosticocids = new ObservableCollection<DiagnosticoLista>();
            if (pSumarioAM.SumarioAvaliacaoMedicaCODiagnosticoCollection.Count > 0)
            {                
                foreach (var item in pSumarioAM.SumarioAvaliacaoMedicaCODiagnosticoCollection)
                {
                    DiagnosticoLista novo = new DiagnosticoLista(this)
                    {
                        Cid = item.CID10,
                        Complemento = item.Complemento
                    };

                    this._listadiagnosticocids.Add(novo);
                }
            }
        }

        public vmListaDeProblemas(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO pSumarioAM, Usuarios pUsuario, bool pIsCidPrincipal)
        {
            this._iscidprincipal = pIsCidPrincipal;
            this._usuarios = new wrpUsuarios(pUsuario);
            this._sumarioavaliacaomedica = new wrpSumarioAvaliacaoMedica(pSumarioAM.Paciente.TipoDoPaciente)
            {
                Atendimento = new wrpAtendimento(pSumarioAM.Atendimento)
                ,
                Paciente = new wrpPaciente(pSumarioAM.Paciente)
            };

            this._listadiagnosticocids = new ObservableCollection<DiagnosticoLista>();
            if (pSumarioAM.SumarioAvaliacaoMedicaCTINEODiagnosticoCollection.Count > 0)
            {
                foreach (var item in pSumarioAM.SumarioAvaliacaoMedicaCTINEODiagnosticoCollection)
                {
                    DiagnosticoLista novo = new DiagnosticoLista(this)
                    {
                        Cid = item.CID10,
                        Complemento = item.Complemento
                    };

                    this._listadiagnosticocids.Add(novo);
                }
            }
        }
        #endregion

        #region Propriedades Publicas
        public bool IsCidPrincipal
        {
            get { return this._iscidprincipal; }
        }

        public ObservableCollection<DiagnosticoLista> ListaDiagnosticoCids
        {
            get
            {
                return this._listadiagnosticocids;
            }
        }

        public wrpProblemasPacienteCollection ProblemasPaciente
        {
            get
            {
                return new wrpProblemasPacienteCollection(this._sumarioavaliacaomedica.Paciente.DomainObject.ProblemasPaciente.Where(x => x.Status == StatusAlergiaProblema.Ativo).ToList());
            }
        }

        public wrpCid Cid
        {
            get
            {
                return _sumarioavaliacaomedica.Atendimento.Cid;
            }
        }

        public bool TodosMarcados
        {
            get
            {
                if (this._listadiagnosticocids != null)
                    return this._listadiagnosticocids.Count(x => x.Selecionado == true) == this._listadiagnosticocids.Count;
                return false;
            }
            set
            {
                foreach (var item in this._listadiagnosticocids)
                {
                    item.Selecionado = value;
                }
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmListaDeProblemas>(x => x.ListaDiagnosticoCids));
                this.OnPropertyChanged(ExpressionEx.PropertyName<vmListaDeProblemas>(x => x.TodosMarcados));
            }
        }
        #endregion

        #region Metodos Publicos

        #endregion

        #region Commands
        protected override void CommandIncluir(object param)
        {

            if (this._iscidprincipal)
            {
                if (this._sumarioavaliacaomedica.Paciente.ProblemasPaciente.Count(x => !x.CID.IsNull() && x.CID.Id == this._sumarioavaliacaomedica.Atendimento.Cid.Id && x.Status.Equals(StatusAlergiaProblema.Ativo) && x.DataFim.IsNull()) > 0)
                {
                    DXMessageBox.Show("Não é permitido adicionar este 'CID' – [" + this._sumarioavaliacaomedica.Atendimento.Cid.Id + "], pois já existe na lista de problemas!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (this._sumarioavaliacaomedica.Paciente.ProblemasPaciente.Count(x => !x.CID.IsNull() && x.CID.Id == this._sumarioavaliacaomedica.Atendimento.Cid.Id) > 0)
                    if (DXMessageBox.Show("CID já existente para o paciente. Deseja incluí-lo mesmo assim?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }

                wrpProblemasPaciente novo = new wrpProblemasPaciente
                {
                    Paciente = this._sumarioavaliacaomedica.Paciente,
                    Usuario = this._usuarios,
                    DataInicio = DateTime.Now,
                    Profissional = this._usuarios.Profissional,
                    CID = this._sumarioavaliacaomedica.Atendimento.Cid
                };

                this._sumarioavaliacaomedica.Paciente.ProblemasPaciente.Add(novo);
                this._sumarioavaliacaomedica.Paciente.Save();
                DXMessageBox.Show("Cid incluído na lista de problemas!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                base.CommandFechar(null);
            }
            else
            {
                bool incluiu = false;

                //if (this._sumarioavaliacaomedica.Atendimento.CIDs.Count(x => x.Id.Equals(this._sumarioavaliacaomedica.Atendimento.Cid.Id)) > 0)
                //{
                //    DXMessageBox.Show("Não é permitido adicionar este 'CID', pois já existe no atendimento!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    return;
                //}


                foreach (var item in this._listadiagnosticocids.Where(x => x.Selecionado == true))
                {
                    if (this._sumarioavaliacaomedica.Paciente.ProblemasPaciente.Count(x => x.CID.Id == item.Cid.Id) > 0)
                    {
                        if (this._sumarioavaliacaomedica.Atendimento.CIDs.Count(x => x.Id.Equals(this._sumarioavaliacaomedica.Atendimento.Cid.Id)) > 0)
                        {
                            DXMessageBox.Show("Não é permitido adicionar este 'CID', pois já existe no atendimento!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                            continue;
                        }

                        if (DXMessageBox.Show("O CID: " + item.Cid.Id + " - " + item.Cid.Descricao + Environment.NewLine
                            + " já existe para o paciente. Deseja incluí-lo mesmo assim?",
                            "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            continue;
                        }
                    }

                    incluiu = true;
                    wrpProblemasPaciente novo = new wrpProblemasPaciente
                    {
                        Paciente = this._sumarioavaliacaomedica.Paciente,
                        Usuario = this._usuarios,
                        DataInicio = DateTime.Now,
                        Profissional = this._usuarios.Profissional,
                        CID = item.Cid
                    };

                    this._sumarioavaliacaomedica.Paciente.ProblemasPaciente.Add(novo);
                    this._sumarioavaliacaomedica.Paciente.Save();
                }

                if (incluiu)
                {
                    DXMessageBox.Show("Cid's incluídos na lista de problemas!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                    base.CommandFechar(null);
                }
            }
        }
        #endregion

        #region Propriedades Privadas
        private bool _iscidprincipal { get; set; }
        private wrpSumarioAvaliacaoMedica _sumarioavaliacaomedica { get; set; }
        private ObservableCollection<DiagnosticoLista> _listadiagnosticocids { get; set; }
        private wrpUsuarios _usuarios { get; set; }
        #endregion

        public class DiagnosticoLista : wrpDiagnostico
        {
            public DiagnosticoLista(vmListaDeProblemas pVm)
            {
                this._vm = pVm;
            }

            private bool _selecionado { get; set; }
            private vmListaDeProblemas _vm { get; set; }
            public bool Selecionado
            {
                get { return this._selecionado; }
                set
                {
                    this._selecionado = value;
                    if (this._selecionado)
                    {
                        foreach (var item in this._vm._listadiagnosticocids)
                        {
                            foreach (var item1 in this._vm._sumarioavaliacaomedica.Atendimento.CIDs)
                            {
                                if (item.Cid.Id.Equals(item1.Id))
                                {
                                    DXMessageBox.Show("Não é permitido adicionar este 'CID' – [" + item1.Id + "], pois já existe na lista de problemas!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    this._selecionado = false;
                                }
                            }
                        }
                    }
                   
                    this._vm.OnPropertyChanged(ExpressionEx.PropertyName<vmListaDeProblemas>(x => x.TodosMarcados));
                    this.OnPropertyChanged(ExpressionEx.PropertyName<DiagnosticoLista>(x => x.Selecionado));
                }
            }
        }
    }
}
