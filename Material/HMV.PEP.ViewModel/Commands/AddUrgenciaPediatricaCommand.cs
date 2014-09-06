using System;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;
using HMV.PEP.ViewModel.UrgenciaP;
using HMV.Core.Wrappers.ObjectWrappers;
using System.Collections.Generic;
using HMV.Core.Domain.Model;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddUrgenciaPediatricaCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor
        public AddUrgenciaPediatricaCommand(object pViewModel)
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
            if (ViewModel.GetType() == typeof(vmUrgenciaPediatrica))
            {
                if ((ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado == null)                
                    return false;
                if ((ViewModel as vmUrgenciaPediatrica).PesoInformado == 0)
                    return false;
                if (!(ViewModel as vmUrgenciaPediatrica).IsValid)
                    return false;
                if (!(ViewModel as vmUrgenciaPediatrica).HabilitaCalcular)
                    return false;
                if (!(ViewModel as vmUrgenciaPediatrica).HabilitaSalvar)
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
            
            if (ViewModel.GetType() == typeof(vmUrgenciaPediatrica))
            {
                (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject.UrgenciaPediatricaAtendimentoItens = new List<UrgenciaPediatricaAtendimentoItem>();
                foreach (var item in (ViewModel as vmUrgenciaPediatrica).UrgenciaItemsCabecalho1)
                {                                        
                    var novo = new UrgenciaPediatricaAtendimentoItem((ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject);
                    novo.Droga = item.Descricao;
                    novo.DoseCalculada = item.Valor;
                    novo.UrgenciaPediatricaItem = item.Item;

                    (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject.UrgenciaPediatricaAtendimentoItens.Add(novo);
                }

                foreach (var item in (ViewModel as vmUrgenciaPediatrica).UrgenciaItemsCabecalho2)
                {
                    var novo = new UrgenciaPediatricaAtendimentoItem((ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject);
                    novo.Droga = item.Descricao;                    
                    novo.DoseCalculada = item.Valor;
                    novo.UrgenciaPediatricaItem = item.Item;

                    (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject.UrgenciaPediatricaAtendimentoItens.Add(novo);
                }

                foreach (var item in (ViewModel as vmUrgenciaPediatrica).UrgenciaItems)
                {
                    var novo = new UrgenciaPediatricaAtendimentoItem((ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject);
                    novo.Apresentacao = item.Apresentacao;
                    novo.Dose = item.Dose;
                    novo.Droga = item.Droga;
                    novo.DoseCalculada = item.Valor;
                    novo.UrgenciaPediatricaItem = item.Item;

                    (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.DomainObject.UrgenciaPediatricaAtendimentoItens.Add(novo);
                }
                
                (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado.Save();
                (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentos.Add((ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado);                                
               // (ViewModel as vmUrgenciaPediatrica).UrgenciaPediatricaAtendimentoSelecionado = null;
            }
        }

        #endregion
    }
}

