using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using HMV.Core.Framework.Commands;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Interfaces;
using System.Linq;
using HMV.Core.Framework.Helper;
using System.Windows;

namespace HMV.Core.Framework.ViewModelBaseClasses
{
    public abstract class ViewModelBase : ValidationViewModelBase//, IEditableObject, ICopyable
    {
        #region ----- Propriedades Privadas -----
        private CommandMap _commands;
        private bool _chamafecharapossalvar;
        //private HybridDictionary _oldState;
        private bool _validarCamposObrigatorios;
        #endregion

        #region ----- Construtor -----
        static ViewModelBase()
        {
            eventArgCache = new Dictionary<string, PropertyChangedEventArgs>();
        }

        protected ViewModelBase()
        {
            this._chamafecharapossalvar = true;
            this._commands = new CommandMap();
            this._commands.AddCommand(enumCommand.CommandAbrir, this.CommandAbrir, this.CommandCanExecuteAbrir);
            this._commands.AddCommand(enumCommand.CommandAbrirEx, this.CommandAbrirEx, this.CommandCanExecuteAbrirEx);
            this._commands.AddCommand(enumCommand.CommandAbrirEx01, this.CommandAbrirEx01, this.CommandCanExecuteAbrirEx01);
            this._commands.AddCommand(enumCommand.CommandAbrirCadastoBase, this.CommandAbrirCadastroBase, this.CommandCanExecuteCadastoBase);
            this._commands.AddCommand(enumCommand.CommandIncluir, this.CommandIncluir, this.CommandCanExecuteIncluir);
            this._commands.AddCommand(enumCommand.CommandAlterar, this.CommandAlterar, this.CommandCanExecuteAlterar);
            this._commands.AddCommand(enumCommand.CommandPesquisar, this.CommandPesquisar, this.CommandCanExecutePesquisar);
            this._commands.AddCommand(enumCommand.CommandSelecionar, this.CommandSelecionar, this.CommandCanExecuteSelecionar);
            this._commands.AddCommand(enumCommand.CommandSalvar, this.CommandSalvar, this.CommandCanExecuteSalvar);
            this._commands.AddCommand(enumCommand.CommandExcluir, this.CommandExcluir, this.CommandCanExecuteExcluir);
            this._commands.AddCommand(enumCommand.CommandImprimir, this.CommandImprimir, this.CommandCanExecuteImprimir);
            this._commands.AddCommand(enumCommand.CommandVisualizar, this.CommandVisualizar, this.CommandCanExecuteVisualizar);
            this._commands.AddCommand(enumCommand.CommandFechar, this.CommandFechar, this.CommandCanExecuteFechar);
            this._commands.AddCommand(enumCommand.CommandCopiar, this.CommandCopiar, this.CommandCanExecuteCopiar);
            this.HabilitaBotoesBarra();
        }
        #endregion

        #region ----- Propriedades Públicas -----
        public bool MostraSelecionar { get; private set; }
        public bool MostraExcluir { get; private set; }
        public bool MostraIncluir { get; private set; }
        public bool MostraAlterar { get; private set; }
        public bool IsCollapsed { get { return false; } }

        public bool ChamaFecharAposSalvar { get { return this._chamafecharapossalvar; } set { this._chamafecharapossalvar = value; } }

        public CommandMap Commands
        {
            get { return _commands; }
        }

        public bool ValidarCamposObrigatorios
        {
            get { return this._validarCamposObrigatorios; }
            set
            {
                this._validarCamposObrigatorios = value;
                base.OnPropertyChanged(ExpressionEx.PropertyName<ViewModelBase>(x => x.ValidarCamposObrigatorios));
            }
        }

        //SERVE APENAS PARA O PROJETO DO IEP/PÓS
        public BaseCadastrosPOS? BaseCadastroPOS;
        #endregion

