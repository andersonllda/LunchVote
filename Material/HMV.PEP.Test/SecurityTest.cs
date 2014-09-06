using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMV.Core.Interfaces;
using HMV.Core.Domain.Model;
using StructureMap;
using System.Configuration;

namespace HMV.PEP.Test
{
    [TestClass]
    public class SecurityTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        [ExpectedException(typeof(HMV.Core.Framework.Exception.AutorizacaoSistemaException), "Usuario não possui acesso ao sistema: Usuario: H10531HO Sistema: 626")]
        public void quando_buscar_usuario_H10531HO_com_senha_criptografada_valida_deve_retornar_excecao_sem_acesso_ao_sistema()
        {
            ISecurityService serv = ObjectFactory.GetInstance<ISecurityService>();
            Usuarios usu = serv.ValidarUsuario("H10531HO", "W46DEFuHGaSeYiUYcgkosw0XYZabcd", "HOMOLOG_02");
        }

        [TestMethod]
        [ExpectedException(typeof(HMV.Core.Framework.Exception.AutenticationException), "Usuário ou senha inválidos, Usuario: H2555HO")]
        public void quando_buscar_usuario_H2555HO_com_senha_123456_deve_retornar_excecao_falha_de_autenticacao()
        {
            ISecurityService serv = ObjectFactory.GetInstance<ISecurityService>();
            Usuarios usu = serv.ValidarUsuario("H2555HO", "123456", "HOMOLOG_02");
        }

        [TestMethod]
        [ExpectedException(typeof(HMV.Core.Framework.Exception.AutenticationException), "Usuário ou senha inválidos, Usuario: HINVALIDOHO")]
        public void quando_busca_usuario_HINVALIDOHO_com_senha_xxx_deve_retornar_excecao_falha_de_autenticacao()
        {
            ISecurityService serv = ObjectFactory.GetInstance<ISecurityService>();
            Usuarios usu = serv.ValidarUsuario("HINVALIDOHO", "xxx", "HOMOLOG_02");
        }

        [TestMethod]
        public void quando_buscar_parametro_de_configuracao_igual_a_Sistema_retornar_o_id_do_sistema()
        {
            int idSistema;
            if (!int.TryParse(ConfigurationManager.AppSettings["Sistema"], out idSistema))
                throw new System.Configuration.SettingsPropertyNotFoundException("Sistema não configurado.");
            Assert.AreNotEqual(idSistema, 0);

        }

      

              
    }
}
