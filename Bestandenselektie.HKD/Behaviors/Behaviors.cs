using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Bestandenselektie.HKD.Behaviors
{
    public static class Behaviors
    {
        public static readonly DependencyProperty ExpandingBehaviorProperty =
            DependencyProperty.RegisterAttached("ExpandingBehavior", typeof(ICommand), typeof(Behaviors),
                new PropertyMetadata(OnExpandingBehaviorChanged));

        public static void SetExpandingBehavior(DependencyObject o, ICommand value)
        {
            o.SetValue(ExpandingBehaviorProperty, value);
        }
        
        public static ICommand GetExpandingBehavior(DependencyObject o)
        {
            return (ICommand)o.GetValue(ExpandingBehaviorProperty);
        }
        
        private static void OnExpandingBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeViewItem treeViewItem)
            {
                if (e.NewValue is ICommand command)
                {
                    treeViewItem.Expanded += (sender, args) =>
                    {
                        if (command.CanExecute(args))
                        {
                            command.Execute(args);

                        }

                        args.Handled = true;
                    };
                }
            }
        }
    }
}
