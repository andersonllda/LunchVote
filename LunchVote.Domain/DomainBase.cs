using LunchVote.Core.Interfaces;
using LunchVote.Domain.Repository;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain
{
    public abstract class DomainBase : IDomainBase
    {
        public virtual bool addObject<T>(IList<T> lista, T pObject)
        {
            if (pObject == null)
                throw new NullReferenceException();

            if (lista == null)
                lista = new List<T>();

            if (lista.FirstOrDefault(x => x.Equals(pObject)) != null)
                return false;

            lista.Add(pObject);
            return true;
        }

        public virtual Boolean removeObject<T>(IList<T> lista, T pObject)
        {
            if (pObject == null)
                throw new NullReferenceException();

            if (lista == null)
                return false;

            return lista.Remove(lista.FirstOrDefault(x => x.Equals(pObject)));
        }

        public virtual void Refresh()
        {
            Type baseType = typeof(IRepositoryFilter<>);
            Type selectedType = this.GetType();
            Type genericType = baseType.MakeGenericType(new Type[] { selectedType });
            var rep = ObjectFactory.GetInstance(genericType);            
            rep.GetType().GetMethod("Refresh").Invoke(rep, new object[] { this });
        }
    }
}
