using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace HMV.Core.Framework.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveXmlInvalidCharacters(this string s)
        {
            if (s.IsEmptyOrWhiteSpace()) return string.Empty;

            return Regex.Replace(
                s,
                @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD\u10000-\u10FFFF]",
                string.Empty);
        }

        /// <summary>
        /// Converte para string mesmo se o objeto estiver NULO.
        /// </summary>
        /// <returns>string</returns>
        public static string ToNullSafeString(this object value)
        {
            return value == null ? String.Empty : value.ToString();
        }

        public static string ConvertNullToStringEmpty(this string obj)
        {
            return (obj == null) ? string.Empty : obj;
        }

        public static int? ConvertStringToInt(this string value)
        {
            int ret;
            if (int.TryParse(value, out ret))
                return ret;
            else
                return null;
        }

        /// <summary>
        /// Remove todos caracteres que não são números da string.
        /// </summary>
        /// <returns>string</returns>
        public static string RemoveNonNumber(this string s)
        {
            MatchCollection col = Regex.Matches(s, "[0-9]");
            StringBuilder sb = new StringBuilder();
            foreach (Match m in col)
                sb.Append(m.Value);
            return sb.ToString();
        }

        /// <summary>
        /// Remove todos caracteres que não são números da string.
        /// </summary>
        /// <returns>string</returns>
        public static string RemoveNonNumberNew(this string s)
        {
            return Regex.Replace(s, "[^0-9]*", "");
        }

        /// <summary>
        /// Remove os caracteres especiais ({ ";", ".", ":", ",", "+", "*" }).
        /// </summary>
        /// <returns>string</returns>
        public static string RemoveCaracterEspecial(this string texto)
        {
            string[] trim = { ";", ".", ":", ",", "+", "*" };
            foreach (string c in trim)
                texto = texto.Replace(c, string.Empty);
            return texto;
        }

        /// <summary>
        /// Retorna a quantidade X de caracteres da esquerda para a direita da string.
        /// </summary>        
        /// <returns>string</returns>
        public static string Left(this string param, int length)
        {
            if (param != null)
            {
                if (param.Length <= length)
                    return param;
                return param.Substring(0, length);
            }
            return param;
        }

        /// <summary>
        /// Retorna a quantidade X de caracteres da direita para a esquerda da string.
        /// </summary>
        /// <returns>string</returns>
        public static string Right(this string param, int length)
        {
            if (param != null)
                if (param.Length >= length)
                    return param.Substring(param.Length - length, length);
            return param;
        }

        /// <summary>
        /// Retorna a quantidade X de caracteres da string iniciando no index Y.
        /// </summary>
        /// <returns>string</returns>
        public static string Mid(this string param, int startIndex, int length)
        {
            if (param != null)
                if ((param.Length >= startIndex) && (param.Length <= length))
                    return param.Substring(startIndex, length);
            return param;
        }

        /// <summary>
        /// Retorna os caracteres da string iniciando no index Y.
        /// </summary>
        /// <returns>string</returns>
        public static string Mid(this string param, int startIndex)
        {
            if (param != null)
                if (param.Length > startIndex)
                    return param.Substring(startIndex);
            return param;
        }

        /// <summary>
        /// Formata a string conforme os parâmetros.
        /// </summary>
        /// <returns>string</returns>
        public static string FormatWith(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        /// <summary>
        /// Verifica se a string está nulla ou vazia.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsEmpty(this string g)
        {
            return string.IsNullOrEmpty(g);
        }

        /// <summary>
        /// Verifica se a string NÃO está nulla ou vazia.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsNotEmpty(this string g)
        {
            return !string.IsNullOrEmpty(g);
        }

        /// <summary>
        /// Verifica se a string está nulla ou vazia ou apenas com espaços em branco.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsEmptyOrWhiteSpace(this string g)
        {
            return string.IsNullOrWhiteSpace(g);
        }

        /// <summary>
        /// Verifica se a string NÃO está nulla ou vazia ou apenas com espaços em branco.
        /// </summary>
        /// <returns>bool</returns>
        public static bool IsNotEmptyOrWhiteSpace(this string g)
        {
            return !string.IsNullOrWhiteSpace(g);
        }

        /// <summary>
        /// Converte a string para double.
        /// </summary>
        /// <returns>double</returns>
        public static double ToDouble(this string g)
        {
            double valor;
            if (!string.IsNullOrWhiteSpace(g) && double.TryParse(g.Replace('.', ','), out valor))
                return valor;
            else
                return 0;
        }

        /// <summary>
        /// Converte a string para decimal.
        /// </summary>
        /// <returns>double</returns>
        public static decimal ToDecimal(this string g)
        {
            decimal valor;
            if (!string.IsNullOrWhiteSpace(g) && decimal.TryParse(g.Replace('.', ','), out valor))
                return valor;
            else
                return 0;
        }

        /// <summary>
        /// Converte a string para int.
        /// </summary>
        /// <returns>int</returns>
        public static int ToInt(this string g)
        {
            int valor;
            if (!string.IsNullOrWhiteSpace(g) && int.TryParse(g.Replace('.', ','), out valor))
                return valor;
            else
                return 0;
        }

        /// <summary>
        /// Converte a string para DateTime.
        /// </summary>
        /// <returns>DateTime</returns>
        public static DateTime ToDateTimeExact(this string g, string format)
        {
            DateTime valor;

            if (!string.IsNullOrWhiteSpace(g) && DateTime.TryParseExact(g, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out valor))
                return valor;
            else
                return DateTime.Now;
        }

        /// <summary>
        /// Converte a string para Título(Sempre Primeira Letra Em Maiúsculo).
        /// </summary>
        /// <returns>string</returns>
        public static string ToTitle(this string param)
        {
            System.Globalization.CultureInfo cultureinfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            param = cultureinfo.TextInfo.ToTitleCase(param.ToLower());

            return param;
        }

        /// <summary>
        /// Usa split para separar a string e convert para inteiro, separa por ',' ou ';' ou ' '.
        /// </summary>
        /// <returns>IList int</returns>
        public static IList<int> ToListInt(this string g)
        {
            try
            {
                char separador = g.IndexOf(',') > 0 ? ',' : (g.IndexOf(';') > 0 ? ';' : ' ');

                if (!string.IsNullOrWhiteSpace(g))
                    return g.Split(separador).Select(x => Convert.ToInt32(x)).ToList();
                else
                    return new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// Usa split para separar a string, separa por ',' ou ';' ou ' '.
        /// </summary>
        /// <returns>IList int</returns>
        public static IList<string> ToListString(this string g)
        {
            try
            {
                char separador = g.IndexOf(',') > 0 ? ',' : (g.IndexOf(';') > 0 ? ';' : ' ');

                if (!string.IsNullOrWhiteSpace(g))
                    return g.Split(separador).ToList();
                else
                    return new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        #region ---- Concatena string ----

        /// <summary>
        /// Concatena string 
        /// </summary>
        /// <returns>string</returns>
        public static string Append<T>(this string param, object objeto, Expression<Func<T, Object>> expression)
        {
            return Append<T>(param, string.Empty, objeto, expression, string.Empty, false);
        }

        public static string AppendLine<T>(this string param, object objeto, Expression<Func<T, Object>> expression)
        {
            return Append<T>(param, string.Empty, objeto, expression, string.Empty, true);
        }

        public static string Append<T>(this string param, string textoLeft, object objeto, Expression<Func<T, Object>> expression)
        {
            return Append<T>(param, textoLeft, objeto, expression, string.Empty, false);
        }

        public static string AppendLine<T>(this string param, string textoLeft, object objeto, Expression<Func<T, Object>> expression)
        {
            return Append<T>(param, textoLeft, objeto, expression, string.Empty, true);
        }

        public static string Append<T>(this string param, object objeto, Expression<Func<T, Object>> expression, string textoRight)
        {
            return Append<T>(param, string.Empty, objeto, expression, textoRight, false);
        }

        public static string AppendLine<T>(this string param, object objeto, Expression<Func<T, Object>> expression, string textoRight)
        {
            return Append<T>(param, string.Empty, objeto, expression, textoRight, true);
        }

        public static string Append<T>(this string param, string textoLeft, object objeto, Expression<Func<T, Object>> expression, string textoRight)
        {
            return Append<T>(param, textoLeft, objeto, expression, textoRight, false);
        }

        public static string AppendLine<T>(this string param, string textoLeft, object objeto, Expression<Func<T, Object>> expression, string textoRight)
        {
            return Append<T>(param, textoLeft, objeto, expression, textoRight, true);
        }

        private static string Append<T>(this string param, string textoLeft, object objeto, Expression<Func<T, Object>> expression, string textoRight, bool newLine)
        {
            string property = Expression.ExpressionEx.PropertyName<T>(expression);
            if (objeto != null)
            {
                object valueObject = objeto.GetType().GetProperty(property).GetValue(objeto, null);
                if (valueObject != null)
                {
                    string value = string.Empty;
                    if (objeto.GetType().GetProperty(property).GetType() == typeof(DateTime))
                        value = ((DateTime)valueObject).ToShortDateString();
                    else
                        value = valueObject.ToString();

                    if (!string.IsNullOrWhiteSpace(value))
                        param += (newLine ? Environment.NewLine : string.Empty) + textoLeft + value + textoRight;
                }
            }
            return param;
        }

        public static string Append(this string param, string texto)
        {
            if (texto != null)
                param += texto;
            return param;
        }

        public static string AppendLine(this string param, string texto)
        {
            if (texto != null)
                param += Environment.NewLine + texto;
            return param;
        }

        #endregion


        /// <summary>
        /// Converte a string para um array de bytes.
        /// </summary>
        /// <returns>byte[]</returns>
        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static bool IsPalavraAcentuada(this string pPalavra)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç\"";
            for (int i = 0; i < comAcentos.Length; i++)
            {
                if (pPalavra.Contains(comAcentos[i]))
                    return true;                
            }
            return false;
        }

        public static string RemoveAcentos(this string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
                return obj;

            return obj.Translate("ÁÀÃÂ", 'A').Translate("áãâà", 'a')
                .Translate("ÉÊ", 'E').Translate("éê", 'e')
                .Replace('Í', 'I').Replace('í', 'i')
                .Translate("ÓÕÔ", 'O').Translate("óõô", 'o')
                .Replace('Ú', 'U').Replace('ú', 'u')
                .Replace('Ç', 'C').Replace('ç', 'c');
        }

        public static string CompletaPath(this string obj)
        {
            if (!obj.EndsWith("\\"))
                return obj + "\\";

            return obj;
        }

        public static string Translate(this string obj, string input, char to)
        {
            if (string.IsNullOrWhiteSpace(obj))
                return obj;

            string ret = obj;
            for (int j = 0; j < input.Count(); j++)
                ret = ret.Replace(input[j], to);

            return ret;
        }

        public static string RemoverAcentos(this string texto)
        {
            string s = texto.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < s.Length; k++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }
            }
            return sb.ToString();
        }

        public static string Fonetizar(this string str, bool consulta = false)
        {
            str = RemoveAcentos(str.ToUpperInvariant());

            if (str.Equals("H"))
            {
                str = "AGA";
            }

            str = SomenteLetras(str);

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            //Eliminar palavras especiais
            str = str.Replace(" LTDA ", " ");

            //Eliminar preposições
            var preposicoes = new[] { " DE ", " DA ", " DO ", " AS ", " OS ", " AO ", " NA ", " NO ", " DOS ", " DAS ", " AOS ", " NAS ", " NOS ", " COM " };
            str = preposicoes.Aggregate(str, (current, preposicao) => current.Replace(preposicao, " "));

            //Converte algarismos romanos para números
            var algRomanos = new[] { " V ", " I ", " IX ", " VI ", " IV ", " II ", " VII ", " III ", " X ", " VIII " };
            var numeros = new[] { " 5 ", " 1 ", " 9 ", " 6 ", " 4 ", " 2 ", " 7 ", " 3 ", " 10 ", " 8 " };
            for (int i = 0; i < algRomanos.Length; i++)
            {
                str = str.Replace(algRomanos[i], numeros[i]);
            }

            //Converte numeros para literais
            var algarismosExtenso = new[] { "ZERO", "UM", "DOIS", "TRES", "QUATRO", "CINCO", "SEIS", "SETE", "OITO", "NOVE" };
            for (int i = 0; i < 10; i++)
            {
                str = str.Replace(i.ToString(), algarismosExtenso[i]);
            }

            //Elimina preposições e artigos
            var letras = new[] { " A ", " B ", " C ", " D ", " E ", " F ", " G ", " H ", " I ", " J ", " K ", " L ", " M ", " N ", " O ", " P ", " Q ", " R ", " S ", " T ", " U ", " V ", " X ", " Z ", " W ", " Y " };
            str = letras.Aggregate(str, (current, letra) => current.Replace(letra, " "));

            str = str.Trim();
            var particulas = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var fonetizados = new string[particulas.Length];
            for (var i = 0; i < particulas.Length; i++)
            {
                fonetizados[i] = FonetizarParticula(particulas[i]);
            }
            string fonetizado = string.Join(" ", fonetizados).Trim();

            if (consulta)
            {
                fonetizado = "%" + string.Join("%%", fonetizados) + "%";
            }
            return fonetizado;
        }

        private static string SomenteLetras(string texto)
        {
            const string letras = "ABCDEFGHIJKLMNOPQRSTUVXZWY ";
            var resultado = string.Empty;
            var letraAnt = texto[0];
            foreach (var letraT in texto)
            {
                foreach (var letraC in letras.Where(letraC => letraC == letraT).TakeWhile(letraC => letraAnt != ' ' || letraT != ' '))
                {
                    resultado += letraC;
                    letraAnt = letraT;
                    break;
                }
            }

            return resultado.ToUpperInvariant();
        }

        private static string FonetizarParticula(string str)
        {
            string aux2;
            int j;
            const string letras = "ABPCKQDTEIYFVWGJLMNOURSZX9";
            const string codFonetico = "123444568880AABCDEEGAIJJL9";

            str = str.ToUpperInvariant();
            string aux = str[0].ToString();

            //Elimina os caracteres repetidos
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i - 1] != str[i])
                {
                    aux += str[i];
                }
            }

            //Iguala os fonemas parecidos
            if (aux[0].Equals('W'))
            {
                if (aux[1].Equals('I'))
                {
                    aux = aux.Remove(0, 1).Insert(0, "U");
                }
                else if ("A,E,O,U".Contains(aux[1]))
                {
                    aux = aux.Remove(0, 1).Insert(0, "V");
                }
            }
            aux = SubstituiTerminacao(aux);

            var caracteres = new[]
                                  {
                                      "TSCH", "SCH", "TSH", "TCH", "SH", "CH", "LH", "NH", "PH", "GN", "MN", "SCE", "SCI", "SCY"
                                      , "CS", "KS", "PS", "TS", "TZ", "XS", "CE", "CI", "CY", "GE", "GI", "GY", "GD", "CK", "PC"
                                      , "QU", "SC", "SK", "XC", "SQ", "CT", "GT", "PT"
                                  };
            var caracteresSub = new[]
                                     {
                                         "XXXX", "XXX", "XXX", "XXX", "XX", "XX", "LI", "NN", "FF", "NN", "NN", "SSI", "SSI",
                                         "SSI", "SS", "SS", "SS", "SS", "SS", "SS", "SE", "SI", "SI", "JE", "JI", "JI", "DD",
                                         "QQ", "QQ", "QQ", "SQ", "SQ", "SQ", "99", "TT", "TT", "TT"
                                     };
            for (int i = 0; i < caracteres.Length; i++)
            {
                aux = aux.Replace(caracteres[i], caracteresSub[i]);
            }

            //Trata consoantes mudas
            aux = TrataConsoanteMuda(aux, 'B', 'I');
            aux = TrataConsoanteMuda(aux, 'D', 'I');
            aux = TrataConsoanteMuda(aux, 'P', 'I');

            //Trata as letras
            //Retira letras iguais
            if (aux[0].Equals('H'))
            {
                aux2 = Convert.ToString(aux[1]);
                j = 2;
            }
            else
            {
                aux2 = Convert.ToString(aux[0]);
                j = 1;
            }

            while (j < aux.Length)
            {
                if (aux[j] != aux[j - 1] && aux[j] != 'H')
                {
                    aux2 += aux[j];
                }
                j++;
            }

            aux = aux2;

            //Transforma letras em códigos fonéticos
            return aux.Select(chr => letras.IndexOf(chr)).Aggregate(string.Empty, (current, n) => current + codFonetico[n]);
        }

        private static string TrataConsoanteMuda(string str, char consoante, char complemento)
        {
            var i = str.IndexOf(consoante);
            while (i > -1)
            {
                if (i >= str.Length - 1 || !"AEIOU".Contains(str[i + 1]))
                {
                    str = str.Insert(i + 1, Convert.ToString(complemento));
                    i++;
                }
                i = str.IndexOf(consoante, ++i);
            }
            return str;
        }

        private static string SubstituiTerminacao(string str)
        {
            str = RemoveAcentos(str);

            var terminacao = new[] { "N", "B", "D", "T", "W", "AM", "OM", "OIM", "UIM", "CAO", "AO", "OEM", "ONS", "EIA", "X", "US", "TH" };
            var terminacaoSub = new[] { "M", "", "", "", "", "N", "N", "N", "N", "SSN", "N", "N", "N", "IA", "IS", "OS", "TI" };
            var tamanhoMinStr = new[] { 2, 3, 3, 3, 3, 2, 2, 2, 2, 3, 2, 2, 2, 2, 2, 2, 3 };
            int tamanho = 0;
            do
            {
                for (int i = 0; i < terminacao.Length; i++)
                {
                    if (str.EndsWith(terminacao[i]) && str.Length >= tamanhoMinStr[i])
                    {
                        var startIndex = str.Length - terminacao[i].Length;
                        str = str.Remove(startIndex, terminacao[i].Length)
                            .Insert(startIndex, terminacaoSub[i]);
                    }
                    else if (str.Length < tamanhoMinStr[i])
                    {
                        tamanho = tamanhoMinStr[i];
                        break;
                    }
                }
            } while (str.EndsWith("N") && str.Length >= tamanho);
            return str;
        }
    }
}