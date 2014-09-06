using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using DevExpress.Xpf.SpellChecker;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.PEP.ViewModel.PEP.Evolucao;
using DevExpress.XtraSpellChecker;
using DevExpress.Xpf.Editors;
using System;
using DevExpress.XtraSpellChecker.Native;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.Xpf.RichEdit.Menu;
using DevExpress.XtraRichEdit.Commands;
using HMV.Core.Framework.Extensions;
using System.Collections.Generic;
using HMV.Core.Framework.Commands;
using System.Windows.Documents;
using DevExpress.Xpf.Core;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Utils;
using System.Windows.Input;
using WindowsInput;

namespace HMV.PEP.WPF.Evolucao
{
    /// <summary>
    /// Interaction logic for winCadEvolucao.xaml
    /// </summary>
    ///  
    public partial class winCadEvolucao : WindowBase
    {      
        public winCadEvolucao(ViewModelBase pVM)
            : base(pVM)
        {
            InitializeComponent();
            SpellCheckTextControllersManager.Default.
                RegisterClass(typeof(RichEditControl), typeof(RichEditSpellCheckController));            
        }

        private void richEdit_Loaded(object sender, RoutedEventArgs e)
        {
            CustomRichEditCommandFactoryService svc = new CustomRichEditCommandFactoryService(richEdit, richEdit.GetService<IRichEditCommandFactoryService>());
            richEdit.RemoveService(typeof(IRichEditCommandFactoryService));
            richEdit.AddService(typeof(IRichEditCommandFactoryService), svc);         
            App.SPChecker.SpellingFormType = SpellingFormType.Outlook;
            richEdit.SpellChecker = App.SPChecker;
            App.SPChecker.SpellCheckMode = SpellCheckMode.AsYouType;         
        }

        //private void TextEdit_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        //{           
        //    if (string.IsNullOrEmpty(txtEvolucao.Text))
        //        lbQtdCaracter.Content = "Máximo 12000 caracteres";
        //    else lbQtdCaracter.Content = string.Format(txtEvolucao.Text.Length < 12000 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 12000 - txtEvolucao.Text.Length);
        //}

        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //if ((this.DataContext as VMPEPEvolucao).BuscaEvolucoesNaoImpressas().Count > 0)
            //ChamaEvolucoes();
        }

        private void btnEvolucaoPadrao_Click(object sender, RoutedEventArgs e)
        {
            var win = new winSelEvolucaoPadrao((this.DataContext as VMPEPEvolucao).Evolucao);
            win.ShowDialog(this);
        }

        DevExpress.XtraRichEdit.API.Native.DocumentPosition pos;
        bool pControlaTam;
        private void richEdit_TextChanged(object sender, EventArgs e)
        {
            //richEdit.Document.CaretPosition = richEdit.Document.Range.End;
            if (((RichEditControl)sender).Text.Length > 12000 && !pControlaTam)
            {
                pos = richEdit.Document.CaretPosition;
                pControlaTam = true;
                Undo();
                pControlaTam = false;
            }

            (this.DataContext as VMPEPEvolucao).Evolucao.Evolucao = ((RichEditControl)sender).Text;

            if (string.IsNullOrEmpty(richEdit.Text))
                lbQtdCaracter.Content = "Máximo 12000 caracteres";
            else lbQtdCaracter.Content = string.Format(richEdit.Text.Length < 12000 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 12000 - richEdit.Text.Length);
        }
        public void Undo()
        {
            richEdit.Text = (this.DataContext as VMPEPEvolucao).Evolucao.Evolucao;
            richEdit.Document.CaretPosition = pos.ToInt() > 12000 ? richEdit.Document.Range.End : pos;
        }

        private void richEdit_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            for (int i = e.Menu.ItemLinks.Count - 1; i >= 0; i--)
            {
                if (e.Menu.ItemLinks[i] is DevExpress.Xpf.Bars.BarItemLink)
                {
                    RichEditMenuItem item = ((DevExpress.Xpf.Bars.BarItemLink)((e.Menu.ItemLinks[i]))).Item as RichEditMenuItem;
                    if (item != null && item.Command is RichEditUICommand)
                    {
                        if (RichEditCommandId.CopySelection != ((RichEditUICommand)item.Command).CommandId
                            && RichEditCommandId.CutSelection != ((RichEditUICommand)item.Command).CommandId
                            && RichEditCommandId.PasteSelection != ((RichEditUICommand)item.Command).CommandId)
                            e.Menu.ItemLinks.Remove(e.Menu.ItemLinks[i]);

                        if (RichEditCommandId.PasteSelection == ((RichEditUICommand)item.Command).CommandId)
                            item.Command = new CustomPasteCommand();
                    }

                    if (item != null && item.Command is XpfICommandAdapter)
                    {
                        if ((item as DevExpress.Xpf.Bars.BarItem).Content.ToString() == "Ignore")
                            (item as DevExpress.Xpf.Bars.BarItem).Content = "Ignorar";
                        if ((item as DevExpress.Xpf.Bars.BarItem).Content.ToString() == "Ignore All")
                            (item as DevExpress.Xpf.Bars.BarItem).Content = "Ignorar Tudo";
                        if ((item as DevExpress.Xpf.Bars.BarItem).Content.ToString() == "Add to Dictionary")
                            (item as DevExpress.Xpf.Bars.BarItem).Content = "Adicionar ao dicionário";
                        if ((item as DevExpress.Xpf.Bars.BarItem).Content.ToString() == "Delete Repeated Word")
                            (item as DevExpress.Xpf.Bars.BarItem).Content = "Deletar Palavra Repetida";
                    }
                }
            }
        }

        public class CustomPasteCommand : RichEditUICommand
        {
            protected override void ExecuteCommand(RichEditControl control, RichEditCommandId commandId, object parameter)
            {           
                ((RichEditControl)control).Document.Paste(DevExpress.XtraRichEdit.DocumentFormat.PlainText);
            }
        }

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
            InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
        }      
    }

    public class CustomRichEditCommandFactoryService : IRichEditCommandFactoryService
    {
        readonly IRichEditCommandFactoryService service;
        readonly RichEditControl control;
        public CustomRichEditCommandFactoryService(RichEditControl control, IRichEditCommandFactoryService service)
        {
            Guard.ArgumentNotNull(control, "control");
            Guard.ArgumentNotNull(service, "service");
            this.control = control;
            this.service = service;
        }

        #region IRichEditCommandFactoryService Members
        public RichEditCommand CreateCommand(RichEditCommandId id)
        {
            if (id == RichEditCommandId.PasteSelection)
                return new CustomPasteSelectionCommand(control);
            return service.CreateCommand(id);
        }
        #endregion
    }
    public class CustomPasteSelectionCommand : PasteSelectionCommand
    {
        public CustomPasteSelectionCommand(RichEditControl control)
            : base(control)
        {

        }
        public override void Execute()
        {
            ((RichEditControl)this.Control).Document.Paste(DevExpress.XtraRichEdit.DocumentFormat.PlainText);
        }
    }
}
