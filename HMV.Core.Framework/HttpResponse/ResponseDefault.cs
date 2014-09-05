using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace HMV.Core.Framework.HttpResponse
{
    public class ResponseDefault : ApiController
    {        
        /// <summary>
        /// Método para retornar um Status HTTP
        /// </summary>
        /// <param name="status">Aqui voce passa o Status HTTP para retorno</param>
        /// <returns>Retorna um Status HTTP</returns>
        public HttpResponseMessage hmvResponse(HttpStatusCode status)
        {
            return this.Request.CreateResponse(status, new object());
        }
        
        /// <summary>
        /// Método para retornar um Status HTTP do Post
        /// </summary>
        /// <param name="p">Manda o Objeto que usou no post para retorno no cliente</param>
        /// <returns>Retorna o Status OK e o Objeto do Post</returns>
        public HttpResponseMessage hmvResponsePost(object p)
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, p);
        }
        
        /// <summary>
        /// Método para retornar uma lista de Objetos
        /// </summary>
        /// <param name="p">Lista de Objetos para retornar</param>
        /// <returns>HTTP Status 'OK' e Lista de Objetos </returns>
        public HttpResponseMessage hmvResponseList(object p)
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, p);
        }

        /// <summary>
        /// Retorna um objeto. Usado no Get
        /// </summary>
        /// <param name="p">Objeto a ser retornado</param>
        /// <returns>Retorna um objeto</returns>
        public HttpResponseMessage hmvResponseObject(object p)
        {
            return this.Request.CreateResponse(HttpStatusCode.OK, p);
        }
        
        /// <summary>
        /// Retorna o Status em caso de erro
        /// </summary>
        /// <param name="erro">Exception quando ocorre erro</param>
        /// <returns>Retorna o Status de Erro e a Exception gerada</returns>
        public HttpResponseMessage hmvResponseError(System.Exception erro)
        {
            return this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, erro);
        }

        public ResponseDefault(HttpRequestMessage request) {
            this.Request = request;
        }
    }
}
