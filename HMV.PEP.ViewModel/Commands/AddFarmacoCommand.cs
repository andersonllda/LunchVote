using System;
using System.Linq;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddFarmacoCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddFarmacoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmFarmacos))
            {
                if ((ViewModel as vmFarmacos).FarmacoSelecionado != null)
                    if ((ViewModel as vmFarmacos).Farmacos.Where(x => x.Produto.Id == (ViewModel as vmFarmacos).FarmacoSelecionado.Produto.Id).Count() > 0)
                    {
                        return false;
                    }
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

            if (ViewModel.GetType() == typeof(vmFarmacos))
            {
                (ViewModel as vmFarmacos).Farmacos.Add((ViewModel as vmFarmacos).FarmacoSelecionado);
                (ViewModel as vmFarmacos).FarmacoSelecionado = null;
            }
        }

        #endregion
    }
}

