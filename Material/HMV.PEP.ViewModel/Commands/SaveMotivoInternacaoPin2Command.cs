using System;
using System.Windows.Input;
using HMV.Core.Interfaces;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.SumarioDeAtendimento;
using HMV.PEP.ViewModel.PEP.MotivoInternacaoPin2;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.ViewModel.Commands
{
    public class SaveMotivoInternacaoPin2Command : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;

        #endregion

        #region Construtor

        public SaveMotivoInternacaoPin2Command(object pViewModel)
        {
            ViewModel = pViewModel;
        }
        #endregion

        #region ICommand Members

        /// <summary>
        /// Se o método pode ser executado.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (ViewModel.GetType() == typeof(vmMotivoInternacaoPim2))
            {
                if ((ViewModel as vmMotivoInternacaoPim2).vmPin2.Pin2Selecionado.IsNull() || !(ViewModel as vmMotivoInternacaoPim2).Atendimento.MotivoInternacao.HasItems())
                    return true;
            }
            return true;
        }

        /// <summary>
        /// Dispara quando o canexecute muda.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>        
        /// Executa o procedimento do comando referente a tarefa.
        /// </summary>
        public void Execute(object parameter)
        {
            if (!this.CanExecute(parameter)) return;

            if (ViewModel.GetType() == typeof(vmMotivoInternacaoPim2))
            {
                if ((ViewModel as vmMotivoInternacaoPim2).IsVisiblePim2 && (ViewModel as vmMotivoInternacaoPim2).Atendimento.MotivoInternacao.Count > 0)
                    (ViewModel as vmMotivoInternacaoPim2).Atendimento.PIN2.Add((ViewModel as vmMotivoInternacaoPim2).vmPin2.Pin2Selecionado);

                if ((ViewModel as vmMotivoInternacaoPim2).Atendimento.MotivoInternacao.Count > 0)
                    (ViewModel as vmMotivoInternacaoPim2).Atendimento.Save();

                //(ViewModel as vmMotivoInternacaoPim2).RefreshMotivoInternacao();
            }
        }

        #endregion
    }
}

