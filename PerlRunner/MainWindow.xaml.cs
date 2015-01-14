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
        string _wherePerl = string.Empty;

        TabItem _tabActive
        {
            get
            {
                return (TabItem)this.TabOFiles.SelectedItem;
            }
        }

        TextBox _txtActive
        {
            get
            {
                return (TextBox)_tabActive.Content;
            }
        }

        string _activeFileLoc
        {
            get
            {
                string headerVal = ((TabItem)this.TabOFiles.SelectedItem).Header.ToString();
                return this.dFileLocs[headerVal];
            }
        }

        Dictionary<string, string> dFileLocs = new Dictionary<string, string>();

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
                if (!_tabActive.Name.Equals("FauxTab"))
                {
                    _saveBuffer(_tabActive);
                    this.txtOutput.Text = CmdHelper.Execute(_wherePerl, _activeFileLoc);
                }
            }
            catch (Exception ex)
            {
                this.txtOutput.Text += "Unable to execute Perl script.\n\n" + ex.ToString();
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
                    // NOTE: Never mix data with view like this, okay? LOOK AWAY!!! I'M HIDEOUS!!!
                    string openFile = dlg.FileName;
                    string openFileName = dlg.FileName.Substring(dlg.FileName.LastIndexOf(@"\") + 1);

                    if (dFileLocs.ContainsKey(openFileName))
                    {
                        this.txtOutput.Text = "A file named " + openFileName + " is already open. Please restart PerlRunner\n"
                            + "(or, in a later version, close the existing tab) to open this file.\n"
                            + "\n"
                            + "With any luck, this won't be a limitation in the future.\n"
                            + "\n"
                            + "\t(It is on github, and pull requests are welcome, you know. ;^D)";
                        MessageBox.Show("Not gonna open that right now.\n\n(see bottom window)", "Not gonna do it.");
                    }
                    else
                    {
                        this.dFileLocs.Add(openFileName, openFile);

                        TextBoxTabsToSpaces txt = new TextBoxTabsToSpaces();
                        txt.FontFamily = new FontFamily("Courier New");
                        txt.AcceptsReturn = true;
                        txt.AcceptsTab = true;
                        txt.Background = Brushes.LightBlue;
                        txt.Text = File.ReadAllText(openFile);

                        TabItem tab = new TabItem();
                        tab.Header = openFileName;
                        tab.Content = txt;
                        this.TabOFiles.Items.Insert(this.TabOFiles.Items.Count, tab);
                        this.TabOFiles.SelectedItem = tab;
                        CommandManager.InvalidateRequerySuggested();

                        txt.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = ("Unable to open file.\n\n" + ex.ToString());
            }

        }

        private void CommandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrWhiteSpace(_txtActive.Text);
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _saveBuffer(_tabActive);
        }

        private void CommandBinding_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Loaded");
            bool success = false;

            this.Title = string.Format("PerlRunner {0} (c) 2015  !!!!USE AT YOUR OWN RISK!!!!", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

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
                this.txtOutput.Text += "\n\n!!! Unable to find perl installation. Using:\n" + _wherePerl;
            }

            this.txtOutput.Text += "\nApp icon is part of the Creative Commons and requires attribution for the author: Nicolas Mollet.\n"
                + "Thanks Nicolas.";
        }

        private string _fileForTab(TabItem tab)
        {
            return this.dFileLocs[tab.Header.ToString()];
        }

        private void _saveBuffer(TabItem tabIn)
        {
            try
            {
                TextBox txt = (TextBox)tabIn.Content;
                File.WriteAllText(_fileForTab(tabIn), txt.Text);
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = "Saving the file didn't work.\n\n" + ex.ToString();
            }
        }
    }
}
