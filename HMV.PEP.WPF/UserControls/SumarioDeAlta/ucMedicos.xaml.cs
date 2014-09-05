using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucMedicos.xaml
    /// </summary>
    public partial class ucMedicos : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucMedicos()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmMedicos((pData as vmSumarioAlta).SumarioAlta);            
        }

        private void btnIncluir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            winSelProfissional w = new winSelProfissional();
            w.ShowDialog(base.OwnerBase);
            if (w.GetPrestador() != null)
            {
                (this.DataContext as vmMedicos).PrestadorSelecionado = new wrpPrestador(w.GetPrestador());
                (this.DataContext as vmMedicos).AddPrestadorCommand.Execute(null);
            }
            gcMedico.RefreshData();
        }       
    }
}
