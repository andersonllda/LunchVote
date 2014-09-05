using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using HMV.Core.Domain.Model.PEP.SCI;
using HMV.PEP.Interfaces;
using StructureMap;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.Windows.SCI
{
    /// <summary>
    /// Interaction logic for winJustificativa.xaml
    /// </summary>
    public partial class winJustificativaSCI : WindowBase
    {
        private SCIParecer _parecer;
        public winJustificativaSCI(SCIParecer parecer)
        {
            InitializeComponent();
            _parecer = parecer;

            txtSolicitacao.Text = _parecer.ItemPrescricao.TipoPrescricaoMedica.Descricao;
            txtJustificativa.Focus();

        }

        private void txtJustificativa_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtJustificativa.Text))
                lblJustificativa.Content = "Máximo 4000 caracteres";
            else
                lblJustificativa.Content =
                    string.Format(txtJustificativa.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtJustificativa.Text.Length); 
        }

        private void btnEnviarSCI_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJustificativa.Text))
            {
                DXMessageBox.Show("Informe a justificativa!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                txtJustificativa.Focus();
            }
            else
            {
                ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
                serv.Discorda(_parecer.ID, App.Usuario, txtJustificativa.Text);
                this.DialogResult = true;
                this.Close();
            }
        }


    }
}
