using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace HMV.Core.Framework.Interfaces
{
    [ServiceContract]
    public interface IBaseWCF<T>
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

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Retorno<T> Salvar(T item);

        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Retorno<T> Deletar(T item);

        //O PUT DEVERÁ SER IMPLEMENTADO APENAS SE NECESSÁRIO NAS INTERFACES FILHAS -- AA
        //[OperationContract]
        //[WebInvoke(Method = "PUT", UriTemplate = "", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        //Retorno<T> Alterar(T item);  
    }

}
