using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddProcedimentoAltaCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddProcedimentoAltaCommand(object pViewModel)
        {
            this.ViewModel = pViewModel;
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Se o método pode ser executado.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (ViewModel.GetType() == typeof(vmProcedimento))
            {
                if ((ViewModel as vmProcedimento).procedimentoAltaSelecionado != null)
                    return true;
                //if ((ViewModel as vmProcedimento).ProcedimentosAlta
                //    .Where(x => x.Cirurgia.cd_cirurgia == (ViewModel as vmProcedimento).procedimentoAltaSelecionado.Cirurgia.cd_cirurgia && 
                //        x.Data.Date == (ViewModel as vmProcedimento).procedimentoAltaSelecionado.Data.Date).Count() > 0)
                //{
                //    return false;
                //}
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

            if (ViewModel.GetType() == typeof(vmProcedimento))
            {
                if ((ViewModel as vmProcedimento).ProcedimentosAlta.Count(s => s.Cirurgia.cd_cirurgia == (ViewModel as vmProcedimento).procedimentoAltaSelecionado.Cirurgia.cd_cirurgia) == 0)
                    (ViewModel as vmProcedimento).ProcedimentosAlta.Add((ViewModel as vmProcedimento).procedimentoAltaSelecionado);
                //(ViewModel as vmProcedimento).procedimentoAltaSelecionado = null;
                (ViewModel as vmProcedimento).Refresh();
            }
        }

        #endregion
    }
}

