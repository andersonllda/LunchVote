using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.WPF.UserControls;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for ucListaPacientesInternados.xaml
    /// </summary>
    public partial class ucTabPesquisaPacientes : UserControlBase, IUserControl
    {
        #region --- Construtor ---
        public ucTabPesquisaPacientes(bool acessoTotalProntuario)
        {
            InitializeComponent();
            
            _acessoTotalProntuario = acessoTotalProntuario;
            
            if ( !acessoTotalProntuario ) 
                MensagemHMV.Visibility = System.Windows.Visibility.Collapsed;

            rbInternado.Checked += new RoutedEventHandler(rbIntenadoAmbulatorio_Checked);
            rbIntenadoAmbulatorio_Checked(new object(), new RoutedEventArgs());
        }
        #endregion

        #region --- Propriedades Privadas ---
        private bool _acessoTotalProntuario;
        ucListaPacientesInternados listaPacientesInternados;
        ucListaPacientesAmbulatoriais listaPacientesAmbulatoriais;
        ucListaPacientesExternos listaPacientesExternos;
        #endregion

        #region --- Propriedades Públicas ---
        public bool CancelClose { get; set; }
        public bool AcessoTotalProntuario { get { return _acessoTotalProntuario; } }
        public event EventHandler<EventArgs> DoubleClick;
        public event EventHandler<EventArgs> RadioButtonClick;
        public vPacienteInternado pacientesDTO { get { return listaPacientesInternados.GetFocusedRow(); } }
        public bool Internado { get { return rbInternado.IsChecked.Value; } }
        public bool Ambulatorial { get { return rbAmbulatorio.IsChecked.Value; } }
        public bool Externo { get { return rbExternos.IsChecked.Value; } }
        public int Atendimento
        {
            get
            {
                if (Internado)
                {
                    if (listaPacientesInternados.GetFocusedRow() == null)
                        return 0;
                    return listaPacientesInternados.GetFocusedRow().Atendimento;
                }
                else if (Ambulatorial)
                {
                    if (listaPacientesAmbulatoriais.GetFocusedRow() == null)
                        return 0;
                    else
                        return listaPacientesAmbulatoriais.GetFocusedRow().Atendimento;
                }
                else
                {
                    if (listaPacientesExternos.GetFocusedRow() == null)
                        return 0;
                    else
                        return listaPacientesExternos.GetFocusedRow().Atendimento;
                }
            }
        }
        #endregion

        #region --- Métodos Privados ---
        #endregion

        #region --- Métodos Públicos ---
        public void SetData(object pData)
        {
            throw new NotImplementedException();
        }
        public void AtualizaDados()
        {
            if (rbInternado.IsChecked == true)
                listaPacientesInternados.LoadPacientesInternados();
            else if (rbAmbulatorio.IsChecked == true)
                listaPacientesAmbulatoriais.LoadPacientes();
            else
                listaPacientesExternos.LoadPacientesExternos();
        }
        #endregion

        #region --- Eventos ---
        private void rbIntenadoAmbulatorio_Checked(object sender, RoutedEventArgs e)
        {
            if (gridPaciente == null) return;

            if (RadioButtonClick != null)
                RadioButtonClick(null, null);

            //lblMsgAvisoCirurgia.Visibility = System.Windows.Visibility.Collapsed;

            if (rbInternado.IsChecked == true)
            {                
                if (this.listaPacientesInternados == null)
                {
                    this.listaPacientesInternados = new ucListaPacientesInternados();
                    this.listaPacientesInternados.DoubleClick += new EventHandler<EventArgs>(listaPacientesInternados_DoubleClick);
                    gridPaciente.Children.Add(this.listaPacientesInternados);
                }

                if ( listaPacientesAmbulatoriais != null ) 
                    listaPacientesAmbulatoriais.Visibility = System.Windows.Visibility.Hidden;

                if (listaPacientesExternos != null)
                    listaPacientesExternos.Visibility = System.Windows.Visibility.Hidden;

                listaPacientesInternados.Visibility = System.Windows.Visibility.Visible;
                listaPacientesInternados.Inicializa();
            }
            else if (rbAmbulatorio.IsChecked == true)
            {
                if (this.listaPacientesAmbulatoriais == null)
                {
                    this.listaPacientesAmbulatoriais = new ucListaPacientesAmbulatoriais();
                    this.listaPacientesAmbulatoriais.DoubleClick += new EventHandler<EventArgs>(listaPacientesInternados_DoubleClick);
                    gridPaciente.Children.Add(this.listaPacientesAmbulatoriais);
                }

                if (listaPacientesInternados != null)
                    listaPacientesInternados.Visibility = System.Windows.Visibility.Hidden;

                if (listaPacientesExternos != null)
                    listaPacientesExternos.Visibility = System.Windows.Visibility.Hidden;

                listaPacientesAmbulatoriais.Visibility = System.Windows.Visibility.Visible;
                listaPacientesAmbulatoriais.Inicializa();
            }
            else
            {
                //lblMsgAvisoCirurgia.Visibility = System.Windows.Visibility.Visible;

                if (this.listaPacientesExternos == null)
                {
                    this.listaPacientesExternos = new ucListaPacientesExternos();
                    this.listaPacientesExternos.DoubleClick += new EventHandler<EventArgs>(listaPacientesInternados_DoubleClick);
                    gridPaciente.Children.Add(this.listaPacientesExternos);
                }

                if (listaPacientesAmbulatoriais != null)
                    listaPacientesAmbulatoriais.Visibility = System.Windows.Visibility.Hidden;

                if (listaPacientesInternados != null)
                    listaPacientesInternados.Visibility = System.Windows.Visibility.Hidden;

                listaPacientesExternos.Visibility = System.Windows.Visibility.Visible;
                listaPacientesExternos.Inicializa();
            }
        }

        private void listaPacientesInternados_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(null, null);
        }
        #endregion  
    }
}
