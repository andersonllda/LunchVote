
using System.Collections.Generic;
namespace HMV.Core.Framework.Interfaces
{
    interface IViewModelCollectionBase
    {
        void Limpa();
        void AdicionaDomain(object pItem);
        object CloneDomain();
    }
}