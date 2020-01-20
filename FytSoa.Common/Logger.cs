using System;
using System.IO;
using System.Text;
using NLog;
using NLog.Web;

namespace FytSoa.Common
{
    /// <summary>
    /// 日志模块
    /// </summary>
    public class Logger
    {
        NLog.Logger _logger;
        private Logger(NLog.Logger logger)
        {
            _logger = logger;
        }
        public Logger(string name) : this(LogManager.GetLogger(name))
        {

        }

        /// <summary>
        /// 单例
        /// </summary>
        public static Logger Default { get; private set; }
        static Logger()
        {
            Default = new Logger(LogManager.GetCurrentClassLogger());
        }

        /// <summary>
        /// 自定义写日志路径
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="path">写入地址</param>
        /// <returns></returns>
        public void Process(string msg, string path="")
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var config = new NLog.Config.LoggingConfiguration();
            var logfile = new NLog.Targets.FileTarget() { FileName = "logs/${date:format=yyyyMM}/${shortdate}_log.txt", Encoding = Encoding.GetEncoding("GB2312") };
            if (!string.IsNullOrEmpty(path))
            {
                path += "/";
                logfile = new NLog.Targets.FileTarget() { FileName = "logs/" + path + "${shortdate}_log.txt", Encoding = Encoding.GetEncoding("GB2312") };
            }
            config.LoggingRules.Add(new NLog.Config.LoggingRule("*", LogLevel.Debug, logfile));
            LogManager.Configuration = config;
            _logger.Debug(msg);
        }

        #region Debug
        public void Debug(string msg, params object[] args)
        {
            _logger.Debug(msg, args);
            LogManager.Shutdown();
        }

        public void Debug(string msg, Exception err)
        {
            _logger.Debug(err, msg);
            LogManager.Shutdown();
        }
        #endregion

        #region Info
        public void Info(string msg, params object[] args)
        {
            _logger.Info(msg, args);
            LogManager.Shutdown();
        }

        public void Info(string msg, Exception err)
        {
            _logger.Info(err, msg);
            LogManager.Shutdown();
        }
        #endregion

        #region Warn
        public void Warn(string msg, params object[] args)
        {
            _logger.Warn(msg, args);
            LogManager.Shutdown();
        }

        public void Warn(string msg, Exception err)
        {
            _logger.Warn(err, msg);
            LogManager.Shutdown();
        }
        #endregion

        #region Trace
        public void Trace(string msg, params object[] args)
        {
            _logger.Trace(msg, args);
            LogManager.Shutdown();
        }

        public void Trace(string msg, Exception err)
        {
            _logger.Trace(err, msg);
            LogManager.Shutdown();
        }
        #endregion

        #region Error
        public void Error(string msg, params object[] args)
        {
            _logger.Error(msg, args);
            LogManager.Shutdown();
        }

        public void Error(string msg, Exception err)
        {
            _logger.Error(err, msg);
            LogManager.Shutdown();
        }
        #endregion

        #region Fatal
        public void Fatal(string msg, params object[] args)
        {
            _logger.Fatal(msg, args);
            LogManager.Shutdown();
        }

        public void Fatal(string msg, Exception err)
        {
            _logger.Fatal(err, msg);
            LogManager.Shutdown();
        }
        #endregion
    }
}
