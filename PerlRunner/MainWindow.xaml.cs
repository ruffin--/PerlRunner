﻿using PerlRunner.Utils;
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
        const string cstrNewFile = "New file";

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
                bool bSaved = true;
                if (_tabActive.Header.Equals(cstrNewFile))
                {
                    bSaved = _saveAsOrSaveNew();
                }
                else
                {
                    _saveBuffer(_tabActive);
                }

                if (bSaved)
                {
                    string argsForPerl = _activeFileLoc;

                    // Kludgey options from menus setup.
                    if (this.mniOptionl.IsChecked)
                    {
                        argsForPerl = "-l " + argsForPerl;
                    }
                    if (this.mniOptionW.IsChecked)
                    {
                        argsForPerl = "-W " + argsForPerl;
                    }
                    // eo kludgey options from menus setup.

                    this.txtOutput.Text = CmdHelper.Execute(_wherePerl, argsForPerl);
                }
                else
                {
                    this.txtOutput.Text = "Code not executed because new file is unsaved to disk.\nHopefully not an issue in a later version.";
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

        private void CommandBinding_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
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
                    string openFilePath = dlg.FileName;
                    string openFileName = dlg.FileName.Substring(dlg.FileName.LastIndexOf(@"\") + 1);

                    _createTab(openFileName, openFilePath);
                }
            }
            catch (Exception ex)
            {
                this.txtOutput.Text = ("Unable to open file.\n\n" + ex.ToString());
            }
        }

        private void CommandBinding_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.TabOFiles.Items.Count > 0;
        }

        private void CommandBinding_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_tabActive.Header.Equals(cstrNewFile))
                _saveAsOrSaveNew();
            else
                _saveBuffer(_tabActive);
        }

        private void CommandBinding_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _createTab(cstrNewFile);
        }

        private void CommandBinding_SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.TabOFiles.Items.Count > 0;
        }

        private void CommandBinding_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!_tabActive.Header.Equals("Open a File"))
                _saveAsOrSaveNew();
        }

        private void CommandBinding_CloseTab_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.TabOFiles.Items.Count > 0;
        }

        private void CommandBinding_CloseTab_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (this.TabOFiles.Items.Count > 0) // overly defensive. CanExecute should've stopped it already.
                {
                    if (_tabActive.Header.Equals(cstrNewFile))
                    {
                        MessageBoxResult result = MessageBox.Show("Save new file before closing?", "Save new file?", 
                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (MessageBoxResult.Yes == result)
                        {
                            _saveBuffer(_tabActive);
                            this.txtOutput.Text += "\nSaved and now closing tab.";
                        }
                    }
                    else
                    {
                        _saveBuffer(_tabActive);
                        this.txtOutput.Text = "Saving before closing."; // TODO: Check for unsaved edits instead.
                    }

                    this.dFileLocs.Remove(_tabActive.Header.ToString());
                    this.TabOFiles.Items.Remove(_tabActive);
                }
            }
            catch (Exception ex)
            {
                this.txtOutput.Text += "Unable to close tab.\n\n" + ex.ToString();
            }
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
                + "Thanks Nicolas.\n";
        }

        private void _createTab(string openFileName, string openFilePath = "")
        {
            if (dFileLocs.ContainsKey(openFileName))
            {
                string strTextWarning = string.Empty;
                string strMsgBoxWarning = string.Empty;

                if (openFileName.Equals(cstrNewFile))
                {
                    strTextWarning = "You already have a new file open. Please save\n"
                        + "the existing new file before opening another.\n"
                        + "\n"
                        + "With any luck, this won't be a limitation in the future.\n"
                        + "\n"
                        + "\t(It is on github, and pull requests are welcome, you know. ;^D)";
                    strMsgBoxWarning = "Save existing new file first, please.\n\n(see bottom window)";
                }
                else
                {
                    strTextWarning = "A file named " + openFileName + " is already open. Please close the\n"
                        + "existing tab by this name to open this file.\n"
                        + "\n"
                        + "With any luck, this won't be a limitation in the future.\n"
                        + "\n"
                        + "\t(It is on github, and pull requests are welcome, you know. ;^D)";
                    strMsgBoxWarning = "Not gonna open that right now.\n\n(see bottom window)";
                }
                this.txtOutput.Text = strTextWarning;
                MessageBox.Show(strMsgBoxWarning, "Not gonna do it.");
            }
            else
            {
                this.dFileLocs.Add(openFileName, openFilePath);

                TextBoxTabsToSpaces txt = new TextBoxTabsToSpaces();
                txt.FontFamily = new FontFamily("Courier New");
                txt.AcceptsReturn = true;
                txt.AcceptsTab = true;
                txt.Background = Brushes.LightBlue;
                if (!string.IsNullOrEmpty(openFilePath)) txt.Text = File.ReadAllText(openFilePath);
                txt.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                txt.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                TabItem tab = new TabItem();
                tab.Header = openFileName;
                tab.Content = txt;
                this.TabOFiles.Items.Insert(this.TabOFiles.Items.Count, tab);
                this.TabOFiles.SelectedItem = tab;
                CommandManager.InvalidateRequerySuggested();

                tab.Focus();
                UpdateLayout();
                txt.Focus();

                this.txtOutput.Text = openFileName.Equals(cstrNewFile) ? "New file created" : openFileName + " opened successfully.";
            }
        }

        private bool _saveAsOrSaveNew()
        {
            bool bSavedSuccessfully = false;
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = string.Empty;
            dlg.DefaultExt = ".pl"; // Default file extension
            dlg.Filter = "Perl scripts (.pl)|*.pl|All Files|*.*"; // Filter files by extension 

            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result.HasValue && result.Value)
            {
                // Open document
                // NOTE: Never mix data with view like this, okay? LOOK AWAY!!! I'M HIDEOUS!!!
                string saveFilePath = dlg.FileName;
                string saveFileName = saveFilePath.Substring(saveFilePath.LastIndexOf(@"\") + 1);

                if (saveFileName.Equals(cstrNewFile))
                {
                    string strMsg = @"No, you can't call your saved file ""New file""\n"
                        + "without an extension, smart-aleck. ;^)";
                    this.txtOutput.Text = strMsg;
                    MessageBox.Show(strMsg, "I see what you did there.");
                }
                else if (dFileLocs.ContainsKey(saveFileName))
                {
                    this.txtOutput.Text = "A file named " + saveFileName + " is already open. Please \n"
                        + "close the existing tab by this name to save this file with that name.\n"
                        + "\n"
                        + "With any luck, this won't be a limitation in the future.\n"
                        + "\n"
                        + "\t(This app is on github, and pull requests are welcome, you know. ;^D)";
                    MessageBox.Show("Not gonna open that right now.\n\n(see bottom window)", "Not gonna do it.");
                }
                else
                {
                    dFileLocs.Remove(_tabActive.Header.ToString());
                    dFileLocs.Add(saveFileName, saveFilePath);
                    _tabActive.Header = saveFileName;
                    _saveBuffer(_tabActive);
                    bSavedSuccessfully = true;

                    this.txtOutput.Text = saveFileName + " saved successfully.";
                }
            }

            return bSavedSuccessfully;
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
