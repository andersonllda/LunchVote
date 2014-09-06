using System.Windows;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.ViewModel.UrgenciaP;
using HMV.PEP.ViewModel.UTINEOFolhaP;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Windows;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucUTINEOFolhaParada.xaml
    /// </summary>
    public partial class ucUTINEOFolhaParada : UserControlBase, IUserControl
    {
        public ucUTINEOFolhaParada() { }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {
            winUTINEOFolhaParada win = new winUTINEOFolhaParada((vmUTINEOFolhaParada)this.DataContext, false);
            win.ShowDialog(base.OwnerBase);
        }             

        public void SetData(object pData)
        {
            this.DataContext = new vmUTINEOFolhaParada(pData as Atendimento);
            InitializeComponent();
        }

        public bool CancelClose { get; set; } 
         
        private void btnVisualizar_Click(object sender, RoutedEventArgs e)
        {
            winUTINEOFolhaParada win = new winUTINEOFolhaParada((vmUTINEOFolhaParada)this.DataContext, true);
            win.Show();
            win.Relatorio();
            win.Close();
        }       
    }
}
