using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace HMV.Core.Framework.WPF.Helpers
{
    public static class ExportFileHelper
    {
        public static void ExportPrintScreemToPng(string pFileName)
        {
            int TelaLargura = Screen.PrimaryScreen.Bounds.Width;
            int TelaAltura = Screen.PrimaryScreen.Bounds.Height;
            PictureBox picTela = new PictureBox();

            Graphics g;
            Bitmap b = new Bitmap(TelaLargura, TelaAltura);
            g = Graphics.FromImage(b);
            System.Drawing.Point p = new System.Drawing.Point();
            g.CopyFromScreen(p, p, Screen.PrimaryScreen.Bounds.Size);
            picTela.Image = b;
            picTela.Image.Save(pFileName, System.Drawing.Imaging.ImageFormat.Png);
        }

        public static void ExportStringToTXT(string pFileName, params object[] args)
        {
            if (!File.Exists(pFileName))
            {
                StreamWriter txt = new StreamWriter(pFileName, true, Encoding.ASCII);
                foreach (var item in args)
                    txt.WriteLine(item);
                txt.Close();
            }
            else
            {
                StreamWriter txt = File.AppendText(pFileName);
                foreach (var item in args)
                    txt.WriteLine(item);
                txt.Close();
            }
        }
    }
}
