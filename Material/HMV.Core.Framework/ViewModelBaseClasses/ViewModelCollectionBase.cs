using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using HMV.Core.Framework.Interfaces;
using HMV.Core.Framework.Extensions;

namespace HMV.Core.Framework.ViewModelBaseClasses
{

    public abstract class ViewModelCollectionBase<VM, DM> : ObservableCollection<VM>, IViewModelCollectionBase
    {
        #region Propriedades Privadas
        private IList<DM> m_DomainList;
        private ICollection<DM> m_DomainCollection;
        private bool m_EventsDisabled;
        #endregion

        #region Construtor
        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="domainCollection">A domain collection(DM) que deve ser espelhada(wrapped).</param>
        public ViewModelCollectionBase(IList<DM> domainList)
        {
            // Seta a List do domínio.
            m_DomainList = domainList;

            // Desabilita os eventos.
            m_EventsDisabled = true;

            if (domainList == null) return;

            /* Note que não pode-se simplismente instanciar um novo objeto VM(ViewModel), por que
             * a expressão new() nao vai deixar usarmos um construtor parametrizado com um type genérico.
             * Veja em http://msdn.microsoft.com/en-us/library/tass7xkw.aspx.
             * Então, eu utilizei Activator.CreateInstance() no lugar.*/

            // Popula essa List com os objetos da VM. 
            foreach (var domainObject in domainList)
            {
                var paramList = new object[] { domainObject };
                var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
                this.Add(wrapperObject);
            }

            // Ativa os eventos.
            m_EventsDisabled = false;
        }

        public ViewModelCollectionBase(ICollection<DM> domainCollection)
        {
            // Seta a collection do domínio.
            m_DomainCollection = domainCollection;

            // Desabilita os eventos.
            m_EventsDisabled = true;

            if (domainCollection == null) return;

            /* Note que não pode-se simplismente instanciar um novo objeto VM(ViewModel), por que
             * a expressão new() nao vai deixar usarmos um construtor parametrizado com um type genérico.
             * Veja em http://msdn.microsoft.com/en-us/library/tass7xkw.aspx.
             * Então, eu utilizei Activator.CreateInstance() no lugar.*/

            // Popula essa collection com os objetos da VM. 
            foreach (var domainObject in domainCollection)
            {
                var paramList = new object[] { domainObject };
                var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
                this.Add(wrapperObject);
            }

            // Ativa os eventos.
            m_EventsDisabled = false;
        }
        public ViewModelCollectionBase()
        {
            
        }
        public ViewModelCollectionBase(List<VM> list)
            : base(list)
        {
            
        }
        public ViewModelCollectionBase(IEnumerable<VM> collection)
            : base(collection)
        {
            
        }
         
        #endregion

        #region Event Handlers
        /// <summary>        
        /// Atualiza a DM(Domain Model) collection quando a VM(View Model) collection mudar.
        /// </summary>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Chama classe base.
            base.OnCollectionChanged(e);

            // Sai se os eventos estão desabilitados.
            if (m_EventsDisabled) return;

            // Faz um push (empurra) das mudanças para a DM collection.
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.AddDomainObjects(e);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    RemoveDomainObjects(e);
                    break;
            }
        }
        #endregion

        #region Métodos Privados
        /// <summary>        
        /// Faz um push (empurra) as adições da VM collection para a DM collection.
        /// </summary>
        private void AddDomainObjects(NotifyCollectionChangedEventArgs e)
        {
            // Faz um push para da VM.
            foreach (ViewModelObjectBase<DM> wrapperObject in e.NewItems)
            {
                var domainObject = wrapperObject.DomainObject;
                if (m_DomainList != null)
                    m_DomainList.Add(domainObject);
                if (m_DomainCollection != null)
                    m_DomainCollection.Add(domainObject);
            }
        }

        /// <summary>        
        /// Faz um push das exclusões da VM collection para a DM collection.
        /// </summary>
        private void RemoveDomainObjects(NotifyCollectionChangedEventArgs e)
        {
            // Faz um push das exclusões da VM.
            foreach (ViewModelObjectBase<DM> wrapperObject in e.OldItems)
            {
                var domainObject = wrapperObject.DomainObject;
                if (m_DomainList != null)
                    m_DomainList.Remove(domainObject);
                if (m_DomainCollection != null)
                    m_DomainCollection.Remove(domainObject);
            }
        }
        #endregion

        #region IViewModelCollectionBase Members
        /// <summary>        
        /// Clona o domínio da collection wrapper
        /// </summary>
        public object CloneDomain()
        {
            if (!m_DomainList.IsNull())
                return m_DomainList.Select(item => (DM)(item as ICopyable).Clone()).ToList();
            return null;
        }
       
        /// <summary>        
        /// Limpa a coleção da wrapper e seu domínio.
        /// </summary>
        public void Limpa()
        {
            base.ClearItems();
            if (m_DomainList != null)
                m_DomainList.Clear();
            if (m_DomainCollection != null)
                m_DomainCollection.Clear();
        }

        /// <summary>        
        /// Adiciona um domain object na collection wrapper.
        /// </summary>
        public void AdicionaDomain(object pItem)
        {
            var paramList = new object[] { pItem };
            var wrapperObject = (VM)Activator.CreateInstance(typeof(VM), paramList);
            this.Add(wrapperObject);
        }
        #endregion
    }
}
