using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddPlanoPosAltaCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;

        #endregion

        #region Construtor

        public AddPlanoPosAltaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmPosAlta))
            {
                if ((ViewModel as vmPosAlta).IsCadastroOutros)
                {
                    if (string.IsNullOrWhiteSpace((ViewModel as vmPosAlta).PlanoPosAltaSelecionado.NomeComercial) && string.IsNullOrWhiteSpace((ViewModel as vmPosAlta).PlanoPosAltaSelecionado.DescricaoProduto))
                        return false;
                }
                if ((ViewModel as vmPosAlta).PlanoPosAltaSelecionado != null)
                    if (!(ViewModel as vmPosAlta).PlanoPosAltaSelecionado.IsValid)
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

            if (ViewModel.GetType() == typeof(vmPosAlta))
            {
                if ((ViewModel as vmPosAlta).TipoMedicamento != null)
                    (ViewModel as vmPosAlta).PlanosPosAlta.Add((ViewModel as vmPosAlta).PlanoPosAltaSelecionado);

                if ((ViewModel as vmPosAlta).PlanoPosAltaSelecionado != null)
                    (ViewModel as vmPosAlta).PlanoPosAltaSelecionado.EndEdit();
                
                (ViewModel as vmPosAlta).SetaPlanoPosAltaSelecionado();
            }
        }

        #endregion
    }
}
