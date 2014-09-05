using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddCidCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddCidCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmAltaMedica))
            {
                if ((ViewModel as vmAltaMedica).CidSelecionado != null)
                {
                    if ((ViewModel as vmAltaMedica).SumarioAlta.Cids.Where(x => x.Id == (ViewModel as vmAltaMedica).CidSelecionado.Id).Count() > 0)
                        return false;
                    if (!((ViewModel as vmAltaMedica).CidSelecionado.Sexo == Core.Domain.Enum.SexoCid.Ambos || (ViewModel as vmAltaMedica).SumarioAlta.Atendimento.Paciente.Sexo == Core.Domain.Enum.Sexo.Indefinido))
                        if (!((ViewModel as vmAltaMedica).CidSelecionado.Sexo.ToString() == (ViewModel as vmAltaMedica).SumarioAlta.Atendimento.Paciente.Sexo.ToString()))
                        {
                            DXMessageBox.Show("Este CID não atende ao sexo do paciente, selecione um novo CID.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                            return false;
                        }
                    if ((ViewModel as vmAltaMedica).CidSelecionado.OPC.HasValue && (ViewModel as vmAltaMedica).CidSelecionado.OPC == SimNao.Nao)
                    {
                        DXMessageBox.Show("Este CID não atende ao sexo do paciente, selecione um novo CID.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
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

            if (ViewModel.GetType() == typeof(vmAltaMedica))
            {
                (ViewModel as vmAltaMedica).SumarioAlta.Atendimento.DomainObject.AddCid((ViewModel as vmAltaMedica).CidSelecionado.DomainObject);
                (ViewModel as vmAltaMedica).SumarioAlta.Atendimento.Save();

                (ViewModel as vmAltaMedica).SumarioAlta.Cids.Add((ViewModel as vmAltaMedica).CidSelecionado);
                (ViewModel as vmAltaMedica).CidSelecionado = null;
            }
        }

        #endregion
    }
}

