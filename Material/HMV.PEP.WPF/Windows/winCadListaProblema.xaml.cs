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
    /// Interaction logic for winCadListaProblema.xaml
    /// </summary>
    public partial class winCadListaProblema : WindowBase
    {
        private Atendimento _atendimento;

        public winCadListaProblema(Paciente pPaciente, Atendimento pAtendimento)
        {
            InitializeComponent();

            this._atendimento = pAtendimento;

            var aa = App.Usuario;
            this.DataContext = new ProblemasPaciente
            {
                Paciente = pPaciente,
                Usuario = App.Usuario,
                DataInicio = DateTime.Today,
                Profissional = App.Usuario.Profissional
            };
        }

        public winCadListaProblema(ProblemasPaciente pProblema, Atendimento pAtendimento)
        {
            InitializeComponent();

            this._atendimento = pAtendimento;

            bEditCID.IsReadOnly = true;
            bEditCID.IsEnabled = false;
            txtDescricaoCID.IsEnabled = false;
            dtInicio.IsEnabled = false;

            this.DataContext = pProblema;
            chkInativo.IsChecked = pProblema.Status == StatusAlergiaProblema.Inativo ? true : false;

            if ((this.DataContext as ProblemasPaciente).Comentario.IsNotEmptyOrWhiteSpace())
            {
                rdCid.IsChecked = false;
                rdEvento.IsChecked = true;
            }
        }

        public bool ObrigarCIDeOcultarEventosRelevantes
        {
            set
            {
                txtEventoRelevante.IsEnabled = !value;                
            }
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void bEditCID_EditValueChanging(object sender, EditValueChangingEventArgs e)
        {
            try
            {
                Cid cid;
                if (e.NewValue.IsNotNull())
                {
                    if (bEditCID.IsEnabled)
                    {
                        ICidService serv = ObjectFactory.GetInstance<ICidService>();
                        cid = serv.FiltraPorCid10(e.NewValue.ToString());
                    }
                    else
                    {
                        IRepositorioDeCid rep = ObjectFactory.GetInstance<IRepositorioDeCid>();
                        cid = rep.OndeCid10Igual(e.NewValue.ToString()).Single();
                    }

                }
                else
                    cid = null;

                ProblemasPaciente prob = (this.DataContext as ProblemasPaciente);
                this.DataContext = null;
                prob.CID = cid;
                this.DataContext = prob;
            }
            catch (BusinessValidatorException err)
            {
                IList<InvalidValue> erros = err.GetErros();
                DXMessageBox.Show(erros[0].Message);
            }
        }

        private void btnGravar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProblemasPaciente problema = (ProblemasPaciente)this.DataContext;
                problema.Atendimento = _atendimento;
                if (problema.CID.IsNull() && problema.Comentario.IsEmptyOrWhiteSpace())
                {
                    DXMessageBox.Show("Selecione ou digite um CID válido ou preencha os Eventos Relevantes!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Paciente pac = problema.Paciente;

                if (problema.DataFim != null)
                {
                    if (problema.DataFim < problema.DataInicio || problema.DataFim > DateTime.Now)
                    {
                        if (problema.DataInicio.ToString("d") == DateTime.Now.ToString("d"))
                            DXMessageBox.Show("'Data de Resolução' só pode ser " + problema.DataInicio.ToString("d"), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                            DXMessageBox.Show(string.Format("'Data de Resolução' tem que estar entre {0} - {1}", problema.DataInicio.ToString("d"), DateTime.Now.ToString("d")), "Atenção", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                if (problema.ID == 0)
                {
                    if (problema.CID.IsNotNull() && pac.ProblemasPaciente.Count(x => x.CID != null && x.CID.Id == problema.CID.Id) > 0)
                    {
                        if (!this._atendimento.IsNull() && this._atendimento.Cids().Count(x => x.Id == problema.CID.Id) > 0)
                        {
                            DXMessageBox.Show("Não é permitido adicionar este 'CID', pois já existe no atendimento!", "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        if (DXMessageBox.Show("CID já existente para o paciente. Deseja incluí-lo mesmo assim?", "Atenção:", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                        {
                            return;
                        }
                    }
                    pac.AddProblemasPaciente(problema);
                }
                else
                {
                    pac.AlteraProblemasPaciente(problema);
                }

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

        private void ButtonInfo_Click(object sender, RoutedEventArgs e)
        {
            winSelCID win = new winSelCID(false);
            //win.txtCodigoCid.Text = bEditCID.Text;
            win.ShowDialog(this);
            if (win.CID != null)
                bEditCID.Text = win.CID.Id.ToString();
        }

        private void chkInativo_Checked(object sender, RoutedEventArgs e)
        {
            (this.DataContext as ProblemasPaciente).Status = StatusAlergiaProblema.Ativo;
            if (chkInativo.IsChecked.HasValue)
                if (chkInativo.IsChecked.Value)
                {
                    (this.DataContext as ProblemasPaciente).Status = StatusAlergiaProblema.Inativo;
                    if (!nedita2)
                    {
                        (this.DataContext as ProblemasPaciente).DataFim = DateTime.Today.Date;
                        dtResolucao.EditValue = DateTime.Today.Date;
                    }
                }
        }

        bool nedita;
        bool nedita2;
        private void chkInativo_Unchecked(object sender, RoutedEventArgs e)        
        {
            if (this.DataContext.IsNull())
                return;
            (this.DataContext as ProblemasPaciente).Status = StatusAlergiaProblema.Ativo;
            (this.DataContext as ProblemasPaciente).DataFim = null;
            nedita = true;
            dtResolucao.Clear();
            nedita = false;
        }

        private void bEditCID_LostFocus(object sender, RoutedEventArgs e)
        {
            ProblemasPaciente problema = (ProblemasPaciente)this.DataContext;
            if (!bEditCID.Text.IsEmptyOrWhiteSpace() && problema.CID.IsNull())
            {
                ButtonInfo_Click(this, null);
            }
        }

        private void dtResolucao_EditValueChanged(object sender, EditValueChangedEventArgs e)
        {
            if (dtResolucao.EditValue.IsNotNull() && !nedita)
            {
                (this.DataContext as ProblemasPaciente).Status = StatusAlergiaProblema.Inativo;
                nedita2 = true;
                chkInativo.IsChecked = true;
                nedita2 = false;
            }
            else
            {
                chkInativo.IsChecked = false;
            }
        }

        private void rdCid_Checked(object sender, RoutedEventArgs e)
        {
            if (this.DataContext.IsNotNull())
            {
                if (rdCid.IsChecked == true)
                {
                    (this.DataContext as ProblemasPaciente).Comentario = string.Empty;
                    txtEventoRelevante.Text = string.Empty;
                }
                else
                {
                    (this.DataContext as ProblemasPaciente).CID = null;
                    bEditCID.Text = string.Empty;
                    (this.DataContext as ProblemasPaciente).Descricao = string.Empty;
                    txtComplemento.Text = string.Empty;
                    (this.DataContext as ProblemasPaciente).DataFim = null;
                    dtInicio.Clear();
                    dtInicio.EditValue = DateTime.Today.Date;
                    dtResolucao.Text = string.Empty;
                    dtResolucao.Clear();
                    chkInativo.IsChecked = false;
                }
            }
        }


    }
}
