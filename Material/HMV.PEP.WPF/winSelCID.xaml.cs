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
using DevExpress.Xpf.Core;
using HMV.PEP.Interfaces;
using StructureMap;
using HMV.Core.DataAccess;
using HMV.Core.Domain.Model;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using HMV.PEP.DTO;
using HMV.Core.Domain.Enum;
using DevExpress.Xpf.Grid;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;

namespace HMV.PEP.WPF
{
    /// <summary>
    /// Interaction logic for winSelCID.xaml
    /// </summary>
    public partial class winSelCID : WindowBase
    {
        IList<TreeViewItem> listaDeItensEmAzul;
        public Cid CID { get; private set; }
        IList<CidDTO> lista;
        bool ApenasCidMV;

        public winSelCID(bool pApenasCidMV)
        {
            InitializeComponent();
            this.ApenasCidMV = pApenasCidMV;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtCodigoCid.Focus();
            carrega();
        }

        private void carrega()
        {
            myLoad.Visibility = System.Windows.Visibility.Visible;
            ThreadStart ts = delegate
            {
                //rz using (IUnitOfWork uow = ObjectFactory.GetInstance<IUnitOfWork>())
                {
                    if (ApenasCidMV)
                    {
                        if (App.listaDeCidMv == null)
                            App.listaDeCidMv = ObjectFactory.GetInstance<ICidService>().ListaCIDs(true);
                        lista = App.listaDeCidMv;
                    }
                    else
                    {
                        if (App.listaDeCid == null)
                            App.listaDeCid = ObjectFactory.GetInstance<ICidService>().ListaCIDs();
                        lista = App.listaDeCid;
                    }

                    /*ICidService srv = ObjectFactory.GetInstance<ICidService>();
                    if (ApenasCidMV)
                        lista = srv.ListaCIDs(true);
                    else
                        lista = srv.ListaCIDs();*/
                    
                    treeVewCIDs.Dispatcher.Invoke(new Action(() =>
                    {
                        TreeViewItem tviCapitulo = new TreeViewItem();
                        TreeViewItem tviCategoria = new TreeViewItem();
                        TreeViewItem tviSubCategoria = new TreeViewItem();

                        foreach (var itemcap in (
                            from cap in lista
                            select new { IdCapitulo = cap.IdCapitulo, DescricaoCapitulo = cap.DescricaoCapitulo }).Distinct())
                        {
                            tviCapitulo = new TreeViewItem();
                            tviCapitulo.Header = itemcap.DescricaoCapitulo;
                            tviCapitulo.Tag = itemcap.IdCapitulo;

                            foreach (var itemcat in (
                                from cat in lista
                                where cat.IdCapitulo == itemcap.IdCapitulo
                                select new { IdCategoria = cat.IdCategoria, DescricaoCategoria = cat.DescricaoCategoria }).Distinct())
                            {
                                tviCategoria = new TreeViewItem();
                                tviCategoria.Header = itemcat.DescricaoCategoria;
                                tviCategoria.Tag = itemcat.IdCategoria;
                                tviCapitulo.Items.Add(tviCategoria);

                                foreach (var itemsub in (
                                    from sub in lista
                                    where sub.IdCategoria == itemcat.IdCategoria
                                    select new { IdSubCategoria = sub.IdSubCategoria, DescricaoSubCategoria = sub.DescricaoSubCategoria }).Distinct())
                                {
                                    tviSubCategoria = new TreeViewItem();
                                    tviSubCategoria.Header = itemsub.DescricaoSubCategoria;
                                    tviSubCategoria.Tag = itemsub.IdSubCategoria;
                                    tviCategoria.Items.Add(tviSubCategoria);
                                }
                            }
                            treeVewCIDs.Items.Add(tviCapitulo);
                        }
                        consutaCID();
                    }), null);
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
                delegate
                {
                    txtCodigoCid.Focus();
                    myLoad.Visibility = System.Windows.Visibility.Collapsed;
                }, null, null);
            };
            ts.BeginInvoke(delegate(IAsyncResult aysncResult) { ts.EndInvoke(aysncResult); }, null);
        }

        #region Codigo_antigo
        //listaDeItensEmAzul = new List<TreeViewItem>();
        //myLoad.Visibility = System.Windows.Visibility.Visible;
        //ThreadStart ts = delegate
        //{
        //    using (IUnitOfWork uow = ObjectFactory.GetInstance<IUnitOfWork>())
        //    {
        //        ICidService srv = ObjectFactory.GetInstance<ICidService>();
        //        IList<Capitulo> capitulo = srv.ListaCapitulos();
        //        srv.OrdenaPorDescricao();

