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
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.PEP.Interfaces;
using StructureMap;

namespace HMV.PEP.WPF.Windows.SCI
{
    /// <summary>
    /// Interaction logic for winJustificativa.xaml
    /// </summary>
    public partial class winCiente : WindowBase
    {
        private SCIParecer _parecer;
        public winCiente(SCIParecer parecer)
        {
            InitializeComponent();
            _parecer = parecer;

            txtJustificativa.Text = _parecer.JustificativaSCI;
            txtSolicitacao.Text = _parecer.ItemPrescricao.TipoPrescricaoMedica.Descricao;
            
            txtParecer.Text = HMV.Core.Framework.Types.Enum<TipoSCI>.GetCustomDisplayOf(_parecer.Parecer.Value);

            if (_parecer.NumeroDias.HasValue && _parecer.NumeroDias.Value > 0)
            {
                txtDias.Text += "Liberado por " + _parecer.NumeroDias.Value.ToString() + " dias";
            }
            else
                txtDias.Visibility = System.Windows.Visibility.Hidden;

            if (_parecer.Parecer.Value == TipoSCI.Contrario)
                btnAceite.Visibility = System.Windows.Visibility.Hidden;
            else
            {
                btnConcordo.Visibility = System.Windows.Visibility.Hidden;
                btnDiscordo.Visibility = System.Windows.Visibility.Hidden;
            }
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
            this.DialogResult = true;
            this.Close();
        }

        private void btnCiente_Click(object sender, RoutedEventArgs e)
        {
            ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
            serv.Ciente(_parecer.ID, App.Usuario);
            this.DialogResult = true;
            this.Close();
            }

        private void btnDiscordo_Click(object sender, RoutedEventArgs e)
        {
            winJustificativaSCI win = new winJustificativaSCI(_parecer);
            win.ShowDialog(this);
            this.DialogResult = true;
            this.Close();
        }

        private void btnConcordo_Click(object sender, RoutedEventArgs e)
        {
            ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
            serv.Concorda(_parecer.ID, App.Usuario);
            this.DialogResult = true;
            this.Close();
        }

    }
}
