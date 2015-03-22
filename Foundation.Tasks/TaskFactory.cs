using System;
using System.Collections;

namespace Foundation.Tasks
{
    /// <summary>
    /// A task encapsulates future work that may be waited on.
    /// - Support running actions in background threads 
    /// - Supports running coroutines with return results
    /// - Use the WaitForRoutine method to wait for the task in a coroutine
    /// </summary>
    /// <example>
    /// <code>
    ///     var task = Task.Run(() =>
    ///     {
    ///        //Debug.Log does not work in
    ///        Debug.Log("Sleeping...");
    ///        Task.Delay(2000);
    ///        Debug.Log("Slept");
    ///    });
    ///    // wait for it
    ///    yield return StartCoroutine(task.WaitRoutine());
    ///
    ///    // check exceptions
    ///    if(task.IsFaulted)
    ///        Debug.LogException(task.Exception)
    ///</code>
    ///</example>
    public partial class Task
    {
        #region Task
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task Run(Action action)
        {
            var task = new Task(action);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnMain(Action action)
        {
            var task = new Task(action, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnCurrent(Action action)
        {
            var task = new Task(action, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task Run<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnMain<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunOnCurrent<TP>(Action<TP> action, TP param)
        {
            var task = new Task(action, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        #endregion
        
        #region Coroutine

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(IEnumerator function)
        {
            var task = new Task(function);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(Func<IEnumerator> function)
        {
            var task = new Task(function());
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task RunCoroutine(Func<Task, IEnumerator> function)
        {
            var task = new Task();
            task.Strategy = TaskStrategy.Coroutine;
            task._routine = function(task);
            task.Start();
            return task;
        }
        #endregion

        #region Task With Result
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> Run<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param);
            task.Start();
            return task;
        }
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnMain<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function, TaskStrategy.MainThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnMain<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param, TaskStrategy.MainThread);
            task.Start();
            return task;
        } 
        
        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnCurrent<TResult>(Func<TResult> function)
        {
            var task = new Task<TResult>(function, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunOnCurrent<TParam, TResult>(Func<TParam, TResult> function, TParam param)
        {
            var task = new Task<TResult>(function, param, TaskStrategy.CurrentThread);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a new running task
        /// </summary>
        public static Task<TResult> RunCoroutine<TResult>(IEnumerator function)
        {
            var task = new Task<TResult>(function);
            task.Start();
            return task;
        }

        /// <summary>
        /// Creates a task which passes the task as a parameter
        /// </summary>
        public static Task<TResult> RunCoroutine<TResult>(Func<Task<TResult>, IEnumerator> function)
        {
            var task = new Task<TResult>();
            task.Strategy = TaskStrategy.Coroutine;
            task.Paramater = task;
            task._routine = function(task);
            task.Start();
            return task;
        }
        #endregion

        #region success / fails

        /// <summary>
        /// A default task in the success state
        /// </summary>
        static Task _successTask = new Task(TaskStrategy.Custom) { Status = TaskStatus.Success };

        /// <summary>
        /// A default task in the success state
        /// </summary>
        public static Task<T> SuccessTask<T>(T result)
        {
            return new Task<T>(TaskStrategy.Custom) { Status = TaskStatus.Success, Result = result };
        }

        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task SuccessTask()
        {
            return _successTask;
        }


        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task FailedTask(string exception)
        {
            return FailedTask(new Exception(exception));
        }

        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task FailedTask(Exception ex)
        {
            return new Task(TaskStrategy.Custom) { Status = TaskStatus.Faulted, Exception = ex };
        }

        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task<T> FailedTask<T>(string exception)
        {
            return FailedTask<T>(new Exception(exception));
        }

        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task<T> FailedTask<T>(Exception ex)
        {
            return new Task<T>(TaskStrategy.Custom) { Status = TaskStatus.Faulted, Exception = ex };
        }


        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task<T> FailedTask<T>(string exception, T result)
        {
            return FailedTask(new Exception(exception), result);
        }

        /// <summary>
        /// A default task in the faulted state
        /// </summary>
        public static Task<T> FailedTask<T>(Exception ex, T result)
        {
            return new Task<T>(TaskStrategy.Custom) { Status = TaskStatus.Faulted, Exception = ex, Result = result };
        }
        #endregion

    }
}
