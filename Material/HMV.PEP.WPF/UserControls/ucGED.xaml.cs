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
using HMV.Core.Framework.WPF;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Windows.Forms.Integration;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucGED.xaml
    /// </summary>
    public partial class ucGED : UserControlBase, IUserControl
    {
        public bool MostraAtendimento { get; set; }
        public bool Abriu { get; set; }

        public ucGED()
        {
            InitializeComponent(); 
        }

        public void SetData(object pData)
        {
            int idpacinte = 0;
            int idatendimento = 0;
            if (pData != null)
            {
                if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
                {
                    idpacinte = (pData as Paciente).ID;
                }
                else if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
                {
                    idatendimento = (pData as Atendimento).ID;
                    idpacinte = (pData as Atendimento).Paciente.ID;
                }
            }

            if (!MostraAtendimento)
                idatendimento = 0;

            //if (idatendimento == 0)
            //{
            //    winSelecionaAtendimentoGED win = new winSelecionaAtendimentoGED(idpacinte);
            //    if (win.ShowDialog(base.OwnerBase) == true)
            //        idatendimento = win.IdAtendimento;
            //}

            //if (idpacinte != 0 && idatendimento != 0)
            //{
            Abriu = true;
            WindowsFormsHost host = new WindowsFormsHost();

            HMV.PEP.GED.ucGED ged = new HMV.PEP.GED.ucGED(App.Usuario.cd_usuario, idpacinte, idatendimento);
            host.Child = ged;
            //host.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            //host.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.grid1.Children.Add(host);
            //}


            //string prontuarioDireto = idpacinte.ToString(); //Número de registro paciente
            //String atendimentoDireto = idatendimento.ToString(); //Número do Atendimento
            //string usuarioDireto = App.Usuario.cd_usuario;//Nome de login do Usuário que esta realizando a chamada

            //ASCIIEncoding encoding = new ASCIIEncoding();

            //string postData = "prontuarioDireto=" + prontuarioDireto;
            //postData += ("&atendimentoDireto=" + atendimentoDireto);
            //postData += ("&usuarioDireto=" + usuarioDireto);
            //byte[] data = encoding.GetBytes(postData);

            //string vHeaders = "Content-Type: application/x-www-form-urlencoded";

            //IRepositorioDeParametrosClinicas rep = ObjectFactory.GetInstance<IRepositorioDeParametrosClinicas>();

            //this.browser.Navigate(new Uri(rep.BuscaURLdoGED().Single().Valor), "", data, vHeaders);
            //this.browser.ObjectForScripting = new ScriptingHelper();

            //string _html = "<html >" +
            //  "<BODY bgcolor=\"#006699\" style=\"overflow: hidden;\">" +
            // "</body>" +
            // "</html>";

            //this.browserBandaid.NavigateToString(_html);

            //browser.Navigate("http://10.0.0.207:9009/gedwebhmv.aspx", "_blank", data, vHeaders);


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
}
