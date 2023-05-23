/****************************************************
    文件：UIEntity.cs
	作者：Clear
    日期：2022/1/25 16:24:51
    类型: 框架核心脚本(请勿修改)
	功能：UI实体
*****************************************************/
using System;

namespace FutureCore
{
    public abstract class UIEntity
    {
        public abstract void SetVisible(bool arse);

        private string thisName;
        public string Name
        {
            get
            {
                return thisName;
            }
            set
            {
                if (value!=thisName)
                {
                    SetName(value);
                }
            }
        }

        protected virtual void SetName(string value)
        {
            thisName = value;
        }
        public virtual void OpenUIAnim(Action onComplete)
        {
            onComplete?.Invoke();
        }
        public virtual void CloseUIAnim(Action onComplete)
        {
            onComplete?.Invoke();
        }

        public virtual void Dispose() 
        {
            thisName = null;
        }
   

    }
}