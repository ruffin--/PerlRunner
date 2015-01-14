using PerlRunner.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace PerlRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string _openFile = string.Empty;
        string _wherePerl = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string output = string.Empty;
            string err = string.Empty;

            try
            {
                this.txtOutput.Text = CmdHelper.Execute(@"C:\strawberry\perl\bin\perl.exe", _openFile);
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = "Unable to execute Perl script.\n\n" + ex.ToString();
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;    // Man, this is wonkily scoped. Fires only when within context. Don't think you can have it true from start.
        }

        private void CommandBinding_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                // http://msdn.microsoft.com/en-us/library/aa969773(v=vs.110).aspx#Common_Dialogs
                // Configure open file dialog box
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.FileName = "temp.pl"; // Default file name
                dlg.DefaultExt = ".pl"; // Default file extension
                dlg.Filter = "Perl scripts (.pl)|*.pl|All Files|*.*"; // Filter files by extension 

                // Show open file dialog box
                Nullable<bool> result = dlg.ShowDialog();

                // Process open file dialog box results 
                if (result.HasValue && result.Value)
                {
                    // Open document 
                    _openFile = dlg.FileName;
                    this.Title = _openFile;
                    CommandManager.InvalidateRequerySuggested();
                    this.txtCode.Text = File.ReadAllText(_openFile);
                    this.txtCode.Focus();
                }
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = ("Unable to open file.\n\n" + ex.ToString());
            }

        }

        private void CommandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(_openFile);
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                File.WriteAllText(_openFile, this.txtCode.Text);
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = "Saving the file didn't work.\n\n" + ex.ToString();
            }
        }

        private void CommandBinding_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
            bool success = false;
            try
            {
                _wherePerl = CmdHelper.Execute("where.exe", "perl");
                this.txtOutput.Text += "\n\nFound Perl installation here:\n" + _wherePerl;
                CommandManager.InvalidateRequerySuggested();
                success = true;
            }
            catch (Exception)
            {
                success = false;    // overly explicit.
            }

            if (!success || string.IsNullOrWhiteSpace(_wherePerl))
            {
                _wherePerl = @"C:\Strawberry\perl\bin\perl.exe";
            }
        }

    }
}
