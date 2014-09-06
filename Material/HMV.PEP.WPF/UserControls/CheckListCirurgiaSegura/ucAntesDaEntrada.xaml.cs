using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using HMV.Core.Interfaces;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
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
            this.DataContext = (pData as vmCheckList).vmAntesDaEntrada;
            this.AlergiasEvento.SetData((this.DataContext as vmAntesDaEntrada).vmAlergias);
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