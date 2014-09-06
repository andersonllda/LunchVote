using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddPrestadorCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;        
        #endregion

        #region Construtor       

        public AddPrestadorCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmMedicos))
            {
                if ((ViewModel as vmMedicos).Prestadores.Where(x => x.Id == (ViewModel as vmMedicos).PrestadorSelecionado.Id).Count() > 0)
                {
                    //DXMessageBox.Show("Este médico já está adicionado.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (ViewModel.GetType() == typeof(vmMedicos))
            {
                (ViewModel as vmMedicos).Prestadores.Add((ViewModel as vmMedicos).PrestadorSelecionado);
                (ViewModel as vmMedicos).PrestadorSelecionado = null;
            }           
        }

        #endregion
    }
}

