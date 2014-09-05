using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace HMV.PEP.WPF.UserControls.VB6
{
    public partial class IUserControlVB6 : UserControl
    {
        public IUserControlVB6()
        {
            InitializeComponent();
        }

        public void SetData(string pComponente)
        {
            string pUser = App.Usuario.cd_usuario;
            string pBanco = App.Banco;
            int pCodClinica = Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"]);
            int pCodSubclinica = Convert.ToInt32(ConfigurationManager.AppSettings["ClinicaDefault"]);

            axctlIntegra.set_Componente(ref pComponente);

            axctlIntegra.Inicializa(ref pUser, ref pBanco, ref pCodClinica, ref pCodSubclinica);
        }

        public void ChamaMetodo(string pMetodo, object pArgs)
        {
            int pType = 4;
            axctlIntegra.ChamaMetodo(ref pMetodo, ref pType, ref pArgs);
        }

        public void ChamaMetodo(string pMetodo)
        {
            int pType = 1;
            axctlIntegra.ChamaMetodoArgsNull(ref pMetodo, ref pType);
        }

        public void ChamaMetodo(string pMetodo, int pType, object pArgs)
        {
            axctlIntegra.ChamaMetodo(ref pMetodo, ref pType, ref pArgs);
        }
    }
}
