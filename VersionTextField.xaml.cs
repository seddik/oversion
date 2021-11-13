using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace oVersion
{
    /// <summary>
    /// Interaction logic for VersionTextField.xaml
    /// </summary>
    public partial class VersionTextField : UserControl
    {
        public VersionTextField()
        {
            InitializeComponent();

            btn.Click += Btn_Click;
            chk.Click += Chk_Click;
        }

        private void Chk_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text))
                Text = "0";
            else
                Text = new string((Text ?? "").Where(X => char.IsDigit(X)).ToArray());

            Text = (Convert.ToInt32(Text) + 1).ToString();

        } 
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(VersionTextField), new PropertyMetadata("0", OnChange));

        private static void OnChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var t = d as VersionTextField;
            if (t == null)
                return;

            t.txt.Text = e.NewValue?.ToString();
        }


         
    }
}
