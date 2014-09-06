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
using System.Windows.Navigation;
using System.Windows.Shapes;
using StructureMap;
using HMV.Core.Domain.Model;
using HMV.Core.DataAccess;
using HMV.PEP.WPF.Cadastros;
using HMV.PEP.Interfaces;
using HMV.Core.Framework.Exception;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.WPF;


namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winProblemaComentarios.xaml
    /// </summary>
    public partial class winComentarios : WindowBase
    {
        public ProblemasPaciente problema;
        public Alergia alergia;
        public string comentario;

        public winComentarios(ProblemasPaciente pProblema)
        {
            InitializeComponent();

            this.DataContext = this;

            problema = pProblema;
            Carrega();
        }

        public winComentarios(Alergia pAlergia)
        {
            InitializeComponent();

            this.DataContext = this;

            alergia = pAlergia;
            Carrega();
        }

        public string NomeTela
        {
            get
            {
                if (problema != null)
                    return "Comentários do Problema";
                return "Comentários da Alergia";
            }
        }

        private void Carrega()
        {
            if (problema != null) comentario = problema.Comentario;
            if (alergia != null) comentario = alergia.Comentario;

            if (comentario != null)
                txtComentario.Text = comentario;
            else
                txtComentario.Text = "Nenhum comentário cadastrado.";
        }

        private void btnIncluir_Click(object sender, RoutedEventArgs e)
        {

            Comentario comentario = new Comentario(App.Usuario, txtDescricao.Text);

            if (problema != null)
            {
                try
                {
                    problema.addComentario(comentario);
                    Paciente paciente = problema.Paciente;

                    IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                    srv.Salvar(paciente);
                }
                catch (BusinessValidatorException bv) { DXMessageBox.Show(bv.Message, "Atenção", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            }

            if (alergia != null)
            {
                alergia.addComentario(comentario);
                Paciente paciente = alergia.Paciente;

                IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                srv.Salvar(paciente);
            }

            txtDescricao.Text = string.Empty;
            txtDescricao.Focus();

            Carrega();

        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void txtComentario_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            MaximoCaracter();
        }

        private void txtDescricao_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            MaximoCaracter();
        }

        private void MaximoCaracter()
        {            
            if (string.IsNullOrWhiteSpace(txtDescricao.Text.Trim()))
            {
                txtDescricao.IsEnabled = true;
                int t = (4000 - (txtDescricao.Text.Trim().Length + (txtComentario.Text.Replace("Nenhum comentário cadastrado.", "").Length + App.Usuario.nm_usuario.Length + 24)));
                if (txtDescricao.MaxLength.Equals(0))
                    txtDescricao.IsEnabled = false;
                    txtDescricao.MaxLength = t < 1 ? 0 : t;
            }

            label1.Content = string.Format("Máximo {0} caracter", (txtDescricao.MaxLength == 0 ? txtDescricao.MaxLength : txtDescricao.MaxLength - txtDescricao.Text.Length));
        }
    }
}