        #region ----- Eventos Públicos -----
        public event EventHandler EventCommandAbrir;
        public event EventHandler EventCommandAbrirEx;
        public event EventHandler EventCommandAbrirEx01;
        public event EventHandler EventCommandIncluir;
        public event EventHandler EventCommandAlterar;
        public event EventHandler EventCommandImprimir;
        public event EventHandler EventCommandVisualizar;
        public event EventHandler EventCommandPesquisar;
        public event EventHandler EventCommandCopiar;
        public event EventHandler<BaseCadastrosPOSEventArgs> EventCommandAbrirCadastoBase;
        public event Action ActionCommandFechar;
        #endregion

        #region ----- Métodos Privados -----
        public virtual void RefreshViewModel()
        {

        }
        #endregion

        #region ----- Métodos Públicos -----
        public void HabilitaBotoesBarra(bool pIncluir = true, bool pAlterar = true, bool pExcluir = true, bool pSelecionar = false)
        {
            this.MostraIncluir = pIncluir;
            this.MostraAlterar = pAlterar;
            this.MostraExcluir = pExcluir;
            this.MostraSelecionar = pSelecionar;

            this.OnPropertyChanged<ViewModelBase>(x => x.MostraIncluir);
            this.OnPropertyChanged<ViewModelBase>(x => x.MostraAlterar);
            this.OnPropertyChanged<ViewModelBase>(x => x.MostraExcluir);
            this.OnPropertyChanged<ViewModelBase>(x => x.MostraSelecionar);
        }

        public void HabilitaBotaoSelecionar()
        {
            this.HabilitaBotoesBarra(false, false, false, true);
        }
        #endregion

