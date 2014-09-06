using System;
using System.Windows.Input;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoPreAnestesica;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for ucPreMedicacao.xaml
    /// </summary>
    public partial class ucPreMedicacao : UserControlBase
    {
        public ucPreMedicacao()
        {
            InitializeComponent();
            base.TelaIncluirType = new TelaType().SetType<winCadPreMedicacao>();
        }

        private void gPreMedicacao_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.ExecutaCommandAlterarSelecionar(null);
        }

        protected override void ChamaTelaIncluir(object sender, EventArgs e)
        {
            base.ChamaTelaIncluir(sender, e);
            this.grdPreMedicacao.RefreshData();
        }
    }
}