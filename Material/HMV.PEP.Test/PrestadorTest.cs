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

namespace HMV.PEP.Test
{
    [TestClass]
    public class PretsadorTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext) 
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void busca_Lista_de_atendimentos_adicionados_Na_Minha_Lista_de_Pacientes_onde_registro_do_prestador_igual_a_628()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeCodigoIgual(628).Single();

            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3161442).Single();

            prestador.removeAtendimentoDaMinhaListaDePacientes(ate);
            rep.Save(prestador);

            prestador.addAtendimentoNaMinhaListaDePacientes(ate);
            rep.Save(prestador);

            Assert.IsTrue(prestador
                .getAtendimentosAdicionadosNaMinhaListaDePacientes()
                .Count() > 0);
        }

        [TestMethod]
        public void busca_Lista_de_atendimentos_adicionados_Na_Minha_Lista_de_Pacientes_onde_registro_do_prestador_igual_a_1585()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeCodigoIgual(1585).Single();

            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3161442).Single();

            prestador.removeAtendimentoDaMinhaListaDePacientes(ate);
            rep.Save(prestador);

            prestador.addAtendimentoNaMinhaListaDePacientes(ate);
            rep.Save(prestador);

            Assert.IsTrue(prestador
                .getAtendimentosAdicionadosNaMinhaListaDePacientes()
                .Count() > 0);
        }

        [TestMethod]
        public void adiciona_remove__atendimentos_a_Na_Minha_Lista_de_Pacientes_onde_registro_do_prestador_igual_a_628()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeRegistroIgual("628").Single();
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3161442).Single();

            prestador.removeAtendimentoDaMinhaListaDePacientes(ate);
            rep.Save(prestador);
            Assert.IsTrue(prestador
                .getAtendimentosAdicionadosNaMinhaListaDePacientes()
                .Count(x => x.ID == 3161442) == 0);

             prestador.addAtendimentoNaMinhaListaDePacientes(ate);
            rep.Save(prestador);
            Assert.IsTrue(prestador
                .getAtendimentosAdicionadosNaMinhaListaDePacientes()
                .Count(x => x.ID == 3161442) == 1);

            prestador.removeAtendimentoDaMinhaListaDePacientes(ate);
            rep.Save(prestador);
            Assert.IsTrue(prestador
                .getAtendimentosAdicionadosNaMinhaListaDePacientes()
                .Count(x => x.ID == 3161442) == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void adiciona_atendimentos_nulo_Na_Minha_Lista_de_Pacientes_onde_registro_do_prestador_igual_a_11723()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeRegistroIgual("11723").Single();
            prestador.addAtendimentoNaMinhaListaDePacientes(null);
            Assert.Fail("Deve ser informado o atendimento.");
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessValidatorException))]
        public void adiciona_atendimentos_b_ja_existente_Na_Minha_Lista_de_Pacientes_onde_registro_do_prestador_igual_a_11723()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeRegistroIgual("11723").Single();
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1300109).Single();
            prestador.addAtendimentoNaMinhaListaDePacientes(ate);
            rep.Save(prestador);
            prestador.addAtendimentoNaMinhaListaDePacientes(ate);
            Assert.Fail("Atendimento já foi adicionado.");
        }

      
        /* Não localizei esse regra no dominio !!!!
        [TestMethod]
        [ExpectedException (typeof(BusinessValidatorException))]
        public void quando_remover_atendimento_2648792_do_prestador_4625_retornar_um_erro()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeCodigoIgual(4625).Single();
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(2648792).Single();            
            prestador.removeAtendimentoDaMinhaListaDePacientes(ate);
            rep.Save(prestador);
            
            Assert.Fail("Não é possível remover o atendimento  por que e do prestador.");
        }*/
  
        [TestMethod]
        public void busca_equipe_do_prestador_onde_registro_igual_010058()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeRegistroIgual("9700").Single();
            IList<Prestador> equipe = prestador.getEquipeMedica();

            Assert.IsTrue(equipe.Count > 0);
        }

        [TestMethod]
        public void busca_equipe_do_prestador_onde_registro_igual_11071()
        {
            IRepositorioDePrestadores rep = ObjectFactory.GetInstance<IRepositorioDePrestadores>();
            Prestador prestador = rep.OndeRegistroIgual("11071").Single();
            IList<Prestador> equipe = prestador.getEquipeMedica();

            Assert.IsTrue(equipe.Count == 0);
        }

        [TestMethod]
        public void busca_equipe_do_prestador_onde_usuario_igual_M10058RS()
        {
            IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
            Prestador prestador = serv.FiltraPorID("M9700RS").Prestador;
            IList<Prestador> equipe = prestador.getEquipeMedica();

            Assert.IsTrue(equipe.Count > 0);
        }



        [TestMethod]
        public void verifica_se_prestador_eh_medico_onde_usuario_igual_H2555HO()
        {
            IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
            Prestador prestador = serv.FiltraPorID("H2555HO").Prestador;
            Assert.IsTrue(prestador.Conselho.isMedico());

        }

        [TestMethod]
        public void verifica_se_prestador_eh_medico_onde_usuario_igual_H10126HO()
        {
            IUsuariosService serv = ObjectFactory.GetInstance<IUsuariosService>();
            Prestador prestador = serv.FiltraPorID("H10126HO").Prestador;
            Assert.IsTrue(!prestador.Conselho.isMedico());

        }

        [TestMethod]
        public void quando_buscar_usuario_verifica_tem_permissao_para_adicionar_pacientes_na_sua_lista_onde_usuario_igual_H2555HO()
        {
            Usuarios usuario = ObjectFactory.GetInstance<IUsuariosService>().FiltraPorID("H2555HO");
            Assert.IsTrue(usuario.AcessoTotalProntuario(11));

        }

        [TestMethod]
        public void quando_busca_pelo_nome_prestador_retorna_todos_os_prestadores_que_o_nome_contem_MARCIO()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            serv.FiltraPorNome("MARCIO");
            IList<Prestador> lista = serv.Carrega();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void quando_buscar_prestador_pelo_registro_26450_retorna_um_prestador()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            serv.FiltraPorRegistro("26450");
            Prestador prestador = serv.Carrega().Single();
            Assert.IsNotNull(prestador);
        }

        [TestMethod]
        public void quando_buscar_prestador_pelo_registro_26450__e_por_nome_ANDRE_retorna_um_prestador()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            serv.FiltraPorRegistro("26450");
            serv.FiltraPorNome("ANDRE");
            Prestador prestador = serv.Carrega().Single();
            Assert.IsNotNull(prestador);
        }

        [TestMethod]
        public void quando_buscar_no_repositorio_de_prestador_todos_prestadores_retornar_somente_os_que_tem_registro_cadastrado()
        {
            IRepositorioDePrestadoresAtivos rep = ObjectFactory.GetInstance<IRepositorioDePrestadoresAtivos>();
            rep.OndeRegistroInformado();
            IList<Prestador> lista = rep.List();
            Assert.AreEqual(lista.Count(x=>x.Registro == null ),0);
        }

        [TestMethod]
        public void quando_buscar_no_servico_de_prestador_todos_prestadores_retornar_somente_os_que_tem_registro_cadastrado()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            serv.FiltraOndeRegistroInformado();
            IList<Prestador> lista = serv.Carrega();
            Assert.AreEqual(lista.Count(x => x.Registro == null), 0);
        }

        [TestMethod]
        public void quando_buscar_prestador_id_igual_4637_retornar_O_usuario_vinculado_ao_prestador()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(4637);
            IList<Usuarios> usuarios = prestador.getUsuarios();
            Assert.AreEqual(usuarios.Count, 1);
        }

        [TestMethod]
        public void quando_buscar_prestador_id_igual_1430_retornar_Os_usuarios_vinculado_ao_prestador()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1430);
            IList<Usuarios> usuarios = prestador.getUsuarios();
            Assert.AreEqual(usuarios.Count, 2);
        }

        [TestMethod]
        public void quando_buscar_pelo_repositorio_de_prestador_onde_clinica_igual_11_retornar_todos_os_prestadores_vinculados_a_clinica_11()
        {
            IRepositorioDePrestadoresAtivos rep = ObjectFactory.GetInstance<IRepositorioDePrestadoresAtivos>();
            IList<Prestador> lista = rep.OndeClinicaIgual(11).List();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void quando_buscar_pelo_servico_de_prestador_onde_clinica_igual_parametro_retornar_todos_os_prestadores_vinculados_a_clinica()
        {
            int idClin = 0;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["ClinicaDefault"], out idClin))
                throw new NullReferenceException("Parametro 'ClinicaDefault' deve existir no arquivo de configuração.");
            
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            serv.FiltraPorClinica(idClin);
            IList<Prestador> lista = serv.Carrega(); 
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_prestador_id_igual_3076_adiciona_na_equipe_do_prestador_id_1430()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1430);
            Prestador equipe = serv.FiltraPorId(3076);

            prestador.removeEquipeMedica(equipe);
            serv.Save(prestador);

            prestador.addEquipeMedica(equipe);
            serv.Save(prestador);

            Assert.AreEqual(serv.FiltraPorId(1430)
                .getEquipeMedica().Count(x => x.Id == 3076), 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessValidatorException))]
        public void busca_prestador_id_igual_3076_e_adiciona_duas_vezes_na_equipe_do_prestador_id_1430()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1430);
            Prestador equipe = serv.FiltraPorId(3076);
            prestador.addEquipeMedica(equipe);
            serv.Save(prestador);

            prestador = serv.FiltraPorId(1430);
            prestador.addEquipeMedica(equipe);
            serv.Save(prestador);

            Assert.AreEqual(serv.FiltraPorId(1430)
                .getEquipeMedica().Count(x => x.Id == 3076), 1);
        }

        [TestMethod]
        public void busca_prestador_id_igual_1430_e_remove_da_equipe_o_prestador_id_3076()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1430);
            Prestador equipe = prestador.getEquipeMedica().Where(x => x.Id == 3076).FirstOrDefault();
            if ( equipe != null ) 
                prestador.removeEquipeMedica(equipe);
            serv.Save(prestador);

            Assert.AreEqual(serv.FiltraPorId(1430)
                .getEquipeMedica().Count(x => x.Id == 3076), 0);
        }

        [TestMethod]
        public void busca_prestador_id_igual_1430_e_remove_da_equipe_o_prestador_id_3101_que_nao_esta_na_equipe()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1430);
            Prestador equipe = serv.FiltraPorId(3101);

            prestador.removeEquipeMedica(equipe);
            serv.Save(prestador);

            Assert.AreEqual(serv.FiltraPorId(1430)
                .getEquipeMedica().Count(x => x.Id == 3101), 0);
        }

        [TestMethod]
        public void busca_equipe_medica_dr_mauricio_1970()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(1970);

            Assert.IsTrue(prestador.getEquipeMedica().Count > 0);
        }

        [TestMethod]
        public void busca_as_evolucoes_padrao_do_prestador_3533()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(6586);
            IList<EvolucaoPadrao> evos = prestador.EvolucaoPadrao;
            Assert.IsTrue(evos.Count > 0);
            Assert.IsNotNull(evos.FirstOrDefault(x => x.Id == 88));
            Assert.AreEqual(evos.FirstOrDefault(x => x.Id == 88).Titulo, "Evol 1");
        }

        [TestMethod]
        public void adiciona_uma_nova_evolucao_padrao_do_prestador_604()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(604);
            EvolucaoPadrao evo = new EvolucaoPadrao("NOVA HISTERECTOMIA","TESTE");
            prestador.AddEvolucaoPadrao(evo);
            serv.Save(prestador);
            Assert.IsNotNull(prestador.EvolucaoPadrao.FirstOrDefault(x => x.Id == evo.Id));
            Assert.AreEqual(prestador.EvolucaoPadrao.FirstOrDefault(x => x.Id == evo.Id).Titulo, "NOVA HISTERECTOMIA");
            Assert.AreEqual(prestador.EvolucaoPadrao.FirstOrDefault(x => x.Id == evo.Id).Descricao, "TESTE");

            prestador.RemoveEvolucaoPadrao(evo);
            serv.Save(prestador);

            Assert.IsNull(prestador.EvolucaoPadrao.FirstOrDefault(x => x.Id == evo.Id));

        }

        [TestMethod]
        public void busca_lista_de_recomendacoes_padrao_prestador_3092()
        {
            IPrestadorService serv = ObjectFactory.GetInstance<IPrestadorService>();
            Prestador prestador = serv.FiltraPorId(604);

            Recomendacao rec = ObjectFactory.GetInstance<IRepositorioDeRecomendacao>().OndeIdIgual(1).Single();
            RecomendacaoPadrao recPadrao = new RecomendacaoPadrao(rec,"REC RODRIGO","minha recomendação padrão");
            prestador.AddRecomendacaoPadrao(recPadrao);
            serv.Save(prestador);
            
            Assert.IsNotNull(prestador.RecomendacaoPadrao.FirstOrDefault(x => x.Id == recPadrao.Id));

            prestador.RemoveRecomendacaoPadrao(recPadrao);
            serv.Save(prestador);
            
            Assert.IsNull(prestador.RecomendacaoPadrao.FirstOrDefault(x => x.Id == recPadrao.Id));
        }

    }
}
