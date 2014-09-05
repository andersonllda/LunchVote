using System.Windows;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for winSelAvisoCirurgia.xaml
    /// </summary>
    public partial class winSelAvisoCirurgia : WindowBase
    {
        public winSelAvisoCirurgia(vmSumarioAvaliacaoPreAnestesica pvm) : base(pvm)
        {
            InitializeComponent();          
        }
    }
}
