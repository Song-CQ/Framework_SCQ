using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace XL.Editor
{
    public class CopyComponentTool 
    {
#if UNITY_EDITOR
        static Component[] copiedComponents;
        [MenuItem("GameObject/Copy Current Components #W", false, 1)]
        static void CopyComponents()
        {
            copiedComponents = Selection.activeGameObject.GetComponents<Component>();
        }
      
        [MenuItem("GameObject/Paste Current Components #E", false,1)]
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

        [MenuItem("GameObject/Remove Current Components #R", false, 1)]
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


#endif


    }
}
