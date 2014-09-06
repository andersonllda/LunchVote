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
using System.Collections;
using HMV.PEP.Test.SendEmail;
using HMV.Core.Domain.Model.PEP.ProcessoDeEnfermagem;
using HMV.Core.Framework.Helper;

namespace HMV.PEP.Test
{
    [TestClass]
    public class StringTest 
    {

        [TestMethod]
        public void teste_stringHelper_busca_descricao_do_objeto()
        {
            TipoHabitacao obj = new TipoHabitacao() { Descricao = "casa" };
            Assert.AreEqual(StringHelper.Append<TipoHabitacao>(obj, x => x.Descricao), "casa");
        }

        [TestMethod]
        public void teste_stringHelper_busca_descricao_do_objeto_nulo()
        {
            TipoHabitacao obj = null;
            Assert.AreEqual(StringHelper.Append<TipoHabitacao>(obj, x => x.Descricao), string.Empty);
        }


    }
}