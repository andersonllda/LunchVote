using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.PEP.Interfaces;
using NHibernate.Validator.Engine;
using StructureMap;
using HMV.Core.Domain.Model.PEP.SumarioDeAvaliacaoPreAnestesica;
using HMV.Core.Framework.WPF;
using HMV.Core.Domain.Model.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Wrappers.ObjectWrappers.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.Core.Framework.Extensions;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;

namespace HMV.PEP.WPF.Cadastros
{
    /// <summary>
    /// Interaction logic for winCadCID.xaml
    /// </summary>
    public partial class winCadCID : WindowBase
    {
        public winCadCID(SumarioAvaliacaoMedica sumario)
        {
            InitializeComponent();

            this.DataContext = new Diagnostico
            {
                SumarioAvaliacaoMedica = sumario,
                Usuario = App.Usuario
            };
            AbreListaCid();
        }

        public winCadCID(Diagnostico diagnostico)
        {
            InitializeComponent();
            this.DataContext = diagnostico;
            AbreListaCid();
        }

        public winCadCID(SumarioAvaliacaoPreAnestesica preanestesica)
        {
            InitializeComponent();
            this.DataContext = new Diagnostico();

        }

        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO _vm;
        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO _vm2;
        public winCadCID(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO pvm)
        {
            InitializeComponent();
            this.DataContext = new Diagnostico();
            this._vm = pvm;
        }

        public winCadCID(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO pvm)
        {
            InitializeComponent();
            this.DataContext = new Diagnostico();
            this._vm2 = pvm;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool fecha = true;
                Diagnostico diagnostico = (Diagnostico)this.DataContext;
                if (this._vm.IsNotNull())
                    fecha = this._vm.AddDiagnostico(diagnostico.Cid, diagnostico.Complemento);
                else if (this._vm2.IsNotNull())
                    fecha = this._vm2.AddDiagnostico(diagnostico.Cid, diagnostico.Complemento);
                else
                {
                    diagnostico.SumarioAvaliacaoMedica.AddDiagnostico(diagnostico, false, App.Usuario.Profissional);

                    if (diagnostico.SumarioAvaliacaoMedica.Paciente.ProblemasPaciente.Count(x => !x.CID.IsNull() && x.CID.Id == diagnostico.Cid.Id) > 0)
                        if (DXMessageBox.Show("CID já existente para o paciente. Deseja incluí-lo mesmo assim?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            if (fecha)
                            {
                                this.DialogResult = true;
                                this.Close();
                               
                            }

                            return;
                        }

                    ProblemasPaciente novo = new ProblemasPaciente()
                    {
                        Paciente = diagnostico.SumarioAvaliacaoMedica.Paciente,
                        Usuario = App.Usuario,
                        DataInicio = DateTime.Now,
                        Profissional = App.Usuario.Profissional,
                        CID = diagnostico.Cid
                    };
                    diagnostico.SumarioAvaliacaoMedica.AddProblemasPaciente(novo);
                }

                if (fecha)
                {
                    try
                    {
                        this.DialogResult = true;
                    }
                    catch (InvalidOperationException)
                    {                        
                      
                    }
                    
                    this.Close();
                }
            }
            catch (BusinessValidatorException ex)
            {
                DXMessageBox.Show(ex.GetErros()[0].Message, "Alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void txtCid_EditValueChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            try
            {
                if (e.IsNotNull() && e.NewValue.IsNotNull())
                {
                    ICidService serv = ObjectFactory.GetInstance<ICidService>();
                    Cid cid = serv.FiltraPorCid10(e.NewValue.ToString());

                    if (cid != null)
                    {
                        txtDescricaoCIDPrincipal.Text = cid.Descricao;

                        Diagnostico diagnostico = (Diagnostico)this.DataContext;
                        diagnostico.Cid = cid;
                        this.DataContext = diagnostico;
                    }
                    else
                        txtDescricaoCIDPrincipal.Text = "";
                }
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            AbreListaCid();
        }

        private void AbreListaCid()
        {
            winSelCID win = new winSelCID(false);
            //win.txtCodigoCid.Text = txtCid.Text;
            win.ShowDialog(this.Owner);
            if (win.CID != null)
                txtCid.Text = win.CID.Id.ToString();
        }
    }
}
