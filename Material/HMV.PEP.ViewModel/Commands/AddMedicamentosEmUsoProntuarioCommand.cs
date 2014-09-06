using System;
using System.Linq;
using HMV.PEP.ViewModel.PEP;
using System.Windows.Input;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddMedicamentosEmUsoProntuarioCommand : ICommand
    {

        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddMedicamentosEmUsoProntuarioCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmMedicamentosEmUsoProntuario))
            {
                if ((ViewModel as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado == null)
                    return false;
                if (string.IsNullOrWhiteSpace((ViewModel as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado.Medicamento))
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

            if (ViewModel.GetType() == typeof(vmMedicamentosEmUsoProntuario))
            {
                if ((ViewModel as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado != null)
                {
                    if ((ViewModel as vmMedicamentosEmUsoProntuario).Paciente.MedicamentosEmUso.Where(x => x.ID == (ViewModel as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado.ID).Count() == 0)
                    {
                        (ViewModel as vmMedicamentosEmUsoProntuario).RemoveSemMedicamentoEmUso();
                        (ViewModel as vmMedicamentosEmUsoProntuario).Paciente.MedicamentosEmUso.Add((ViewModel as vmMedicamentosEmUsoProntuario).medicamentosEmUsoProntuarioSelecionado);                       
                    }
                }
                (ViewModel as vmMedicamentosEmUsoProntuario).SavePacienteCommand.Execute(null);
            }
        }

        #endregion

    }
}
