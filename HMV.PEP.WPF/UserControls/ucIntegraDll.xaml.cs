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
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucIntegraDll.xaml
    /// </summary>
    public partial class ucIntegraDll : UserControlBase, IUserControl
    {
        public ucIntegraDll()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            tabDLL.Children.Add(pData as UserControlBase);            
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
