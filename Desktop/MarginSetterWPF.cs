using System.Windows;

namespace AAM.Helpers.Desktop
{
    /// <summary>
    /// A class to provide means of setting margin to all child elements in StackPanel. <br></br>
    /// Reference: <a href="https://gist.github.com/angularsen/90040fb174f71c5ab3ad">https://gist.github.com/angularsen/90040fb174f71c5ab3ad</a> <br></br>
    /// Usage Example: StackPanel local:MarginSetter.Margin="5"  &lt;== you need to put that in XAML file style (Style attribute)
    /// </summary>
    public class MarginSetterWPF
    {
        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarginProperty);
        }

        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetterWPF), new UIPropertyMetadata(new Thickness(), MarginChangedCallback));

        public static void MarginChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure this is put on a panel
            var panel = sender as System.Windows.Controls.Panel;

            if (panel == null) return;


            panel.Loaded += new RoutedEventHandler(panel_Loaded);

        }

        static void panel_Loaded(object sender, RoutedEventArgs e)
        {
            var panel = sender as System.Windows.Controls.Panel;

            // Go over the children and set margin for them:
            foreach (var child in panel.Children)
            {
                var fe = child as FrameworkElement;

                if (fe == null) continue;

                fe.Margin = MarginSetterWPF.GetMargin(panel);
            }
        }
    }
}
