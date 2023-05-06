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
    public class DragOverBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty DragOverCommandProperty =
            DependencyProperty.Register("DragOverCommand", typeof(ICommand), typeof(DragOverBehavior), new PropertyMetadata(null));

        public ICommand DragOverCommand
        {
            get { return (ICommand)GetValue(DragOverCommandProperty); }
            set { SetValue(DragOverCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DragOver += OnDragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DragOver -= OnDragOver;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            if (DragOverCommand != null && DragOverCommand.CanExecute(e))
            {
                DragOverCommand.Execute(e);
            }
        }
    }
}
