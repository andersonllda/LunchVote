using System.Windows;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CentroObstetrico.SumarioDeAvaliacaoMedicaCO;
using HMV.PEP.ViewModel.PEP.SumarioDeAvaliacaoMedicaCTINEO;

namespace HMV.PEP.WPF.Cadastros.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for winCadHiposteseDiagnostico.xaml
    /// </summary>
    public partial class winCadHiposteseDiagnostico : WindowBase
    {
        public winCadHiposteseDiagnostico(Core.Domain.Model.SumarioAvaliacaoMedica sumario)
        {
            InitializeComponent();

            this.DataContext = new Hipotese
            {
                SumarioAvaliacaoMedica = sumario
            };
        }

        public winCadHiposteseDiagnostico(Hipotese hipotese)
        {
            InitializeComponent();
            this.DataContext = hipotese;
        }

        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO _vm;
        private vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO _vm2;
        public winCadHiposteseDiagnostico(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCO pvm)
        {
            InitializeComponent();
            this.DataContext = new Hipotese();
            this._vm = pvm;
        }

        public winCadHiposteseDiagnostico(vmDiagnosticoHipoteseSumarioAvaliacaoMedicaCTINEO pvm)
        {
            InitializeComponent();
            this.DataContext = new Hipotese();
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
                Hipotese hipotese = (Hipotese)this.DataContext;
                if (this._vm.IsNotNull())
                   fecha = this._vm.AddHipotese(hipotese.Complemento);
                else if (this._vm2.IsNotNull())
                    fecha = this._vm2.AddHipotese(hipotese.Complemento);
                else
                    hipotese.SumarioAvaliacaoMedica.AddHipotese(hipotese);

                if (fecha)
                {
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (BusinessValidatorException ex)
            {
                DXMessageBox.Show(ex.GetErros()[0].Message, "Alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtDescricaoHipoteses.Focus();
        }

        private void txtDescricaoHipoteses_EditValueChanging(object sender, DevExpress.Xpf.Editors.EditValueChangingEventArgs e)
        {
            
        }

        private void txtDescricaoHipoteses_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDescricaoHipoteses.Text))
                label1.Content = "Máximo 128 caracteres";
            else label1.Content = string.Format(txtDescricaoHipoteses.Text.Length < 127 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 128 - txtDescricaoHipoteses.Text.Length); 
        }
    }
}
