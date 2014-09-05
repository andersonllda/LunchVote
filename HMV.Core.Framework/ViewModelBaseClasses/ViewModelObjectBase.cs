using System;
namespace HMV.Core.Framework.ViewModelBaseClasses
{
    
    public abstract class ViewModelObjectBase<DM> : ViewModelBase
    {
        #region Construtor
        protected ViewModelObjectBase(DM domainObject)
        {
            this.DomainObject = domainObject;            
        }
        #endregion

        #region Propriedades
        //Se está no mesmo projeto deve ter o modificador de acesso INTERNAL se o projeto é separado trocar para PUBLIC.
        public DM DomainObject { get; set; }    
        #endregion
    }
}
