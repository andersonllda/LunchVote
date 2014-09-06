using Microsoft.VisualStudio.TestTools.UnitTesting;
using HMV.Core.Domain.Repository.ClassificacaoPaciente;
using StructureMap;
using System.Collections.Generic;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem.ClassificacaoPaciente;
using System.Linq;
using HMV.Core.Domain.Repository;
using HMV.Core.Domain.Model;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Wrappers.CollectionWrappers;

namespace HMV.PEP.Test
{
    [TestClass]
    public class ProcessoEnfermagemTest : BaseTestClass
    {
        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        #region ----------- SCP -----------
        [TestMethod]
        public void busca_tipos_avaliacao_SCP()
        {
            IRepositorioDeTipoAvaliacaoSCP rep = ObjectFactory.GetInstance<IRepositorioDeTipoAvaliacaoSCP>();
            Assert.AreEqual(rep.List().Count, 48);
        }

        [TestMethod]
        public void busca_somente_grupos_tipos_avaliacao_SCP()
        {
            IRepositorioDeTipoAvaliacaoSCP rep = ObjectFactory.GetInstance<IRepositorioDeTipoAvaliacaoSCP>();
            IList<TipoAvaliacaoPaciente> lst = (rep.List());
            var x = (from z in lst
                     select z.Descricao).Distinct();
            Assert.AreEqual(x.Count(), 12);

            wrpTipoAvaliacaoPacienteCollection wrp = new wrpTipoAvaliacaoPacienteCollection(rep.List().Select(t => t).ToList());
            var r = (from T in wrp
                     select T.Descricao).Distinct();
            Assert.AreEqual(r.Count(), 12);
        }

        [TestMethod]
        public void insere_SCP_sem_itens_atendimento_1300073()
        {
            IRepositorioDeSCP rep = ObjectFactory.GetInstance<IRepositorioDeSCP>();
            SCP scp = new SCP(ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1300073).Single(), ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeUsuarios>().OndeCodigoIgual("M9700RS").Single());
            rep.Save(scp);
            Assert.IsTrue(scp.ID > 0);
        }

        [TestMethod]
        public void insere_SCP_com_itens_atendimento_2969090()
        {
            IRepositorioDeSCP rep = ObjectFactory.GetInstance<IRepositorioDeSCP>();
            wrpSCPCollection wrp = new wrpSCPCollection(rep.List().Select(t => t).ToList());
            wrpSCP wr = new wrpSCP(new SCP(ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(2969090).Single(), ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeUsuarios>().OndeCodigoIgual("M9700RS").Single()));
            wr.Itens.Add(new wrpClassificacaoDePacientesItens(new ClassificacaoDePacientesItens(wr.DomainObject, new TipoAvaliacaoPaciente() { ID = 1, IDNota = 3, Nota = 3 })));
            wr.Save();
            wrpSCPCollection wrptest = new wrpSCPCollection(rep.List().Select(t => t).ToList());
            Assert.AreEqual(wrptest.ToList().Count(), wrp.ToList().Count() + 1);
        }

        [TestMethod]
        public void busca_itens_scp_30()
        {
            IRepositorioDeSCP rep = ObjectFactory.GetInstance<IRepositorioDeSCP>();
            SCP scp = rep.FiltraPorID(30).Single();
            Assert.AreEqual(scp.Itens.Count, 12);
        }

        [TestMethod]
        public void busca_scp_por_atendimento_1300073()
        {
            IRepositorioDeSCP rep = ObjectFactory.GetInstance<IRepositorioDeSCP>();
            IList<SCP> scp = rep.FiltraPorAtendimento(1300073).List();
            Assert.IsTrue(scp.Count > 0);
        }

        [TestMethod]
        public void busca_atendimento_2086793_e_lista_SCP()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = rep.OndeCodigoAtendimentoIgual(2086793).Single();
            Assert.AreEqual(atend.SCP.Count, 2);
        }

        //[TestMethod]
        //public void scp_10713_delete_itens()
        //{
        //    IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
        //    wrpAtendimento atend = new wrpAtendimento(rep.OndeCodigoAtendimentoIgual(1376557).Single());
        //    wrpSCP _SCPSelecionado = new wrpSCP(new Core.Domain.Model.PEP.ProcessoDeEnfermagem.ClassificacaoPaciente.SCP(atend.DomainObject, ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeUsuarios>().OndeCodigoIgual("M9700RS").Single()));
            
        //    IRepositorioDeSCP res = ObjectFactory.GetInstance<IRepositorioDeSCP>();
        //    SCP scp = res.FiltraPorID(413).Single();

        //    atend.DomainObject.SCP.Remove(scp);
        //    res.Delete(scp);
        //}

        //[TestMethod]
        //public void deleta_scp_do_atendimento_1376557()
        //{
        //    IRepositorioDeSCP repscp = ObjectFactory.GetInstance<IRepositorioDeSCP>();
        //    SCP scp = repscp.FiltraPorID(2326).Single();
        //    repscp.Delete(scp);
        //    Assert.IsNull(repscp.FiltraPorID(10736).List().FirstOrDefault());
        //}

        //[TestMethod]
        //public void deleta_scp_12_itens()
        //{
        //    IRepositorioDeSCP repscp = ObjectFactory.GetInstance<IRepositorioDeSCP>();
        //    SCP scp = repscp.FiltraPorID(12).Single();
        //    scp.Itens.Clear();
        //    repscp.Save(scp);
        //    Assert.AreEqual(scp.Itens.Count, 0);        
        //}
#endregion

        #region ----------------- NAS --------------

        [TestMethod]
        public void insere_nas_com_itens_atendimento_1300073()
        {
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = rep.OndeCodigoAtendimentoIgual(1300073).Single();
            NAS nas = new NAS(atend, ObjectFactory.GetInstance<Core.Domain.Repository.IRepositorioDeUsuarios>().OndeCodigoIgual("M9700RS").Single());
            nas.Itens.Add(new ClassificacaoDePacientesItens(nas, new TipoAvaliacaoPaciente() { ID = 23, IDNota = 1, Nota = 1.4 }));
            atend.NAS.Add(nas);
            rep.Save(atend);
            Assert.IsTrue(nas.ID > 0);
        }

        [TestMethod]
        public void remove_nas_do_atendimento_1300073()
        {

            insere_nas_com_itens_atendimento_1300073();
            IRepositorioDeAtendimento rep = ObjectFactory.GetInstance<IRepositorioDeAtendimento>();
            Atendimento atend = rep.OndeCodigoAtendimentoIgual(1300073).Single();

            NAS nas = atend.NAS.Select(z=>z).FirstOrDefault();
            if (nas !=null)
                atend.NAS.Remove(nas);

            rep.Save(atend);

            Assert.IsTrue(nas.ID > 0);
        }




        #endregion
    }
}
