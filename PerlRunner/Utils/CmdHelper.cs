using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerlRunner.Utils
{
    public class CmdHelper
    {
        public static string Execute(string strExePath, string strArgs)
        {
            string strReturn = string.Empty;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = new System.Diagnostics.ProcessStartInfo(strExePath, strArgs);
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;

            p.Start();
            // instead of p.WaitForExit(), do
            StringBuilder sbOut = new StringBuilder();
            StringBuilder sbErr = new StringBuilder();

            while (!p.HasExited)
            {
                sbOut.Append(p.StandardOutput.ReadToEnd());
                sbErr.Append(p.StandardError.ReadToEnd());
            }

            strReturn = sbOut.ToString();
            if (0 < sbErr.Length) strReturn += System.Environment.NewLine 
                + System.Environment.NewLine
                + "----------------" + System.Environment.NewLine
                + "*** WARNINGS ***" + System.Environment.NewLine
                + "----------------" + System.Environment.NewLine
                + sbErr.ToString();

            return strReturn;
        }
    }
}
