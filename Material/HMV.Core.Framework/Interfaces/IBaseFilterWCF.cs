using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace HMV.Core.Framework.Interfaces
{
    [ServiceContract]
    public interface IBaseFilterWCF<T>
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Retorno<IList<T>> Carregar();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Retorno<T> CarregarPorID(string id);

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/Descricao/{descricao}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Retorno<IList<T>> CarregarPorDescricao(string descricao);
    }
}
