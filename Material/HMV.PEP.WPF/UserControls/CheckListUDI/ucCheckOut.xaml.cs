using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;
using HMV.PEP.WPF.Windows.CheckListUDI;

namespace HMV.PEP.WPF.UserControls.CheckListUDI
{
    /// <summary>
    /// Interaction logic for ucCheckOut.xaml
    /// </summary>
    public partial class ucCheckOut : UserControlBase, IUserControl
    {
        public ucCheckOut()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckListUDI).vmCheckOut;            
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        private void btnIncluir_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as vmCheckOut).Novo();
            var win = new winCadCheckoutMaterialUDI((this.DataContext as vmCheckOut));            
            win.ShowDialog(this.OwnerBase);
        }
    }
}