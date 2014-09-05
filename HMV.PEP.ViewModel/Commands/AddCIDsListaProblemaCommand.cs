using System;
using System.Windows.Input;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using System.Collections.Generic;
using System.Linq;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddCIDsListaProblemaCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor
        public AddCIDsListaProblemaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmCIDListaProblema))
            {
                var IncluiCid = (from x in (ViewModel as vmCIDListaProblema).CidMVCollection
                                 select x).Where(y => y.Incluir.Equals(true)).ToList();
                if (IncluiCid.Count > 0)
                    return true;
            }

            return false;
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
        //    if (!this.CanExecute(parameter)) return;

        //    if (ViewModel.GetType() == typeof(vmCIDsAtendimento))
        //    {               
        //        (ViewModel as vmCIDsAtendimento).Atendimento.DomainObject.AddCid((ViewModel as vmCIDsAtendimento)._cidselecionado.DomainObject);
        //        (ViewModel as vmCIDsAtendimento).Atendimento.CIDs.Add((ViewModel as vmCIDsAtendimento)._cidselecionado);
        //        (ViewModel as vmCIDsAtendimento).EndEdit();
        //    }
        }
        #endregion
    }
}
