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
        
        public class TaskProcedure
        {
            public TaskSequence taskSequence;
            public Action<TaskProcedure> onTaskFunc;
            public Action onComplete;

            public void DelayInvokeComplete(float time)
            {
                
            }

            public void InvokeComplete()
            {
                onComplete?.Invoke();
                
            }

            public void TaskFunc()
            {
                onTaskFunc?.Invoke(this);
            }

        }
        
    }
}