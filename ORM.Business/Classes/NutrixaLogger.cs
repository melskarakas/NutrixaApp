using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Business.Classes
{
    public static class NutrixaLogger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Logger));

        static NutrixaLogger()
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            hierarchy.Root.RemoveAllAppenders();

            var patternLayout = new PatternLayout
            {
                ConversionPattern = "%date %-5level - %message%newline%exception"
            };
            patternLayout.ActivateOptions();

            string logFolder = @"C:\NutrixaLogs";
            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            var roller = new RollingFileAppender
            {
                AppendToFile = true,
                File = logFolder+ "\\",
                Layout = patternLayout,
                RollingStyle = RollingFileAppender.RollingMode.Date,
                DatePattern = "yyyy-MM-dd'.txt'",
                StaticLogFileName = false
            };

            roller.ActivateOptions();

            hierarchy.Root.AddAppender(roller);
            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }

        public static void LogError(string mesaj, Exception ex)
        {
            log.Error($"[ERROR]- {mesaj}", ex);
        }
        public static void LogInfo(string mesaj)
        {
            log.Info($"[INFO]- {mesaj}");
        }
    }
}
