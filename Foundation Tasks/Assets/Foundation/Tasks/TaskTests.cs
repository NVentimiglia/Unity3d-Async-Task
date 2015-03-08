using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Tasks
{
    /// <summary>
    /// Example of how to use the task library
    /// </summary>
    [AddComponentMenu("Foundation/Tasks/TaskTests")]
    public class TaskTests : MonoBehaviour
    {
        public Text Output;
       
        public IEnumerator Start()
        {
            Application.logMessageReceived+=Application_logMessageReceived;
            yield return 1;
            Debug.Log("Tests (9)");

            Task.Run(() => Debug.Log("1 Run Complete"));
            yield return new WaitForSeconds(1);
            Task.Run(Test2, "2 Run With Param Complete");
            yield return new WaitForSeconds(1);
            Task.RunCoroutine(Test3);
            yield return new WaitForSeconds(1);
            Task.RunCoroutine(Test4()).ContinueWith(t => Debug.Log("4 complete"));
            yield return new WaitForSeconds(1);
            Task.RunCoroutine(Test5).ContinueWith(t => Debug.Log("5 complete"));
            yield return new WaitForSeconds(1);
            Task.Run(() => { return "6 Run with Result Complete"; }).ContinueWith(t => Debug.Log(t.Result));
            yield return new WaitForSeconds(1);
            Task.Run<string, string>(Test7, "7 Run with Param and Result").ContinueWith(t => Debug.Log(t.Result));
            yield return new WaitForSeconds(1);
            var t1 = Task.RunCoroutine<string>(Test8);
            yield return new WaitForSeconds(1);
            Debug.Log(t1.Result);
            Task.RunCoroutine<string>(Test9).ContinueWith(t => Debug.Log(t.Result));
        }

void Application_logMessageReceived(string condition, string stackTrace, LogType type)
{
    if(Output)
    {
        Output.text= condition;
    }
}


        void Test2(string param)
        {
            Debug.Log(param);
        }

        IEnumerator Test3()
        {
            yield return 1;
            Debug.Log("3 Coroutine Complete");
        }

        IEnumerator Test5(Task task)
        {
            yield return 1;
            if (task == null)
                Debug.LogWarning("wtf");
            Debug.Log("5 Coroutine with Task State Complete");
        }

        IEnumerator Test4()
        {
            yield return 1;
            Debug.Log("4 Coroutine");
        }

        string Test7(string param)
        {
            param += " Complete";
            return param;
        }

        IEnumerator Test8(Task<string> task)
        {
            yield return 1;
            Debug.Log("8 Coroutine With Result...");
            //set result or exception
            task.Result = "8 Complete";
        }
        IEnumerator Test9(Task<string> task)
        {
            yield return 1;
            if (task == null)
                Debug.LogWarning("wtf");

            task.Result = ("9 Coroutine with Task State Complete");
        }
       
        
        void MainTest()
        {
            Task.RunOnMain(() =>
            {
                Debug.Log("Sleeping...");
                Task.Delay(2000);
                Debug.Log("Slept");
            });
        }

        void Background()
        {
            Task.Run(() =>
            {
                Debug.Log("Sleeping...");
                Task.Delay(2000);
                Debug.Log("Slept");
            });
        }

        void Routine()
        {
            Task.RunCoroutine(RoutineFunction());
        }

        IEnumerator RoutineFunction()
        {
            Debug.Log("Sleeping...");
            yield return new WaitForSeconds(2);
            Debug.Log("Slept");
        }



        void BackgroundToMain()
        {
            Task.Run(() =>
            {

                Debug.Log("Thread A Running");

                var task = Task.RunOnMain(() =>
                   {
                       Debug.Log("Sleeping...");
                       Task.Delay(2000);
                       Debug.Log("Slept");
                   });

                while (task.IsRunning)
                {
                    Debug.Log(".");
                    Task.Delay(100);
                }

                Debug.Log("Thread A Done");
            });
        }


        void BackgroundToRotine()
        {
            Task.Run(() =>
            {
                Debug.Log("Thread A Running");

                var task = Task.RunCoroutine(RoutineFunction());

                while (task.IsRunning)
                {
                    Debug.Log(".");
                    Task.Delay(500);
                }

                Debug.Log("Thread A Done");
            });

        }

        void BackgroundToBackground()
        {
            Task.Run(() =>
            {
                Debug.Log("1 Sleeping...");

                Task.Run(() =>
                {
                    Debug.Log("2 Sleeping...");
                    Task.Delay(2000);
                    Debug.Log("2 Slept");
                });
                Task.Delay(2000);
                Debug.Log("1 Slept");
            });

        }

        void BackgroundToBackgroundException()
        {
            var task1 = Task.Run(() =>
            {
                Debug.Log("1 Go");

                var task2 = Task.Run(() =>
                {
                    Task.Delay(100);
                    Debug.Log("2 Go");
                    throw new Exception("2 Fail");
                });

                task2.Wait();

                if (task2.IsFaulted)
                    throw task2.Exception;
            });

            task1.Wait();

            Debug.Log(task1.Status + " " + task1.Exception.Message);

        }

        void BackgroundException()
        {
            var task1 = Task.Run(() =>
            {
                throw new Exception("Hello World");
            });

            task1.Wait();

            Debug.Log(task1.Status + " " + task1.Exception.Message);

        }


        void CoroutineTaskState()
        {
            Task.RunCoroutine<string>(CoroutineTaskStateAsync).ContinueWith(o => Debug.Log(o.Result));
        }

        IEnumerator CoroutineTaskStateAsync(Task<string> task)
        {
            yield return 1;

            task.Result = "Hello World";
        }

    }
}
