using System.Windows;
using System.Windows.Controls;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucExamesRealizados.xaml
    /// </summary>
    public partial class ucExamesRealizados : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucExamesRealizados()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).ExamesRealizados;
        }

        private void txtExamesRealizados_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtExamesRealizados.Text))
                label1.Content = "Máximo 4000 caracteres";
            else label1.Content = string.Format(txtExamesRealizados.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtExamesRealizados.Text.Length); 
        }    
    }
}
