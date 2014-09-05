using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using HMV.Core.Domain.Views.PEP;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.WPF;
using HMV.PEP.Services;
using HMV.PEP.ViewModel;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.PassagemPlantaoE;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.LayoutControl;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.Commands;
using DevExpress.Xpf.Grid;
using DevExpress.XtraEditors;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winPassagemPlantaoEmergencia.xaml
    /// </summary>
    public partial class winPassagemPlantaoEmergencia : WindowBase
    {
        bool _pediatrico;
        private CommandMap _commands;
        public winPassagemPlantaoEmergencia(bool pPediatrico)
        {
            InitializeComponent();

            _pediatrico = pPediatrico;

            UIHelper.SetBusyState();
            TableView gv = (TableView)gdEvolucao.View;
            gv.ScrollingMode = ScrollingMode.Smart;
            
            this._commands = new CommandMap();
            this._commands.AddCommand("RemoveMarcasCommand", this.RemoveMarcasCommand);
            this._commands.AddCommand("RemoveMarcasCommand2", this.RemoveMarcasCommand2);
            this._commands.AddCommand("RemoveMarcasCommand3", this.RemoveMarcasCommand3);
            this._commands.AddCommand("MarcarAzul", this.MarcarAzul);
            this._commands.AddCommand("MarcarAzul2", this.MarcarAzul2);
            this._commands.AddCommand("MarcarAzul3", this.MarcarAzul3);
            this._commands.AddCommand("MarcarAmarelo", this.MarcarAmarelo);
            this._commands.AddCommand("MarcarAmarelo2", this.MarcarAmarelo2);
            this._commands.AddCommand("MarcarAmarelo3", this.MarcarAmarelo3);
            this._commands.AddCommand("MarcarVermelho", this.MarcarVermelho);
            this._commands.AddCommand("MarcarVermelho2", this.MarcarVermelho2);
            this._commands.AddCommand("MarcarVermelho3", this.MarcarVermelho3);
            this._commands.AddCommand("ColarTextoSimples", this.ColarTextoSimples); 

            //Busco as informações das listas aqui para reutilizar os serviços do PEP.
            EmergenciaServiceBase _service;
            if (_pediatrico)
                _service = new EmergenciaPediatricaService();
            else
                _service = new EmergenciaAdultoService();

            IList<vPacienteInternado> listInternados = _service.BuscaPacientesInternados();
            IList<vEmergenciaPEP> listUrgencia = _service.BuscaEmAtendimento(string.Empty);

            this.DataContext = new vmPassagemPlantaoEmergencia(App.Usuario, listInternados, listUrgencia, _pediatrico);
            DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true;           

            UIHelper.SetBusyState();
        }

        public CommandMap Commands
        {
            get { return _commands; }
        }

        private void ColarTextoSimples(object obj)
        {           
            string paste = Clipboard.GetText();
            var rb = (obj as RichTextBox);

            string text = new TextRange(rb.Document.ContentStart, rb.Document.ContentEnd).Text;

            if (paste.Length + text.Length > 11000)
            {
                DXMessageBox.Show("O texto copiado excede a quantidade máxima de caracteres deste campo!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Clipboard.SetText(paste);
            Clipboard.SetDataObject(paste);
            rb.Selection.Text = paste;
            //rb.ScrollToEnd();
            rb.CaretPosition = rb.Document.ContentEnd;
            //rb.Paste();
            foreach (var block in rb.Document.Blocks)
            {
                Paragraph p = block as Paragraph;
                p.LineHeight = 10;
                p.FontFamily = new FontFamily("Segoe UI");
                p.FontSize = 12;
                p.Margin = new Thickness(0);
            }          
        }    
        public event EventHandler ExecuteFechou;
        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            this.Fechou();
        }

        protected virtual void Fechou()
        {
            if (ExecuteFechou != null) ExecuteFechou(this, EventArgs.Empty);
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmPassagemPlantaoEmergencia).ListaEvolucoes.Count(x => x.Imprime == true) > 0)
            {
                if ((this.DataContext as vmPassagemPlantaoEmergencia).MostraEnfermagem)
                {
                    UIHelper.SetBusyState();
                    rptPassagemPlantaoEnf report = new rptPassagemPlantaoEnf();
                    report.lblObservacoes.Text = (this.DataContext as vmPassagemPlantaoEmergencia).ObservacaoEvolucao;
                    report.DataSource = (this.DataContext as vmPassagemPlantaoEmergencia).ListaEvolucoesImpressao;
                    
                    winRelatorio win = new winRelatorio(report, true, "Passagem de Plantão", false);
                    UIHelper.SetBusyState();
                    win.ShowDialog(this);
                }
                else
                {
                    UIHelper.SetBusyState();
                    rptPassagemPlantao report = new rptPassagemPlantao();
                    report.lblObservacoes.Text = (this.DataContext as vmPassagemPlantaoEmergencia).ObservacaoEvolucao;
                    report.DataSource = (this.DataContext as vmPassagemPlantaoEmergencia).ListaEvolucoesImpressao;
                    
                    winRelatorio win = new winRelatorio(report, true, "Passagem de Plantão", false);
                    UIHelper.SetBusyState();
                    win.ShowDialog(this);
                }
            }
        }

        private void Localizar_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UIHelper.SetBusyState();
            (this.DataContext as vmPassagemPlantaoEmergencia).CarregaLeitosVagos();
            winPassagemPlantaoLocalizarPaciente win = new winPassagemPlantaoLocalizarPaciente(this.DataContext as vmPassagemPlantaoEmergencia);
            UIHelper.SetBusyState();
            win.ShowDialog(this);
        }

        private void Historico_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UIHelper.SetBusyState();
            (this.DataContext as vmPassagemPlantaoEmergencia).CarregaHistorico();
            winPassagemPlantaoHistorico win = new winPassagemPlantaoHistorico(this.DataContext as vmPassagemPlantaoEmergencia);
            UIHelper.SetBusyState();
            win.ShowDialog(this);
        }

        private void btnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmPassagemPlantaoEmergencia).ExecuteSalvar();
            Memory.MinimizeMemory();
            UIHelper.SetBusyState();
            EmergenciaServiceBase _service;
            if (_pediatrico)
                _service = new EmergenciaPediatricaService();
            else
                _service = new EmergenciaAdultoService();
            IList<vPacienteInternado> listInternados = _service.BuscaPacientesInternados();
            IList<vEmergenciaPEP> listUrgencia = _service.BuscaEmAtendimento(string.Empty);
            (this.DataContext as vmPassagemPlantaoEmergencia).Atualiza(listInternados, listUrgencia);
            UIHelper.SetBusyState();
            Memory.MinimizeMemory();
        }

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Memory.MinimizeMemory();
            (this.DataContext as vmPassagemPlantaoEmergencia).CloseConnection();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UIHelper.SetBusyState();
            (this.DataContext as vmPassagemPlantaoEmergencia).CarregaHistoricoObservacao();
            winPassagemPlantaoHistoricoObservacao win = new winPassagemPlantaoHistoricoObservacao(this.DataContext as vmPassagemPlantaoEmergencia);
            UIHelper.SetBusyState();
            win.ShowDialog(this);
        }

        internal string Select(RichTextBox rtb, Color color)
        {
            TextSelection textRange = rtb.Selection;
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));
            return rtb.Selection.Text;
        }

        private void RemoveMarcasCommand(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB");
                this.Select((RichTextBox)rb, Colors.Transparent);
            }
            catch { }
        }
        private void RemoveMarcasCommand2(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB2");
                this.Select((RichTextBox)rb, Colors.Transparent);
            }
            catch { }
        }
        private void RemoveMarcasCommand3(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB3");
                this.Select((RichTextBox)rb, Colors.Transparent);
            }
            catch { }
        }

        private void MarcarAzul(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB");
                this.Select((RichTextBox)rb, Colors.LightBlue);
            }
            catch { }
        }
        private void MarcarAzul2(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB2");
                this.Select((RichTextBox)rb, Colors.LightBlue);
            }
            catch { }
        }
        private void MarcarAzul3(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB3");
                this.Select((RichTextBox)rb, Colors.LightBlue);
            }
            catch { }
        }

        private void MarcarAmarelo(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB");
                this.Select((RichTextBox)rb, Colors.Yellow);
            }
            catch { }
        }
        private void MarcarAmarelo2(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB2");
                this.Select((RichTextBox)rb, Colors.Yellow);
            }
            catch { }
        }
        private void MarcarAmarelo3(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB3");
                this.Select((RichTextBox)rb, Colors.Yellow);
            }
            catch { }
        }

        private void MarcarVermelho(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB");
                this.Select((RichTextBox)rb, Colors.LightCoral);
            }
            catch { }
        }
        private void MarcarVermelho2(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB2");
                this.Select((RichTextBox)rb, Colors.LightCoral);
            }
            catch { }
        }
        private void MarcarVermelho3(object obj)
        {
            try
            {
                var _obj = (obj as Button).FindAncestor(typeof(LayoutGroup));
                var rb = (_obj as LayoutGroup).FindName("mainRTB3");
                this.Select((RichTextBox)rb, Colors.LightCoral);
            }
            catch { }
        }

        private void mainRTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = tr.Text.Replace("\r", "").Replace("\n", "");
            if (text.Length >= 11000)
            {
                e.Handled = true;
            }
        }

        private void mainRTB2_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            RichTextBox rtb = (RichTextBox)sender;
            TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = tr.Text.Replace("\r", "").Replace("\n", "");
            if (text.Length >= 3000)
            {
                e.Handled = true;
            }
        }

        private void TextEdit_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void TextEdit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
                if (e.Key == Key.Enter)
                {
                    btnPesquisar.Focus();
                }
            
        }       
    }
}