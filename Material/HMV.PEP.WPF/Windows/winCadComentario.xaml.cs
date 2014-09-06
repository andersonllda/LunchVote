using System.Collections.Generic;
using System.Windows;
using HMV.Core.Domain.Model;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Cadastros
{
    public partial class winCadComentario : WindowBase
    {
        ProblemasPaciente problema;
        Alergia alergia;

        public winCadComentario(ProblemasPaciente pProblema)
        {
            InitializeComponent();
            problema = pProblema;
        }

        public winCadComentario(Alergia pAlergia)
        {
            InitializeComponent();
            alergia = pAlergia;
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (txtDescricao.Text != null)
            {
                Comentario comentario = new Comentario(App.Usuario, txtDescricao.Text);

                if (problema != null)
                {
                    problema.addComentario(comentario);
                    Paciente paciente = problema.Paciente;

                    IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                    srv.Salvar(paciente);
                }

                if(alergia != null)
                {
                    alergia.addComentario(comentario);
                    Paciente paciente = alergia.Paciente;

                    IPacienteService srv = ObjectFactory.GetInstance<IPacienteService>();
                    srv.Salvar(paciente);
                }

                
                this.DialogResult = true;
                this.Close();
            }   
        }
    }
}
