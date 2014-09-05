using System;
using System.Collections.Generic;
using System.Linq;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Interfaces;
using StructureMap;
using System.Runtime.Serialization;

namespace HMV.Core.Domain.Model
{
    [DataContract] //NAO ESQUECER DE COMENTAR NOVAMENTE ESTA LINHA APOS LIBERAR O ATENDIMENTO AO PACIENTE!!!!
    public abstract class DomainBase : IDomainBase //,ICopyable
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

        //public virtual object GetById(object pId)
        //{
        //    Type baseType = typeof(IRepositoryFilter<>);
        //    Type selectedType = this.GetType();
        //    Type genericType = baseType.MakeGenericType(new Type[] { selectedType });
        //    var rep = ObjectFactory.GetInstance(genericType);
        //    return rep.GetType().GetMethod("FindBy").Invoke(rep, new object[] { pId });
        //}
        //public virtual object GetList()
        //{
        //    Type baseType = typeof(IRepositoryFilter<>);
        //    Type selectedType = this.GetType();
        //    Type genericType = baseType.MakeGenericType(new Type[] { selectedType });
        //    var rep = ObjectFactory.GetInstance(genericType);
        //    return rep.GetType().GetMethod("List").Invoke(rep, null);
        //}


        //public virtual object Clone()
        //{
        //    return this.MemberwiseClone();
        //}

        //public virtual object CloneDeep()
        //{
        //    PropertyInfo[] props = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);

        //    object novo = this.MemberwiseClone();

        //    foreach (PropertyInfo pr in props)
        //    {
        //        if (pr.CanWrite)
        //            if (!pr.GetValue(this, null).IsNull())
        //                if (!pr.PropertyType.GetInterface("ICopyable").IsNull())
        //                    pr.SetValue(novo, (pr.GetValue(this, null) as ICopyable).Clone(), null);
        //                else
        //                    pr.SetValue(novo, pr.GetValue(this, null), null);
        //    }

        //    return novo;
        //}
    }

}
