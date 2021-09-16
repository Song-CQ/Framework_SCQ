using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public abstract class BaseVOModel
    {

        /// <summary>
        /// 表名
        /// </summary>
        public abstract string SheetName { get; }
        /// <summary>
        /// 是否有key
        /// </summary>
        public abstract bool HasStringKey { get; } 
        /// <summary>
        /// 是否有id
        /// </summary>
        public abstract bool HasStringId { get; }
        /// <summary>
        /// 是否静态表
        /// </summary>
        public abstract bool HasStaticField { get; }

        /// <summary>
        /// 表head字段
        /// </summary>
        public abstract List<string> HeadFields { get; }

        
        
        public abstract void Init();
        public abstract void Reset();
        public abstract void Dispose();
    }
}
