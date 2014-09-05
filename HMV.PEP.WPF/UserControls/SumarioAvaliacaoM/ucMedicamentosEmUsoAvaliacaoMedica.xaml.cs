using System.Windows;
using System.Windows.Controls;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.PEP.DTO;
using DevExpress.Xpf.Editors;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucMedicamentosEmUsoAvaliacaoMedica.xaml
    /// </summary>
    public partial class ucMedicamentosEmUsoAvaliacaoMedica : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucMedicamentosEmUsoAvaliacaoMedica()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.ucMedicamentosEmUsoEvento.DataContext = pData;
        }
    }
}
