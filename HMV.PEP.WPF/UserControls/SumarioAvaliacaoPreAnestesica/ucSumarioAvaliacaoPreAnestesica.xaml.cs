using System.Linq;
using System.Windows.Data;
using DevExpress.Xpf.LayoutControl;
using HMV.Core.Domain.Enum;
using HMV.Core.Framework.ViewModelBaseClasses;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.SumarioAvaliacaoPreAnestesica;
using DevExpress.Xpf.Printing;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.SumarioDeAvaliacaoPreAnestesica;
using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.Core.Framework.Helper;
using DevExpress.Xpf.Core;
using HMV.Core.Framework.DevExpress.v12._1.Extensions;


namespace HMV.PEP.WPF.UserControls.SumarioAvaliacaoPreAnestesica
{
    /// <summary>
    /// Interaction logic for ucSumarioAvaliacaoPreAnestesica.xaml
    /// </summary>
    public partial class ucSumarioAvaliacaoPreAnestesica : UserControlBase, IUserControl
    {

        public event EventHandler ExecuteMethod;

        protected virtual void OnExecuteMethod()
        {
            if (ExecuteMethod != null) ExecuteMethod(this, EventArgs.Empty);
        }

        public ucSumarioAvaliacaoPreAnestesica()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmSumarioAvaliacaoPreAnestesica);
            base.SetData(this.DataContext as ViewModelBase);            

            this.SumarioAvaliacaoPreAnestesicaCabecalho.SetData((pData as vmSumarioAvaliacaoPreAnestesica).vmSumarioAvaliacaoPreAnestesicaCabecalho);

