using System;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Editors.DateNavigator.Controls;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winCalendario.xaml
    /// </summary>
    public partial class winCalendario : WindowBase
    {
        public DateTime Data
        {
            get { return this.DtCaledario.FocusedDate; }
        }

        public winCalendario(DateTime pData)
        {
            InitializeComponent();            
            this.DtCaledario.FocusedDate = pData;           
        }     

        private void DtCaledario_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.DtCaledario.FocusedDate > DateTime.Now.Date)
            {
                this.DtCaledario.FocusedDate = DateTime.Now.Date;
                this.DtCaledario.UpdateLayout();
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }     
    }
}
