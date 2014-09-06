using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using DevExpress.Xpf.LayoutControl;
using HMV.Core.Framework.Extensions;
using HMV.Core.Framework.WPF;
using HMV.Core.Interfaces;
using HMV.PEP.ViewModel.PEP.CheckListDeCirurgia;
using HMV.PEP.WPF.Windows.CheckListCirurgiaSegura;

namespace HMV.PEP.WPF.UserControls.CheckListCirurgiaSegura
{
    /// <summary>
    /// Interaction logic for ucTransOperatorio.xaml
    /// </summary>
    public partial class ucTransOperatorio : UserControlBase, IUserControl
    {
        public ucTransOperatorio()
        {
            InitializeComponent();
        }

        public void SetData(object pData)
        {
            this.DataContext = (pData as vmCheckList).vmTransOperatorio;
            this.InkCorpoHumano.EditingMode = InkCanvasEditingMode.Select;
            this.InkCorpoHumano.ChildrenChanged += new EventHandler(Ink_ChildrenChanged);
            if ((pData as vmCheckList).vmTransOperatorio.TransOperatorio.IsNotNull())
            {
                byte[] signature = (pData as vmCheckList).vmTransOperatorio.TransOperatorio.CorpoHumanoImg;
                if (signature.IsNotNull())
                {
                    MyInkCanvas ink = new MyInkCanvas();
                    ink = XamlReader.Load(new XmlTextReader(new StringReader(signature.GetString().Replace("<x:Null />", "")))) as MyInkCanvas;                    
                    this.GridInk.Children.Clear();
                    this.GridInk.Children.Add(ink);
                    ink.Drop += new DragEventHandler(InkCanvas_Drop);
                    ink.MouseDown += new MouseButtonEventHandler(InkCanvas_MouseDown);
                    ink.SelectionMoved += new EventHandler(Ink_SelectionMoved);
                    ink.ChildrenChanged += new EventHandler(Ink_ChildrenChanged);
                    ink.EditingMode = InkCanvasEditingMode.Select;
                }
            }
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

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            object data = new ImageInfo() { Height = 20, Width = double.NaN, Uri = new Uri(img.Source.ToString()) };
            DragDrop.DoDragDrop(img, data, DragDropEffects.Copy);
        }

        private void InkCanvas_Drop(object sender, DragEventArgs e)
        {
            InkCanvas ink = (sender as MyInkCanvas);
            ImageInfo imageInfo = e.Data.GetData(typeof(ImageInfo)) as ImageInfo;
            if (imageInfo != null)
            {
                Image image = new Image();

                image.Height = imageInfo.Height;
                image.Width = imageInfo.Width;
                image.Source = new BitmapImage(imageInfo.Uri);
                Point position = e.GetPosition(ink);
                InkCanvas.SetLeft(image, position.X);
                InkCanvas.SetTop(image, position.Y);
                ink.Children.Add(image);
                AdornerLayer adornerlayer = AdornerLayer.GetAdornerLayer(image);
                MyImageAdorner adorner = new MyImageAdorner(image);
                adornerlayer.Add(adorner);
                ink.EditingMode = InkCanvasEditingMode.None;
            }           
        }

        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            InkCanvas ink = (sender as MyInkCanvas);
            ink.EditingMode = InkCanvasEditingMode.Select;
            foreach (UIElement element in ink.Children)
            {
                AdornerLayer adornerlayer = AdornerLayer.GetAdornerLayer(element);
                var adorners = adornerlayer.GetAdorners(element);
                if (adorners != null)
                {
                    for (int i = 0; i < adorners.Length; i++)
                    {
                        adornerlayer.Remove(adorners[i]);
                    }
                }
            }            
        }

        private void Ink_SelectionMoved(object sender, EventArgs e)
        {
            this._setaCorpoHumano((sender as MyInkCanvas));
        }

        private void Ink_ChildrenChanged(object sender, EventArgs e)
        {
            this._setaCorpoHumano((sender as MyInkCanvas));
        }

        private void _setaCorpoHumano(InkCanvas ink)
        {          
            ink.Width = ink.ActualWidth;
            ink.Height = ink.ActualHeight;
            byte[] bytexaml = XamlWriter.Save(ink).GetBytes();
            (this.DataContext as vmTransOperatorio).TransOperatorio.CorpoHumanoImg = bytexaml;
        }

        private void LayoutGroup_SelectedTabChildChanged(object sender, DevExpress.Xpf.Core.ValueChangedEventArgs<FrameworkElement> e)
        {
            SelecionaTab((sender as LayoutGroup).SelectedTabIndex);  
        }

