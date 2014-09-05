using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoM;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucNotasAdicionais.xaml
    /// </summary>
    public partial class ucNotasAdicionais : UserControlBase, IUserControl
    {
        public ucNotasAdicionais()
        {
            InitializeComponent();           
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmNotasAdicionaisUC(((pData as object[])[0] as vmNotasAdicionais).SumarioAvaliacaoMedica, (pData as object[])[1].ToString());
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
