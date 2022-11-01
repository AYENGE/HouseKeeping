using System;
using System.Configuration;
using System.Reflection;
using log4net;
using MOH.BatchJobs.TriggerEmailAutomateFile.Common;

namespace MOH.BatchJobs.HouseKeeping
{
    class Program
    {
        public static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                _log.Info("MOH.BatchJobs.HouseKeeping START");
                HouseKeep houseKeep = new HouseKeep();
                houseKeep.HouseKeeping();
                //Send email
                EmailSend emailSend = new EmailSend();
                string _EmailSubject = ConfigurationManager.AppSettings["EmailSubject"];
                string htmlString = @"<html><body>";
                htmlString = htmlString + @"<p>";
                htmlString = htmlString + "Hi Team" + "," + @"</p> <P>";
                htmlString = htmlString + "House Keeping is done... " + @"</p> <p>";
                htmlString = htmlString + @"  <p> <b>Note : </b> Please do not reply to this email. </p> <p> Thank you </p> </body> </html> ";
                emailSend.SendingEmail("", _EmailSubject, htmlString);
                _log.Info("MOH.BatchJobs.HouseKeeping End");
            }
            catch (Exception ex)
            {
                _log.Error("# ERROR: " + ex.Message);
                _log.Error("# STACK TRACE: " + ex.StackTrace);
            }
        }
    }
}
