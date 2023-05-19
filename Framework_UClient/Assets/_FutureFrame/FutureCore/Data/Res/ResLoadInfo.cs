/****************************************************
    文件: ResLoadInfo.cs
    作者: Clear
    日期: 2023/5/17 17:33:8
    类型: 框架核心脚本(请勿修改)
    功能: 资源加载
*****************************************************/
using System.Collections.Generic;

namespace FutureCore
{
    public class ResLoadInfo
    {
        public delegate void callback(ResLoadInfo res, object param);
        // 原始url列表
        public string[] pathArr;

        // 载入完毕触发的回调
        private callback fn;

        // 回调参数
        private object fnPara;

        // 文件数
        private int fileCount;

        // 根目录
        public string dataRoot = "";
        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool IsCompleted { get; private set;} = false;

        private bool delayUnload = false;
        private bool delayUnload_deepMode = false;

        public Dictionary<string, ResLoadNode> nodeDict = new Dictionary<string, ResLoadNode>();

        public ResLoadNode content;

        public void Start(string[] pathArr, callback fn = null, object fnPara = null, string dataRoot = "")
        {
            this.dataRoot = dataRoot;
            this.pathArr = pathArr;
            this.fn = fn;
            this.fnPara = fnPara;
            this.fileCount = pathArr.Length;
            this.IsCompleted = false;
            this.LoadAll();
        }


        private void LoadAll()
        {
            if (fileCount<=0)
            {
                fn?.Invoke(this,fnPara);
                this.IsCompleted = true;
                return;
            }
            foreach (var item in pathArr)
            {
                ResLoadNode node = ResLoadNode.Get();
                node.AddLoadCallBack(OnNodeLoaded);
                node.Load(dataRoot,item);                
            }
            
        }

        private void OnNodeLoaded(ResLoadNode node)
        {
            nodeDict[node.Path] = node;
            fileCount--;

            if (fileCount <= 0)
            {
           
                if (nodeDict.Count==1)
                {
                    content = node;
                }
                fn?.Invoke(this, fnPara);

                this.IsCompleted = true;
                
                if (delayUnload)
                {
                    UnLoad(delayUnload_deepMode);
                }
              
            }
           
        }

        public void UnLoad(bool unloadUsedObjects = false)
        {
            if (IsCompleted == false)
            {
                //表示在加载完成的回调中执行了unLoad，为保证其他回调运行时未被unload
                this.delayUnload = true;
                this.delayUnload_deepMode = unloadUsedObjects;
                return;
            }


            foreach (var item in nodeDict)
            {
                item.Value.UnLoad(unloadUsedObjects);
            }

            this.content = null;
            this.pathArr = null;
            this.fn = null;
            this.fnPara = null;
            this.fileCount = 0;
            this.dataRoot = string.Empty;
            this.IsCompleted = false;
            this.delayUnload = false;
            this.delayUnload_deepMode = false;

            ObjectPoolStatic<ResLoadInfo>.Release(this);
        }

    }
}