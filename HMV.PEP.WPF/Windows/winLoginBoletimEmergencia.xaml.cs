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
using System.Windows.Shapes;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;
using HMV.Core.Interfaces;
using StructureMap;
using HMV.PEP.Services;
using HMV.PEP.DTO;
using DevExpress.Xpf.Core;
using System.Configuration;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winLoginBoletimEmergencia.xaml
    /// </summary>
    public partial class winLoginBoletimEmergencia : WindowBase
    {
        public winLoginBoletimEmergencia()
        {
            InitializeComponent();
            
            //Vem com o usuario atual preenchido
            //if (App.Usuario.IsNotNull())
                txtUser.Text = App.UsuarioDTO.ID;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            IGenericSecurityService serv = ObjectFactory.GetInstance<IGenericSecurityService>();
            try
            {
                serv.ValidaSenhaViaFunctionCriptoDescripto(txtUser.Text, txtPass.Password);
                int idSistema;
                if (!int.TryParse(ConfigurationManager.AppSettings["SistemaAntigo"], out idSistema))
                    throw new System.Configuration.SettingsPropertyNotFoundException("Sistema não configurado. Adicione a Key [Sistema Antigo] no App.config.");
                Usuarios usu = serv.FiltraUsuario(txtUser.Text, idSistema);
                if (usu.IsNotNull())
                {
                    if (App.Usuario != usu)
                    {
                        App.Usuario = usu;
                        InicializacaoService servdto = new InicializacaoService();
                        Retorno<UsuarioDTO> ret = servdto.BuscaConfiguracaoDoUsuario(App.Usuario.ID);
                        App.UsuarioDTO = ret.Data;
                        this.DialogResult = true;
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                DXMessageBox.Show(ex.Message, "Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnfechar_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Deseja realmente fechar o sistema?", "Login", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(-1);
                return;
            }
        }

        private void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(sender, null);
        }
    }
}
