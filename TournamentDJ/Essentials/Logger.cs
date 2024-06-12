using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Reflection;
using System.ComponentModel;
using Windows.ApplicationModel.Store.Preview.InstallControl;
using System.Collections.ObjectModel;

namespace TournamentDJ.Essentials
{
    //This is a Singleton
    public class Logger : NotifyObject
    {
        private static readonly Logger _logger = new Logger();
        static Logger() 
        { 
        }

        private Logger() 
        { 
        }
        
        public static Logger LoggerInstance { get { return _logger; } }

        private string m_exePath = string.Empty;
        public string LogText
        {
            get { return Get<string>(); }
            private set { Set(value); }
        }

        public void CreateLog()
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            LogWrite("--------LOGFILE CREATED---------");
        }
        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                StringBuilder sb = new StringBuilder(LogText);
                StringBuilder newEntry = new StringBuilder();

                newEntry.AppendFormat("\r\n");
                newEntry.AppendFormat("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                newEntry.AppendFormat("  {0}", logMessage);

                txtWriter.Write(newEntry);
                sb.Append(newEntry);

                LogText = sb.ToString();
            }

            catch (Exception ex)
            {
            }
        }
    }
}