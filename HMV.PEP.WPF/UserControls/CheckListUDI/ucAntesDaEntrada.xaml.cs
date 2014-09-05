using System;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using HMV.PEP.ViewModel.PEP.CheckListDeUDI;

namespace HMV.PEP.WPF.UserControls.CheckListUDI
{
    /// <summary>
    /// Interaction logic for ucSalaCirurgica.xaml
    /// </summary>
    public partial class ucAntesDaEntrada : UserControlBase, IUserControl
    {
        public ucAntesDaEntrada()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckListUDI).vmAntesDaEntrada;
            this.AlergiasEvento.SetData((this.DataContext as HMV.PEP.ViewModel.PEP.CheckListDeUDI.vmAntesDaEntrada).vmAlergias);
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
    }
}