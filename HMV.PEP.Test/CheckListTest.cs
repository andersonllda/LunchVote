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
using HMV.Core.Domain.Enum;
using HMV.PEP.DTO;
using HMV.Core.DTO;
using HMV.PEP.Consult;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Domain.Model.PEP.CheckListCirurgia;

namespace HMV.PEP.Test
{
    [TestClass]
    public class CheckListTest : BaseTestClass
    {

        [ClassInitialize]
        public static void MyTestInitialize(TestContext testContext)
        {
            BaseTestClass.BaseTestInitialize(testContext);
        }

        [TestMethod]
        public void insere_checklist()
        {
            IRepositorioDeCheckList rep = ObjectFactory.GetInstance<IRepositorioDeCheckList>();
            CheckListCirurgia check = new CheckListCirurgia(BaseTestClass.Usuario);
            check.Atendimento = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1302310).Single();
            check.AvisoCirurgia = check.Atendimento.DescricaoCirurgica.FirstOrDefault();
            check.Cirurgia = check.AvisoCirurgia.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia;
            check.Prestador = check.AvisoCirurgia.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador;
            check.Paciente = check.Atendimento.Paciente;
            check.AntesEntradaPaciente = new AntesEntradaPaciente(BaseTestClass.Usuario);

            rep.Save(check);
            Assert.IsTrue(check.ID > 0);
        }

        [TestMethod]
        public void insere_checklist_com_antes_entrada_paciente()
        {
            IRepositorioDeCheckList rep = ObjectFactory.GetInstance<IRepositorioDeCheckList>();
            CheckListCirurgia check = new CheckListCirurgia(BaseTestClass.Usuario);
            check.Atendimento = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1302310).Single();
            check.AvisoCirurgia = check.Atendimento.DescricaoCirurgica.FirstOrDefault();
            check.Cirurgia = check.AvisoCirurgia.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia;
            check.Prestador = check.AvisoCirurgia.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador;
            check.Paciente = check.Atendimento.Paciente;
            check.AntesEntradaPaciente = new AntesEntradaPaciente(BaseTestClass.Usuario);
            check.AntesEntradaPaciente.AvaliacaoPreAnestesica = SimNao.Sim;
            check.AntesEntradaPaciente.AvaliacaoPreAnestesicaObservacao = "Teste";
            check.AntesEntradaPaciente.CheckList = check;
            rep.Save(check);
            Assert.IsTrue(check.ID > 0);
        }

        [TestMethod]
        public void insere_checklist_com_trans_operatorio()
        {
            IRepositorioDeCheckList rep = ObjectFactory.GetInstance<IRepositorioDeCheckList>();
            CheckListCirurgia check = new CheckListCirurgia(BaseTestClass.Usuario);
            check.Atendimento = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(1302310).Single();
            check.AvisoCirurgia = check.Atendimento.DescricaoCirurgica.FirstOrDefault();
            check.Cirurgia = check.AvisoCirurgia.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia;
            check.Prestador = check.AvisoCirurgia.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador;
            check.Paciente = check.Atendimento.Paciente;
            check.TransOperatorio = new TransOperatorio(BaseTestClass.Usuario);
            
            check.AntesEntradaPaciente.CheckList = check;
            rep.Save(check);
            Assert.IsTrue(check.ID > 0);
                      
        }


        [TestMethod]
        public void sss()
        {
            IRepositorioDeCheckList rep = ObjectFactory.GetInstance<IRepositorioDeCheckList>();
            CheckListCirurgia check = new CheckListCirurgia(BaseTestClass.Usuario);
            check.Atendimento = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3869656).List().FirstOrDefault();
            check.AvisoCirurgia = check.Atendimento.DescricaoCirurgica.FirstOrDefault();
            check.Cirurgia = check.AvisoCirurgia.ProcedimentosCirurgicos.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Cirurgia;
            check.Prestador = check.AvisoCirurgia.EquipesMedicas.Where(x => x.Principal.Equals(SimNao.Sim)).FirstOrDefault().Prestador;
            check.Paciente = check.Atendimento.Paciente;
            check.TransOperatorio = new TransOperatorio(BaseTestClass.Usuario);
           // check.TransOperatorio.Assepsia.CloroTopico = SimNao.Nao;
            check.TransOperatorio.CheckList = check;
            rep.Save(check);
            Assert.IsTrue(check.ID > 0);

        }


        [TestMethod]
        public void fuck()
        {
            
            Atendimento ate = ObjectFactory.GetInstance<IRepositorioDeAtendimento>().OndeCodigoAtendimentoIgual(3869656).List().FirstOrDefault();
            Assert.IsNotNull(ate.DescricaoCirurgica.FirstOrDefault().CheckList);

        }
    }
}
