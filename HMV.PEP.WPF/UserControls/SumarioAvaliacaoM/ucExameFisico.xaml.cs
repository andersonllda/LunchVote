using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using HMV.Core.Domain.Model;
using HMV.Core.DTO;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;
using System.Linq;
using HMV.PEP.WPF.Windows.SumarioAvaliacaoM;

namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoM
{
    /// <summary>
    /// Interaction logic for ucExameFisico.xaml
    /// </summary>
    public partial class ucExameFisico : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }

        public ucExameFisico()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as SumarioAvaliacaoMedica).ExameFisico;
            BuscaValoresAdmissaoAssistencial();
        }

        private void BuscaValoresAdmissaoAssistencial()
        {
            if (!(this.DataContext as ExameFisico).SumarioAvaliacaoMedica.Atendimento.AdmissaoAssistencial.IsNull()
                && (this.DataContext as ExameFisico).SumarioAvaliacaoMedica.Atendimento.AdmissaoAssistencial.Count(x => !x.DataConclusao.IsNull()) > 0)
            {
                var adassist = (this.DataContext as ExameFisico).SumarioAvaliacaoMedica.Atendimento.AdmissaoAssistencial.Where(x => !x.DataConclusao.IsNull()).Last();

                if ((this.DataContext as ExameFisico).Altura == 0)
                    (this.DataContext as ExameFisico).Altura = adassist.Altura.IsNull() ? 0 : adassist.Altura.Value;

                if ((this.DataContext as ExameFisico).FrequenciaCardiaca == 0)
                    (this.DataContext as ExameFisico).FrequenciaCardiaca = adassist.FrequenciaCardiaca.IsNull() ? 0 : adassist.FrequenciaCardiaca.Value;

                if ((this.DataContext as ExameFisico).FrequenciaRespiratoria == 0)
                    (this.DataContext as ExameFisico).FrequenciaRespiratoria = adassist.FrequenciaRespiratoria.IsNull() ? 0 : adassist.FrequenciaRespiratoria.Value;

                if (!(this.DataContext as ExameFisico).Peso.HasValue || (this.DataContext as ExameFisico).Peso == 0)
                    (this.DataContext as ExameFisico).Peso = adassist.Peso.IsNull() ? 0 : adassist.Peso.Value;

                if (!(this.DataContext as ExameFisico).TemperaturaAxila.HasValue || (this.DataContext as ExameFisico).TemperaturaAxila == 0)
                    (this.DataContext as ExameFisico).TemperaturaAxila = adassist.TAX.IsNull() ? 0 : adassist.TAX.Value;

                if ((this.DataContext as ExameFisico).PressaoArterial.IsNull())
                    (this.DataContext as ExameFisico).PressaoArterial = new PA((this.DataContext as ExameFisico))
                    {
                        Alta = adassist.PA.Split('/').First().RemoveNonNumber().ConvertStringToInt()
                        ,
                        Baixa = adassist.PA.Split('/').Last().RemoveNonNumber().ConvertStringToInt()
                    };
                else
                {
                    if (adassist.PA.IsNotNull())
                    {
                        if ((this.DataContext as ExameFisico).PressaoArterial.Alta.IsNull())
                            (this.DataContext as ExameFisico).PressaoArterial.Alta = adassist.PA.Split('/').First().RemoveNonNumber().ConvertStringToInt();

                        if ((this.DataContext as ExameFisico).PressaoArterial.Baixa.IsNull())
                            (this.DataContext as ExameFisico).PressaoArterial.Baixa = adassist.PA.Split('/').Last().RemoveNonNumber().ConvertStringToInt();
                    }
                }
                
                ExameFisico exame = (ExameFisico)this.DataContext;
                this.DataContext = null;
                this.DataContext = exame;
            }
        }

        private void gdExameFisico_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ((TableView)gdExameFisico.View).Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => ((TableView)gdExameFisico.View).BestFitColumns()));
        }

        private void ckbSelectAll_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckEdit)sender).Name == "ckbSelectAll")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdExameFisico.ItemsSource)
                    {
                        item.SemParticularidades = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.NaoAvaliado = false;
                    }
                }
                else
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdExameFisico.ItemsSource)
                    {
                        item.SemParticularidades = false;
                    }
                }
            }
            else if (((CheckEdit)sender).Name == "ckbSelectAllN")
            {
                if (((CheckEdit)sender).IsChecked.Value)
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdExameFisico.ItemsSource)
                    {
                        item.NaoAvaliado = !string.IsNullOrWhiteSpace(item.Observacoes) ? false : ((CheckEdit)sender).IsChecked.Value;
                        item.SemParticularidades = false;
                    }
                }
                else
                {
                    foreach (SumarioAvaliacaoMedicaItensDetalheDTO item in (IList<SumarioAvaliacaoMedicaItensDetalheDTO>)gdExameFisico.ItemsSource)
                    {
                        item.NaoAvaliado = false;
                    }
                }
            }

            (this.DataContext as ExameFisico).ItensExameFisico = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)gdExameFisico.ItemsSource;
            ExameFisico exame = (ExameFisico)this.DataContext;
            this.DataContext = null;
            this.DataContext = exame;
            gdExameFisico.RefreshData();
        }

        private void txtObs_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtObs.Text))
                lbQtdCaracter.Content = "Máximo 2000 caracteres";
            else lbQtdCaracter.Content = string.Format(txtObs.Text.Length < 2000 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 2000 - txtObs.Text.Length);
        }

        private void txtPAUm_Validate(object sender, ValidationEventArgs e)
        {
            //if (!string.IsNullOrEmpty((txtPAUm.Text)))
            if (!e.Value.IsNull())
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                    if (int.Parse(e.Value.ToString()) > 300)
                    {
                        e.IsValid = false;
                        e.ErrorContent = "A P.A sistólica deve ser menor que 300!";
                    }
            MsgErros(e);
        }

        private void txtPADois_Validate(object sender, ValidationEventArgs e)
        {
            //if (!string.IsNullOrEmpty((txtPADois.Text)))
            if (!e.Value.IsNull())
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                    if (int.Parse(e.Value.ToString()) > 200)
                    {
                        e.ErrorContent = "A P.A diastólica deve ser menor que 200!";
                        e.IsValid = false;
                    }
            MsgErros(e);
        }

        private void txtPeso_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((this.DataContext as ExameFisico) != null)
            {
                lblIMC.Text = (this.DataContext as ExameFisico).IMC.ToString();
                lblSC.Text = (this.DataContext as ExameFisico).SC.ToString();
            }
        }

        private void txtAltura_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if ((this.DataContext as ExameFisico) != null)
            {
                lblIMC.Text = (this.DataContext as ExameFisico).IMC.ToString();
                lblSC.Text = (this.DataContext as ExameFisico).SC.ToString();
            }
        }

        private void txtSAT_Validate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null)
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                    if (int.Parse(e.Value.ToString()) > 100)
                    {
                        e.ErrorContent = "A saturação deve ser menor que 100!";
                        e.IsValid = false;

                    }
            MsgErros(e);
        }

        private void txtFR_Validate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null)
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                    if (int.Parse(e.Value.ToString()) > 200)
                    {
                        e.ErrorContent = "A frequência respiratória deve ser menor que 200!";
                        e.IsValid = false;

                    }
            MsgErros(e);
        }

        private void txtFC_Validate(object sender, ValidationEventArgs e)
        {
            if (e.Value != null)
                if (!string.IsNullOrEmpty(e.Value.ToString()))
                    if (int.Parse(e.Value.ToString()) > 300)
                    {
                        e.ErrorContent = "A frequência cardíaca deve ser menor que 300!";
                        e.IsValid = false;

                    }
            MsgErros(e);
        }

        private void txtTAX_Validate(object sender, ValidationEventArgs e)
        {
            double d = 0;
            if (e.Value != null)
                if (!string.IsNullOrWhiteSpace(e.Value.ToString()))
                {
                    double.TryParse(e.Value.ToString().Replace('.', ','), out d);
                    if (d < 30 || d > 45)
                    {
                        e.ErrorContent = "A TAX deve estar entre 30 e 45!";
                        e.IsValid = false;
                    }
                }
            MsgErros(e);
        }

        private void txtAltura_Validate(object sender, ValidationEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
                if (Convert.ToInt32(e.Value) != 0)
                {
                    if (e.Value != null)
                        if (!string.IsNullOrEmpty(e.Value.ToString()))
                            if (int.Parse(e.Value.ToString()) < 1 || int.Parse(e.Value.ToString()) > 240)
                            {
                                e.ErrorContent = "A altura deve de estar entre 1 e 240!";
                                e.IsValid = false;
                            }
                    MsgErros(e);
                }
        }

        private void txtPeso_Validate(object sender, ValidationEventArgs e)
        {
            //if (e.Value != null)
            //    if (!string.IsNullOrEmpty(e.Value.ToString()))
            //    {
            //        if (!string.IsNullOrEmpty(e.Value.ToString()))
            //        {
            //            e.Value.ToString().
            //            if (double.Parse(e.Value.ToString().Replace('.', ',')) > 400 || double.Parse(e.Value.ToString().Replace('.', ',')) < 1)
            //            {
            //                e.ErrorContent = "O peso deve de estar entre 1 e 400!";
            //                e.IsValid = false;
            //            }
            //        }
            //        MsgErros(e);
            //    }

            //e.IsValid = true;

        }

        private bool pcontrolaobs;

        private void viewExameFisico_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (!pcontrolaobs)
                if ((sender as TableView).Grid.ItemsSource != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        (this.DataContext as ExameFisico).ItensExameFisico = (List<SumarioAvaliacaoMedicaItensDetalheDTO>)(sender as TableView).Grid.ItemsSource;
                        (sender as TableView).Grid.ItemsSource = (this.DataContext as ExameFisico).ItensExameFisico;
                        (sender as TableView).FocusedRow = e.Row;
                    }));
                    e.Handled = true;
                }
        }

        private void viewExameFisico_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.SemParticularidades)
                               || e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.NaoAvaliado))
            {
                (sender as TableView).CommitEditing();
            }
            else if (e.Column.FieldName == ExpressionEx.PropertyName<SumarioAvaliacaoMedicaItensDetalheDTO>(x => x.Observacoes))
            {
                pcontrolaobs = true;
                ((TableView)sender).Grid.SetCellValue(e.RowHandle, e.Column, e.Value);
                pcontrolaobs = false;
            }
        }

        private void MsgErros(ValidationEventArgs e)
        {
            if (lbErros != null)
                lbErros.Text = string.Empty;
            if (e.IsValid == false)
                lbErros.Text = e.ErrorContent.ToString();
            else
                if (lbErros != null)
                    lbErros.Text = string.Empty;
        }

        private void gdExameFisico_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            viewExameFisico.CommitEditing();
        }

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            SumarioAvaliacaoMedicaItensDetalheDTO item = gdExameFisico.GetFocusedRow() as SumarioAvaliacaoMedicaItensDetalheDTO;
            winObservacao win = new winObservacao(item.Observacoes);
            if (win.ShowDialog(base.OwnerBase).Value == true)
            {
                if (item != null)
                {
                    viewExameFisico.Grid.SetCellValue(viewExameFisico.GetSelectedRowHandles()[0], "Observacoes", win.Texto);
                    viewExameFisico.CommitEditing();
                }
            }
        }
    }
}
