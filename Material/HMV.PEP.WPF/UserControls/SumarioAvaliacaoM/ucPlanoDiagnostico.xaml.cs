using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucPlanoDiagnostico.xaml
    /// </summary>
    public partial class ucPlanoDiagnostico : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucPlanoDiagnostico()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).PlanoDiagnosticoTerapeutico;
        }

        private void txtConduta_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtConduta.Text))
                labelconduta.Content = "Máximo 4000 caracteres";
            else labelconduta.Content = string.Format(txtConduta.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtConduta.Text.Length); 
        }

        private void txtExamesSelecionados_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            //if (string.IsNullOrEmpty(txtExamesSelecionados.Text))
            //    labelExamesolicitado.Content = "Máximo 4000 caracteres";
            //else labelExamesolicitado.Content = string.Format(txtExamesSelecionados.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtExamesSelecionados.Text.Length); 
        }

        private void txtPacCir_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            //if (string.IsNullOrEmpty(txtPacCir.Text))
            //    labelcirurgia.Content = "Máximo 4000 caracteres";
            //else labelcirurgia.Content = string.Format(txtPacCir.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtPacCir.Text.Length); 
        }   
    }
}
