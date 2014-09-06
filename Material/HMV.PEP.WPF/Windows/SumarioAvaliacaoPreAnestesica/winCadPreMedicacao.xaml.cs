using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winCadPreMedicacao.xaml
    /// </summary>
    public partial class winCadPreMedicacao : WindowBase
    {
        public winCadPreMedicacao(vmPreMedicacao pvm) : base(pvm)
        {
            InitializeComponent();
        }
    }
}
