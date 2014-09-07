using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchVote.IoC
{
    public static class IoCWorker
    {
        public static void ConfigureFake()
        {
            ObjectFactory.Configure(i =>
            {
                i.AddRegistry<FakeRepositoryRegistry>();
            });
        }
    }
}