        public void SelecionaTab(int index)
        {
            (this.OwnerBase as winCadCheckList).habilitabotoes(index);
            tabsselecionada.SelectedTabIndex = index;
        }
    }

    public class MyInkCanvas : InkCanvas
    {
        public event EventHandler ChildrenChanged;

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (this.ChildrenChanged != null)
                this.ChildrenChanged(this, null);
        }
    }

    public class ImageInfo
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Uri Uri { get; set; }
    }

    public class MyImageAdorner : Adorner
    {
        Thumb rotateHandle;
        Thumb moveHandle;
        System.Windows.Shapes.Path outline;
        VisualCollection visualChildren;
        Point center;
        TranslateTransform translate;
        RotateTransform rotation;
        TransformGroup transformGroup;
        const int HANDLEMARGIN = 10;

        public MyImageAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
            rotateHandle = new Thumb();
            rotateHandle.Cursor = Cursors.Hand;
            rotateHandle.Width = 5;
            rotateHandle.Height = 5;
            rotateHandle.Background = Brushes.Blue;

            rotateHandle.DragDelta += new DragDeltaEventHandler(rotateHandle_DragDelta);
            rotateHandle.DragCompleted += new DragCompletedEventHandler(rotateHandle_DragCompleted);

            moveHandle = new Thumb();
            moveHandle.Cursor = Cursors.SizeAll;
            moveHandle.Width = 10;
            moveHandle.Height = 10;
            moveHandle.Background = Brushes.Blue;

            moveHandle.DragDelta += new DragDeltaEventHandler(moveHandle_DragDelta);
            moveHandle.DragCompleted += new DragCompletedEventHandler(moveHandle_DragCompleted);


            outline = new System.Windows.Shapes.Path();
            outline.Stroke = Brushes.Blue;
            outline.StrokeThickness = 1;

            visualChildren.Add(outline);
            visualChildren.Add(rotateHandle);
            visualChildren.Add(moveHandle);

            rotation = new RotateTransform();
            translate = new TranslateTransform();
            transformGroup = new TransformGroup();
        }

        void moveHandle_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            MoveNewTransformToAdornedElement(translate);
        }

        void moveHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);

            double deltaX = pos.X - center.X;
            double deltaY = pos.Y - center.Y;

            translate.X = deltaX;
            translate.Y = deltaY;
            outline.RenderTransform = translate;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            center = new Point(AdornedElement.RenderSize.Width / 2,
                      AdornedElement.RenderSize.Height / 2);

            Rect handleRect = new Rect(0, 0 - (AdornedElement.RenderSize.Height / 2 + HANDLEMARGIN),
                       AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height);

            rotateHandle.Arrange(handleRect);

            Rect finalRect = new Rect(finalSize);
            moveHandle.Arrange(finalRect);
            outline.Data = new RectangleGeometry(finalRect);
            outline.Arrange(finalRect);
            return finalSize;
        }

        void rotateHandle_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);

            double deltaX = pos.X - center.X;
            double deltaY = pos.Y - center.Y;

            double angle;
            if (deltaY.Equals(0))
            {
                if (!deltaX.Equals(0))
                {
                    angle = 90;
                }
                else
                {
                    return;
                }
            }
            else
            {
                double tan = deltaX / deltaY;
                angle = Math.Atan(tan);

                angle = angle * 180 / Math.PI;
            }

            // If the mouse crosses the vertical center, 
            // find the complementary angle.
            if (deltaY > 0)
            {
                angle = 180 - Math.Abs(angle);
            }

            // Rotate left if the mouse moves left and right
            // if the mouse moves right.
            if (deltaX < 0)
            {
                angle = -Math.Abs(angle);
            }
            else
            {
                angle = Math.Abs(angle);
            }

            if (Double.IsNaN(angle))
            {
                return;
            }

            // Apply the rotation to the outline.
            rotation.Angle = angle;
            rotation.CenterX = center.X;
            rotation.CenterY = center.Y;
            outline.RenderTransform = rotation;
        }

        /// <summary>
        /// Rotates to the same angle as outline.
        /// </summary>
        void rotateHandle_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            MoveNewTransformToAdornedElement(rotation);
        }

        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        private void MoveNewTransformToAdornedElement(Transform transform)
        {
            if (transform == null)
            {
                return;
            }
            var newTransform = transform.Clone();
            newTransform.Freeze();
            transformGroup.Children.Insert(0, newTransform);
            AdornedElement.RenderTransform = transformGroup;

            outline.RenderTransform = Transform.Identity;
            this.InvalidateArrange();
        }
    }
}