            if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica.DataEmissao.IsNotNull())
            {
                Finalizar();
                return;
            }

            groupContainer.Children.Clear();
            var grupos = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status == Status.Ativo).ToList().OrderBy(x => x.OrdemTela);
            foreach (var group in grupos)
            {
                int d = groupContainer.Children.Count;

                Grid pl = new Grid();
                pl.Tag = d;
                pl.DataContext = group;
                pl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                Binding binding = new Binding("Descricao");
                binding.Source = group;
                BindingOperations.SetBinding(pl, LayoutControl.TabHeaderProperty, binding);

                groupContainer.Children.Add(pl);
            }

            //base.TelaAbrirType = new TelaType(typeof(winImportaDados));
        }

        public bool CancelClose { get; set; }
        private vmSumarioAvaliacaoPreAnestesicaItem _tabanterior;

        ucSumarioAvaliacaoPreAnestesicaItens uc;
        private void groupContainer_SelectedTabChildChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<System.Windows.FrameworkElement> e)
        {
            if (this.DataContext.IsNull())
                return;

            UIHelper.SetBusyState();
            var pl = (Grid)e.NewValue;

            if (pl.Children.Count == 0)
            {
                uc = new ucSumarioAvaliacaoPreAnestesicaItens();
                uc.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                uc.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                uc.MaxHeight = 5000;
                uc.SetData(new vmSumarioAvaliacaoPreAnestesicaItem((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica, (wrpAnestesiaGrupo)pl.DataContext
                    , new GetSettings().IsCorpoClinico, (int)pl.Tag
                    , (this.DataContext as vmSumarioAvaliacaoPreAnestesica).Novo
                    , (this.DataContext as vmSumarioAvaliacaoPreAnestesica).Paciente
                    , (this.DataContext as vmSumarioAvaliacaoPreAnestesica).ImportaAdmissaoNovamente)); //(this.DataContext as vmSumarioAvaliacaoPreAnestesica).EventoAnestesia

                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).VMItens.Add((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem));

                if ((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).AnestesiaGrupo.MostraMedicamentos == SimNao.Sim)
                {
                    Binding bindingHeader = new Binding("vmPreMedicacao");
                    bindingHeader.Source = (uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem);
                    BindingOperations.SetBinding(uc.PreMedicacao, UserControl.DataContextProperty, bindingHeader);
                }

                if ((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).AnestesiaGrupo.MostraExameFisico == SimNao.Sim)
                {
                    Binding bindingHeader1 = new Binding("vmExameFisicoEvento");
                    bindingHeader1.Source = (uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem);
                    BindingOperations.SetBinding(uc.ExameFisico, UserControl.DataContextProperty, bindingHeader1);
                }

                if ((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).AnestesiaGrupo.MostraAlergias == SimNao.Sim)
                {
                    Binding bindingHeader2 = new Binding("vmAlergiasEvento");
                    bindingHeader2.Source = (uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem);
                    BindingOperations.SetBinding(uc.Alergias, UserControl.DataContextProperty, bindingHeader2);
                    uc.Alergias.SetData((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).vmAlergiasEvento);
                }

                if ((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).AnestesiaGrupo.MostraMedicamentosEmUso == SimNao.Sim)
                {
                    Binding bindingHeader3 = new Binding("vmMedicamentosEmUsoEvento");
                    bindingHeader3.Source = (uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem);
                    BindingOperations.SetBinding(uc.MedicamentosEmUso, UserControl.DataContextProperty, bindingHeader3);
                }
                pl.Children.Add(uc);
            }
            else            
                uc = (ucSumarioAvaliacaoPreAnestesicaItens)pl.Children[0];

            //Salva ao trocar de aba.
            if (this._tabanterior.IsNull())
                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SalvarAba((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem));
            else
                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SalvarAba(this._tabanterior);

            this._tabanterior = (uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem);

            if ((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).MostraRelatorio)
            {
                uc.ucRelatorio.Visibility = Visibility.Visible;
                uc.ucRelatorio.SetData((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem).SumarioAvaliacaoPreAnestesica);
                btnSalvar.Visibility = Visibility.Visible;
                // btnAtualiza.Visibility = Visibility.Visible;
            }
            else
            {
                uc.ucRelatorio.Visibility = Visibility.Collapsed;
                btnSalvar.Visibility = Visibility.Collapsed;
                //btnAtualiza.Visibility = Visibility.Collapsed;
            }

            if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioPreAnestesicoFinalizado)
            {
                btnSalvar.ButtonText = "Imprimir";
                btnSalvar.Width = 80;
                // btnAtualiza.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnSalvar.ButtonText = "Finalizar e Imprimir";
                btnSalvar.Width = 130;
            }
            UIHelper.SetBusyState();
        }

        private void Click_Visualizar(object sender, System.Windows.RoutedEventArgs e)
        {
            UIHelper.SetBusyState();
            uc.ucRelatorio.SetData((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica);
            winRelatorio win = new winRelatorio(uc.ucRelatorio.report, false, "Sumário de Avaliação Pré-Anestésica", !(this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioPreAnestesicoFinalizado);

            win.ShowDialog(this.OwnerBase);
            UIHelper.SetBusyState();
        }

        private void FechaSumario()
        {
            if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).MesmoAtendimento)
            {
                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).FinalizaImprime();
                Finalizar();
                uc.ucRelatorio.SetData((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica);
                uc.ucRelatorio.report.Watermark.Text = string.Empty;
                uc.ucRelatorio.report.Imprime(2);
            }
            else
                DXMessageBox.Show("O Aviso de Cirurgia foi vinculado a um Atendimento diferente no MV!" + Environment.NewLine
                    + "Atendimento atual: " + (this.DataContext as vmSumarioAvaliacaoPreAnestesica).Atendimento.ID + Environment.NewLine
                    + "Atendimento do Aviso de Cirurgia: " + (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AvisoCirurgia.Atendimento.ID, "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSalvar_Click(object sender, RoutedEventArgs e)
        {            
            foreach (var item in (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status.Equals(Status.Ativo)))
            {
                if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).VMItens.Count(x => x.AnestesiaGrupo.ID.Equals(item.ID)).Equals(0))
                    groupContainer.SelectedTabIndex = item.OrdemTela - 1;
            }

            if ((sender as HMVButton).ButtonText.Equals("Finalizar e Imprimir"))
            {
                if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).PodeFinalizar)
                    FechaSumario();
                else
                {
                    if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).VerificaAtualizacaoAtendimento((this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica.ID))
                    {
                        //Atualiza();
                        FechaSumario();
                    }
                    else
                    {
                        if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).AvisoCirurgia.IsNull())
                        {
                            DXMessageBox.Show("Não há aviso de cirurgia para este paciente, crie um Aviso de Cirurgia no MV e tente novamente!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.OnExecuteMethod();
                        }
                        else
                            DXMessageBox.Show("Aviso de Cirurgia sem Atendimento, vincule o Atendimento no MV e tente novamente!", "Atenção:", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            else
                uc.ucRelatorio.report.Print();
            
        }

        private void Finalizar()
        {
            UIHelper.SetBusyState();
            groupContainer.Children.Clear();
            var group = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status == Status.Ativo).ToList()
                .Where(x => x.MostraAlergias == SimNao.Nao
                         && x.MostraExameFisico == SimNao.Nao
                         && x.MostraGrid == SimNao.Nao
                         && x.MostraMedicamentos == SimNao.Nao
                         && x.MostraMedicamentosEmUso == SimNao.Nao).FirstOrDefault();
            if (group.IsNotNull())
            {
                int d = groupContainer.Children.Count;

                Grid pl = new Grid();
                pl.Tag = d;
                pl.DataContext = group;
                pl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                Binding binding = new Binding("Descricao");
                binding.Source = group;
                BindingOperations.SetBinding(pl, LayoutControl.TabHeaderProperty, binding);

                groupContainer.Children.Add(pl);
            }
            UIHelper.SetBusyState();
        }

        private void UserControlBase_Unloaded(object sender, RoutedEventArgs e)
        {
            UIHelper.SetBusyState();
            if (this._tabanterior.IsNull())
                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SalvarAba((uc.DataContext as vmSumarioAvaliacaoPreAnestesicaItem));
            else
                (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SalvarAba(this._tabanterior);
            //(this.DataContext as vmSumarioAvaliacaoPreAnestesica).Salvar();
            UIHelper.SetBusyState();
        }

        private void btnImportar_Click(object sender, RoutedEventArgs e)
        {            
            int idSumario = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica.ID;
            winImportaDados win = new winImportaDados((this.DataContext as vmSumarioAvaliacaoPreAnestesica));
            if (win.ShowDialog(base.OwnerBase) == true)
            {
                this.SetData((this.DataContext as vmSumarioAvaliacaoPreAnestesica));
            }

            UIHelper.SetBusyState();
            vmSumarioAvaliacaoPreAnestesica vm = new vmSumarioAvaliacaoPreAnestesica((this.DataContext as vmSumarioAvaliacaoPreAnestesica).Paciente.DomainObject, App.Usuario
                                                , idSumario, (this.DataContext as vmSumarioAvaliacaoPreAnestesica).Atendimento.DomainObject);
            UIHelper.SetBusyState();

            this.DataContext = null;
            this.DataContext = vm;
            base.SetData(this.DataContext as ViewModelBase);

            groupContainer.Children.Clear();
            var grupos = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status == Status.Ativo).ToList().OrderBy(x => x.OrdemTela);
            foreach (var group in grupos)
            {
                int d = groupContainer.Children.Count;

                Grid pl = new Grid();
                pl.Tag = d;
                pl.DataContext = group;
                pl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                Binding binding = new Binding("Descricao");
                binding.Source = group;
                BindingOperations.SetBinding(pl, LayoutControl.TabHeaderProperty, binding);

                groupContainer.Children.Add(pl);
            }
        }

        //private void Atualiza()
        //{
        //    int idSumario = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).SumarioAvaliacaoPreAnestesica.ID;
        //    UIHelper.SetBusyState();
        //    vmSumarioAvaliacaoPreAnestesica vm = new vmSumarioAvaliacaoPreAnestesica((this.DataContext as vmSumarioAvaliacaoPreAnestesica).Paciente.DomainObject, App.Usuario
        //                                            , idSumario, (this.DataContext as vmSumarioAvaliacaoPreAnestesica).Atendimento.DomainObject);
        //    UIHelper.SetBusyState();

        //    this.DataContext = null;
        //    this.DataContext = vm;
        //    base.SetData(this.DataContext as ViewModelBase);

        //    groupContainer.Children.Clear();
        //    var grupos = (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status == Status.Ativo).ToList().OrderBy(x => x.OrdemTela);
        //    foreach (var group in grupos)
        //    {
        //        int d = groupContainer.Children.Count;

        //        Grid pl = new Grid();
        //        pl.Tag = d;
        //        pl.DataContext = group;
        //        pl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

        //        Binding binding = new Binding("Descricao");
        //        binding.Source = group;
        //        BindingOperations.SetBinding(pl, LayoutControl.TabHeaderProperty, binding);

        //        groupContainer.Children.Add(pl);
        //    }

        //    foreach (var item in (this.DataContext as vmSumarioAvaliacaoPreAnestesica).AnestesiaTipo.AnestesiaGrupos.Where(x => x.Status.Equals(Status.Ativo)))
        //    {
        //        if ((this.DataContext as vmSumarioAvaliacaoPreAnestesica).VMItens.Count(x => x.AnestesiaGrupo.ID.Equals(item.ID)).Equals(0))
        //            groupContainer.SelectedTabIndex = item.OrdemTela - 1;
        //    }
        //}
    }
}
