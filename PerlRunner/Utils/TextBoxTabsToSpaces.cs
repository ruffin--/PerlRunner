using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace PerlRunner.Utils
{
    public class TextBoxTabsToSpaces : TextBox
    {
        private int _tabLength = 4;
        private string _tabSub = new string(' ', 4);

        public int TabLength
        {
            get
            {
                return _tabLength;
            }
            set
            {
                _tabLength = value;
                _tabSub = new string(' ', value);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab && !e.KeyboardDevice.IsKeyDown(Key.LeftCtrl) && !e.KeyboardDevice.IsKeyDown(Key.RightCtrl))
            {
                int intCaretLoc = base.CaretIndex;

                if (base.SelectedText.Contains(System.Environment.NewLine))
                {
                    intCaretLoc = base.SelectionStart;
                    base.SelectedText = base.SelectedText.Replace(System.Environment.NewLine, System.Environment.NewLine + _tabSub);
                }
                base.Text = base.Text.Insert(intCaretLoc, _tabSub);
                base.CaretIndex = intCaretLoc + _tabLength;
                e.Handled = true;
            }
            // TODO: Consider a switch for straight key checks.
            else if (e.Key == Key.Return)
            {
                base.SelectedText = string.Empty;
                string strAutoIndent = string.Empty;
                int intCaretLoc = base.CaretIndex;

                if (base.Text.Length > 0)
                {
                    int intPrevNewLine = base.Text.Substring(0, intCaretLoc).LastIndexOf(System.Environment.NewLine);
                    string strLastLine = intPrevNewLine >= 0 ? base.Text.Substring(intPrevNewLine + System.Environment.NewLine.Length, intCaretLoc - intPrevNewLine) :
                        base.Text.Substring(0, intCaretLoc);

                    int i = 0;
                    while (i < strLastLine.Length)
                    {
                        if (strLastLine[i].Equals(' '))
                        {
                            strAutoIndent += " ";
                        }
                        else
                        {
                            break;
                        }
                        i++;    // we coulda put in the if line, but that's just a little hipster-ist.
                    }

                }
                base.Text = base.Text.Insert(intCaretLoc, System.Environment.NewLine + strAutoIndent);
                base.CaretIndex = intCaretLoc + (System.Environment.NewLine + strAutoIndent).Length;
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                int intCaretLoc = base.CaretIndex;

                if (base.Text.Length >= _tabLength && base.SelectedText.Length.Equals(0))
                {
                    // TODO: This is horribly inefficient. These strings could be giant.
                    string strToCaret = base.Text.Substring(0, intCaretLoc);
                    if (strToCaret.EndsWith(_tabSub))
                    {
                        strToCaret = strToCaret.Substring(0, strToCaret.Length - _tabLength);
                        base.Text = strToCaret + base.Text.Substring(intCaretLoc);
                        base.CaretIndex = intCaretLoc - _tabLength;
                        e.Handled = true;
                    }
                }
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}