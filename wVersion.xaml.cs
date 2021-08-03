using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
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
using System.Text.RegularExpressions;

namespace oVersion
{
    /// <summary>
    /// Interaction logic for wVersion.xaml
    /// </summary>
    public partial class wVersion : DialogWindow
    {

        List<Project> Projects { get; set; } = new List<Project>();

        public wVersion()
        {

        }
        public wVersion(EnvDTE.Projects projs)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            InitializeComponent();
            b_save.Click += B_save_Click;
            b_cancel.Click += B_cancel_Click;

            foreach (Project item in projs)
            {
                this.Projects.Add(item);
            }

            lst.ItemsSource = this.Projects;
            lst.DisplayMemberPath = "Name";
            lst.SelectAll();

            lst.SelectionChanged += Lst_SelectionChanged;

            var props = Projects[0].Properties;

            var prop = GetProp(props, "Version") ?? GetProp(props, "AssemblyVersion");
            if (prop == null)
                return;

            var version = prop.Value as Version;
            if (version == null)
                version = GetVersion(prop.Value.ToString());

            if (version == null)
                return;

            t_major.Text = version.Major.ToString();
            t_minor.Text = version.Minor.ToString();
            t_build.Text = version.Build.ToString();
            t_rev.Text = version.Revision.ToString();

            Lst_SelectionChanged(null, null);
        }

        Version GetVersion(string version)
        {
            try
            {
                return new Version(version);
            }
            catch
            {
                return null;
            }
        }
        Property GetProp(Properties props, string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                return props.Item(path);
            }
            catch
            {
                return null;
            }
        }

        private void B_cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            h_count.Text = "Selected projects : " + lst.SelectedItems.Count;


        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void B_save_Click(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (Project item in lst.SelectedItems)
            {
                var prop = GetProp(item.Properties, "Version") ?? GetProp(item.Properties, "AssemblyVersion");
                if (prop == null)
                    continue;

                prop.Value = new Version(int.Parse(t_major.Text), int.Parse(t_minor.Text), int.Parse(t_build.Text), int.Parse(t_rev.Text));
                item.Save();
            }

            Close();
        }
    }
}
