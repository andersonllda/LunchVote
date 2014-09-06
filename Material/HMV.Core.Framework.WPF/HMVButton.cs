using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;

namespace HMV.Core.Framework.WPF
{
    public class HMVButton : Button
    {
        public HMVButton()
        {
            DefaultStyleKey = typeof(HMVButton);
        }

        public ImageSource ButtonImage
        {
            get { return (ImageSource)GetValue(ButtonImageProperty); }
            set { SetValue(ButtonImageProperty, value); }
        }

        public static readonly DependencyProperty ButtonImageProperty =
            DependencyProperty.Register("ButtonImage",
                                        typeof(ImageSource),
                                        typeof(HMVButton),
                                        new PropertyMetadata());

        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty = 
            DependencyProperty.Register("ButtonText",
                                        typeof(string),
                                        typeof(HMVButton),
                                        new PropertyMetadata("Button Text"));

     
    }
}
