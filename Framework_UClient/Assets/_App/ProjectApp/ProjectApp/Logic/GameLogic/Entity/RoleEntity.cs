/****************************************************
    文件: RoleEntity.cs
    作者: Clear
    日期: 2024/8/15 17:59:23
    类型: 逻辑脚本
    功能: Nothing
*****************************************************/
using FutureCore;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using static FutureCore.UIEventListener;
using static UnityEditorInternal.VersionControl.ListControl;

namespace ProjectApp
{
    public class RoleEntity : BaseRole, IDrag
    {

        public Base_Data Data => base.data;

        public UIEventListener Listener { get; private set; }

        private PointerHandler beginDrag_Delegate;
        private PointerHandler drag_Delegate;
        private PointerHandler endDrag_Delegate;

        public void AddListener(PointerHandler _BeginDrag, PointerHandler _Drag_Delegate, PointerHandler _EndDrag_Delegate)
        {
            beginDrag_Delegate = _BeginDrag;
            drag_Delegate = _Drag_Delegate;
            endDrag_Delegate = _EndDrag_Delegate;

            Listener.BeginDrag += beginDrag_Delegate;
            Listener.Drag += drag_Delegate;
            Listener.EndDrag += endDrag_Delegate;

        }
        public void RomveListener()
        {
            Listener.BeginDrag -= beginDrag_Delegate;
            Listener.Drag -= drag_Delegate;
            Listener.EndDrag -= endDrag_Delegate;
            beginDrag_Delegate = null;
            drag_Delegate = null;
            endDrag_Delegate = null;
        }


        public void BeginDrag()
        {
            
        }

        public void Drag()
        {
          
        }

        public void EndDrag()
        {
        
        }

        
    }
}