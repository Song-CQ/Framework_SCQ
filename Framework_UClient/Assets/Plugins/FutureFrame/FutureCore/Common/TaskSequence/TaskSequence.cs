using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FutureCore
{
    /// <summary>
    /// 任务列表
    /// </summary>
    public class TaskSequence 
    {
        public Action onFinish;
        private List<TaskProcedure> taskList;
        private bool isCancel = false;
        
        public TaskSequence Add(Action<TaskProcedure> taskFunc)
        {
            TaskProcedure taskProcedure = ObjectPoolStatic<TaskProcedure>.Get();
            taskProcedure.onTaskFunc = taskFunc;
            taskProcedure.taskSequence = this;
            taskList.Add(taskProcedure);
            return this;
        }

        public TaskSequence Add(bool result, Action<TaskProcedure> trueTaskFunc)
        {
            if (!result)
            {
                return this;
            }
            return Add(trueTaskFunc);
        }

        public TaskSequence Add(bool result, Action<TaskProcedure> trueTaskFunc, Action<TaskProcedure> falseTaskFunc)
        {
            if (result)
            {
                return Add(trueTaskFunc);
            }
            else
            {
                return Add(falseTaskFunc); 
            }
        }


        public TaskSequence Add(float delayTime,Action<TaskProcedure> taskFunc)
        {
            TaskProcedure taskProcedure = ObjectPoolStatic<TaskProcedure>.Get();
            taskProcedure.onTaskFunc = (task) =>
            {
                TimerUtil.Simple.AddTimer(delayTime, () =>
                {
                    taskFunc?.Invoke(task);
                });
            };
            taskProcedure.taskSequence = this;
            taskList.Add(taskProcedure);
            return this;
        }
        
        public TaskSequence AddDelay(float delayTime)
        {
            TaskProcedure taskProcedure = ObjectPoolStatic<TaskProcedure>.Get();
            taskProcedure.onTaskFunc = (task) =>
            {
                TimerUtil.Simple.AddTimer(delayTime, () =>
                {
                    task.onComplete?.Invoke();
                });
            };
            taskProcedure.taskSequence = this;
            taskList.Add(taskProcedure);
            return this;
        }

        public TaskSequence Run()
        {
            if (isCancel)
            {
                return null;
            }
            for (var index = 0; index < taskList.Count-1; index++)
            {
                var nextIndex = index+1;
                taskList[index].onComplete = () =>
                {
                    taskList[nextIndex].TaskFunc();
                };
            }

            if (taskList.Count>0)
            {
                taskList[0].TaskFunc();
            }
            return this;
        }

        public TaskSequence RunDelay(float delayTime)
        {
            TimerUtil.Simple.AddTimer(delayTime, () =>
            {
                Run();
            });
            return this;
        }

        public TaskSequence Clear()
        {
            onFinish = null;
            foreach (var item in taskList)
            {
                item.Dispose();
                ObjectPoolStatic<TaskProcedure>.Release(item);
            }
            taskList.Clear();
            return this;
        }


        public TaskSequence Cancel()
        {
            isCancel = true;
            Clear();
            return this;
        }
        
        /// <summary>
        /// 任务步骤
        /// </summary>
        public class TaskProcedure
        {
            public TaskSequence taskSequence;
            public Action<TaskProcedure> onTaskFunc;
            public Action onComplete;

            public void InvokeComplete()
            {
                onComplete?.Invoke();
            }
            public void DelayInvokeComplete(float delayTime)
            {
                TimerUtil.Simple.AddTimer(delayTime, InvokeComplete);
            }
            public void TaskFunc()
            {
                onTaskFunc?.Invoke(this);
            }
            public void Dispose()
            {
                taskSequence = null;
                onComplete = null;
                onTaskFunc = null;
            }
           
            
        }
        
    }
}