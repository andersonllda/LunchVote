using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using System.Windows.Controls;
using System;
using System.Reflection;
using System.IO;
using HMV.Core.DTO;
using HMV.Core.Framework.ViewModelBaseClasses;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucAlergiasAvaliacaoMedica.xaml
    /// </summary>
    public partial class ucAlergiasAvaliacaoMedica : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucAlergiasAvaliacaoMedica()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {          
            //this.ucAlergiaEvento.DataContext = pData;
            this.ucAlergiaEvento.SetData(pData as ViewModelBase);
        }
    }
}
