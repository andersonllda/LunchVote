using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Exception;
using HMV.Core.Interfaces;
using System.Configuration;
using HMV.Core.Domain.Model.PEP.CheckListCirurgia;
using HMV.Core.Domain.Enum;
using NHibernate;
using HMV.Core.DataAccess;

namespace HMV.PEP.Test
{
    [TestClass]
    public class UsuarioTest : BaseTestClass
    {

        private static int idClinica = 0;

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
            if (!int.TryParse(ConfigurationManager.AppSettings["ClinicaDefault"], out idClinica))
                throw new System.Configuration.SettingsPropertyNotFoundException("Clinica não configurada.");

        }

        [TestMethod]
        public void quando_buscar_o_usuario_h2555ho_verificar_se_tem_acesso_total_ao_prontuario_retornar_verdadeiro()
        {
            Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            Assert.IsTrue(usuario.AcessoTotalProntuario(idClinica));
        }

        [TestMethod]
        public void quando_buscar_o_usuario_H9245HO_verificar_se_tem_acesso_total_ao_prontuario_retornar_falso()
        {
            Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H9245HO");
            Assert.IsFalse(usuario.AcessoTotalProntuario(idClinica));
        }

        [TestMethod]
        public void quando_buscar_o_usuario_H9458HO_verificar_se_tem_acesso_total_ao_prontuario_retornar_falso()
        {
            Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H9458HO");
            Assert.IsFalse(usuario.AcessoTotalProntuario(idClinica));
        }
        

        [TestMethod]
        public void quando_buscar_usuario_HOO349EG_verificar_se_tem_acesso_total_ao_prontuario_retornar_falso()
        {
            Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("HOO349EG");
            Assert.IsFalse(usuario.AcessoTotalProntuario(idClinica));
        }

        //[TestMethod]
        //public void quando_buscar_oNome_de_exibicao_do_usuario_M24962RS_retornar_o_nome_Giuliano_Barboza_Borile()
        //{
        //    Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("M24962RS");
        //    Assert.AreEqual(usuario.NomeExibicaoProfissional(idClinica, false), "Dr. Giuliano Barboza Borille");
        //}

        //[TestMethod]
        //public void quando_buscar_oNome_de_exibicao_do_usuario_H2555HO_retornar_o_nome_DrPonto_Deoclecio_Martini()
        //{



        //    Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
        //    Assert.AreEqual(usuario.NomeExibicaoProfissional(idClinica, false), "Dr. Adamastor Humberto Pereira");
        //}     
    }
}
