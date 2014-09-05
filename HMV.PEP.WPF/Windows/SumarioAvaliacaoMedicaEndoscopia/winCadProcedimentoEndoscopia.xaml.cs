using HMV.Core.Framework.WPF;

using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoMedicaEndoscopia
{
    /// <summary>
    /// Interaction logic for winCadProcedimentoEndoscopia.xaml
    /// </summary>
    public partial class winCadProcedimentoEndoscopia : WindowBase
    {
        public winCadProcedimentoEndoscopia(ViewModelBase pVM)
            : base(pVM)
        {
            InitializeComponent();                       
        }       
    }
}
