using System.Windows.Controls;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using DevExpress.Xpf.Editors;
using System.Windows;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucHistoriaPerinatal.xaml
    /// </summary>
    public partial class ucHistoriaPerinatal : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucHistoriaPerinatal()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            if ((pData as SumarioAvaliacaoMedica).Paciente.HistoriaPerinatal == null)
                (pData as SumarioAvaliacaoMedica).Paciente.HistoriaPerinatal = new HistoriaPerinatal((pData as SumarioAvaliacaoMedica).Paciente);

            this.DataContext = (pData as SumarioAvaliacaoMedica).Paciente.HistoriaPerinatal;
        }

        private bool CanRefresh;

        private void Refresh()
        {            
            HistoriaPerinatal x = (HistoriaPerinatal)this.DataContext;
            this.DataContext = null;
            this.DataContext = x;
        }

        private void Refresh_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (!CanRefresh)
            {
                CanRefresh = true;
                this.Refresh();
                CanRefresh = false;
            }            
        }

        private void Refresh_Checked(object sender, RoutedEventArgs e)
        {
            if (!CanRefresh)
            {   
                CanRefresh = true;
                this.Refresh();
                CanRefresh = false;
            }

        }

        private void cheCesarianaEletiva_Checked(object sender, RoutedEventArgs e)
        {
            Refresh_Checked(sender, e);
            this.txtMotivoCesariana.Text = string.Empty;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Refresh_Checked(sender, e);
            txtMeses.Text = null;
        }

        private void txtIdadeGestacionalIntercorrencia_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIdadeGestacionalIntercorrencia.Text))
                label1.Content = "Máximo 80 caracteres";
            else label1.Content = string.Format(txtIdadeGestacionalIntercorrencia.Text.Length < 79 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 80 - txtIdadeGestacionalIntercorrencia.Text.Length); 
        }

        private void txtIntercorrencia_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtIntercorrencia.Text))
                label3.Content = "Máximo 80 caracteres";
            else label3.Content = string.Format(txtIntercorrencia.Text.Length < 79 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 80 - txtIntercorrencia.Text.Length); 
        }

        private void txtMotivoCesariana_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMotivoCesariana.Text))
                label2.Content = "Máximo 80 caracteres";
            else label2.Content = string.Format(txtMotivoCesariana.Text.Length < 79 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 80 - txtMotivoCesariana.Text.Length); 
        }
    
        private void cheNormal_Checked(object sender, RoutedEventArgs e)
        {
            txtSemanas.Text = "0";
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void cheAtermo_Checked(object sender, RoutedEventArgs e)
        {
            txtSemanas.Text = "0";
        }

        //private bool _informacoesdisponiveissim { get; set; }

        //public bool InformacoesDisponiveisSim
        //{
        //    get
        //    {
        //        return this._informacoesdisponiveissim;
        //    }
        //    set
        //    {
        //        if (optSimInfoDisponivel.IsChecked == true)
        //        {
        //            this._informacoesdisponiveissim = true;
        //            optSimInfoDisponivel.IsChecked = true;
        //        }
        //        else
        //        {
        //            this._informacoesdisponiveissim = false;
        //            optNaoInfoDisponivel.IsChecked = false;
        //        }
        //    }
        //}
    }
}
