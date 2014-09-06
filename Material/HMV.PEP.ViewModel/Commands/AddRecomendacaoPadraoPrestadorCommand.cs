using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using System.Collections;
using HMV.Core.Wrappers;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddRecomendacaoPadraoPrestadorCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddRecomendacaoPadraoPrestadorCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmRecomendacaoPadrao))
            {
                if (String.IsNullOrWhiteSpace((ViewModel as vmRecomendacaoPadrao).NovoTitulo))
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

            if (ViewModel.GetType() == typeof(vmRecomendacaoPadrao))
            {                
                foreach (var item in (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.ListaDeSumarioRecomendacao.Where(x => !String.IsNullOrWhiteSpace(x.Descricao)))
                {
                    wrpRecomendacaoPadrao wrpRec = new wrpRecomendacaoPadrao(item.Recomendacao.DomainObject, (ViewModel as vmRecomendacaoPadrao).NovoTitulo, item.Descricao)
                    {
                        Prestador = (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.Usuarios.Prestador,
                        IsRN = (ViewModel as vmRecomendacaoPadrao).IsRN ? SimNao.Sim : SimNao.Nao
                    };

                    (ViewModel as vmRecomendacaoPadrao).recomendacaoPadrao.Add(wrpRec);
                }

                (ViewModel as vmRecomendacaoPadrao).Refresh();
            }
        }
        #endregion
    }
}

