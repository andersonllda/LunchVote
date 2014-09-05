using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.Core.Wrappers;
using HMV.Core.Framework.Exception;
using NHibernate.Validator.Engine;
using System.Windows;
using DevExpress.Xpf.Core;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Linq;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Extensions;
using HMV.Core.Domain.Enum;

namespace HMV.PEP.ViewModel.Commands
{
    public class RemoveCIDsAtendimentoCommand : ICommand
    {
        #region Propriedades Privadas
        private readonly object ViewModel;
        #endregion

        #region Construtor
        public RemoveCIDsAtendimentoCommand(object pViewModel)
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
            //if (ViewModel.GetType() == typeof(vmAltaMedica))
            //{
            //    if ((ViewModel as vmAltaMedica).CidSelecionado == null)
            //        return false;
            //}
            //return true;



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
                (ViewModel as vmCIDsAtendimento).CidsAtendimento.Remove((ViewModel as vmCIDsAtendimento)._cidselecionado);

                // se existir o CID para a lista de problemas neste atendimento deve excluir
                if ((ViewModel as vmCIDsAtendimento).Atendimento != null)
                {
                    ICidService serv = ObjectFactory.GetInstance<ICidService>();
                    Cid meucid = serv.FiltraPorCid10((ViewModel as vmCIDsAtendimento)._cidselecionado.Id);
                    if (meucid != null)
                    {
                        Paciente _paciente = (ViewModel as vmCIDsAtendimento).Atendimento.Paciente.DomainObject;

                        IPacienteService pac = ObjectFactory.GetInstance<IPacienteService>();
                        ProblemasPaciente problema = _paciente.ProblemasPaciente.Where(x => x.CID.IsNotNull() && x.CID.Id.Equals(meucid.Id) && x.Atendimento.IsNotNull() && x.Atendimento.ID.Equals((ViewModel as vmCIDsAtendimento).Atendimento.ID)).SingleOrDefault();
                        if (problema != null)
                        {
                            problema.Status = StatusAlergiaProblema.Excluído;
                            pac.Salvar(_paciente);
                        }
                    }
                }

                (ViewModel as vmCIDsAtendimento).Editou();
                (ViewModel as vmCIDsAtendimento).EndEdit();
            }
        }
        #endregion

    }
}
