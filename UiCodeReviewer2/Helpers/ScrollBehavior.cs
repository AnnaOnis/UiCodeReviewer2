using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace UiCodeReviewer2.Views.Pages
{
    public static class ScrollBehavior
    {
        public static readonly DependencyProperty ScrollToEndProperty =
            DependencyProperty.RegisterAttached("ScrollToEnd", typeof(bool), typeof(ScrollBehavior), new PropertyMetadata(false, OnScrollToEndChanged));

        public static bool GetScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollToEndProperty);
        }

        public static void SetScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollToEndProperty, value);
        }

        private static void OnScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox scrollViewer && (bool)e.NewValue)
            {
                scrollViewer.ScrollToEnd();
            }
        }
    }
}
