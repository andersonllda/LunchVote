using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP;
using System.Linq;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucTelaInicio2.xaml
    /// </summary>
    public partial class ucTelaInicio2 : UserControlBase, IUserControl
    {
        public ucTelaInicio2()
        {
            InitializeComponent();           
        }


        public bool CancelClose
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public void SetData(object pData)
        {
            this.DataContext = pData;
            this.ProcedimentosRealizados.SetData(new vmProcedimentosRealizados((this.DataContext as Atendimento).Paciente, true));
            this.gdListaProblemasAtivos.ItemsSource = (this.DataContext as Atendimento).Paciente.ProblemasPaciente.ToList();
            //this.gdListaProblemasInativos.ItemsSource = (this.DataContext as Atendimento).Paciente.ProblemasPaciente.Where(x => x.Status == Core.Domain.Enum.StatusAlergiaProblema.Inativo).ToList();
            this.gdAlergias.ItemsSource = new vmAlergias((pData as Atendimento).Paciente, App.Usuario, new GetSettings().IsCorpoClinico, pData as Atendimento).ListaAlergias;
            this.gdMedicamentos.ItemsSource = new vmMedicamentosEmUsoProntuario((pData as Atendimento).Paciente, App.Usuario).ListaMedicamentosEmUso;
        }
    }
}
