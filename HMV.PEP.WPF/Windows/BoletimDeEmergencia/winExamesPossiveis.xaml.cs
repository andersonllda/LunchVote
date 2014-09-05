using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HMV.PEP.ViewModel.SumarioDeAlta;
using HMV.PEP.ViewModel.PEP;
using HMV.Core.Domain.Repository;
using StructureMap;
using HMV.Core.Domain.Model;
using System.Windows.Input;
using HMV.Core.Framework.WPF;

namespace HMV.PEP.WPF.Windows
{
    /// <summary>
    /// Interaction logic for winExamesPossiveis.xaml
    /// </summary>
    public partial class winExamesPossiveis : WindowBase
    {
        public IList<Procedimento> RetornaProcedimento { get; set; }
        /// <summary>
        /// *****Atenção*****
        /// Use a propriedade 'RetornaProcedimento' para retornar uma lista de procedimento selecionado
        /// </summary>
        public winExamesPossiveis()
        {
            InitializeComponent();
            this.DataContext = new vmExamesPossiveis();
            //base.CommandBindings.Add(
            //    new CommandBinding(
            //        ApplicationCommands.Undo,
            //        (sender, e) => // Execute
            //        {
            //            e.Handled = true;
            //            this.tree.Focus();
            //        },
            //        (sender, e) => // CanExecute
            //        {
            //            e.Handled = true;
            //        }));
            this.tree.Focus();
        }

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            RetornaProcedimento = new List<Procedimento>();
            List<vmExamesPossiveis> lst = (tree.ItemsSource as List<vmExamesPossiveis>);
            foreach (vmExamesPossiveis item in lst)
            {
                if (item._vmExamesPossiveis.Where(x => x.IsChecked == true).Count() > 0)
                {
                    foreach (var T in item._vmExamesPossiveis.Where(x => x.IsChecked == true))
                    {
                        GrupoDeProcedimento gp = new GrupoDeProcedimento();
                        gp = new GrupoDeProcedimento();
                        gp.ID = int.Parse(item.ID);
                        gp.Descricao = item.Name;
                        RetornaProcedimento.Add(new Procedimento()
                        {
                            Ativo = Core.Domain.Enum.SimNao.Sim,
                            Descricao = T.Name,
                            ID = T.ID,
                            GrupoDeProcedimento = gp
                        });
                    }
                }
            }
            //this.RetornaProcedimento = (this.DataContext as vmExamesPossiveis).RetornaProcedimentos();
            this.DialogResult = true;
            this.Close();
        }

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }
    }
}