        //        treeVewCIDs.Dispatcher.Invoke(new Action(() =>
        //        {
        //            treeVewCIDs.Items.Clear();
        //            treeVewCIDs.Items.Refresh();

        //            TreeViewItem itemTreeViewItemUm = new TreeViewItem() { Header = "--------PADRÕES DO CENTRO-------", FontSize = 11 };
        //            treeVewCIDs.Items.Add(itemTreeViewItemUm);

        //            foreach (var item in capitulo.ToList())
        //            {
        //                TreeViewItem itemTreeViewItem = new TreeViewItem() { Header = item.Descricao };
        //                itemTreeViewItem.Tag = item;

        //                carregaCategoria(itemTreeViewItem, item);
        //                treeVewCIDs.Items.Add(itemTreeViewItem);
        //            }

        //        }), null);

        //    }

        //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (EventHandler)
        //    delegate
        //    {
        //        myLoad.Visibility = System.Windows.Visibility.Collapsed;
        //    }, null, null);
        //};
        //ts.BeginInvoke(delegate(IAsyncResult aysncResult) { ts.EndInvoke(aysncResult); }, null);  

        //}

        //private void carregaCategoria(TreeViewItem itemTreeViewItem, Capitulo _item)
        //{
        //    treeVewCIDs.Items.Refresh();
        //    foreach (var itemCategoria in _item.Categorias)
        //    {
        //        var item = new TreeViewItem() { Header = itemCategoria.Descricao };
        //        item.Tag = itemCategoria;

        //        carregaSubCategorias(item, itemCategoria);
        //        itemTreeViewItem.Items.Add(item);
        //    }
        //}

        //private void carregaSubCategorias(TreeViewItem itemTreeViewItem, Categoria _Item)
        //{
        //    treeVewCIDs.Items.Refresh();
        //    foreach (var itemSubCategoria in _Item.SubCategorias)
        //    {
        //        var item = new TreeViewItem() { Header = itemSubCategoria.Descricao };
        //        item.Tag = itemSubCategoria;
        //        itemTreeViewItem.Items.Add(item);
        //    }
        //}

        //private void carregaGrd()
        //{

        //    using (IUnitOfWork uow = ObjectFactory.GetInstance<IUnitOfWork>())
        //    {
        //ICidService srv = ObjectFactory.GetInstance<ICidService>();

        //TreeViewItem item = (TreeViewItem)treeVewCIDs.SelectedItem;

        //if (item.Tag == null)
        //{
        //    return;
        //}

        //if (item.Tag.GetType() == null)
        //{
        //    return;
        //}

        //if (typeof(Capitulo) == item.Tag.GetType())
        //{
        //    Capitulo TreeCapi = (Capitulo)item.Tag;
        //    gdConsultaCIDs.DataSource = srv.FiltraPorCapitulo(Convert.ToInt32(TreeCapi.ID));
        //}
        //else if (typeof(Categoria) == item.Tag.GetType())
        //{
        //    Categoria TreeCapi = (Categoria)item.Tag;
        //    gdConsultaCIDs.DataSource = srv.FiltraPorCategoria(Convert.ToString(TreeCapi.ID));
        //}
        //else
        //{
        //    SubCategoria TreeCapi = (SubCategoria)item.Tag;
        //    gdConsultaCIDs.DataSource = srv.FiltraPorSubCategoria(Convert.ToString(TreeCapi.ID));
        //}

        //    }

        //}


        #endregion

        private void btnFechar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSelecionar_Click(object sender, RoutedEventArgs e)
        {
            selecionaCID();
        }

