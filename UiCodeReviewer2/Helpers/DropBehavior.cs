using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace UiCodeReviewer2.Views.Pages
{
    public class DropBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(DropBehavior), new PropertyMetadata(null));

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Drop += OnDrop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Drop -= OnDrop;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            e.Handled = false;
            if (DropCommand != null && DropCommand.CanExecute(e))
            {
                DropCommand.Execute(e);
            }
        }
    }
}
