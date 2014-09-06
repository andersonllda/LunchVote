using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model.DocumentosEletronicos;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.BoletimEmergencia;
using System.Windows.Input;
using HMV.PEP.Consult;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class BoletimEmergenciaTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);            
        }

        [TestMethod]
        public void testa_Atendimento_Com_Mais_de_Um_Boletim()
        {
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = repAtend.OndeCodigoAtendimentoIgual(3149272).Single();
            Assert.IsTrue(atend.BoletinsDeEmergencia.Count > 1);
        }

        [TestMethod]
        public void testa_Atendimento_Um_Boletim_E_Sem_Alta()
        {
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = repAtend.OndeCodigoAtendimentoIgual(3148962).Single();
            Assert.IsTrue(atend.BoletinsDeEmergencia.Count == 1);
            Assert.IsNull(atend.UltimoBoletimDeEmergencia.DataAlta);
        }

        [TestMethod]
        public void testa_Atendimento_Sem_Boletim_De_Emergencia() //pass
        {
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = repAtend.OndeCodigoAtendimentoIgual(1505897).Single();

            Assert.AreEqual(atend.BoletinsDeEmergencia.Count, 0);
            Assert.IsNull(atend.UltimoBoletimDeEmergencia);
        }

        [TestMethod]
        public void testa_Atendimento_Um_Boletim_E_Com_Alta_3149094()
        { //2917681 esta no homolog01 peguei um ID do homolog02 (3149094) e funcionou!
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = repAtend.OndeCodigoAtendimentoIgual(3149094).Single();

            Assert.AreEqual(atend.BoletinsDeEmergencia.Count, 1);
            Assert.IsNotNull(atend.UltimoBoletimDeEmergencia.DataAlta);
        }

        [TestMethod]
        public void verifica_se_procedimento_40103170_eh_exame_possivel_para_boletins()
        {
            IRepositorioDeProcedimento rep = ObjectFactory.GetInstance<IRepositorioDeProcedimento>();
            Procedimento proc = rep.OndeIdIgual("40103170").Single();

            Assert.IsTrue(proc.IsExamePossivelBoletimEmergencia);
        }

        [TestMethod]
        public void verifica_se_procedimento_54100054_eh_exame_possivel_para_boletins()
        {
            IRepositorioDeProcedimento rep = ObjectFactory.GetInstance<IRepositorioDeProcedimento>();
            Procedimento proc = rep.OndeIdIgual("54100054").Single();

            Assert.IsFalse(proc.IsExamePossivelBoletimEmergencia);
        }


        [TestMethod]
        public void testa_Atendimento_Com_DocumentosMV()
        {
            IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            wrpAtendimento atend = new wrpAtendimento(repAtend.OndeCodigoAtendimentoIgual(2904358).Single());

            Assert.IsTrue(atend.Documentos.Count > 0);

            Assert.IsNotNull(atend.Documentos.Select(x => x.Respostas));

        }

        //[TestMethod]
        //public void testa_vmBoletim_novo_boletim()
        //{
        //    IRepositorioDeAtendimento repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
        //    Atendimento atend = repAtend.OndeCodigoAtendimentoIgual(2713387).Single();
        //    Usuarios usuario = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

        //    vmBoletimEmergencia vmb = new vmBoletimEmergencia(atend, usuario);
            
        //    foreach (var item in vmb.vmItensDeRegistro.TiposAvaliacao)
        //    {
        //        if (item.ObrigatorioBoletim == SimNao.Sim)
        //        {                    
        //            vmb.vmItensDeRegistro.TipoAvaliacaoSelecionado = item;
        //            vmb.vmItensDeRegistro.TextoSelecionado = "TESTE";
        //            vmb.vmItensDeRegistro.AddBoletimAvaliacaoCommand.Execute(null);
        //        }
        //    }
        //    if (vmb.BoletimEmergencia.Classificacoes.Count == 0)
        //    {
        //        vmb.vmClassificacao.ClassificacaoSelecionada = vmb.vmClassificacao.ClassificacaoRiscoCores.First();
        //        vmb.vmClassificacao.AddClassificacaoRiscoCommand.Execute(null);
        //    }            
        //    vmCadastroAlta vmcadastroalta = new vmCadastroAlta(vmb);
        //    vmcadastroalta.DestinoSelecionado = CadastroAltaDestino.Domicilio;
        //    vmcadastroalta.Salva();

        //    vmb.NovoBoletim();

        //    foreach (var item in vmb.vmItensDeRegistro.TiposAvaliacao)
        //    {
        //        if (item.ObrigatorioBoletim == SimNao.Sim)
        //        {                    
        //            vmb.vmItensDeRegistro.TipoAvaliacaoSelecionado = item;
        //            vmb.vmItensDeRegistro.TextoSelecionado = "TESTE";
        //            vmb.vmItensDeRegistro.AddBoletimAvaliacaoCommand.Execute(null);
        //        }
        //    }

        //    vmb.SalvaBoletim();

        //    repAtend = null;
        //    repAtend = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
        //    atend = repAtend.OndeCodigoAtendimentoIgual(2713387).Single();
        //    usuario = ObjectFactory.GetInstance<IRepositorioDeUsuarios>().OndeCodigoIgual("H2555HO").Single();

        //    vmb = new vmBoletimEmergencia(atend, usuario);

        //    Assert.IsTrue(vmb.BoletimEmergencia.BoletimAvaliacao.Count > 0);
        //}

        //[TestMethod]
        //public void testa_consult_ListaPacientesEmergencia()
        //{
        //   IMinhaListaPacientesConsult consult = ObjectFactory.GetInstance<IMinhaListaPacientesConsult>();
        //    var ret = consult.ListaPacientesEmergencia(false);

        //    Assert.IsTrue(ret.Count > 0);
        //}


        [TestMethod]
        public void verifica_lista_de_destino_somente_ativos()
        {
            IRepositorioDeAltaDestino rep = ObjectFactory.GetInstance<IRepositorioDeAltaDestino>();
            wrpAltaDestinoCollection alt = new wrpAltaDestinoCollection(rep.List());
            Assert.IsTrue(alt.Count > 0);
        }
    }
}
