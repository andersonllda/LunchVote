using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;

namespace HMV.Core.Framework.Interfaces
{
    [DataContract]
    public class Retorno<T>
    {
        public Retorno()
        {
            Message = "OK";
            Success = true;
            Sucess = true;
            Banco = RetornoParametro.Banco;
        }

        public Retorno(T data)
        {
            Data = data;
            Message = "OK";
            Success = true;
            Sucess = true;
        }

        public Retorno(T data, string banco)
        {
            Data = data;
            Message = "OK";
            Success = true;
            Sucess = true;
            Banco = banco;
        }

        public Retorno(bool sucess, string message)
        {
            Message = message;
            Success = sucess;
            Sucess = sucess;
            Banco = RetornoParametro.Banco;
        }

        public Retorno(bool sucess, string message, string banco)
        {
            Message = message;
            Success = sucess;
            Sucess = sucess;
            Banco = banco;
        }

        [DataMember]
        public T Data { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool Sucess { get; set; }
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Banco { get; set; }
    }

    public static class RetornoParametro
    {
        public static string Banco;
    }

}
