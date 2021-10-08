namespace XL.EditorTool
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEditor;
    public class GameObjectTool
    {
        static Component[] copiedComponents;
        [MenuItem("GameObject/GameObject工具/提出父物体", priority = 0)]
        static void test()
        {
            GameObject[] games = Selection.gameObjects;
            foreach (var item in games)
            {
                item.transform.parent = null;
            }
           
        }
        [MenuItem("GameObject/GameObject工具/拷贝全部组件 #W", false, 0)]
        static void CopyComponents()
        {
            copiedComponents = Selection.activeGameObject.GetComponents<Component>();
        }
        [MenuItem("GameObject/GameObject工具/拷贝全部组件", true)]
        static bool CheckCopyComponents()
        {
            return Selection.gameObjects.Length == 1;

        }

        [MenuItem("GameObject/GameObject工具/复制全部组件 #E", false,1)]
        static void PasteComponents()
        {
            Debug.Log("复制完成");
            foreach (var targetGameObject in Selection.gameObjects)
            {
                if (!targetGameObject || copiedComponents == null) continue;



                var targetComponents = new List<Component>(targetGameObject.GetComponents<Component>());

                foreach (var copiedComponent in copiedComponents)
                {
                    if (!copiedComponent) continue;

                    var targetComponent = targetComponents.Find((item) => item.GetType() == copiedComponent.GetType());
                    UnityEditorInternal.ComponentUtility.CopyComponent(copiedComponent);

                    if (targetComponent != null)
                    {

                        UnityEditorInternal.ComponentUtility.PasteComponentValues(targetComponent);

                    }
                    else
                    {
                        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetGameObject);

                    }
                }
            }
        }

        [MenuItem("GameObject/GameObject工具/复制全部组件", true)]
        static bool CheckPasteComponents()
        {
            return Selection.gameObjects.Length == 1;

        }

        [MenuItem("GameObject/GameObject工具/删除全部组件 #R", false, 2)]
        static void RemoveComponents()
        {
            foreach (var con in Selection.gameObjects)
            {
                copiedComponents = con.GetComponents<Component>();
                foreach (var item in copiedComponents)
                {
                    if ((item as Transform) == null)
                        GameObject.DestroyImmediate(item);
                }
            }  
            
        }
        [MenuItem("GameObject/GameObject工具/删除全部组件", true)]
        static bool CheckRemoveComponents()
        {
            return Selection.gameObjects.Length == 1;            
            
        }

        [MenuItem("GameObject/GameObject工具/在同级的顶部产生空物体", priority = 3)]
        private static void AddEmpty()
        {
            GameObject Empty = new GameObject();
            Empty.transform.SetParent(Selection.activeGameObject.transform.parent);
            Empty.transform.SetSiblingIndex(0);
        }
        [MenuItem("GameObject/GameObject工具/在同级的顶部产生空物体", true)]
        private static bool CheckAddEmpty()
        {
            return (Selection.gameObjects.Length > 0);
        }
        [MenuItem("GameObject/GameObject工具/在当前对象的下面产生空物体", priority = 4)]
        private static void InsertEmpty()
        {
            GameObject Empty = new GameObject();
            Empty.transform.SetParent(Selection.activeGameObject.transform.parent);
            Empty.transform.SetSiblingIndex(Selection.activeGameObject.transform.GetSiblingIndex() + 1);
        }
        [MenuItem("GameObject/GameObject工具/并入一个空物体", true)]
        private static bool CheckInsertEmpty()
        {
            return (Selection.gameObjects.Length > 0);
        }
        [MenuItem("GameObject/GameObject工具/并入一个空物体", priority = 5)]
        private static void PackEmpty()
        {
            GameObject Empty;
            if (GameObject.Find("pack"))
            {
                return;
            }
            else
            {
                Empty = new GameObject();
                Empty.transform.position = Vector3.zero;
            }
            Empty.name = "pack";

            GameObject[] gobj;
            gobj = Selection.gameObjects;

            if (gobj[0].transform.parent != null)
            {
                Empty.transform.SetParent(gobj[0].transform.parent);
            }
            Empty.transform.SetSiblingIndex(gobj[0].transform.GetSiblingIndex());
            for (int i = 0; i < gobj.Length; i++)
            {
                gobj[i].transform.SetParent(Empty.transform);
            }
            Empty.name = "packDone";

        }

        [MenuItem("GameObject/GameObject工具/并入一个空物体", true)]
        private static bool CheckPackEmpty()
        {
            return (Selection.gameObjects.Length >= 2);
        }



       
        
        
    }
}
