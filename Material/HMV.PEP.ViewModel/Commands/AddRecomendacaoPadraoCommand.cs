using System;
using System.Windows.Input;
using HMV.PEP.ViewModel.SumarioDeAlta;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Linq;
using HMV.Core.Wrappers;
using HMV.Core.Wrappers.ObjectWrappers;

namespace HMV.PEP.ViewModel.Commands
{
    public class AddRecomendacaoPadraoCommand : ICommand
    {
        #region Propriedades Privadas

        private readonly object ViewModel;
        #endregion

        #region Construtor

        public AddRecomendacaoPadraoCommand(object pViewModel)
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
                /*if ((ViewModel as vmRecomendacaoPadrao).tituloSelecionado != null)
                    if ((ViewModel as vmCausaExterna).CausasExternas.Where(x => x.Cid.Id == (ViewModel as vmCausaExterna).CausaExternaSelecionada.Cid.Id).Count() > 0)
                    {
                        return false;
                    }*/
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

            if (ViewModel.GetType() == typeof(vmRecomendacaoPadrao) && (ViewModel as vmRecomendacaoPadrao).consultaRecomendacao != null)
            {
                foreach (var item in (ViewModel as vmRecomendacaoPadrao).consultaRecomendacao )
	            {
                    if ((ViewModel as vmRecomendacaoPadrao).vmRecomendacao.SumarioAlta.recomendacoes.Count(x => x.Recomendacao.Id == item.Recomendacao.Id) != 0)
                        (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.SumarioAlta.recomendacoes.FirstOrDefault(x => x.Recomendacao.Id == item.Recomendacao.Id).Descricao = item.Descricao;
                    else
                        (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.SumarioAlta.recomendacoes.Add(new wrpSumarioRecomendacao(item.Recomendacao.DomainObject, item.Descricao
                            , (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.SumarioAlta.Atendimento.DomainObject));
            	}

                (ViewModel as vmRecomendacaoPadrao).vmRecomendacao.AtualizaLista();
            }
        }
        #endregion
    }
}

