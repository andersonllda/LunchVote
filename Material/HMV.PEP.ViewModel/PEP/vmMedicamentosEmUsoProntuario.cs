using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.PEP.ViewModel.Commands;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.PEP;
using DevExpress.Xpf.Core;
using System.Windows;
using HMV.Core.Framework.Types;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Constant;


namespace HMV.PEP.ViewModel.PEP
{
    public class vmMedicamentosEmUsoProntuario : ViewModelBase
    {
        #region Contrutor

        public vmMedicamentosEmUsoProntuario(Paciente pPaciente, Usuarios pUsuario)
        {
            this._usuario = new wrpUsuarios(pUsuario);
            this._paciente = new wrpPaciente(pPaciente);
            this.AddMedicamentosEmUsoProntuarioCommand = new AddMedicamentosEmUsoProntuarioCommand(this);
            this.SavePacienteCommand = new SavePacienteCommand(this);

            carrega();
            inicialverificaSemMedicamentosEmUso();
            FiltraEmUso = true;
        }

        private wrpMedicamentosEmUsoProntuarioCollection _listaDeMedicamentos;
        
        private void montaListaDeMedicamentoRegraDosEventos()
        {
            /*
            // Busca todos os atendimento do paciente 
            IRepositorioDeAtendimento repAte = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            IList<Atendimento> atendimentos = repAte.OndeCodigoPacienteIgual(this.Paciente.ID).List();
            
            // Busca todos os eventos dos atendimentos 
            List<MedicamentoEmUsoEvento> eventos = new List<MedicamentoEmUsoEvento>();
            foreach (var item in atendimentos)
            {
                IRepositorioDeEventoMedicamentosEmUso repEvento = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                eventos.AddRange(repEvento.OndeAtendimentoIgual(item).List());
            }

            // Localiza o Ultimo evento 
            MedicamentoEmUsoEvento ultimoEvento = eventos.OrderBy(x => x.Data).LastOrDefault();

            IList<MedicamentosEmUsoProntuario> medicamentos = null;

            // Busca os medicamentos do Ultimo evento. 
            if (ultimoEvento != null)
            {
                IRepositorioDeEventoMedicamentosEmUso repUltimoEvento = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                medicamentos = repUltimoEvento.OndeChaveIgual(ultimoEvento.Chave).List().Select(x => x.MedicamentosEmUso).ToList();

                // Adiciona os medicamentos que não tem evento, que foram adicionados direto na lista de medicamentos. 
                foreach (var item in this._paciente.DomainObject.MedicamentosEmUso.Except(medicamentos).ToList())
                {
                    IRepositorioDeEventoMedicamentosEmUso rep = ObjectFactory.GetInstance<IRepositorioDeEventoMedicamentosEmUso>();
                    if (rep.OndeIdDoMedicamentoIgual(item.ID).List().Count == 0)
                        medicamentos.Add(item);
                }

                // Verifica se tem algum medicamento sem uso 
                if (medicamentos.Count(x => x.Status == StatusMedicamentosEmUso.NaoFazUso) > 0)
                {
                    // Se existir algum sem uso remove, e mantem somente o sem uso de medicamentos. 
                    IList<int> ids_medicamento = medicamentos.Where(x => x.Status != StatusMedicamentosEmUso.NaoFazUso).Select(x => x.ID).ToList();
                    foreach (var item in ids_medicamento)
                        medicamentos.Remove(medicamentos.Where(x => x.ID == item).FirstOrDefault());
                }
            }

            if (medicamentos == null)
                _listaDeMedicamentos = this._paciente.MedicamentosEmUso;
            else
                _listaDeMedicamentos = new wrpMedicamentosEmUsoProntuarioCollection(medicamentos);
             * */


            if (this._paciente.MedicamentosEmUso.Count(x => x.Status == StatusMedicamentosEmUso.EmUso) > 0)
            {
                // Se existir algum medimento em uso, nao deve mostar o medimento do tipo nao faz uso, ele foi adicionado em algum evento. 
                _listaDeMedicamentos = new wrpMedicamentosEmUsoProntuarioCollection(
                    this._paciente.DomainObject.MedicamentosEmUso.Where(x=>x.Status != StatusMedicamentosEmUso.NaoFazUso && x.Status != StatusMedicamentosEmUso.Temporario).ToList());
            }
            else if (this._paciente.MedicamentosEmUso.Count(x => x.Status == StatusMedicamentosEmUso.EmUso) > 1)
            {
                // existe mais de um medimento do tipo "Nao faz uso de medimento", deve mostar somente um na tela, 
                _listaDeMedicamentos = new wrpMedicamentosEmUsoProntuarioCollection(
                    this._paciente.DomainObject.MedicamentosEmUso.Where(x => x.Status == StatusMedicamentosEmUso.NaoFazUso).Take(1).ToList());
            }
            else
            {
                _listaDeMedicamentos = new wrpMedicamentosEmUsoProntuarioCollection(
                   this._paciente.DomainObject.MedicamentosEmUso.Where(x=> x.Status != StatusMedicamentosEmUso.Temporario).ToList());
            }

        }

