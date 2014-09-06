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
using HMV.Core.Framework.WPF;
using HMV.PEP.WPF.UserControls.BoletimDeEmergencia;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Model;
using HMV.PEP.WPF.UserControls.CentroObstetrico.BoletimDeEmergencia;

namespace HMV.PEP.WPF.Windows.BoletimDeEmergencia
{
    /// <summary>
    /// Interaction logic for winBoletimEmergencia.xaml
    /// </summary>
    public partial class winBoletimEmergenciaCO : WindowBase
    {
        public winBoletimEmergenciaCO(Atendimento pAtendimento)
        {
            InitializeComponent();
            ucBoletimCO ucBoletim = new ucBoletimCO();
            ucBoletim.SetData(pAtendimento);
            Principal.Children.Add(ucBoletim);
        }
    }
}
