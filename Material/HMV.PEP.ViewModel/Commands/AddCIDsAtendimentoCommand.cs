using System;
using System.Windows.Input;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using System.Collections.Generic;
using System.Linq;
using HMV.PEP.Interfaces;
using StructureMap;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddCIDsAtendimentoCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;

        #endregion

        #region Construtor

        public AddCIDsAtendimentoCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmCIDsAtendimento))
                if ((ViewModel as vmCIDsAtendimento)._cidselecionado != null)
                {
                    wrpCid cid = (from x in (ViewModel as vmCIDsAtendimento).CidsAtendimento
                                  select x).Where(z => z.Id.Equals(((ViewModel as vmCIDsAtendimento)._cidselecionado.Id))).FirstOrDefault();

                    if (cid == null) return true;
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
            if (!this.CanExecute(parameter)) return;

            if (ViewModel.GetType() == typeof(vmCIDsAtendimento))
            {               
                (ViewModel as vmCIDsAtendimento).Atendimento.DomainObject.AddCid((ViewModel as vmCIDsAtendimento)._cidselecionado.DomainObject);
                (ViewModel as vmCIDsAtendimento).Atendimento.CIDs.Add((ViewModel as vmCIDsAtendimento)._cidselecionado);
                (ViewModel as vmCIDsAtendimento).Editou();
                (ViewModel as vmCIDsAtendimento).EndEdit();

                if (parameter != null)
                {                                   
                    ICidService sr = ObjectFactory.GetInstance<ICidService>();
                    sr.verificaSeOCIDJaEstaNaListaDeProblemas((ViewModel as vmCIDsAtendimento)._cidselecionado.DomainObject, (ViewModel as vmCIDsAtendimento).Atendimento.DomainObject, ObjectFactory.GetInstance<HMV.Core.Interfaces.IUsuariosService>().FiltraPorID(parameter as string));                
                }
            }
        }
        #endregion
    }
}
