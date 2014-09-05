using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Windows.Documents;

namespace HMV.Core.Framework.Helper
{
    public static class HTMLHelper
    {
        public static string ConvertSpaceToBR(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;
            return text.Replace(Environment.NewLine, "<br />");
        }
    }

    public class RTFtoHTMLHelper
    {
        public RTFtoHTMLHelper(string prtf)
        {
            rtf = prtf;
        }

        #region "Private Members"

        // A RichTextBox control to use to help with parsing.
        private System.Windows.Forms.RichTextBox _rtfSource = new System.Windows.Forms.RichTextBox();

        #endregion

        #region "Read/Write Properties"

        /// <summary>
        /// Returns/Sets The RTF formatted text to parse
        /// </summary>
        public string rtf
        {
            get { return _rtfSource.Rtf; }
            set { _rtfSource.Rtf = value; }
        }

        #endregion

        #region "ReadOnly Properties"

        /// <summary>
        /// Returns the HTML code for the provided RTF
        /// </summary>
        public string html
        {
            get { return GetHtml(); }
        }

        #endregion

        #region "Private Functions"

        /// <summary>
        /// Returns an HTML Formated Color string for the style from a system.drawing.color
        /// </summary>
        /// <param name="clr">The color you wish to convert</param>
        private string HtmlColorFromColor(System.Drawing.Color clr)
        {
            string strReturn = "";
            if (clr.IsNamedColor)
            {
                strReturn = clr.Name.ToLower();
            }
            else
            {
                strReturn = clr.Name;
                if (strReturn.Length > 6)
                {
                    strReturn = strReturn.Substring(strReturn.Length - 6, 6);
                }
                strReturn = "#" + strReturn;
            }
            return strReturn;
        }

        /// <summary>
        /// Provides the font style per given font
        /// </summary>
        /// <param name="fnt">The font you wish to convert</param>
        private string HtmlFontStyleFromFont(System.Drawing.Font fnt)
        {
            string strReturn = "";
            //style
            if (fnt.Italic)
            {
                strReturn += "italic ";
            }
            else
            {
                strReturn += "normal ";
            }
            //variant
            strReturn += "normal ";
            //weight
            if (fnt.Bold)
            {
                strReturn += "bold ";
            }
            else
            {
                strReturn += "normal ";
            }
            //size
            strReturn += fnt.SizeInPoints + "pt/normal ";
            //family
            strReturn += fnt.FontFamily.Name;
            return strReturn;
        }

        /// <summary>
        /// Parses the given rich text and returns the html.
        /// </summary>
        private string GetHtml()
        {
            string strReturn = "<div>";
            System.Drawing.Color clrForeColor = Color.Black;
            System.Drawing.Color clrBackColor = Color.Black;
            System.Drawing.Font fntCurrentFont = _rtfSource.Font;
            //System.Windows.Forms.HorizontalAlignment altCurrent = HorizontalAlignment.Left;
            int intPos = 0;
            for (intPos = 0; intPos <= _rtfSource.Text.Length - 1; intPos++)
            {
                _rtfSource.Select(intPos, 1);
                //Forecolor
                if (intPos == 0)
                {
                    strReturn += "<span style=\"color:" + HtmlColorFromColor(_rtfSource.SelectionColor) + "\">";
                    clrForeColor = _rtfSource.SelectionColor;
                }
                else
                {
                    if (_rtfSource.SelectionColor != clrForeColor)
                    {
                        strReturn += "</span>";
                        strReturn += "<span style=\"color:" + HtmlColorFromColor(_rtfSource.SelectionColor) + "\">";
                        clrForeColor = _rtfSource.SelectionColor;
                    }
                }
               
                //Font
                if (intPos == 0)
                {
                    strReturn += "<span style=\"font:" + HtmlFontStyleFromFont(_rtfSource.SelectionFont) + "\">";
                    fntCurrentFont = _rtfSource.SelectionFont;
                }
                else
                {
                    if (_rtfSource.SelectionFont.GetHashCode() != fntCurrentFont.GetHashCode())
                    {
                        strReturn += "</span>";
                        strReturn += "<span style=\"font:" + HtmlFontStyleFromFont(_rtfSource.SelectionFont) + "\">";
                        fntCurrentFont = _rtfSource.SelectionFont;
                    }
                }
                //Alignment
                /*if (intPos == 0)
                {
                    strReturn += "<p style=\"text-align:" + _rtfSource.SelectionAlignment.ToString() + "\">";
                    altCurrent = _rtfSource.SelectionAlignment;
                }
                else
                {
                    if (_rtfSource.SelectionAlignment != altCurrent)
                    {
                        strReturn += "</p>";
                        strReturn += "<p style=\"text-align:" + _rtfSource.SelectionAlignment.ToString() + "\">";
                        altCurrent = _rtfSource.SelectionAlignment;
                    }
                }*/
                strReturn += _rtfSource.Text.Substring(intPos, 1);
            }
            //close all the spans
            
            //strReturn += "</p>";
            strReturn += "</span>";
            strReturn += "</span>";
            //strReturn += "</span>";
            //strReturn += "</p>";
            strReturn += "</div>";
            strReturn = strReturn.Replace(Convert.ToString(Convert.ToChar(10)), "<br />");
            return strReturn;
        }

