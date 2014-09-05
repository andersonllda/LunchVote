using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.Exception;
using HMV.PEP.Interfaces;
using NHibernate.Validator.Engine;
using StructureMap;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using System.Windows.Controls;
using HMV.Core.Domain.Repository;


namespace HMV.PEP.WPF.Cadastros
{
    /// <summary>
    /// Interaction logic for winInativarProblema.xaml
    /// </summary>
    public partial class winInativarProblema : WindowBase
    {
        //private Atendimento _atendimento;

        //public winCadListaProblema(Paciente pPaciente, Atendimento pAtendimento)
        //{
        //    InitializeComponent();

        //    this._atendimento = pAtendimento;

        //    var aa = App.Usuario;
        //    this.DataContext = new ProblemasPaciente
        //    {
        //        Paciente = pPaciente,
        //        Usuario = App.Usuario,
        //        DataInicio = DateTime.Today,
        //        Profissional = App.Usuario.Profissional
        //    };
        //}

        public winInativarProblema(ProblemasPaciente pProblema)
        {
            InitializeComponent();

            this.DataContext = pProblema;
            //pProblema.DataFim = DateTime.Today.Date;
            this.dtResolucao.EditValue = DateTime.Today.Date;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProblemasPaciente problema = (ProblemasPaciente)this.DataContext;

                Paciente pac = problema.Paciente;

                problema.Status = StatusAlergiaProblema.Inativo;

                pac.AlteraProblemasPaciente(problema);
            
                IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                srv.Salvar(pac);
            }
            catch (BusinessValidatorException ex)
            {
                DXMessageBox.Show(ex.GetErros()[0].Message, "Alerta", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void dtResolucao_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (dtResolucao.EditValue.IsNotNull())
            {
                (this.DataContext as ProblemasPaciente).DataFim = (DateTime)dtResolucao.EditValue;
            }
        }

    }
}
