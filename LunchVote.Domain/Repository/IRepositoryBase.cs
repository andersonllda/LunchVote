using LunchVote.Core.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.Domain.Repository
{
    public interface IRepositoryBase<TEntidade> : IRepositoryFilter<TEntidade> where TEntidade : DomainBase, IAggregateRoot
    {
        TEntidade Save(TEntidade entidade);        
        void Delete(TEntidade entidade);
    }

    public interface IRepositoryFilter<TEntidade> where TEntidade : DomainBase
    {
        IList<TEntidade> List();
        TEntidade Single();
        IRepositoryFilter<TEntidade> Skip(int count);
        IRepositoryFilter<TEntidade> Take(int count);
        int Count();
        void Refresh(object entidade);      
        void Lock(object entidade);
    }
}
