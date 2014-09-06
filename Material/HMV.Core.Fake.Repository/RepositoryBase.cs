using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.DDD;
using NHibernate;

namespace HMV.Core.Fake.Repository
{
    public class Repository<TEntidade> where TEntidade : DomainBase
    {
        protected static IList<TEntidade> _list;

        public Repository(bool carregarLista = true)
        {
            if (carregarLista)
                _list = Builder<TEntidade>.CreateListOfSize(10).Build();
        }
    }

    public class RepositoryBase<TEntidade> : RepositoryFilter<TEntidade>, IRepositoryBase<TEntidade> where TEntidade : DomainBase, IAggregateRoot
    {
        public RepositoryBase(bool carregarLista = true)
            : base(carregarLista)
        {
        }

        public virtual TEntidade Save(TEntidade entidade, ISession session, ITransaction tx)
        {
             return entidade;
        }


        public virtual TEntidade Save(TEntidade entidade)
        {
            return entidade;
        }
        public ISession GetCurrentSession() { return null; }

        public void Delete(TEntidade entidade)
        {
            //
        }


        public void Commit(ISession session, ITransaction tx)
        {
            //
        }


        public void Rollback(ISession session, ITransaction tx)
        {
           //
        }


        public void SetCurrentSession(ISession session)
        {
           //
        }


        public void Delete(TEntidade entidade, ISession session, ITransaction tx)
        {
           //
        }
    }

    public class RepositoryFilter<TEntidade> : Repository<TEntidade>, IRepositoryFilter<TEntidade> where TEntidade : DomainBase
    {
        public RepositoryFilter(bool carregarLista = true)
            : base(carregarLista)
        {
        }

        public virtual IList<TEntidade> List()
        {
            return _list;
        }

        public virtual TEntidade Single()
        {
            return _list.FirstOrDefault();
        }

        public virtual TEntidade FindBy(object id)
        {
            throw new NotImplementedException();
        }

        public IRepositoryFilter<TEntidade> Skip(int count)
        {
            _list.Skip(count);
            return this;
        }

        public IRepositoryFilter<TEntidade> Take(int count)
        {
            _list.Take(count);
            return this;
        }

        public int Count()
        {
            return _list.Count;
        }

        public void Refresh(object entity)
        {
            //
        }

        public void Lock(object obj)
        {
            //
        }

        public void ReGet(object entidade)
        {
            throw new NotImplementedException();
        }
    }
}
