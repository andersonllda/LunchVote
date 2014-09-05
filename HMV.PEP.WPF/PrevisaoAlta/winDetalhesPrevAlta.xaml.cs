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
using System.Windows.Shapes;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.PrevisaoAlta
{
    /// <summary>
    /// Interaction logic for winDetalhesPrevAlta.xaml
    /// </summary>
    public partial class winDetalhesPrevAlta : WindowBase
    {
        public winDetalhesPrevAlta(ViewModelBase pVM)
            : base(pVM)
        {
            InitializeComponent();
        }
    }
}
