using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace HMV.Core.Framework.WPF.Helpers
{

    public class EnterKeyTraversal
    {
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }
        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }
        static void ue_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var ue = e.OriginalSource as FrameworkElement; if (e.Key == Key.Enter)
            {
                e.Handled = true;
                ue.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private static void ue_Unloaded(object sender, RoutedEventArgs e)
        {
            var ue = sender as FrameworkElement; if (ue == null) return;
            ue.Unloaded -= ue_Unloaded; ue.PreviewKeyDown -= ue_PreviewKeyDown;
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(EnterKeyTraversal), new UIPropertyMetadata(false, IsEnabledChanged));

        static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ue = d as FrameworkElement;
            if (ue == null) return;
            if ((bool)e.NewValue)
            {
                ue.Unloaded += ue_Unloaded;
                ue.PreviewKeyDown += ue_PreviewKeyDown;
            }
            else
            {
                ue.PreviewKeyDown -= ue_PreviewKeyDown;
            }
        }
    }

    [MarkupExtensionReturnType(typeof(object[]))]
    public class EnumValuesExtension : MarkupExtension
    {
        public EnumValuesExtension()
        {
        }

        public EnumValuesExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.EnumType == null)
                throw new ArgumentException("The enum type is not set");
            return Enum.GetValues(this.EnumType);
        }
    }

    public class LocalTimer : INotifyPropertyChanged
    {
        private DispatcherTimer timer;

        public LocalTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.0);
            timer.Tick += new EventHandler(TimerCallback);
        }

        private void TimerCallback(object sender, EventArgs e)
        {
            PropertyChanged(this, new PropertyChangedEventArgs("FormattedDate"));
            PropertyChanged(this, new PropertyChangedEventArgs("FormattedTime"));
        }

        public bool Enabled
        {
            get { return this.timer.IsEnabled; }
            set { if (value) this.timer.Start(); else this.timer.Stop(); }
        }

        public DateTime DataAtual { get { return DateTime.Today; } }

        public string FormattedDate { get { return string.IsNullOrEmpty(this.DateFormat) ? DateTime.Now.ToLongDateString() : DateTime.Now.ToString(this.DateFormat); } }
        public string FormattedTime { get { return string.IsNullOrEmpty(this.TimeFormat) ? DateTime.Now.ToLongTimeString() : DateTime.Now.ToString(this.TimeFormat); } }

        public string TimeFormat { get; set; }
        public string DateFormat { get; set; }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    //No construtor da classe coloque BindingErrorTraceListener.SetTrace();
    public class BindingErrorTraceListener : DefaultTraceListener
    {
        private static BindingErrorTraceListener _Listener;

        public static void SetTrace()
        { SetTrace(SourceLevels.Error, TraceOptions.None); }

        public static void SetTrace(SourceLevels level, TraceOptions options)
        {
            if (_Listener == null)
            {
                _Listener = new BindingErrorTraceListener();
                PresentationTraceSources.DataBindingSource.Listeners.Add(_Listener);
            }

            _Listener.TraceOutputOptions = options;
            PresentationTraceSources.DataBindingSource.Switch.Level = level;
        }

        public static void CloseTrace()
        {
            if (_Listener == null)
            { return; }

            _Listener.Flush();
            _Listener.Close();
            PresentationTraceSources.DataBindingSource.Listeners.Remove(_Listener);
            _Listener = null;
        }

        private StringBuilder _Message = new StringBuilder();

        private BindingErrorTraceListener()
        { }

        public override void Write(string message)
        { _Message.Append(message); }

        public override void WriteLine(string message)
        {
            _Message.Append(message);

            var final = _Message.ToString();
            _Message.Length = 0;

            MessageBox.Show(final, "Binding Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
