using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using DevExpress.Xpf.Core;
using HMV.Core.Domain.Enum;
using HMV.Core.Domain.Model;
using HMV.Core.Domain.Repository;
using HMV.Core.Framework.Expression;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.Core.Wrappers.ObjectWrappers;
using HMV.PEP.ViewModel.PEP;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using HMV.PEP.WPF.Report;
using HMV.PEP.WPF.Report.CheckListCirurgia;
using HMV.PEP.WPF.Windows.CheckListCirurgiaSegura;
using StructureMap;
using System.Linq;
using HMV.ProcessosEnfermagem.ViewModel;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
{
    /// <summary>
    /// Interaction logic for ucCheckListCirurgiaSegura.xaml
    /// </summary>
    public partial class ucCheckListCirurgiaSegura : UserControlBase, IUserControl
    {
        public ucCheckListCirurgiaSegura()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = new vmCheckList((Atendimento)pData, App.Usuario);
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

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (DXMessageBox.Show("Confirma exclusão do 'CheckList UDI' ?", "Atenção", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                ((vmCheckList)this.DataContext).Remover();
            grdCheckList.RefreshData();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            winConfirmaDados wcd = new winConfirmaDados((vmCheckList)this.DataContext);

            if ((this.DataContext as vmCheckList).CheckListdto.CheckList != null || wcd.ShowDialog(this.OwnerBase) == true)
            {
                ((vmCheckList)this.DataContext).IniciaVMS();
                winCadCheckList win = new winCadCheckList(int.Parse((sender as HMVButton).Name.Replace("btn", "")), (vmCheckList)this.DataContext);
                win.ShowDialog(this.OwnerBase);
                grdCheckList.RefreshData();
            }
        }

        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            wrpCheckListCirurgia CheckList = (this.DataContext as vmCheckList).CheckListdto.CheckList;

            var rpt = new RelatorioCheckList(CheckList).Relatorio();

            winRelatorio win = new winRelatorio(rpt, true, "Check List UDI", false);
            win.ShowDialog(this.OwnerBase);
        }

        //public void ExportToPng(Uri path, Canvas surface)
        //{
        //    if (path == null) return;

        //    // Save current canvas transform
        //    Transform transform = surface.LayoutTransform;
        //    // reset current transform (in case it is scaled or rotated)
        //    surface.LayoutTransform = null;

        //    // Get the size of canvas
        //    System.Windows.Size size = new System.Windows.Size(surface.Width, surface.Height);
        //    // Measure and arrange the surface
        //    // VERY IMPORTANT
        //    surface.Measure(size);
        //    surface.Arrange(new Rect(size));

        //    // Create a render bitmap and push the surface to it
        //    RenderTargetBitmap renderBitmap =
        //      new RenderTargetBitmap(
        //        (int)size.Width,
        //        (int)size.Height,
        //        96d,
        //        96d,
        //        PixelFormats.Pbgra32);
        //    renderBitmap.Render(surface);

        //    // Create a file stream for saving image
        //    using (FileStream outStream = new FileStream(path.LocalPath, FileMode.Create))
        //    {
        //        // Use png encoder for our data
        //        PngBitmapEncoder encoder = new PngBitmapEncoder();
        //        // push the rendered bitmap to it
        //        encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
        //        // save the data to the stream
        //        encoder.Save(outStream);

        //        System.Drawing.Image teste = System.Drawing.Image.FromStream(outStream);
        //    }

        //    // Restore previously saved layout
        //    surface.LayoutTransform = transform;
        //}
    }

    public static class EnumExtension
    {
        public static Boolean ConvertSimNaoToBoolean(this object obj)
        {
            if (obj.Equals(SimNao.Sim))
                return true;
            else
                return false;
        }

        public static Boolean ConvertSimNAToBoolean(this object obj)
        {
            if (obj.Equals(SimNA.Sim))
                return true;
            else
                return false;
        }

        public static CheckState ConvertSimNaoToCheckstate(this object obj)
        {
            if (obj.Equals(SimNao.Sim))
                return CheckState.Checked;
            else
                return CheckState.Unchecked;
        }

        public static CheckState ConvertSimNAToCheckstate(this object obj)
        {
            if (obj.Equals(SimNA.Sim))
                return CheckState.Checked;
            else
                return CheckState.Unchecked;
        }

        public static CheckState ConvertSimNaoNAToCheckstate(this object obj)
        {
            if (obj.Equals(SimNao.Sim))
                return CheckState.Checked;
            else if (obj.Equals(SimNao.Nao))
                return CheckState.Unchecked;
            else
                return CheckState.Indeterminate;
        }

        public static CheckState ConvertBooleanToCheckstate(this object obj)
        {
            if (obj.Equals(true))
                return CheckState.Checked;
            else
                return CheckState.Unchecked;
        }
    }
}
