using LunchVote.Core.Commands;
using LunchVote.Core.HMV.Core.Framework.Expression;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Core.ViewModelBaseClasses
{
    public abstract class ViewModelBase : NotifyPropertyChanged
    {
        #region ----- Propriedades Privadas -----
        private CommandMap _commands;        
        private bool _validarCamposObrigatorios;
        #endregion

        #region ----- Construtor -----
        static ViewModelBase()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        protected ViewModelBase()
        {           
            this._commands = new CommandMap();
            this._commands.AddCommand(enumCommand.CommandVotar, this.CommandVotar, this.CommandCanExecuteVotar);            
            this._commands.AddCommand(enumCommand.CommandVisualizar, this.CommandVisualizar, this.CommandCanExecuteVisualizar);
            this._commands.AddCommand(enumCommand.CommandFechar, this.CommandFechar, this.CommandCanExecuteFechar);                  
        }
        #endregion

        #region ----- Propriedades Públicas -----        
        public CommandMap Commands
        {
            get { return _commands; }
        }   
        #endregion               

        #region ----- Commands -----
        /// <summary>
        /// Executa o Método Votar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandVotar(object param)
        {           
        }
        /// <summary>
        /// Verifica se pode executar o CommandVotar 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteVotar(object param)
        {
            return true;
        }
      
        /// <summary>
        /// Executa o Método Visualizar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandVisualizar(object param)
        {            
        }
        /// <summary>
        /// Verifica se pode executar o Visualizar 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteVisualizar(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Fechar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandFechar(object param)
        {           
        }

        /// <summary>
        /// Verifica se pode executar o Fechar 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteFechar(object param)
        {
            return true;
        }        
        #endregion     
    }   
}
