using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using System.Reflection;

namespace HMV.Core.Framework.Helper
{
    public static class LogHelper
    {
        public static void geraLogXML(string path, string nomeArquivo, string titulo, string mensagem)
        {
            try
            {
                string lPath = Path.GetDirectoryName(path);

                FileStream file = new FileStream(Path.Combine(lPath, nomeArquivo+".xml"), FileMode.OpenOrCreate, FileAccess.ReadWrite);
                var lista = new List<LogDTOHelper>();
               
                try
                {
                    var derializer = new XmlSerializer(typeof(List<LogDTOHelper>));
                    lista = (List<LogDTOHelper>)derializer.Deserialize(file);
                }
                catch { }

                lista.Add(new LogDTOHelper(titulo, mensagem, DateTime.Now));
                
                XmlSerializer serializer = new XmlSerializer(typeof(List<LogDTOHelper>));
                serializer.Serialize(file,  lista );
                file.Close();
                file.Dispose();
            }
            catch(System.Exception err)
            {
                string lPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                File.WriteAllText(Path.Combine(lPath, "ErroCritico.xml"), err.ToString());
            }
        }     
    }

    [Serializable]
    public class LogDTOHelper
    {
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public DateTime DataHora { get; set; }
        public string Maquina { get; set; }
        public string Sistema { get; set; }
        public string UsuarioWindows { get; set; }

        public LogDTOHelper() { }

        public LogDTOHelper(string pTitulo, string pMensagem, DateTime pDataHora)
        {
            this.Mensagem = pMensagem;
            this.Titulo = pTitulo;
            this.DataHora = pDataHora;
            this.Maquina = Environment.MachineName;
            this.Sistema = this.GetType().Assembly.GetName().Name;
            this.UsuarioWindows = Environment.UserName;            
        }
    }

}

  