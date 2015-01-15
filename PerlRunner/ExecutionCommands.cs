using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PerlRunner
{
    public static class ExecutionCommands
    {
        static RoutedUICommand executePerl = new RoutedUICommand("GO!!!",
            "ExecutePerl", typeof(ExecutionCommands));

        static RoutedUICommand closeTab = new RoutedUICommand("Close Tab",
            "CloseTab", typeof(ExecutionCommands));

        public static RoutedUICommand ExecutePerl
        {
            get { return executePerl; }
        }

        public static RoutedUICommand CloseTab
        {
            get { return closeTab; }
        }
    }
}
