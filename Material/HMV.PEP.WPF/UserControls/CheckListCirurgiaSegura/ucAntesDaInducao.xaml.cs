using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
{
    /// <summary>
    /// Interaction logic for ucSalaCirurgica.xaml
    /// </summary>
    public partial class ucAntesDaInducao : UserControlBase, IUserControl
    {
        public ucAntesDaInducao()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckList).vmAntesDaInducao;
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