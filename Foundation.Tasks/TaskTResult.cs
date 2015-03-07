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
    public class Task<TResult> : Task
    {
        #region public fields
        Func<TResult> _function;
        Delegate _function2;

        /// <summary>
        /// get the result of the task. Blocking. It is recommended you yield on the wait before accessing this value
        /// </summary>
        public TResult Result;

        #endregion

        #region ctor

        public Task()
        {

        }

        /// <summary>
        /// Returns the task in the Success state.
        /// </summary>
        /// <param name="result"></param>
        public Task(TResult result)
            : this()
        {
            Status = TaskStatus.Success;
            Strategy = TaskStrategy.Custom;
            Result = result;
        }

        /// <summary>
        /// Creates a new background Task strategy
        /// </summary>
        /// <param name="function"></param>
        public Task(Func<TResult> function)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            _function = function;
            Strategy = TaskStrategy.BackgroundThread;
        }

        /// <summary>
        /// Creates a new background Task strategy
        /// </summary>
        public Task(Delegate function, object param)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            _function2 = function;
            Paramater = param;
            Strategy = TaskStrategy.BackgroundThread;
        }

        /// <summary>
        /// Creates a new task with a specific strategy
        /// </summary>
        public Task(Func<TResult> function, TaskStrategy mode)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (mode == TaskStrategy.Coroutine)
                throw new ArgumentException("Mode can not be coroutine");

            _function = function;
            Strategy = mode;
        }


        /// <summary>
        /// Creates a new task with a specific strategy
        /// </summary>
        public Task(Delegate function, object param, TaskStrategy mode)
            : this()
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (mode == TaskStrategy.Coroutine)
                throw new ArgumentException("Mode can not be coroutine");

            _function2 = function;
            Paramater = param;
            Strategy = mode;
        }

        /// <summary>
        /// Creates a new Coroutine  task
        /// </summary>
        public Task(IEnumerator routine)
        {
            if (routine == null)
                throw new ArgumentNullException("routine");


            _routine = routine;
            Strategy = TaskStrategy.Coroutine;
        }

        /// <summary>
        /// Creates a new Task in a Faulted state
        /// </summary>
        /// <param name="ex"></param>
        public Task(Exception ex)
        {
            Exception = ex;
            Strategy = TaskStrategy.Custom;
            Status = TaskStatus.Faulted;
        }

        /// <summary>
        /// Creates a new task
        /// </summary>
        public Task(TaskStrategy mode)
            : this()
        {
            Strategy = mode;
        }
        #endregion

        #region protected methods

        protected override void Execute()
        {
            try
            {
                if (_function2 != null)
                {
                    Result = (TResult)_function2.DynamicInvoke(Paramater);
                }
                else if (_function != null)
                {
                    Result = _function();
                }
                Status = TaskStatus.Success;
            }
            catch (Exception ex)
            {
                Exception = ex;
                Status = TaskStatus.Faulted;
                if (LogErrors)
                    UnityEngine.Debug.LogException(ex);
            }
        }

        #endregion

        /// <summary>
        /// Called after the task is complete
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task<TResult> ContinueWith(Action<Task<TResult>> action)
        {
            if (IsCompleted)
            {
                action(this);
            }
            else
            {
                OnComplete.Add(action);
            }

            return this;
        }
    }
}
