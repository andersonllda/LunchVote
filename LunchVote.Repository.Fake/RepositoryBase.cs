using FizzWare.NBuilder;
using LunchVote.Core.DDD;
using LunchVote.Domain;
using LunchVote.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LunchVote.Repository.Fake
{
    public class Repository<TEntidade> where TEntidade : DomainBase
    {
        protected static IList<TEntidade> _list;

        public Repository(bool carregarLista = true)
        {
            if (carregarLista)
                _list = Builder<TEntidade>.CreateListOfSize(100).Build();
        }
    }

    public class RepositoryBase<TEntidade> : RepositoryFilter<TEntidade>, IRepositoryBase<TEntidade> where TEntidade : DomainBase, IAggregateRoot
    {
        public RepositoryBase(bool carregarLista = true)
            : base(carregarLista)
        {
        }
        public virtual TEntidade Save(TEntidade entidade)
        {
            _list.Add(entidade);
            return entidade;
        }    
        public void Delete(TEntidade entidade)
        {
            _list.Remove(entidade);            
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
            throw new NotImplementedException();
        }
        public void Lock(object obj)
        {
            throw new NotImplementedException();
        }        
    }
}
