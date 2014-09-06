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
using System.Windows.Shapes;
using DevExpress.Xpf.Docking;
using HMV.Core.Framework.WPF;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.Expression;

namespace HMV.Prototipos.WPF.Receituario
{
    /// <summary>
    /// Interaction logic for winReceituario.xaml
    /// </summary>
    public partial class winReceituario : WindowBase
    {
        public winReceituario()
        {
            InitializeComponent();

            InsereTabs();
        }

        #region Enum
        public enum TabsReceituario
        {
            [Description("Receituário")]
            Receituario
        }
        #endregion

        private IList<RuntimeTab<TabsReceituario>> _tabs;

        private void InsereTabs()
        {
            DocumentPanel panel;

            LayoutGroup lg = new LayoutGroup();
            DocumentGroup documentGroupN1 = new DocumentGroup();
            documentGroupN1.MDIStyle = MDIStyle.Tabbed;
            documentGroupN1.ShowCaptionImage = true;
            documentGroupN1.FloatOnDoubleClick = false;
            documentGroupN1.AllowContextMenu = false; documentGroupN1.AllowClose = false;
            documentGroupN1.AllowDock = false; documentGroupN1.AllowDrag = false;
            documentGroupN1.AllowMove = false; documentGroupN1.ShowControlBox = false;
            documentGroupN1.AllowRename = false; documentGroupN1.ShowDropDownButton = false;
            documentGroupN1.ShowRestoreButton = false; documentGroupN1.AllowHide = false;
            documentGroupN1.ClosePageButtonShowMode = ClosePageButtonShowMode.NoWhere;
            documentGroupN1.AllowRestore = false; documentGroupN1.ShowCloseButton = false;

            foreach (var tab in Tabs)
            {
                panel = dockLayoutManagerN1.DockController.AddDocumentPanel(documentGroupN1, tab.Componente);
                panel.Caption = tab.Descricao;
                //guarda o binding para setar APENAS quando entrar a primeira vez na tab...
                panel.Tag = tab.Binding;
                panel.FloatOnDoubleClick = false;
                panel.AllowFloat = false;
                panel.AllowDrag = false;
                panel.AllowHide = false;
                panel.AllowClose = false;
                lg.Add(documentGroupN1);
                dockLayoutManagerN1.LayoutRoot = lg;
                //BindingOperations.SetBinding((panel.Control as UserControlBase), UserControlBase.DataContextProperty, (panel.Tag as Binding));        
            }
        }

        public IList<RuntimeTab<TabsReceituario>> Tabs
        {
            get
            {
                if (_tabs.IsNull())
                    this._montatabs();
                return _tabs;
            }
        }

        private void _montatabs()
        {
            this._tabs = new List<RuntimeTab<TabsReceituario>>();

            this._tabs.Add(new RuntimeTab<TabsReceituario>
            {
                TipoTab = TabsReceituario.Receituario,
                Descricao = TabsReceituario.Receituario.GetEnumDescription(),
                Componente = new Uri(@"/HMV.Prototipos.WPF;component/Receituario/ucConsultaReceituario.xaml", UriKind.Relative),
            });
        }

        private void HMVButton_Click(object sender, RoutedEventArgs e)
        {
            winCadReceituario win = new winCadReceituario(false);
            win.ShowDialog(null);

        }

        private void HMVButton_Click_1(object sender, RoutedEventArgs e)
        {
            winCadReceituario win = new winCadReceituario(true);
            win.ShowDialog(null);
        }

        private void btnNormal_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEspecial_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
