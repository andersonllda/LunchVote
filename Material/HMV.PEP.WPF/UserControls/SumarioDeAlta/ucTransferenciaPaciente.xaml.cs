using System.Windows.Controls;
using DevExpress.Xpf.Editors;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.SumarioDeAlta; using HMV.Core.Wrappers.ObjectWrappers;
using System.Windows;

namespace HMV.PEP.WPF.UserControls.SumarioDeAlta
{
    /// <summary>
    /// Interaction logic for ucTransferenciaPaciente.xaml
    /// </summary>
    public partial class ucTransferenciaPaciente : UserControl, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucTransferenciaPaciente()
        {           
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmTransferencia((pData as vmSumarioAlta).SumarioAlta);
        }        
    }
}
