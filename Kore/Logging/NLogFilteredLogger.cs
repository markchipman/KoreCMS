﻿using System;
using Castle.Core.Logging;
using NLog;

namespace Kore.Logging
{
    public class NLogFilteredLogger : LevelFilteredLogger
    {
        private readonly Logger logger;

        #region Constructors

        public NLogFilteredLogger(string name)
        {
            logger = LogManager.GetLogger(name);

            if (logger.IsDebugEnabled)
            {
                Level = LoggerLevel.Debug;
            }
            else if (logger.IsInfoEnabled)
            {
                Level = LoggerLevel.Info;
            }
            else if (logger.IsWarnEnabled)
            {
                Level = LoggerLevel.Warn;
            }
            else if (logger.IsErrorEnabled)
            {
                Level = LoggerLevel.Error;
            }
            else if (logger.IsFatalEnabled)
            {
                Level = LoggerLevel.Fatal;
            }
        }

        public NLogFilteredLogger(Type type)
            : this(type.FullName)
        {
        }

        public NLogFilteredLogger(string name, LoggerLevel level)
        {
            logger = LogManager.GetLogger(name);
            Level = level;
        }

        public NLogFilteredLogger(Type type, LoggerLevel level)
            : this(type.FullName, level)
        {
        }

        #endregion Constructors

        public override ILogger CreateChildLogger(string loggerName)
        {
            throw new NotSupportedException("NLog does not support child loggers.");
        }

        protected override void Log(LoggerLevel loggerLevel, string loggerName, string message, Exception exception)
        {
            switch (loggerLevel)
            {
                case LoggerLevel.Off: break;
                case LoggerLevel.Fatal: logger.Log(LogLevel.Fatal, message, exception); break;
                case LoggerLevel.Error: logger.Log(LogLevel.Error, message, exception); break;
                case LoggerLevel.Warn: logger.Log(LogLevel.Warn, message, exception); break;
                case LoggerLevel.Info: logger.Log(LogLevel.Info, message, exception); break;
                case LoggerLevel.Debug: logger.Log(LogLevel.Debug, message, exception); break;
                default: throw new ArgumentOutOfRangeException("loggerLevel");
            }
        }
    }
}