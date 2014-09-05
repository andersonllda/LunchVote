using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using HMV.PEP.DTO;
using HMV.PEP.Consult;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Views.PEP;
using HMV.PEP.Services;
using System.Configuration;
using HMV.Core.DataAccess;
using HMV.PEP.IoC;

namespace HMV.PEP.Test
{
    [TestClass]
    public class ServicosDAInicializacaoDoPepTest
    {

        private static InicializacaoService _servico;

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            _servico = new InicializacaoService();
        }

        [TestMethod]
        public void busca_configuracao_do_usuario_h2555ho()
        {
            UsuarioDTO usuario = _servico.BuscaConfiguracaoDoUsuario("DESEN").Data;
            Assert.AreEqual(usuario.ID, "DESEN");
            Assert.IsTrue(usuario.AcessoTotalProntuario);
        }

        [TestMethod]
        public void busca_configuracao_do_sistema()
        {
            IList<string> conf = _servico.BuscaParametrosPAMExPEP("H2555HO");
            Assert.IsTrue(conf.Count > 0);
        }

        [TestMethod]
        public void busca_medicos_assistentes()
        {
            IList<MedicoAssistenteDTO> lista = _servico.ListaMedicosComPacientesInternados();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_unidadas()
        {
            IList<UnidadeInternacaoDTO> lista = _servico.ListaUnidadesInternacaoComPacientesInternados();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_meus_pacientes_prestador_2052()
        {
            IList<MinhaListaDTO> lista = _servico.ListaMeusPacientes(2052);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_minha_agenda_pacientes_prestador_2052()
        {
            IList<AgendaMedicaDTO> lista = _servico.CarregarAgenda(2052, DateTime.Now.AddMonths(-12), DateTime.Now.Date);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_ListaPacientesInternados()
        {
            IList<vPacienteInternado> lista = _servico.ListaPacientesInternados(0,string.Empty,0,0);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void busca_ListaPacientesInternadosPorNome()
        {
            IList<vPacienteInternado> lista = _servico.ListaPacientesInternados(0, "joao", 0, 0);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void wcf_busca_ListaPacientesInternadosPorNome()
        {
            HMV.PEP.WCF.Inicializacao.IPacienteService serv = new HMV.PEP.WCF.Inicializacao.PacienteService();
            var ret = serv.ListaPacienteInternadoPorNome("0", "0", "0", "joao");

            Assert.IsTrue(ret.Data.Count > 0);

        }

        [TestMethod]
        public void busca_ListaPacientesAmbulatorial()
        {
            IList<PacienteAmbulatorialDTO> lista = _servico.ListaPacientesAmbulatorial(0, string.Empty);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void BuscaSemAtendimentos()
        {
            IList<PacienteEmergenciaDTO> lista = _servico.BuscaSemAtendimentos();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void BuscaAtendimentosInternados()
        {
            IList<vPacienteInternado> lista = _servico.BuscaAtendimentosInternados();
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void BuscaPacientesNaEmergencia()
        {
            IList<vEmergenciaPEP> lista = _servico.BuscaPacientesNaEmergencia(false,false,string.Empty);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void BuscaPacienteEmergenciaPAME()
        {
            IList<vEmergenciaPAME> lista = _servico.BuscaPacienteEmergenciaPAME(string.Empty);
            Assert.IsTrue(lista.Count > 0);
        }

        [TestMethod]
        public void teste_performece_wcf()
        {
            for (int i = 0; i < 1000; i++)
            {
                IList<vEmergenciaPEP> lista = _servico.BuscaPacientesNaEmergencia(false, false, string.Empty);
            }
            Assert.IsTrue(true);
        }
        

    }
}
