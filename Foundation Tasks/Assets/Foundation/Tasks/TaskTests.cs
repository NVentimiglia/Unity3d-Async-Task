using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Foundation.Tasks
{
    /// <summary>
    /// Example of how to use the UnityTask library
    /// </summary>
    [AddComponentMenu("Foundation/UnityTasks/UnityTaskTests")]
    public class UnityTaskTests : MonoBehaviour
    {
        public Text Output;
       
        public IEnumerator Start()
        {
            Application.logMessageReceived+=Application_logMessageReceived;
            yield return 1;
            Debug.Log("Tests (9)");

            UnityTask.Run(() => Debug.Log("1 Run Complete"));
            yield return new WaitForSeconds(1);
            UnityTask.Run(Test2, "2 Run With Param Complete");
            yield return new WaitForSeconds(1);
            UnityTask.RunCoroutine(Test3);
            yield return new WaitForSeconds(1);
            UnityTask.RunCoroutine(Test4()).ContinueWith(t => Debug.Log("4 complete"));
            yield return new WaitForSeconds(1);
            UnityTask.RunCoroutine(Test5).ContinueWith(t => Debug.Log("5 complete"));
            yield return new WaitForSeconds(1);
            UnityTask.Run(() => { return "6 Run with Result Complete"; }).ContinueWith(t => Debug.Log(t.Result));
            yield return new WaitForSeconds(1);
            UnityTask.Run<string, string>(Test7, "7 Run with Param and Result").ContinueWith(t => Debug.Log(t.Result));
            yield return new WaitForSeconds(1);
            var t1 = UnityTask.RunCoroutine<string>(Test8);
            yield return new WaitForSeconds(1);
            Debug.Log(t1.Result);
            UnityTask.RunCoroutine<string>(Test9).ContinueWith(t => Debug.Log(t.Result));
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

        IEnumerator Test5(UnityTask UnityTask)
        {
            yield return 1;
            if (UnityTask == null)
                Debug.LogWarning("wtf");
            Debug.Log("5 Coroutine with UnityTask State Complete");
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

        IEnumerator Test8(UnityTask<string> UnityTask)
        {
            yield return 1;
            Debug.Log("8 Coroutine With Result...");
            //set result or exception
            UnityTask.Result = "8 Complete";
        }
        IEnumerator Test9(UnityTask<string> UnityTask)
        {
            yield return 1;
            if (UnityTask == null)
                Debug.LogWarning("wtf");

            UnityTask.Result = ("9 Coroutine with UnityTask State Complete");
        }
       
        
        void MainTest()
        {
            UnityTask.RunOnMain(() =>
            {
                Debug.Log("Sleeping...");
                UnityTask.Delay(2000);
                Debug.Log("Slept");
            });
        }

        void Background()
        {
            UnityTask.Run(() =>
            {
                Debug.Log("Sleeping...");
                UnityTask.Delay(2000);
                Debug.Log("Slept");
            });
        }

        void Routine()
        {
            UnityTask.RunCoroutine(RoutineFunction());
        }

        IEnumerator RoutineFunction()
        {
            Debug.Log("Sleeping...");
            yield return new WaitForSeconds(2);
            Debug.Log("Slept");
        }



        void BackgroundToMain()
        {
            UnityTask.Run(() =>
            {

                Debug.Log("Thread A Running");

                var task = UnityTask.RunOnMain(() =>
                   {
                       Debug.Log("Sleeping...");
                       UnityTask.Delay(2000);
                       Debug.Log("Slept");
                   });

                while (task.IsRunning)
                {
                    Debug.Log(".");
                    UnityTask.Delay(100);
                }

                Debug.Log("Thread A Done");
            });
        }


        void BackgroundToRotine()
        {
            UnityTask.Run(() =>
            {
                Debug.Log("Thread A Running");

                var task = UnityTask.RunCoroutine(RoutineFunction());

                while (task.IsRunning)
                {
                    Debug.Log(".");
                    UnityTask.Delay(500);
                }

                Debug.Log("Thread A Done");
            });

        }

        void BackgroundToBackground()
        {
            UnityTask.Run(() =>
            {
                Debug.Log("1 Sleeping...");

                UnityTask.Run(() =>
                {
                    Debug.Log("2 Sleeping...");
                    UnityTask.Delay(2000);
                    Debug.Log("2 Slept");
                });
                UnityTask.Delay(2000);
                Debug.Log("1 Slept");
            });

        }

        void BackgroundToBackgroundException()
        {
            var task1 = UnityTask.Run(() =>
            {
                Debug.Log("1 Go");

                var task2 = UnityTask.Run(() =>
                {
                    UnityTask.Delay(100);
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
            var task1 = UnityTask.Run(() =>
            {
                throw new Exception("Hello World");
            });

            task1.Wait();

            Debug.Log(task1.Status + " " + task1.Exception.Message);

        }


        void CoroutineUnityTaskState()
        {
            UnityTask.RunCoroutine<string>(CoroutineUnityTaskStateAsync).ContinueWith(o => Debug.Log(o.Result));
        }

        IEnumerator CoroutineUnityTaskStateAsync(UnityTask<string> UnityTask)
        {
            yield return 1;

            UnityTask.Result = "Hello World";
        }

    }
}
