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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.NavBar;
using HMV.Core.Domain.Enum;
using HMV.Core.Interfaces;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.PEP.WPF.Report;
using DevExpress.Xpf.Printing;
using HMV.PEP.DTO;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for ucDadosPessoais.xaml
    /// </summary>
    public partial class ucDadosPessoais : UserControlBase, IUserControl
    {
        public bool CancelClose { get; set; }  

        public ucDadosPessoais()
        {
            InitializeComponent();  
        }

        public void SetData(object pData)
        {
            if (typeof(Atendimento) == pData.GetType() || typeof(Atendimento) == pData.GetType().BaseType)
            {
                this.DataContext = (pData as Atendimento).Paciente;
            }
            else if (typeof(Paciente) == pData.GetType() || typeof(Paciente) == pData.GetType().BaseType)
            {
                this.DataContext = (pData as Paciente);
            }
            else if (typeof(SumarioAvaliacaoMedica) == pData.GetType() || typeof(SumarioAvaliacaoMedica) == pData.GetType().BaseType)
            {
                this.DataContext = (pData as SumarioAvaliacaoMedica).Paciente;
            }

            imprimir();
        }

        public void imprimir()
        {                        
            Paciente paciente = (this.DataContext as Paciente);

            List<DadosPessoaisPacienteDTO> dadosPaciente = new List<DadosPessoaisPacienteDTO>();

            dadosPaciente.Add(new DadosPessoaisPacienteDTO()
            {    
               /******DADOS PACIENTE******/
                ID = paciente.ID
              , Nome = paciente.Nome
              , CodeNome = paciente.NomeUsual == null ? "SEM INFORMAÇÃO" : paciente.NomeUsual
              , Conjuge = paciente.Conjuge == null ? "SEM INFORMAÇÃO" : paciente.Conjuge
              , DataNascimento = paciente.DataNascimento
              , Idade = paciente.Idade.GetDate()
              , CPF = paciente.CPF == null ? "SEM INFORMAÇÃO" : paciente.CPF
              , Sexo = paciente.Sexo.ToString().ToUpper()
              , Identidade = paciente.Identidade == null ? "SEM INFORMAÇÃO" : paciente.Identidade
              , OrgaoEmissor = paciente.OrgaoEmissor == null ? "SEM INFORMAÇÃO" : paciente.OrgaoEmissor
              , Escolaridade = paciente.Escolaridade == null ? "SEM INFORMAÇÃO" : paciente.Escolaridade.Descricao.ToUpper()
              , EstadoCivil = paciente.EstadoCivil.ToString().ToUpper()
              , Profissao = (paciente.Profissao== null) ? "SEM INFORMAÇÃO" : paciente.Profissao.Descricao.ToUpper()
              , Cor = paciente.Cor == null ? "SEM INFORMAÇÃO" : paciente.Cor.ToString().ToUpper()
              , Religiao = paciente.Religiao == null ?"SEM INFORMAÇÃO" : paciente.Religiao.Descricao
              , Etnia = paciente.Etnia == null ? "SEM INFORMAÇÃO" : paciente.Etnia.Descricao.ToString() 
              , NomeDaMae = paciente.NomeMae == null ? "SEM INFORMAÇÃO" : paciente.NomeMae

              /******ENDERECO PACIENTE******/
              , Endereco = paciente.Endereco == null ? "SEM INFORMAÇÃO" : paciente.Endereco
              , NumeroEndereco = paciente.Numero
              , Complemento = paciente.Complemento == null ? "SEM INFORMAÇÃO" : paciente.Complemento
              , Bairro = paciente.Bairro == null ? "SEM INFORMAÇÃO" : paciente.Bairro
              , Cidade = paciente.Cidade == null ? "SEM INFORMAÇÃO" : paciente.Cidade.Descricao
              , Estado = paciente.Cidade == null ? "SEM INFORMAÇÃO" : paciente.Cidade.Estado.Descricao
              , Pais =  paciente.Cidade == null ? "SEM INFORMAÇÃO" : paciente.Cidade.Estado.Pais.Descricao
              , CEP = paciente.CEP == null ? "SEM INFORMAÇÃO" : paciente.CEP

              /******CONTATO PACIENTE******/
              , Celular = (paciente.DDDCelular == null ? "" : paciente.DDDCelular.PadLeft(3,'0').Substring(0,3)) + (paciente.Celular == null ? "SEM INFORMAÇÃO" : paciente.Celular)
              , Fone = (paciente.DDDTelefone == null ? "" : paciente.DDDTelefone.PadLeft(3,'0').Substring(0,3)) + (paciente.Telefone == null ? "SEM INFORMAÇÃO" : paciente.Telefone)
              //, FoneUrgencia = paciente
              , TelefoneComercial = (paciente.DDDComercial == null ? "" : paciente.DDDComercial.PadLeft(3, '0').Substring(0, 3)) + (paciente.Comercial == null ? "SEM INFORMAÇÃO" : paciente.Comercial)
              , Email = paciente.Email == null ? "SEM INFORMAÇÃO" : paciente.Email

              /******PRIMEIRO CONTATO******/
              //, CartaoSUS = "12345"
              , NomeDoPrimeiroContato = paciente.ContatoNome == null ? "SEM INFORMAÇÃO" : paciente.ContatoNome
              , ParentescoDoPrimeiroContato ="SEM INFORMAÇÃO"
              , EnderecoDoPrimeiroContato = paciente.ContatoEndereco == null ? "SEM INFORMAÇÃO" : paciente.ContatoEndereco
              , ComplementoDoPrimeiroContato = paciente.ContatoComplemento == null ? "SEM INFORMAÇÃO" : paciente.ContatoComplemento
              , NumeroDoPrimeiroContato = paciente.ContatoNumero == null ? 0 : paciente.ContatoNumero
              , BairroDoPrimeiroContato = paciente.ContatoBairro == null ? "SEM INFORMAÇÃO" : paciente.ContatoBairro
              , CidadeDoPrimeiroContato = paciente.ContatoCidade == null ? "SEM INFORMAÇÃO" : paciente.ContatoCidade
              , PaisDoPrimeiroContato = paciente.ContatoPais == null ? "SEM INFORMAÇÃO" : paciente.ContatoPais
              , CEPDoPrimeiroContato = paciente.ContatoCEP == null ? "SEM INFORMAÇÃO" : paciente.ContatoCEP 
              , FoneDoPrimeiroContato = paciente.ContatoFone == null ? "SEM INFORMAÇÃO" : paciente.ContatoFone
              //, Companheiro = 'S'              
              //, DescricaoDoConvenio = "UNIMED"
              //, DescricaoUrgencia = "SEM DESCRICAO"                               
           });

            rptDadosPessoaisPaciente report = new rptDadosPessoaisPaciente();
            report.DataSource = dadosPaciente;

            XtraReportPreviewModel model = new XtraReportPreviewModel(report);
            rDocumentoDadosPessoais.Model = model;
            report.CreateDocument(false);
        }         
    }
}
