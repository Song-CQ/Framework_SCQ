/****************************************************
    文件: BaseDragItem.cs
    作者: Clear
    日期: 2024/8/14 17:4:21
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using UnityEngine;

namespace ProjectApp
{
    public class DragEntity
    {
        public DragEntityType entityType;

        private Base_Data data;

        public void SetData(Base_Data data)
        {
            this.data = data;

        }



    }
}