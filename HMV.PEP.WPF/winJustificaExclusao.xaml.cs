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
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.PEP.Interfaces;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winJustificaExclusao.xaml
    /// </summary>
    public partial class winJustificaExclusao : WindowBase
    {
        Paciente paciente;
        ProblemasPaciente problema;
        Alergia alergia;

        public winJustificaExclusao(ProblemasPaciente pProblema)
        {
            InitializeComponent();

            paciente = pProblema.Paciente;
            problema = pProblema;
        }

        public winJustificaExclusao(Alergia pAlergia)
        {
            InitializeComponent();

            paciente = pAlergia.Paciente;
            alergia = pAlergia;
        }       

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnExcluir_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJustificativa.Text))
                return;

            Comentario comentario = new Comentario(App.Usuario, "EXCLUSÃO: " + txtJustificativa.Text);

            if (problema != null)
            {
                problema.addComentario(comentario);

                paciente.RemoveProblemasPaciente(problema);

                IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                srv.Salvar(paciente);
            }

            if (alergia != null)
            {
                alergia.addComentario(comentario);

                paciente.RemoveAlergia(alergia);

                IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                srv.Salvar(paciente);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void txtJustificativa_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            try { label1.Content = string.Format(txtJustificativa.Text.Length < 127 ? "Máximo {0} caracteres" : "Máximo {0} caracter", 128 - txtJustificativa.Text.Length); }
            catch { }
        }
    }
}
