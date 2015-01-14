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
                base.SelectedText = string.Empty;
                int intCaretLoc = base.CaretIndex;
                base.Text = base.Text.Insert(intCaretLoc, _tabSub);
                base.CaretIndex = intCaretLoc + _tabLength;
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}