﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HMV.Core.Framework.Exception;

namespace HMV.Core.Framework.Helper
{
    public static class PageHelper
    {
        public static string GetPage(String url, String query, WebProxy myProxy)
        {
            // Declarações necessárias 
            Stream requestStream = null;
            WebResponse response = null;
            StreamReader reader = null;

            try
            {

                WebRequest request = WebRequest.Create(url);
                request.Proxy = myProxy;
                request.Method = WebRequestMethods.Http.Post;

                // Neste ponto, você está setando a propriedade ContentType da página 
                // para urlencoded para que o comando POST seja enviado corretamente 
                request.ContentType = "application/x-www-form-urlencoded";
                
                StringBuilder urlEncoded = new StringBuilder();

                // Separando cada parâmetro 
                Char[] reserved = { '?', '=', '&' };

                // alocando o bytebuffer 
                byte[] byteBuffer = null;

                // caso a URL seja preenchida 
                if (query != null)
                {
                    int i = 0, j;
                    // percorre cada caractere da url atraz das palavras reservadas para separação 
                    // dos parâmetros 
                    while (i < query.Length)
                    {
                        j = query.IndexOfAny(reserved, i);
                        if (j == -1)
                        {
                            urlEncoded.Append(query.Substring(i, query.Length - i));
                            break;
                        }
                        urlEncoded.Append(query.Substring(i, j - i));
                        urlEncoded.Append(query.Substring(j, 1));
                        i = j + 1;
                    }
                    // codificando em UTF8 (evita que sejam mostrados códigos malucos em caracteres especiais 
                    byteBuffer = Encoding.UTF8.GetBytes(urlEncoded.ToString());

                    request.ContentLength = byteBuffer.Length;
                    requestStream = request.GetRequestStream();
                    requestStream.Write(byteBuffer, 0, byteBuffer.Length);
                    requestStream.Close();
                }
                else
                {
                    request.ContentLength = 0;
                }

                // Dados recebidos 
                response = request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                // Codifica os caracteres especiais para que possam ser exibidos corretamente 
                System.Text.Encoding encoding = System.Text.Encoding.Default;

                // Preenche o reader 
                reader = new StreamReader(responseStream, encoding);

                Char[] charBuffer = new Char[256];
                int count = reader.Read(charBuffer, 0, charBuffer.Length);

                String Dados = "";

                // Lê cada byte para preencher meu string
                while (count > 0)
                {
                    Dados += new String(charBuffer, 0, count);
                    count = reader.Read(charBuffer, 0, charBuffer.Length);
                }

                return Dados;

            }
            catch (System.Exception e)
            {
                // Ocorreu algum erro 
                throw new System.Exception(e.Message); 
                //Console.Write("Erro: " + e.Message);
                //return "";

            } // END: catch 

            finally
            {
                // Fecha tudo 
                if (requestStream != null) requestStream.Close();
                if (response != null) response.Close();
                if (reader != null) reader.Close();
            } // END: finally
        }
    }
}
