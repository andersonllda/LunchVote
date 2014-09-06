using System;
using System.Windows.Input;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.BoletimEmergencia;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddClassificacaoRiscoCommand : ICommand
    {
        #region Propriedades Privadas
        
        private readonly object ViewModel;

        #endregion

        #region Construtor
        public AddClassificacaoRiscoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmClassificacaoRisco))
            {
                if ((ViewModel as vmClassificacaoRisco).ClassificacaoSelecionada == null)
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

            if (ViewModel.GetType() == typeof(vmClassificacaoRisco))
            {
                (ViewModel as vmClassificacaoRisco).BoletimEmergencia.Classificacoes.Add(new wrpClassificacaoRisco((ViewModel as vmClassificacaoRisco).ClassificacaoSelecionada.DomainObject, 
                                                                                        (ViewModel as vmClassificacaoRisco).Usuario.DomainObject, 
                                                                                        (ViewModel as vmClassificacaoRisco).BoletimEmergencia.DomainObject));
                (ViewModel as vmClassificacaoRisco).BoletimEmergencia.Save();
                (ViewModel as vmClassificacaoRisco).Editou();
                (ViewModel as vmClassificacaoRisco).ClassificacaoSelecionada = null;                
            }  
        }
        #endregion
    }
}
