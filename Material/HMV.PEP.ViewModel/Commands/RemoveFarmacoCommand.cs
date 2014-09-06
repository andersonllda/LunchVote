using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using System.Windows;
using System.Linq;
using DevExpress.Xpf.Core;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemoveFarmacoCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor

        public RemoveFarmacoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmFarmacos))
            {
                var t = (ViewModel as vmFarmacos).Farmacos;
                if ((ViewModel as vmFarmacos).Farmacos.Count(x=>x.Selecionado) == 0)
                    return false;
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
                if (DXMessageBox.Show("Deseja realmente Excluir o Fármaco selecionado?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    while ((ViewModel as vmFarmacos).Farmacos.Count(x => x.Selecionado) > 0)
                    {
                        (ViewModel as vmFarmacos).Farmacos.Remove((ViewModel as vmFarmacos).Farmacos.First(x => x.Selecionado));
                    }
                    (ViewModel as vmFarmacos).SumarioAlta.Save();
                    (ViewModel as vmFarmacos).Refresh();
                }                
            }
        }
        #endregion
    }
}

