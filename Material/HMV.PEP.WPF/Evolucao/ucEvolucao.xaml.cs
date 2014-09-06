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
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Model.PEP.EvolucaoNova;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.PEP.Evolucao;

namespace HMV.PEP.WPF.Evolucao
{
    /// <summary>
    /// Interaction logic for ucEvolucao.xaml
    /// </summary>
    public partial class ucEvolucao : UserControlBase, IUserControl
    {
        private FindAndReplaceManager manager;
        private FindOptions findOptions;

        public ucEvolucao()
        {
            InitializeComponent();
        }

        #region SetData
        public void SetData(object pData)
        {
            Atendimento _atendimento = null;
            Paciente _paciente = null;

            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                _atendimento = pData as Atendimento;
                _paciente = _atendimento.Paciente;
            }
            else
                _paciente = pData as Paciente;

            this.DataContext = new VMPEPEvolucao(_paciente, App.Usuario, _atendimento);
            (this.DataContext as VMPEPEvolucao).Refresh += new EventHandler(Refresh);
            base.TelaIncluirType = new TelaType().SetType<winCadEvolucao>(pShowNonModal: true);            
            MontaRTF();
        }

        void Refresh(object sender, EventArgs e)
        {
            MontaRTF();
        }
        #endregion

        public bool CancelClose { get; set; }

        #region Montagem RTF
        private void MontaRTF()
        {
            //FlowDocument flowDoc = new FlowDocument();
            //flowEvolucao. = flowDoc.Blocks;
            flowEvolucao.Blocks.Clear();

            IList<PEPEvolucao> _dados = (this.DataContext as VMPEPEvolucao).CarregaDados();
            int i = 0;

            foreach (PEPEvolucao _reg in _dados)
            {
                Brush background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                if (i % 2 == 0)
                    background = new SolidColorBrush(Color.FromRgb(225, 225, 225));

                addDataeAtendimento(_reg.Data, _reg.Atendimento.ID, background, flowEvolucao);

                addProfissional(montaNomeProfissional(_reg.Usuario), background, flowEvolucao);

                addResumo(_reg.Evolucao, background, flowEvolucao);
                i++;
            }
        }

        private string montaNomeProfissional(Usuarios pUsuario)
        {
            if (pUsuario.Prestador != null)
            {
                string NomePrestador = "[ ";

                if (pUsuario.Prestador.Conselho != null)
                    NomePrestador += pUsuario.Prestador.Conselho.ds_conselho + ": ";

                NomePrestador += pUsuario.Prestador.Registro + " ] - ";

                if (pUsuario.Prestador.TipoPrestador != null && !string.IsNullOrWhiteSpace(pUsuario.Prestador.TipoPrestador.Tratamento))
                    NomePrestador += pUsuario.Prestador.TipoPrestador.Tratamento;
                else
                    NomePrestador += "Dr(a):";

                NomePrestador += " " + pUsuario.Prestador.Nome;

                return NomePrestador;
            }

            return pUsuario.nm_usuario;
        }

        private void addDataeAtendimento(DateTime pData, int pAtendimento, Brush pBackground, FlowDocument document)
        {
            Paragraph para = new Paragraph();
            para.Background = pBackground;
            para.Inlines.Add(Environment.NewLine);

            Bold b = new Bold();
            b.FontFamily = new FontFamily("WingDings");
            b.FontSize = 16;
            b.Inlines.Add("·");
            para.Inlines.Add(b);

            b = new Bold();
            b.FontFamily = new FontFamily("Arial");
            b.FontSize = 12;
            b.Inlines.Add(" Data: " + pData.ToString("dd/MM/yyyy") + " Hora: " + pData.ToString("HH:mm") + "\t" + "\t");
            para.Inlines.Add(b);

            Paragraph para2 = new Paragraph();
            para2.Inlines.Add(Environment.NewLine);
            para2.TextAlignment = TextAlignment.Right;
            para2.Background = pBackground;

            b = new Bold();
            b.FontFamily = new FontFamily("WebDings");
            b.FontSize = 16;
            b.Inlines.Add("Â");
            para2.Inlines.Add(b);

            b = new Bold();
            b.FontFamily = new FontFamily("Arial");
            b.FontSize = 12;
            b.Inlines.Add(" Atendimento: " + pAtendimento.ToString());
            para2.Inlines.Add(b);

            Table t = new Table();
            t.Background = pBackground;
            t.Margin = new Thickness(0);
            t.Columns.Add(new TableColumn());
            t.Columns.Add(new TableColumn());
            TableRow row = new TableRow();
            row.Cells.Add(new TableCell(para));
            row.Cells.Add(new TableCell(para2));

            var rg = new TableRowGroup();

            rg.Rows.Add(row);
            t.RowGroups.Add(rg);
            document.Blocks.Add(t);
        }

        private void addProfissional(string pProfissional, Brush pBackground, FlowDocument document)
        {
            Paragraph para = new Paragraph();
            para.Background = pBackground;
            para.Inlines.Add(Environment.NewLine);

            Bold b = new Bold();
            b.FontFamily = new FontFamily("WebDings");
            b.FontSize = 16;
            b.Inlines.Add("h");
            para.Inlines.Add(b);

            b = new Bold();
            b.FontFamily = new FontFamily("Arial");
            b.FontSize = 12;
            b.Inlines.Add(" Profissional: " + pProfissional);
            para.Inlines.Add(b);

            document.Blocks.Add(para);
        }

        private void addResumo(String texto, Brush pBackground, FlowDocument document)
        {
            if (!String.IsNullOrEmpty(texto))
            {
                Paragraph para = new Paragraph();
                para.Background = pBackground;
                para.Inlines.Add(Environment.NewLine);
                para.Inlines.Add(new Run(texto));

                document.Blocks.Add(para);
            }
        }

        #endregion

        #region Eventos

        private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            txtEvolucao.CaretPosition = txtEvolucao.Document.ContentStart;
        }

        private void HMVButton_Click_1(object sender, RoutedEventArgs e)
        {
            FindAndSelect();
        }

        private void FindAndSelect()
        {
            String findText = txtSearch.Text;
            if (String.IsNullOrEmpty(findText))
            {
                return;
            }

            if (manager == null)
            {
                manager = new FindAndReplaceManager(txtEvolucao.Document);
            }

            manager.CurrentPosition = txtEvolucao.CaretPosition;

            TextRange textRange = manager.FindNext(findText, findOptions);
            if (textRange != null)
            {
                txtEvolucao.Focus();
                txtEvolucao.Selection.Select(textRange.Start, textRange.End);
            }
            else
            {
                if (manager.CurrentPosition.CompareTo(txtEvolucao.Document.ContentEnd) == 0)
                {
                    MessageBox.Show("Nenhuma nova ocorrência encontrada, você chegou ao final do documento!");
                    txtEvolucao.CaretPosition = txtEvolucao.Document.ContentStart;
                    manager.CurrentPosition = txtEvolucao.CaretPosition;
                }
            }
        }

        private void HMVButton_Click_2(object sender, RoutedEventArgs e)
        {
            MontaRTF();
        }

        #endregion


    }
}