        #region ----- Commands -----
        /// <summary>
        /// Executa o Método Abrir do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandAbrir(object param)
        {
            if (EventCommandAbrir != null)
                EventCommandAbrir(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o AbrirEx 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteAbrir(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método AbrirEx do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandAbrirEx(object param)
        {
            if (EventCommandAbrirEx != null)
                EventCommandAbrirEx(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o AbrirEx 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteAbrirEx(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método AbrirEx01 do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandAbrirEx01(object param)
        {
            if (EventCommandAbrirEx01 != null)
                EventCommandAbrirEx01(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o AbrirEx01
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteAbrirEx01(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Abrir Cadastro Base do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandAbrirCadastroBase(object param)
        {
            if (EventCommandAbrirCadastoBase != null)
            {
                EventCommandAbrirCadastoBase(this, new BaseCadastrosPOSEventArgs((BaseCadastrosPOS)param));
            }
        }
        /// <summary>
        /// Verifica se pode executar o Abrir Cadastro Base do Command
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteCadastoBase(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Alterar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandAlterar(object param)
        {
            if (EventCommandAlterar != null)
                EventCommandAlterar(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o Alterar 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteAlterar(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Incluir do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandIncluir(object param)
        {
            if (EventCommandIncluir != null)
                EventCommandIncluir(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o Incluir
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteIncluir(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Excluir do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandExcluir(object param)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Verifica se pode executar o Excluir
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteExcluir(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Imprimir do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandImprimir(object param)
        {
            if (EventCommandImprimir != null)
                EventCommandImprimir(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o Imprimir 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteImprimir(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Visualizar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandVisualizar(object param)
        {
            if (EventCommandVisualizar != null)
                EventCommandVisualizar(this, null);
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
            if (ActionCommandFechar != null)
                ActionCommandFechar();
        }

        /// <summary>
        /// Verifica se pode executar o Fechar 
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteFechar(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Salvar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandSalvar(object param)
        {
            if (this._chamafecharapossalvar)
                if (ActionCommandFechar != null)
                    ActionCommandFechar();
        }
        /// <summary>
        /// Verifica se pode executar o Salvar
        /// </summary>
        /// <returns>bool (Valor Default IsValid da VM)</returns>
        protected virtual bool CommandCanExecuteSalvar(object param)
        {
            return this.IsValid;
        }

        /// <summary>
        /// Executa o Método Selecionar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandSelecionar(object param)
        {
            this.CommandFechar(param);
        }
        /// <summary>
        /// Verifica se pode executar o Selecionar
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteSelecionar(object param)
        {
            return true;
        }

        /// <summary>
        /// Executa o Método Pesquisar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandPesquisar(object param)
        {
            if (this.EventCommandPesquisar != null)
                this.EventCommandPesquisar(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o Pesquisar
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecutePesquisar(object param)
        {
            return true;
        }


        /// <summary>
        /// Executa o Método Copiar do Command
        /// </summary>
        /// <returns></returns>
        protected virtual void CommandCopiar(object param)
        {
            if (this.EventCommandCopiar != null)
                this.EventCommandCopiar(this, null);
        }
        /// <summary>
        /// Verifica se pode executar o Copiar
        /// </summary>
        /// <returns>bool (Valor Default true)</returns>
        protected virtual bool CommandCanExecuteCopiar(object param)
        {
            return true;
        }

        #endregion

        #region ----- IEditableObject Members -----
        /// Usado para trabalhar com transações nos objetos, ou seja, o EndEdit Comita e o CancelEdit da Rollback nas alterações.
        /// <summary>
        /// Inicia a transação.
        /// </summary>
        public virtual void BeginEdit()
        {
            //this._oldState = new HybridDictionary();

            //foreach (PropertyInfo property in this.GetType().GetProperties())
            //{
            //    if (property.CanWrite)
            //    {
            //        //if (!property.GetValue(this, null).IsNull())
            //        //    if (!property.PropertyType.GetInterface("ICopyable").IsNull())
            //        //        _oldState[property.Name] = (property.GetValue(this, null) as ICopyable).CloneDeep();
            //        //    else if (!property.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
            //        //        _oldState[property.Name] = (property.GetValue(this, null) as IViewModelCollectionBase).CloneDomain();
            //        //    else
            //        this._oldState[property.Name] = property.GetValue(this, null);
            //    }
            //}
        }

        /// <summary>
        // Commita a transação.
        /// </summary>
        public virtual void EndEdit()
        {
            //this._oldState = null;
        }

        /// <summary>
        /// Cancela os valores alterados na Wrapper indo no seu respectivo repositório e recuperando os valores gravados no banco, este método desce recursivamente até o terceiro nível.
        /// </summary>
        public virtual void CancelEdit()
        {
            UIHelper.SetBusyState();
            //Toda a wrapper possui um DomainObject (IDomainBase), aqui eu recupero ele no propdomain e faço o refresh() do mesmo com o repositório dinamico (ver metodo Refresh() da classe DomainBase)...            
            PropertyInfo propdomain = this.GetType().GetProperties().Where(x => !x.PropertyType.GetInterface("IDomainBase").IsNull()).FirstOrDefault();

            //Verifica se achou o DomainObject, se nao achou quer dizer que o canceledit foi chamado de uma VM...
            if (!propdomain.IsNull())
            {
                //Aqui é o primeiro nível...
                (propdomain.GetValue(this, null) as IDomainBase).Refresh();

                //Rola TODAS as propriedades do dominio (propdomain) da wrapper que chamou o canceledit e verifica cada propriedade...
                //Atenção aqui são as propriedades do DOMINIO e nao da Wrapper...
                foreach (PropertyInfo propchilddomain in propdomain.GetValue(this, null).GetType().GetProperties())
                {
                    //Se a propriedade for um objeto do tipo IDomainBase e nao estiver nula chama o refresh também...
                    if (!propchilddomain.PropertyType.GetInterface("IDomainBase").IsNull() && !propchilddomain.GetValue(propdomain.GetValue(this, null), null).IsNull())
                    {
                        //Aqui é o segundo nível...
                        (propchilddomain.GetValue(propdomain.GetValue(this, null), null) as IDomainBase).Refresh();

                        //Rola TODAS propriedades da propriedade FILHA do domain da wrapper que chamou o canceledit...
                        //foreach (PropertyInfo propchildofchilddomain in propchilddomain.GetValue(propdomain.GetValue(this, null), null).GetType().GetProperties())
                        //{
                        //    //Verifica também se a propriedade é um objeto IDomainBase e nao está null...
                        //    if (!propchildofchilddomain.PropertyType.GetInterface("IDomainBase").IsNull() && !propchildofchilddomain.GetValue(propchilddomain.GetValue(propdomain.GetValue(this, null), null), null).IsNull())
                        //    {
                        //        //Aqui faz o refresh do terceiro nível...
                        //        (propchildofchilddomain.GetValue(propchilddomain.GetValue(propdomain.GetValue(this, null), null), null) as IDomainBase).Refresh();

                        //        //Aqui eu me empolguei e fiz para o quarto nível MAS deixei aqui comentado caso seja necessário no futuro(distante ou próximo)...
                        //        //Não sei... nao gosto de fofoca!
                        //        //foreach (PropertyInfo propchildofchildofchilddomain in propchildofchilddomain.GetValue(propchilddomain.GetValue(propdomain.GetValue(this, null), null), null).GetType().GetProperties())
                        //        //{
                        //        //    if (!propchildofchildofchilddomain.PropertyType.GetInterface("IDomainBase").IsNull()
                        //        //        && !propchildofchildofchilddomain.GetValue(propchildofchilddomain.GetValue(propchilddomain.GetValue(propdomain.GetValue(this, null), null), null), null).IsNull())
                        //        //        (propchildofchildofchilddomain.GetValue(propchildofchilddomain.GetValue(propchilddomain.GetValue(propdomain.GetValue(this, null), null), null), null) as IDomainBase).Refresh();
                        //        //}
                        //    }
                        //}
                    }
                }

                //Agora é necessário setar para NULL TODAS as collections filhas e filhas das filhas e filhas das filhas das filhas da wrapper que chamou o canceledit...
                //E também é necessário dar o OnPropertyChanged em TODAS as propriedades para que elas se atualizem na view (XAML)...
                foreach (PropertyInfo property in this.GetType().GetProperties())
                {
                    //Verifica se a propriedade REALMENTE existe no objeto alvo para evitar erros e trys caths sem exceptions =)
                    if (!TypeDescriptor.GetProperties(this).Find(property.Name, false).IsNull())
                    {
                        //Seta nulo para as collections filhas para a wrapper que chamou o canceledit ir novamente no domain object e atualizar a mesma
                        if (!property.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
                            // Primeiro nível...
                            try
                            {
                                property.SetValue(this, null, null);
                            }
                            catch (System.Exception ex)
                            {
                                throw new SystemException("A collection: " + property.Name + " da classe " + this.GetType().Name + " está sem o SET!!!" + Environment.NewLine + ex.Message);
                            }


                        //Faz o OnPropertyChanged das propriedades filhas - Primeiro Nível
                        base.OnPropertyChanged(property.Name);

                        //Aqui eu verifico se a propriedade nao está null e se ela é uma wrapper (ou seja se ela deriva em algum ponto da própria ViewModelBase)...
                        //if (!property.GetValue(this, null).IsNull() && property.GetValue(this, null).GetType().HasBaseType(typeof(ViewModelBase)))

                        //    //Rola as propriedades filhas das propriedades filhas da wrapper que chamou o canceledit...                            
                        //    foreach (PropertyInfo propchild in property.GetValue(this, null).GetType().GetProperties())
                        //    {
                        //        //Verifica se a propriedade REALMENTE existe no objeto alvo para evitar erros e trys caths sem exceptions =)
                        //        if (!TypeDescriptor.GetProperties(property.GetValue(this, null)).Find(propchild.Name, false).IsNull())
                        //        {
                        //            //Seta nulo para as collections filhas das wrappers filhas da wrapper que chamou o canceledit para a wrapper filha ir novamente no domain object e atualizar a mesma                            
                        //            if (!propchild.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
                        //                //Segundo nível...
                        //                try
                        //                {
                        //                    propchild.SetValue(property.GetValue(this, null), null, null);
                        //                }
                        //                catch (System.Exception ex)
                        //                {
                        //                    throw new SystemException("A collection: " + propchild.Name + " da classe " + property.GetValue(this, null).GetType().Name + " está sem o SET!!!" + Environment.NewLine + ex.Message);
                        //                }

                        //            //Faz o OnPropertyChanged das filhas das propriedades filhas - Segundo Nível
                        //            (property.GetValue(this, null) as ViewModelBase).OnPropertyChanged(propchild.Name);

                        //            //Aqui eu verifico se a propriedade nao está null e se ela é uma wrapper (ou seja se ela deriva em algum ponto da própria ViewModelBase)...
                        //            if (!propchild.GetValue(property.GetValue(this, null), null).IsNull()
                        //                && propchild.GetValue(property.GetValue(this, null), null).GetType().HasBaseType(typeof(ViewModelBase)))
                        //                //Rola as propriedades filhas das propriedades filhas das propriedades filhas da wrapper que chamou o canceledit...
                        //                foreach (PropertyInfo propchildofchild in propchild.GetValue(property.GetValue(this, null), null).GetType().GetProperties())
                        //                {
                        //                    //Verifica se a propriedade REALMENTE existe no objeto alvo para evitar erros e trys caths sem exeptions =)
                        //                    if (!TypeDescriptor.GetProperties(propchild.GetValue(property.GetValue(this, null), null)).Find(propchildofchild.Name, false).IsNull())
                        //                    {
                        //                        //Seta nulo para as collections filhas das wrapper filhas das wrapper filhas da wrapper que chamou o canceledit 
                        //                        //para a wrapper filha da filha ir novamente no domain object e atualizar a mesma
                        //                        if (!propchildofchild.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
                        //                            //Terceiro Nível...
                        //                            try
                        //                            {
                        //                                propchildofchild.SetValue(propchild.GetValue(property.GetValue(this, null), null), null, null);
                        //                            }
                        //                            catch (System.Exception ex)
                        //                            {
                        //                                throw new SystemException("A collection: " + propchildofchild.Name + " da classe " + propchild.GetValue(property.GetValue(this, null), null).GetType().Name + " está sem o SET!!!" + Environment.NewLine + ex.Message);
                        //                            }

                        //                        //Faz o OnPropertyChanged das filhas das propriedades filhas das propriedades filhas - Terceiro Nível
                        //                        (propchild.GetValue(property.GetValue(this, null), null) as ViewModelBase).OnPropertyChanged(propchildofchild.Name);
                        //                    }
                        //                }
                        //        }
                        //    }
                    }
                }                                
            }
            UIHelper.SetBusyState();
            
            
            
            //if (this._oldState != null)
            //    foreach (PropertyInfo property in this.GetType().GetProperties())
            //    {
            //        if (property.CanWrite)
            //        {
            //            if (!property.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
            //                this._clonaiviewmodelcollectionbase(property, this._oldState[property.Name]);
            //            else
            //                property.SetValue(this, _oldState[property.Name], null);
            //        }
            //    }
            //this._oldState = null;
        }
        #endregion
        
        //#region ----- ICopyable Members -----
        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //}

        //public object CloneDeep()
        //{
        //    PropertyInfo[] props = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        //    object novo = this.MemberwiseClone();

        //    foreach (PropertyInfo pr in props)
        //    {
        //        if (pr.CanWrite)
        //            if (!pr.GetValue(this, null).IsNull())
        //                if (!pr.PropertyType.GetInterface("ICopyable").IsNull())
        //                    pr.SetValue(novo, (pr.GetValue(this, null) as ICopyable).Clone(), null);
        //                else if (!pr.PropertyType.GetInterface("IViewModelCollectionBase").IsNull())
        //                {
        //                    this._clonaiviewmodelcollectionbase(pr, (pr.GetValue(this, null) as IViewModelCollectionBase).CloneDomain());
        //                }
        //                else
        //                    pr.SetValue(novo, pr.GetValue(this, null), null);
        //    }
        //    return novo;
        //}

        //private void _clonaiviewmodelcollectionbase(PropertyInfo property, object target)
        //{
        //    if (target.IsNull()) return;
        //    (property.GetValue(this, null) as IViewModelCollectionBase).Limpa();
        //    foreach (var item in (IEnumerable<ICopyable>)target)
        //    {
        //        (property.GetValue(this, null) as IViewModelCollectionBase).AdicionaDomain(item);
        //    }
        //}
        //#endregion
    }   
}
