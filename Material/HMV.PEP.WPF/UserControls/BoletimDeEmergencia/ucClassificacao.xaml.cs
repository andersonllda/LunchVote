using System.Windows.Controls;
using HMV.Core.Domain.Model;
using HMV.Core.Interfaces;
using HMV.Core.Framework.WPF;


namespace HMV.PEP.WPF.UserControls.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for ucClassificacao.xaml
    /// </summary>
    public partial class ucClassificacao : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucClassificacao()
        {
            InitializeComponent();
        }              

        public void SetData(object pData)
        {
            
        } 
    }
}