        private void viewConsultaCIDs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as TableView).GetRowHandleByMouseEventArgs(e) != GridControl.InvalidRowHandle)
            {
                e.Handled = true;
                selecionaCID();
            }
        }

        private void selecionaCID()
        {
            if (gdConsultaCIDs.VisibleRowCount == 0)
            {
                DXMessageBox.Show("Deve ser selecionado um CID.", string.Empty, MessageBoxButton.OK, MessageBoxImage.Question);
                return;
            }

            ICidService serv = ObjectFactory.GetInstance<ICidService>();
            CID = serv.FiltraPorCid10((gdConsultaCIDs.GetFocusedRow() as Cid).Id);
            this.Close();
        }


        private TreeViewItem ConvertToTreeView(Object pOjeto)
        {
            try
            {
                return pOjeto as TreeViewItem;
            }
            catch
            {
                return null;
            }
        }

        TreeViewItem selectedTVI;
        private void treeVewCIDs_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            e.Handled = false;
            //treeVewCIDs.Dispatcher.Invoke(new Action(() =>
            // {
            selectedTVI = (TreeViewItem)treeVewCIDs.SelectedItem;
            string a = selectedTVI.Tag.ToString();

            TreeViewItem itempai = (TreeViewItem)(sender as TreeView).SelectedItem;
            if (itempai != null)
            {
                desmarcaOsItemAzuis();
                listaDeItensEmAzul.Add(itempai);
                itempai.Foreground = new SolidColorBrush(Colors.Blue);
                TreeViewItem itempai2 = ConvertToTreeView(itempai.Parent);
                TreeViewItem itempai3 = null;
                if (itempai2 != null)
                {
                    itempai2.Foreground = new SolidColorBrush(Colors.Blue);
                    listaDeItensEmAzul.Add(itempai2);
                    itempai3 = ConvertToTreeView(itempai2.Parent);
                    if (itempai3 != null)
                        itempai3.Foreground = new SolidColorBrush(Colors.Blue);
                }

                if (ApenasCidMV)
                {
                    gdConsultaCIDs.ItemsSource = (
                                    from T in lista
                                    orderby T.DescricaoCid
                                    where (itempai3 == null ? (itempai2 != null ? T.IdCategoria : T.IdCapitulo.ToString()) : T.IdSubCategoria) == a
                                    select new Cid { Id = T.IdCid, Descricao = T.DescricaoCid, CidMV = new CidMV { Id = T.IdCidMV }});
                }
                else
                {
                    gdConsultaCIDs.ItemsSource = (
                                    from T in lista
                                    orderby T.DescricaoCid
                                    where (itempai3 == null ? (itempai2 != null ? T.IdCategoria : T.IdCapitulo.ToString()) : T.IdSubCategoria) == a
                                    select new Cid { Id = T.IdCid, Descricao = T.DescricaoCid});
                }
                gdConsultaCIDs.RefreshData();
            }

            //}), null);

        }

        private void desmarcaOsItemAzuis()
        {
            if (listaDeItensEmAzul == null)
                listaDeItensEmAzul = new List<TreeViewItem>();
            foreach (var item in listaDeItensEmAzul)
                item.Foreground = new SolidColorBrush(Colors.Black);
            listaDeItensEmAzul = new List<TreeViewItem>();
            //treeVewCIDs.Items.Refresh();
        }

        private void txtCodigoCid_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            consutaCID();
        }

        private void txtDescricaoCIDPrincipal_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            consutaCID();
        }

        private void btnPesCIDPrincipal_Click(object sender, RoutedEventArgs e)
        {
            consutaCID();
        }

        private void consutaCID()
        {
            if (lista != null && txtCodigoCid.Text != null)
            {
                if (ApenasCidMV)
                {
                    gdConsultaCIDs.ItemsSource = from CID in lista
                                                orderby CID.DescricaoCid
                                                where CID.IdCid.StartsWith(txtCodigoCid.Text) ||
                                                      CID.DescricaoCid.RemoverAcentos().Contains(txtCodigoCid.Text.RemoverAcentos())
                                                select new Cid { Id = CID.IdCid, Descricao = CID.DescricaoCid, CidMV = new CidMV { Id = CID.IdCidMV } };
                }
                else
                {
                    gdConsultaCIDs.ItemsSource = from CID in lista
                                                orderby CID.DescricaoCid
                                                where CID.IdCid.StartsWith(txtCodigoCid.Text) ||
                                                      CID.DescricaoCid.RemoverAcentos().Contains(txtCodigoCid.Text.RemoverAcentos())
                                                select new Cid { Id = CID.IdCid, Descricao = CID.DescricaoCid };
                }

                //SolidColorBrush Color cor = new Color.FromArgb((int)"#ffffff");
                string colorRed = "Red";
                Color c = (Color)ColorConverter.ConvertFromString(colorRed);
                SolidColorBrush fromStringToColor = new SolidColorBrush(c);


                
            }
        }
    }
}
