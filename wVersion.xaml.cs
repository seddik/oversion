using EnvDTE;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
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
        Solution Solution { get; set; }
        public wVersion()
        {

        }
        public wVersion(EnvDTE.Projects projs, EnvDTE.Solution solution)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            InitializeComponent();
            b_save.Click += B_save_Click;
            b_cancel.Click += B_cancel_Click;

            Solution = solution;

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
            t_minor.Text = version.Minor >= 0 ? version.Minor.ToString() : "";
            t_build.Text = version.Build >= 0 ? version.Build.ToString() : "";
            t_rev.Text = version.Revision >= 0 ? version.Minor.ToString() : "";

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
        Property GetProp(EnvDTE.Properties props, string path)
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

                if (string.IsNullOrEmpty(t_major.Text))
                {
                    MessageBox.Show("Major version can't be empty");
                    return;
                }
                if (string.IsNullOrEmpty(t_minor.Text) && (!string.IsNullOrEmpty(t_build.Text) || !string.IsNullOrEmpty(t_rev.Text)))
                {
                    MessageBox.Show("Minor version is empty but build or revision number isn't");
                    return;
                }
                if (string.IsNullOrEmpty(t_build.Text) && !string.IsNullOrEmpty(t_rev.Text))
                {
                    MessageBox.Show("Build number is empty but the revision number isn't");
                    return;
                }

                var ver = new Version(int.Parse(t_major.Text).ToString() + (string.IsNullOrWhiteSpace(t_minor.Text) ? "" : "." + int.Parse(t_minor.Text))
                            + (string.IsNullOrWhiteSpace(t_build.Text) ? "" : "." + int.Parse(t_build.Text))
                            + (string.IsNullOrWhiteSpace(t_rev.Text) ? "" : "." + int.Parse(t_rev.Text))
                            );

                prop.Value = ver;

                //prop = GetProp(item.Properties, "AssemblyFileVersion");
                //if (prop != null)
                //    prop.Value = ver;

                item.Save();



            }

            Close();
        }




    }
}