        #endregion
    }

    public static class JSHelper
    {
        public static void AplicaJS(string arquivo)
        {
            string mensagem;
            var aspas = Convert.ToChar(34);
            List<string> mensagemLinha = new List<string>();

            using (StreamReader texto = new StreamReader(arquivo))
            {
                while ((mensagem = texto.ReadLine()) != null)
                {
                    mensagemLinha.Add(mensagem);
                }
            }

            int registro = mensagemLinha.Count;
            TextBox textbox1 = new TextBox();
            for (int i = 0; i < mensagemLinha.Count; i++)
            {
                textbox1.Text += mensagemLinha[i] + Convert.ToChar(13);
            }
            
            StringBuilder js = new StringBuilder();
            js.Append("<script language=JavaScript>" + Convert.ToChar(13));
            js.Append("var mensagem=" + aspas + "Ação bloqueada." + aspas + ";" + Convert.ToChar(13));
            js.Append("function clickIE() {if (document.all) {(mensagem);return false;}}" + Convert.ToChar(13));
            js.Append("function clickNS(e) {if " + Convert.ToChar(13));
            js.Append("(document.layers||(document.getElementById&&!document.all)) {" + Convert.ToChar(13));
            js.Append("if (e.which==2||e.which==3) {(mensagem);return false;}}}" + Convert.ToChar(13));
            js.Append("if (document.layers) " + Convert.ToChar(13));
            js.Append("{document.captureEvents(Event.MOUSEDOWN);document.onmousedown=clickNS;}" + Convert.ToChar(13));
            js.Append("else{document.onmouseup=clickNS;document.oncontextmenu=clickIE;}" + Convert.ToChar(13));
            js.Append("document.oncontextmenu=new Function(" + aspas + "return false" + aspas + ")" + Convert.ToChar(13));
            js.Append(Convert.ToChar(13));

            js.Append(Convert.ToChar(13));
            js.Append("function keyWhat(e)" + Convert.ToChar(13));
            js.Append("{" + Convert.ToChar(13));
            js.Append("     if(event.keyCode == 80 )" + Convert.ToChar(13));
            js.Append("         { " + Convert.ToChar(13));
            js.Append("              alert(" + aspas + "Para imprimir clique na barra acima." + aspas + ");" + Convert.ToChar(13));
            js.Append("              return false;" + Convert.ToChar(13));
            js.Append("         }" + Convert.ToChar(13));
            js.Append("}" + Convert.ToChar(13));
            js.Append("document.onkeydown=keyWhat;" + Convert.ToChar(13));

            js.Append("</script>" + Convert.ToChar(13));

            File.WriteAllText(arquivo, textbox1.Text + js.ToString());
        }
    }
}
