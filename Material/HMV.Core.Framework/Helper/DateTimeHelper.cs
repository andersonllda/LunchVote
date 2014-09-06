using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HMV.Core.Framework.Helper
{
    public static class DateTimeHelper
    {
        /// DateDiff para C#
        /// Indica qual será o retorno [0 = Dias, 1 = Meses, 2 = Anos]
        /// Data Inicial
        /// Data Final
        /// Retorna a diferença de acordo com o Intervalo escolhido
        public static double dateDiff(int interval, DateTime date1, DateTime date2)
        {
            TimeSpan tsDuration;
            tsDuration = date2 - date1;

            if (interval == 0)
            {
                // Resultado em Dias
                return Convert.ToDouble(tsDuration.Days);
            }
            else if (interval == 1)
            {
                // Resultado em Meses
                return Convert.ToDouble((Convert.ToDouble(tsDuration.Days) / 365) * 12);
            }
            else if (interval == 2)
            {
                // Resultado em Anos
                return Convert.ToDouble((Convert.ToDouble(tsDuration.Days) / 365));
            }
            else
            {
                return 0;
            }
        }

        public static string ConvertDateToString(DateTime pData)
        {
            return pData.ToString("ddMMyyyy");
        }

        public static string ConvertTimeToString(DateTime pData)
        {
            return pData.ToString("HHmmss");
        }

        /// <summary>
        /// Convert string para datetime, string com a hora deve estar no formado HH:MM 
        /// </summary>
        /// <param name="pTime"></param>
        public static DateTime? ConvertStringToTime(string pTime)
        {
            try
            {
                if (string.IsNullOrEmpty(pTime))
                    return null;
                DateTime dataAtual = new DateTime();                
                int hours = Convert.ToInt32(pTime.Split(':').First());
                int minute = Convert.ToInt32(pTime.Split(':').Last());
                return new DateTime(dataAtual.Year, dataAtual.Month, dataAtual.Day, hours, minute, 0);
            }
            catch
            {
                return null;
            }
        }

        public static string Saudacao()
        {
            DateTime tempo = DateTime.Now;
            if (tempo.Hour > 6 && tempo.Hour < 12)
                return "Bom dia";
            else if (tempo.Hour >= 12 && tempo.Hour < 18)
                return "Boa tarde";
            else
                return "Boa noite";
        }
    }
}
