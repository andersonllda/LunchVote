using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucMotivosInternacao.xaml
    /// </summary>
    public partial class ucMotivosInternacao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucMotivosInternacao()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica);
        }

        private void txtDescricao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            //if (string.IsNullOrEmpty(txtDescricao.Text))
            //    label1.Content = "Máximo 4000 caracteres";
            //else label1.Content = string.Format(txtDescricao.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtDescricao.Text.Length); 
        }
    }
}