        #endregion

        #region Propriedades Publicas

        public wrpMedicamentosEmUsoProntuario medicamentosEmUsoProntuarioSelecionado
        {
            get
            {
                return _medicamentosEmUsoProntuarioSelecionado;
            }
            set
            {
                _medicamentosEmUsoProntuarioSelecionado = value;
                base.OnPropertyChanged("medicamentosEmUsoProntuarioSelecionado");
            }
        }

        public DateTime DataInicio
        {
            get
            {
                return this._medicamentosEmUsoProntuarioSelecionado.DataInicio;
            }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.DataInicio = value;
                base.OnPropertyChanged("DataInicio");
            }
        }

        public string Medicamento
        {
            get { return this._medicamentosEmUsoProntuarioSelecionado.Medicamento; }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.Medicamento = value;
                base.OnPropertyChanged("Medicamento");
            }
        }

        public string Dose
        {
            get { return this._medicamentosEmUsoProntuarioSelecionado.Dose; }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.Dose = value;
                base.OnPropertyChanged("Dose");
            }
        }

        public string Via
        {
            get { return this._medicamentosEmUsoProntuarioSelecionado.Via; }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.Via = value;
                base.OnPropertyChanged("Via");
            }
        }

        public string Frequencia
        {
            get { return this._medicamentosEmUsoProntuarioSelecionado.Frequencia; }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.Frequencia = value;
                base.OnPropertyChanged("Frequencia");
            }
        }

        public SimNao UsaMedicamentos
        {
            get { return this._medicamentosEmUsoProntuarioSelecionado.UsaMedicamentos; }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.UsaMedicamentos = value;
                base.OnPropertyChanged("UsaMedicamentos");
            }
        }

        public StatusMedicamentosEmUso Status
        {
            get
            {
                return this._medicamentosEmUsoProntuarioSelecionado.Status;
            }

            set
            {
                this._medicamentosEmUsoProntuarioSelecionado.Status = value;
                base.OnPropertyChanged("Status");
            }
        }

        public wrpMedicamentosEmUsoProntuarioCollection ListaMedicamentosEmUso
        {
            get
            {
                this.OnPropertyChanged("HabilitaBotaoAlterar");  
                return _MedicamentosEmUso;
            }
        }

        public wrpPaciente Paciente { get { return _paciente; } set { _paciente = value; } }

        public wrpUsuarios Usuario { get { return _usuario; } }

        public bool FiltraEmUso
        {
            set
            {
                if (value == true)
                {
                    _FiltraMedicamentos = StatusMedicamentosEmUso.EmUso;
                    _FiltraTodos = false;
                    carrega();
                    this.OnPropertyChanged("ListaMedicamentosEmUso");
                    this.OnPropertyChanged("FiltraEmUso");
                }
            }
            get { return _FiltraTodos ? false : _FiltraMedicamentos == StatusMedicamentosEmUso.EmUso; }

        }

        public bool FiltraEncerrado
        {
            set
            {
                if (value == true)
                {

                    _FiltraTodos = false;
                    _FiltraMedicamentos = StatusMedicamentosEmUso.Encerrado;
                    carrega();
                    this.OnPropertyChanged("ListaMedicamentosEmUso");
                    this.OnPropertyChanged("FiltraEncerrado");
                }
            }
            get { return _FiltraTodos ? false : _FiltraMedicamentos == StatusMedicamentosEmUso.Encerrado; }
        }

        public bool FiltraExcluido
        {
            set
            {
                if (value == true)
                {
                    _FiltraTodos = false;
                    _FiltraMedicamentos = StatusMedicamentosEmUso.Excluído;
                    carrega();
                    this.OnPropertyChanged("ListaMedicamentosEmUso");
                    this.OnPropertyChanged("FiltraExcluido");
                }
            }
            get { return _FiltraTodos ? false : _FiltraMedicamentos == StatusMedicamentosEmUso.Excluído; }
        }

        public bool FiltraTodos
        {
            set
            {
                if (value == true)
                {
                    _FiltraTodos = true;
                    carrega();
                    this.OnPropertyChanged("ListaMedicamentosEmUso");
                }
            }
            get
            {
                return _FiltraTodos;
            }
        }

        public bool HabilitaSemMedicamentos
        {
            get
            {
                if (/*this._paciente.MedicamentosEmUso*/_listaDeMedicamentos.Where(x => x.Status == StatusMedicamentosEmUso.EmUso).ToList().Count > 0)
                    return false;
                else
                    return true;
            }
            set
            {
                _HabilitaSemMedicamentos = value;
            }
        }

        public bool HabilitaDesabilitaBotoesView
        {
            get
            {
                if (/*this._paciente.MedicamentosEmUso*/_listaDeMedicamentos.Where(x => x.Status == StatusMedicamentosEmUso.NaoFazUso).ToList().Count > 0)
                    return false;
                else
                    return true;
            }
            set
            {
                _HabilitaDesabilitaBotoes = value;
            }

        }

        public bool verificaSemMedicamentosEmUso
        {
            get
            {
                inicialverificaSemMedicamentosEmUso();
                return _verificaSemMedicamentosEmUso;
            }
            set
            {
                ModificaMedicamentosEmUso();
            }
        }

        public List<string> CarregaStatusMedicamento
        {
            get
            {
                return Enum<StatusMedicamentosEmUso>.GetCustomDisplay().Where(x => x.ToString() != "Sem uso de medicamentos" && x.ToString() != "Temporário").ToList();
            }
        }

        public string SelecionaStatusMedicamento
        {
            get
            {
                return _medicamentosEmUsoProntuarioSelecionado.Status.ToString();
            }
            set
            {
                if (value == "Em Uso")
                {
                    _medicamentosEmUsoProntuarioSelecionado.Status = StatusMedicamentosEmUso.EmUso;
                }
                if (value == "Encerrado")
                {
                    _medicamentosEmUsoProntuarioSelecionado.Status = StatusMedicamentosEmUso.Encerrado;
                }
                if (value == "Excluído")
                {
                    _medicamentosEmUsoProntuarioSelecionado.Status = StatusMedicamentosEmUso.Excluído;
                }
            }
        }

        public bool EditaData
        {
            get
            {
                return this._editadata;
            }
            set
            {
                this._editadata = value;
            }
        }

        public bool HabilitaBotaoAlterar
        {
            get { return this._MedicamentosEmUso.Count > 0; }
        }

        #endregion

        #region Commands

        public ICommand AddMedicamentosEmUsoProntuarioCommand { get; set; }
        public ICommand SavePacienteCommand { get; set; }

        #endregion

        #region Metodos
        
        public void Refresh(Paciente pac)
        {
            this._paciente = new wrpPaciente(pac);
            carrega();
        }

        public void carrega()
        {
            montaListaDeMedicamentoRegraDosEventos();

            _MedicamentosEmUso = /*this._paciente.MedicamentosEmUso*/_listaDeMedicamentos;
            //wrpMedicamentosEmUsoProntuarioCollection listaAux = new wrpMedicamentosEmUsoProntuarioCollection(_MedicamentosEmUso.Where(x => x.Status == _FiltraMedicamentos).ToList().Select(c => c.DomainObject).ToList());
            wrpMedicamentosEmUsoProntuarioCollection listaAux = new wrpMedicamentosEmUsoProntuarioCollection(_MedicamentosEmUso.Where(x => x.Status == StatusMedicamentosEmUso.NaoFazUso || x.Status == _FiltraMedicamentos).ToList().Select(c => c.DomainObject).ToList());
            
            if (_FiltraTodos == false)
                _MedicamentosEmUso = listaAux;

            this.OnPropertyChanged("ListaMedicamentosEmUso");
            this.OnPropertyChanged("HabilitaSemMedicamentos");
            this.OnPropertyChanged("verificaSemMedicamentosEmUso");
            this.OnPropertyChanged("HabilitaDesabilitaBotoesView");
        }

        public void NovoRegistro()
        {
            this._medicamentosEmUsoProntuarioSelecionado = new wrpMedicamentosEmUsoProntuario(new MedicamentosEmUsoProntuario(this._paciente.DomainObject, this._usuario.DomainObject));
            this._medicamentosEmUsoProntuarioSelecionado.DataInicio = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString());
            this._medicamentosEmUsoProntuarioSelecionado.Status = StatusMedicamentosEmUso.EmUso;
        }

        private void ModificaMedicamentosEmUso()
        {
            this.OnPropertyChanged("verificaSemMedicamentosEmUso");

            if (_verificaSemMedicamentosEmUso)
            {
                if (DXMessageBox.Show("Tem certeza que deseja remover o Item  << Sem uso de medicamentos >> ?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var med = this._paciente.DomainObject.MedicamentosEmUso
                        .Where(x => x.ID == _listaDeMedicamentos.Where(u => u.Status == StatusMedicamentosEmUso.NaoFazUso).FirstOrDefault().ID).FirstOrDefault();
                    med.Status = StatusMedicamentosEmUso.Excluído;
                    
                    //this._paciente.MedicamentosEmUso.Remove(_MedicamentosEmUso.Where(x => x.Status == StatusMedicamentosEmUso.NaoFazUso).FirstOrDefault());
                    this._paciente.Save();
                }
            }
            else
            {
                if (DXMessageBox.Show("Tem certeza que deseja incluir o item  << Sem uso de medicamentos >>.", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {                                        
                    if (this.ListaMedicamentosEmUso != null && this.ListaMedicamentosEmUso.Count(x => x.Medicamento == Constantes.coNaoUsaMedicamentos && x.Status == StatusMedicamentosEmUso.NaoFazUso) == 0)
                    {
                        var antiga = this.ListaMedicamentosEmUso.Where(x => x.Medicamento == Constantes.coNaoUsaMedicamentos && x.Status == StatusMedicamentosEmUso.Excluído).FirstOrDefault();
                        if (antiga == null)
                        {
                            this._medicamentosEmUsoProntuarioSelecionado = new wrpMedicamentosEmUsoProntuario(new MedicamentosEmUsoProntuario(this._paciente.DomainObject, this._usuario.DomainObject));
                            this._medicamentosEmUsoProntuarioSelecionado.Medicamento = Constantes.coNaoUsaMedicamentos;
                            this._medicamentosEmUsoProntuarioSelecionado.DataInicio = DateTime.Now;
                            this._medicamentosEmUsoProntuarioSelecionado.Status = StatusMedicamentosEmUso.NaoFazUso;
                            this._paciente.MedicamentosEmUso.Add(this._medicamentosEmUsoProntuarioSelecionado);
                        }
                        else
                            antiga.Status = StatusMedicamentosEmUso.NaoFazUso;

                        this._paciente.Save();
                    }
                }
            }
            carrega();
        }

        private bool inicialverificaSemMedicamentosEmUso()
        {            
            _verificaSemMedicamentosEmUso = true;
            if (/*this._paciente.MedicamentosEmUso*/_listaDeMedicamentos.Where(x => x.Status == StatusMedicamentosEmUso.NaoFazUso).ToList().Count == 0)
                _verificaSemMedicamentosEmUso = false;

            return _verificaSemMedicamentosEmUso;
        }

        public void RemoveSemMedicamentoEmUso()
        {
            if (_verificaSemMedicamentosEmUso)
            {
                var med = this._paciente.DomainObject.MedicamentosEmUso
                        .Where(x => x.ID == _listaDeMedicamentos.Where(u => u.Status == StatusMedicamentosEmUso.NaoFazUso).FirstOrDefault().ID).FirstOrDefault();
                med.Status = StatusMedicamentosEmUso.Excluído;
                this._paciente.Save();
            }
        }

        #endregion

        #region Propriedades Privadas

        private wrpMedicamentosEmUsoProntuario _medicamentosEmUsoProntuarioSelecionado { get; set; }
        private wrpUsuarios _usuario { get; set; }
        private wrpPaciente _paciente { get; set; }
        private bool _FiltraTodos = true;
        private StatusMedicamentosEmUso _FiltraMedicamentos { get; set; }
        private bool _HabilitaSemMedicamentos { get; set; }
        private bool _verificaSemMedicamentosEmUso { get; set; }
        private bool _HabilitaDesabilitaBotoes { get; set; }
        private wrpMedicamentosEmUsoProntuarioCollection _MedicamentosEmUso { get; set; }
        private bool _InserMedicamento { get; set; }
        private bool _AddOrRemoveItemSemUsoMedicamentos { get; set; }
        private string _SelecionaStatusMedicamento = string.Empty;
        private bool _editadata { get; set; }
        #endregion

    }
}
