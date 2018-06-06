﻿using BotMessageRouting.MessageRouting.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BotMessageRouting.MessageRouting.Handlers
{
    public class ExceptionHandler
    {
        private readonly ILogger _logger;

        public ExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Execute a potentially unsafe asynchronous function and handle any exception that happens in a clean manner
        /// </summary>
        /// <typeparam name="TContract">The type of result expected</typeparam>
        /// <param name="unsafeFunction">The unsafe function reference, typically a lambda expression</param>
        /// <param name="returnDefaultType">Set to false to re-throw the exception. When true, the default of the type is returned</param>
        /// <param name="customHandler">For custom handling of the exception, add a delegate here that accepts an exception as input</param>
        /// <param name="callerMemberName">Name of the offending method that crashed. Can be replaced with a custom message if you want</param>        
        public Task<TContract> GetAsync<TContract>(Func<Task<TContract>> unsafeFunction, bool returnDefaultType = true, Action<Exception> customHandler = null, [CallerMemberName] string callerMemberName = "")
        {
            if (unsafeFunction == null)
            {
                _logger.LogWarning("The function delegate to execute was null", callerMemberName);
                return Task.FromResult(default(TContract));
            }

            try
            {
                return unsafeFunction.Invoke();
            }
            catch(Exception ex)
            {
                if (customHandler != null)
                {
                    customHandler(ex);
                }
                else
                {
                    _logger.LogException(ex, methodName: callerMemberName);
                }
                if (!returnDefaultType)
                    throw;
            }
            return Task.FromResult(default(TContract));       
        }

        /// <summary>
        /// Execute a potentially unsafe function and handle any exception that happens in a clean manner
        /// </summary>
        /// <param name="unsafeFunction">The unsafe function reference, typically a lambda expression</param>
        /// <param name="returnDefaultType">Set to false to re-throw the exception. When true, the default of the type is returned</param>
        /// <param name="customHandler">For custom handling of the exception, add a delegate here that accepts an exception as input</param>
        /// <param name="callerMemberName">Name of the offending method that crashed. Can be replaced with a custom message if you want</param>        
        public TResult Get<TResult>(Func<TResult> unsafeFunction, bool returnDefaultType = true, Action<Exception> customHandler = null, [CallerMemberName] string callerMemberName = "")
        {
            if (unsafeFunction == null)
            {
                _logger.LogWarning("The function delegate to execute was null", callerMemberName);
                return default(TResult);
            }

            try
            {
                return unsafeFunction.Invoke();
            }
            catch(Exception ex)
            {
                if (customHandler != null)
                {
                    customHandler(ex);
                }
                else
                {
                    _logger.LogException(ex, methodName: callerMemberName);                    
                }
                if (!returnDefaultType)
                    throw;
            }
            return default(TResult);
        }

        /// <summary>
        /// Executes a potentially unsafe asynchronous method. 
        /// </summary>
        /// <param name="unsafeFunction">A method delegate that returns an (awaitable) task</param>
        /// <param name="customHandler">For custom handling of the exception, add a delegate here that accepts an exception as input</param>
        /// <param name="callerMemberName">Name of the offending method that crashed. Can be replaced with a custom message if you want</param>        
        /// <returns></returns>
        public Task ExecuteAsync(Func<Task> unsafeFunction, Action<Exception> customHandler = null, [CallerMemberName] string callerMemberName = "")
        {
            if (unsafeFunction == null)
            {
                _logger.LogWarning("The function delegate to execute was null", callerMemberName);
                return Task.CompletedTask;
            }

            try
            {
                return unsafeFunction.Invoke();
            }
            catch(Exception ex)
            {
                if (customHandler != null)
                {
                    customHandler(ex);
                }
                else
                {
                    _logger.LogException(ex, methodName: callerMemberName);
               }
            }
            return Task.CompletedTask;
        }
    }
}
