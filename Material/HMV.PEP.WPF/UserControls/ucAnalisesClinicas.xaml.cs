using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HMV.Core.Interfaces;
using System.Runtime.InteropServices;
using DevExpress.Xpf.Bars;
using HMV.Core.Domain.Model;
using System.Configuration;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls
{
    

    /// <summary>
    /// Interaction logic for ucAnalisesClinicas.xaml
    /// </summary>
    public partial class ucAnalisesClinicas : UserControlBase, IUserControl
    {
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern long DeleteUrlCacheEntry(string lpszUrlName);     

        public ucAnalisesClinicas()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            
            HMV.PEP.Interfaces.IPacienteService srv = StructureMap.ObjectFactory.GetInstance<HMV.PEP.Interfaces.IPacienteService>();
            int IDPaciente = 0;
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
                IDPaciente = (pData as Atendimento).Paciente.ID;
            else
                IDPaciente = (pData as Paciente).ID;

            decimal IDSenha = srv.ValidaSenhaFleury(IDPaciente);
            HMV.PEP.Interfaces.IParametroPEPService srvPara = StructureMap.ObjectFactory.GetInstance<HMV.PEP.Interfaces.IParametroPEPService>();
            string uri = srvPara.LinkFleury().Valor; 
            uri = uri.Replace(":CD_PACIENTE", IDPaciente.ToString()).Replace(":CD_CHAVE", IDSenha.ToString());
            
            DeleteUrlCacheEntry(uri);
            //this.browser.Dispose();
            

            this.browser.Navigate(new Uri(uri));
            this.browser.ObjectForScripting = new ScriptingHelper();
            
            // Gamba para esconder o botão sair do link 
            
            //string _html = "<html >" +
            //"<BODY style=\"overflow: hidden; margin: 0px \">" +
            //"<img src=\"http://hoh2313/RegistraEntregaPalm/imgBlueBrowserAnalise.png\" WIDTH=100% HEIGHT=100%>" +
            //"</body>" +
            //"</html>";

            string _html = "<html >" +
              "<BODY bgcolor=\"#006699\" style=\"overflow: hidden;\">" +
             "</body>" +
             "</html>";

            this.browserBandaid.NavigateToString(_html);

        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

      
    }

    [ComVisible(true)]
    public class ScriptingHelper { public void ShowMessage(string message) { MessageBox.Show(message); } }
}
