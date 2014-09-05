using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Domain.Repository;
using StructureMap;
using System.Collections.Generic;
using HMV.Core.Domain.Views.PEP;
using System.Linq;
using System;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winMensagemSumarioNaoIniciado.xaml
    /// </summary>
    public partial class winSelecionaAtendimentoGED : WindowBase
    {
        public int IdAtendimento { get; set; }
        private int idPaciente;
        
        public winSelecionaAtendimentoGED(int pPaciente)
        {
            InitializeComponent();
            idPaciente = pPaciente;
        }

        private void carregaAtendimentos()
        {
            IRepositoriovAtendimentoGED rep = ObjectFactory.GetInstance<IRepositoriovAtendimentoGED>();
            IList<vAtendimentoGED> ret = rep.OndeCodigoPacienteIgual(idPaciente).List();

            if (ret.Count == 1)
            {
                this.IdAtendimento = ret.FirstOrDefault().Atendimento.ID;
                this.DialogResult = true;
                this.Close();
            }

            List<Lista> _lista = new List<Lista>();
            foreach (var item in ret)
            {
                _lista.Add(new Lista { Id = item.Id, DataAtendimento = item.Atendimento.DataAtendimento });
            }
            gridAtendimentos.ItemsSource = _lista; 
        }

        private void btnFechar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnSelecionar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.IdAtendimento = (gridAtendimentos.GetFocusedRow() as Lista).Id;
            this.DialogResult = true;
            this.Close();
        }

        private void WindowBase_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            carregaAtendimentos();
        }

        public class Lista
        {
            public int Id {get;set;}
            public DateTime DataAtendimento { get; set; }
        }
    }
}
