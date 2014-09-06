using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using System.Windows.Forms;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucResumoAvaliacaoMedica.xaml
    /// </summary>
    public partial class ucResumoAvaliacaoMedica : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }       
        
        SumarioAvaliacaoMedica SumAvalMed;

        public ucResumoAvaliacaoMedica()
        {
            InitializeComponent();

        }

        public void Imprimir()
        {
            ucRelSumarioAvaliacaoMedica1.Imprimir();
        }        

        public void SetData(object pData)
        {
            SumAvalMed = null;
            SumAvalMed = (pData as SumarioAvaliacaoMedica);
            
            ucRelSumarioAvaliacaoMedica1.mostraRTF = true;
                        
            ucRelSumarioAvaliacaoMedica1.SetData(this.SumAvalMed);                   
        }

        private void createButton_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e)
        {

        }    
    }
}
