using log4net;
using System;
using System.Data.Entity.Validation;
using System.Security.Principal;

namespace Corno.Logger
{
    public class LogHandler
    {
        public enum LogType
        {
            General,
            Notify
        }

        private const string DefaultApplicationLogger = "Application";

        /// <summary>
        ///     Logs the error
        /// </summary>
        /// <param name="exception"></param>
        public static Exception LogError(Exception exception)
        {
            var logger = LogManager.GetLogger(DefaultApplicationLogger);

            var message = exception.Message;

            if (exception.GetType() == typeof(DbEntityValidationException))
            {
                var dbEntityValidationException = exception as DbEntityValidationException;

                if (dbEntityValidationException != null)
                {
                    foreach (var eve in dbEntityValidationException.EntityValidationErrors)
                    {
                        message =
                            $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                        foreach (var ve in eve.ValidationErrors)
                            message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                    }
                }
            }

            if (exception.InnerException != null)
            {
                message = exception.InnerException.Message;
                exception = exception.InnerException;

                if (exception.InnerException != null)
                {
                    message = exception.InnerException.Message;
                    exception = exception.InnerException;
                }
            }

            if (logger.IsErrorEnabled)
            {
                logger.Error(message, exception);
            }

            return exception;
            //MessageBox.Show(message);
        }

        /// <summary>
        ///     Logs the error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="url"></param>
        /// <param name="error"></param>
        public static void LogError(string message, IPrincipal user, Uri url, Exception error)
        {
            SetOptionalParametersOnLogger(user, url);
            LogError(error);
        }

        /// <summary>
        ///     Logs the error
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void LogInfo(string message, LogType type)
        {
            var logger = LogManager.GetLogger(type == LogType.Notify ? LogType.Notify.ToString() : DefaultApplicationLogger);
            if (logger.IsInfoEnabled)
            {
                logger.Info(message);
            }
        }

        /// <summary>
        ///     Logs the warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public static void LogWarning(string message, Exception error)
        {
            var logger = LogManager.GetLogger(DefaultApplicationLogger);
            if (error.InnerException != null)
            {
                error = error.InnerException;
            }
            if (logger.IsWarnEnabled)
            {
                logger.Warn(message, error);
            }
        }

        /// <summary>
        ///     Logs the warning
        /// </summary>
        /// <param name="message"></param>
        /// <param name="user"></param>
        /// <param name="url"></param>
        /// <param name="error"></param>
        public static void LogWarning(string message, IPrincipal user, Uri url, Exception error)
        {
            SetOptionalParametersOnLogger(user, url);
            LogWarning(message, error);
        }

        /// <summary>
        ///     Sets up optional parameter on logger
        /// </summary>
        /// <param name="user"></param>
        /// <param name="url"></param>
        private static void SetOptionalParametersOnLogger(IPrincipal user, Uri url)
        {
            //set user to log4net context, so we can use %X{user} in the appenders
            if ((user != null) && user.Identity.IsAuthenticated)
            {
                MDC.Set("user", user.Identity.Name);
            }
            //set url to log4net context, so we can use %X{url} in the appenders
            MDC.Set("url", url.ToString());
        }


        public static string GetDetailError(Exception exception)
        {
            var message = exception.Message;

            if (exception.GetType() == typeof(DbEntityValidationException))
            {
                var dbEntityValidationException = exception as DbEntityValidationException;

                if (dbEntityValidationException != null)
                    foreach (var eve in dbEntityValidationException.EntityValidationErrors)
                    {
                        message =
                            $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                        foreach (var ve in eve.ValidationErrors)
                            message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                    }
            }

            if (exception.InnerException == null) return message;

            message = exception.InnerException.Message;
            if (exception.InnerException != null)
                message = exception.InnerException.Message;

            return message;
        }
    }
}