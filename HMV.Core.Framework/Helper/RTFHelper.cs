using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;

namespace HMV.Core.Framework.Helper
{
    public class RTFHelper
    {
        private Paragraph _paragraph;
        private RichTextBox rtb;

        public RTFHelper()
        {
            _paragraph = new Paragraph();
            rtb = new System.Windows.Controls.RichTextBox();
            rtb.Document = new FlowDocument(_paragraph);
        }

        public FlowDocument FlowDoc()
        {
            return new FlowDocument(_paragraph);
        }

        public void criaTitulo(string pTitulo)
        {
            _paragraph.Inlines.Add(new Bold(new Run(pTitulo))
            {
                Foreground = System.Windows.Media.Brushes.Blue,
                FontSize = 14
            });
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void criaTituloSublinhado(string pTitulo)
        {
            _paragraph.Inlines.Add(new Underline(new Bold(new Run(pTitulo)))
            {
                Foreground = System.Windows.Media.Brushes.Blue,
                FontSize = 14
            });
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void criaSubTitulo(string pTitulo)
        {
            _paragraph.Inlines.Add(new Bold(new Run(pTitulo))
            {
                Foreground = System.Windows.Media.Brushes.Black,
                FontSize = 12
            });
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void criaSubTituloSublinhado(string pTitulo)
        {
            _paragraph.Inlines.Add(new Underline(new Bold(new Run(pTitulo)))
            {
                Foreground = System.Windows.Media.Brushes.Black,
                FontSize = 12
            });
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void criaTexto(string pTexto)
        {
            _paragraph.Inlines.Add(pTexto);
            _paragraph.Inlines.Add(new LineBreak());
        }

        public void criaLinhaEmBranco()
        {
            _paragraph.Inlines.Add(new LineBreak());
        }

        public static string StripRTF(string rtfString)
        {
            string result = rtfString;

            try
            {
                if (IsRichText(rtfString))
                {                 
                    using (System.Windows.Forms.RichTextBox rtfTemp = new System.Windows.Forms.RichTextBox())
                    {
                        rtfTemp.Rtf = rtfString;
                        result = rtfTemp.Text;
                    }
                }
                else
                {
                    result = rtfString;
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        public static bool IsRichText(string testString)
        {
            if ((testString != null) &&
                (testString.Trim().StartsWith("{\\rtf")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
