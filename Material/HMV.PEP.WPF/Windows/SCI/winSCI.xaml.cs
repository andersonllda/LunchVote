using DevExpress.Xpf.Core;
using HMV.Core.Domain.Model.PEP.SCI;
using HMV.Core.Framework.WPF;
using HMV.PEP.Interfaces;
using StructureMap;
using System.Windows;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Types;

namespace HMV.PEP.WPF.Windows.SCI
{
    /// <summary>
    /// Interaction logic for winSCI.xaml
    /// </summary>
    public partial class winSCI : WindowBase
    {
        private SCIParecer _parecer;

        public winSCI(SCIParecer parecer)
        {
            InitializeComponent();
            _parecer = parecer;

            cboSindrome.ItemsSource = Enum<SindromeInfecciosaSCI>.GetCustomDisplay();
            if (parecer.SindromeInfecciosa.HasValue)
            {
                cboSindrome.SelectedItem = Enum<SindromeInfecciosaSCI>.GetCustomDisplayOf(parecer.SindromeInfecciosa.Value);
                cboSindrome.SelectedText = Enum<SindromeInfecciosaSCI>.GetCustomDisplayOf(parecer.SindromeInfecciosa.Value);
            }

            if (parecer.ItemPrescricao.Dose.HasValue)
                txtQuantidade.Text = parecer.ItemPrescricao.Dose.ToString();

            if (parecer.ItemPrescricao.MedicamentoUnidadeMedida.IsNotNull())
                txtUnidade.Text = parecer.ItemPrescricao.MedicamentoUnidadeMedida.Descricao;

            if (parecer.ItemPrescricao.AplicacaoPrescricao.IsNotNull())
                txtVia.Text = parecer.ItemPrescricao.AplicacaoPrescricao.Descricao;

            if (parecer.ItemPrescricao.FrequenciaItemPrescricaoMedica.IsNotNull())
                txtFrequencia.Text = parecer.ItemPrescricao.FrequenciaItemPrescricaoMedica.Descricao;

            if (parecer.Prescricao.DataReferencia.HasValue)
                txtData.Text = parecer.Prescricao.DataReferencia.Value.ToShortDateString();

            if (parecer.ItemPrescricao.NumeroDias.HasValue && parecer.ItemPrescricao.QuantidadeDias.HasValue)
                txtDiasDeUso.Text = parecer.ItemPrescricao.NumeroDias.ToString() + " / " + parecer.ItemPrescricao.QuantidadeDias.ToString();

            txtJustificativa.Text = parecer.JustificativaMedica;
            txtMedicamento.Text = parecer.ItemPrescricao.TipoPrescricaoMedica.Descricao;
            txtJustificativa.Focus();

        }

        private void btnComunicarSCI_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJustificativa.Text))
            {
                DXMessageBox.Show("Informe a justificativa!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                txtJustificativa.Focus();
                return;
            }

            if (cboSindrome.SelectedItem == null || string.IsNullOrWhiteSpace((string)cboSindrome.SelectedItem))
            {
                DXMessageBox.Show("Informe a síndrome infecciosa!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Question);
                cboSindrome.Focus();
                return;
            }

            SindromeInfecciosaSCI sindrome = Enum<SindromeInfecciosaSCI>.From(Enum<SindromeInfecciosaSCI>.GetDescriptionOfCustomDisplay((string)cboSindrome.SelectedItem));

            ISCIService serv = ObjectFactory.GetInstance<ISCIService>();
            serv.AlteraJustificativaMedica(_parecer.ID, txtJustificativa.Text, sindrome);
            this.DialogResult = true;
            this.Close();

        }

        private void txtJustificativa_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtJustificativa.Text))
                lblJustificativa.Content = "Máximo 4000 caracteres";
            else
                lblJustificativa.Content =
                    string.Format(txtJustificativa.Text.Length < 3999 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 4000 - txtJustificativa.Text.Length);
        }
    }
}
