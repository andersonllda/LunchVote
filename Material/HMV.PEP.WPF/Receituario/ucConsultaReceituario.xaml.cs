using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using HMV.Core.Domain.Model;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.Receituario;
using HMV.PEP.WPF.Receituario.Report;
using HMV.Core.Framework.Extensions;
using DevExpress.Xpf.Core;

namespace HMV.PEP.WPF.Receituario
{
    /// <summary>
    /// Interaction logic for ucConsultaReceituario.xaml
    /// </summary>
    public partial class ucConsultaReceituario : UserControlBase, IUserControl
    {
        public ucConsultaReceituario()
        {
            InitializeComponent();           

        }
        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).TipoEspecial = false;
            (this.DataContext as vmReceituario).Novo();
            winCadReceituario win = new winCadReceituario(false, (this.DataContext as vmReceituario));
            win.ShowDialog(this.OwnerBase);
        }

        private void btnEspecial_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).TipoEspecial = true;
            (this.DataContext as vmReceituario).Novo();
            winCadReceituario win = new winCadReceituario(true, (this.DataContext as vmReceituario));
            win.ShowDialog(this.OwnerBase);
        }

        public bool CancelClose
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void SetData(object pData)
        {
            if (pData is Atendimento)
                this.DataContext = new vmReceituario((pData as Atendimento).Paciente, App.Usuario, (pData as Atendimento));
            else if (pData is Paciente)
                this.DataContext = new vmReceituario((pData as Paciente), App.Usuario, null);
        }

        private void btnCopiar_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as vmReceituario).Novo();
            (this.DataContext as vmReceituario).Copia();
            winCadReceituario win = new winCadReceituario(((this.DataContext as vmReceituario).Receituario.TipoReceituario == Core.Domain.Enum.TipoReceituario.Especial)
                , (this.DataContext as vmReceituario));
            win.ShowDialog(this.OwnerBase);
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as vmReceituario).Receituario.IsNull())
            {
                DXMessageBox.Show("Selecione um receituário.", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if ((this.DataContext as vmReceituario).Receituario.TipoReceituario == Core.Domain.Enum.TipoReceituario.Especial)
            {
                var relatorio = new rptReceituarioEspecial();
                relatorio.lblAssinatura.Text = App.Usuario.Prestador.NomeExibicao + "- CRM " + App.Usuario.Prestador.Registro + Environment.NewLine + (this.DataContext as vmReceituario).Receituario.Data.ToString("dd/MM/yyyy HH:mm");
                relatorio.lblCRM.Text = App.Usuario.Prestador.Registro.ToString();
                if (App.Usuario.Prestador.EnderecoProfissional.IsNotNull())
                {
                    relatorio.lblCidade.Text = App.Usuario.Prestador.EnderecoProfissional.Cidade.IsNotNull() ? App.Usuario.Prestador.EnderecoProfissional.Cidade.Descricao : string.Empty;
                    relatorio.lblEndereco.Text = (App.Usuario.Prestador.EnderecoProfissional.TipoLogradouro.IsNotNull() ? App.Usuario.Prestador.EnderecoProfissional.TipoLogradouro.DescricaoResumida : string.Empty) + " "
                                            + App.Usuario.Prestador.EnderecoProfissional.Logradouro + ", "
                                            + App.Usuario.Prestador.EnderecoProfissional.Numero + " "
                                            + App.Usuario.Prestador.EnderecoProfissional.Bairro;

                    relatorio.lbl2Cidade.Text = App.Usuario.Prestador.EnderecoProfissional.Cidade.IsNotNull() ? App.Usuario.Prestador.EnderecoProfissional.Cidade.Descricao : string.Empty;
                    relatorio.lbl2Endereco.Text = (App.Usuario.Prestador.EnderecoProfissional.TipoLogradouro.IsNotNull() ? App.Usuario.Prestador.EnderecoProfissional.TipoLogradouro.DescricaoResumida : string.Empty) + " "
                                            + App.Usuario.Prestador.EnderecoProfissional.Logradouro + ", "
                                            + App.Usuario.Prestador.EnderecoProfissional.Numero + " "
                                            + App.Usuario.Prestador.EnderecoProfissional.Bairro;
                }
                else if (App.Usuario.Prestador.EnderecoResidencial.IsNotNull())
                {
                    relatorio.lblCidade.Text = App.Usuario.Prestador.EnderecoResidencial.Cidade.IsNotNull() ? App.Usuario.Prestador.EnderecoResidencial.Cidade.Descricao : string.Empty; 
                    relatorio.lblEndereco.Text = (App.Usuario.Prestador.EnderecoResidencial.TipoLogradouro.IsNotNull() ? App.Usuario.Prestador.EnderecoResidencial.TipoLogradouro.DescricaoResumida : string.Empty) + " "
                                            + App.Usuario.Prestador.EnderecoResidencial.Logradouro + ", "
                                            + App.Usuario.Prestador.EnderecoResidencial.Numero + " "
                                            + App.Usuario.Prestador.EnderecoResidencial.Bairro;

                    relatorio.lbl2Cidade.Text = App.Usuario.Prestador.EnderecoResidencial.Cidade.IsNotNull() ? App.Usuario.Prestador.EnderecoResidencial.Cidade.Descricao : string.Empty;
                    relatorio.lbl2Endereco.Text = (App.Usuario.Prestador.EnderecoResidencial.TipoLogradouro.IsNotNull() ? App.Usuario.Prestador.EnderecoResidencial.TipoLogradouro.DescricaoResumida : string.Empty) + " "
                                            + App.Usuario.Prestador.EnderecoResidencial.Logradouro + ", "
                                            + App.Usuario.Prestador.EnderecoResidencial.Numero + " "
                                            + App.Usuario.Prestador.EnderecoResidencial.Bairro;
                }
                relatorio.lblEnderecoPaciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Endereco + ", " 
                                                    + (this.DataContext as vmReceituario).Receituario.Paciente.Numero + " " 
                                                    + (this.DataContext as vmReceituario).Receituario.Paciente.Complemento 
                                                    + ((this.DataContext as vmReceituario).Receituario.Paciente.Cidade.IsNotNull() ? " - " + (this.DataContext as vmReceituario).Receituario.Paciente.Cidade.Descricao : string.Empty);
                relatorio.lblNome.Text = App.Usuario.Prestador.NomeExibicao;
                relatorio.lblPaciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Nome;
                relatorio.lblPrescricao.Text = (this.DataContext as vmReceituario).Receituario.Descricao;
                relatorio.lblTelefone.Text = App.Usuario.Prestador.Celular + " " + App.Usuario.Prestador.Telefone1Profissional + " " + App.Usuario.Prestador.Telefone2Profissional;
                //2via
                relatorio.lbl2Assinatura.Text = App.Usuario.Prestador.NomeExibicao + "- CRM " + App.Usuario.Prestador.Registro + Environment.NewLine + (this.DataContext as vmReceituario).Receituario.Data.ToString("dd/MM/yyyy HH:mm");

                relatorio.lbl2CRM.Text = App.Usuario.Prestador.Registro.ToString();

                relatorio.lbl2EnderecoPaciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Endereco + ", "
                                                    + (this.DataContext as vmReceituario).Receituario.Paciente.Numero + " "
                                                    + (this.DataContext as vmReceituario).Receituario.Paciente.Complemento
                                                    + ((this.DataContext as vmReceituario).Receituario.Paciente.Cidade.IsNotNull() ? " - " + (this.DataContext as vmReceituario).Receituario.Paciente.Cidade.Descricao : string.Empty);
                relatorio.lbl2Nome.Text = App.Usuario.Prestador.NomeExibicao;
                relatorio.lbl2Paciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Nome;
                relatorio.lbl2Prescricao.Text = (this.DataContext as vmReceituario).Receituario.Descricao;
                relatorio.lbl2Telefone.Text = App.Usuario.Prestador.Celular + " " + App.Usuario.Prestador.Telefone1Profissional + " " + App.Usuario.Prestador.Telefone2Profissional;

                relatorio.ShowPreviewDialog();
                
            }
            else
            {
                var relatorio = new rptReceituarioNormal();
                relatorio.lblAssinatura.Text = App.Usuario.Prestador.NomeExibicao + "- CRM " + App.Usuario.Prestador.Registro + Environment.NewLine + (this.DataContext as vmReceituario).Receituario.Data.ToString("dd/MM/yyyy HH:mm");               
                relatorio.lblPaciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Nome;
                relatorio.lblPrescricao.Text = (this.DataContext as vmReceituario).Receituario.Descricao;                
                //2via
                relatorio.lbl2Assinatura.Text = App.Usuario.Prestador.NomeExibicao + "- CRM " + App.Usuario.Prestador.Registro + Environment.NewLine + (this.DataContext as vmReceituario).Receituario.Data.ToString("dd/MM/yyyy HH:mm");              
                relatorio.lbl2Paciente.Text = (this.DataContext as vmReceituario).Receituario.Paciente.Nome;
                relatorio.lbl2Prescricao.Text = (this.DataContext as vmReceituario).Receituario.Descricao;                

                relatorio.ShowPreviewDialog();
            }
        }
    }
}
