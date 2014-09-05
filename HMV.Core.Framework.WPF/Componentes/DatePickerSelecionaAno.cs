using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Reflection;

namespace HMV.Core.Framework.WPF.Componentes
{   
    public class DatePickerSelecionaAno : DatePicker
    {   
        protected override void OnCalendarOpened(RoutedEventArgs e)
        {
            var popup = this.Template.FindName("PART_Popup", this) as Popup;
            if (popup != null && popup.Child is System.Windows.Controls.Calendar)
            {
                ((System.Windows.Controls.Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
            }
            ((System.Windows.Controls.Calendar)popup.Child).DisplayModeChanged += new EventHandler<CalendarModeChangedEventArgs>(DatePickerCo_DisplayModeChanged);



        }

        private void DatePickerCo_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var popup = this.Template.FindName("PART_Popup", this) as Popup;
            if (popup != null && popup.Child is System.Windows.Controls.Calendar)
            {
                var _calendar = popup.Child as System.Windows.Controls.Calendar;
                if (_calendar.DisplayMode == CalendarMode.Year)
                {
                    _calendar.DisplayMode = CalendarMode.Decade;

                    if (IsDropDownOpen)
                    {
                        this.SelectedDate = GetSelectedMonth(_calendar.DisplayDate);
                        this.IsDropDownOpen = false;
                        ((System.Windows.Controls.Calendar)popup.Child).DisplayModeChanged -= new EventHandler<CalendarModeChangedEventArgs>(DatePickerCo_DisplayModeChanged);
                    }
                }
            }
        }


        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            FieldInfo fiTextBox = typeof(DatePicker).GetField("_textBox", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiTextBox != null)
            {
                DatePickerTextBox dateTextBox = (DatePickerTextBox)fiTextBox.GetValue(this);
                PropertyInfo piWatermark = typeof(DatePickerTextBox).GetProperty("Watermark", BindingFlags.Instance | BindingFlags.NonPublic);
                if (piWatermark != null)
                {
                    piWatermark.SetValue(dateTextBox, "-", null);
                }
            }
        }

        private DateTime? GetSelectedMonth(DateTime? selectedDate)
        {
            if (selectedDate == null)
            {
                selectedDate = DateTime.Now;
            }
            return selectedDate;
        }
    }
}
