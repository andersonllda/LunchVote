using System.Collections.Generic;
using System.Configuration;
using HMV.Core.Framework.Helper;
using HMV.Core.Framework.Interfaces;

namespace HMV.Core.Framework.Web
{
    public class BaseService<T>
    {
        public Retorno<IList<T>> Carregar()
        {
            Retorno<IList<T>> ret = Get<IList<T>>();

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<T> Carregar(string url)
        {
            Retorno<T> ret = Get<T>(url);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<Y> Carregar<Y>(string url)
        {
            Retorno<Y> ret = Get<Y>(url);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<IList<T>> CarregarLista(string url)
        {
            Retorno<IList<T>> ret = Get<IList<T>>(url);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<IList<Y>> CarregarLista<Y>(string url)
        {
            Retorno<IList<Y>> ret = Get<IList<Y>>(url);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<T> CarregarPorID(string id)
        {
            Retorno<T> ret = Get<T>(id);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<IList<T>> CarregarPorDescricao(string descricao)
        {
            Retorno<IList<T>> ret;

            if (!string.IsNullOrWhiteSpace(descricao))
                ret = Get<IList<T>>("Descricao/" + descricao);
            else
                ret = Get<IList<T>>();

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<T> Salvar(T pitem)
        {
            Retorno<T> ret = Post<T>(pitem);

            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        public Retorno<T> Deletar(T pitem)
        {
            Retorno<T> ret = Delete<T>(pitem);
            if (ret.Success || ret.Sucess)
                return ret;

            throw new System.Exception(ret.Message);
        }

        #region Json
        public string url { get { return _url; } }
        private string _url { get; set; }

        public BaseService(string purlService, bool pPortaCore = false)
        {
            if (pPortaCore)
                _url = ConfigurationManager.AppSettings["urlBase_" + BaseServiceParametro.Banco.ToUpper()].Replace("@PORTA", ConfigurationManager.AppSettings["PortaCore"]);
            else
                _url = ConfigurationManager.AppSettings["urlBase_" + BaseServiceParametro.Banco.ToUpper()].Replace("@PORTA", ConfigurationManager.AppSettings["PortaAplicacao"]);

            _url += purlService;
        }

        public void SetUrl(string url)
        {
            _url = url;
        }

        private Retorno<T> Get<T>()
        {
            try
            {
                return JsonHelper.urlJsonToObject<Retorno<T>>(url);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        public Retorno<T> Get<T>(string url)
        {
            string _url = this._url + url;

            try
            {
                return JsonHelper.urlJsonToObject<Retorno<T>>(_url);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        private Retorno<T> Post<T>(object pObjeto)
        {
            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("POST", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        public Retorno<T> Post<T>(string url, object pObjeto)
        {
            string _url = this._url + url;
            
            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("POST", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        public Retorno<T> Put<T>(object pObjeto)
        {
            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("PUT", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        public Retorno<T> Put<T>(string url, object pObjeto)
        {
            string _url = this._url + url;

            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("PUT", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        private Retorno<T> Delete<T>(object pObjeto)
        {
            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("DELETE", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }

        public Retorno<T> Delete<T>(string url, object pObjeto)
        {
            string _url = this._url + url;

            try
            {
                return JsonHelper.urlJsonRestToObject<Retorno<T>>("DELETE", _url, pObjeto);
            }
            catch (System.Exception err)
            {
                throw new System.Exception(err.ToString());
            }
        }
        #endregion
    }

    public static class BaseServiceParametro
    {
        public static string Banco { get; set; }
    }
}
