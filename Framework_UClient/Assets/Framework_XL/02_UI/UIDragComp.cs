/* function:
 * 在制定区域内部进行拖选，可以获得选中卡片的信息
   靠近中间区域的会变大，两侧变小
   松手的时候会返回到最靠近的一个卡片当中
 *
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
      
public class UIDragComp : MonoBehaviour, IDragHandler, IEndDragHandler
{
    //-------------------------- Unity obj ------------
    public Transform contant;
    public Transform center;
    Transform _select;
    public List<Transform> objs;
    Coroutine moveToCenter;

    public XL.UI.UIWindow uiWindow;

    public Transform select { get { return _select; } }
    public int objCount { get { return objs.Count; } }
    public int selcetIndex { get { return objs.IndexOf(_select); } }

    Transform leftTrans;
    Transform rightTrans;
    //------------------------- init ------

    void Awake()
    {
        SetRightLeft();
        FindSelectObj();
       
    }

   

    void SetRightLeft()
    {
        float right = -9999;
        float left = 9999;
        Transform rightObj = null;
        Transform leftObj = null;
        for (int i = 0; i < objs.Count; i++)
        {
            if (objs[i].position.y > right)
            {
                rightObj = objs[i];
                right = objs[i].transform.position.y;
            }
            if (objs[i].position.y < left)
            {
                leftObj = objs[i];
                left = objs[i].transform.position.y;
            }
        }
        leftTrans = leftObj;
        rightTrans = rightObj;
    }

    //------------------ function ---------


    Transform FindSelectObj()
    {
        Transform nearly = null;
        float minDis = 999;
        for (int i = 0; i < objs.Count; i++)
        {
            float dis = Vector2.Distance(objs[i].position, center.position);
            if (dis < minDis)
            {
                nearly = objs[i];
                minDis = dis;
            }
        }
        _select = nearly;
        if (uiWindow)
        {
            uiWindow.RefreshUI();
        }
        return nearly;
    }

    IEnumerator MoveToSelect()
    {
        Vector3 centerPos = center.position;
        Vector3 targetPos = _select.position;

        float distance = Vector3.Distance(centerPos, targetPos);
        while (distance > 0.03f)
        {
            centerPos = center.position;
            targetPos = _select.position;

            Vector3 pos = Vector3.Lerp(targetPos, centerPos, 0.3f);

            Vector3 contantMove = pos - _select.position;
            contant.Translate(contantMove);

            distance = Vector3.Distance(centerPos, _select.position);

            ScaleTheSize();
            yield return null;
        }
        moveToCenter = null;
    }

    void ScaleTheSize()
    {
        foreach (Transform trans in objs)
        {
            float distance = Vector2.Distance(trans.position, center.position);
            float tLerp = distance / (Screen.height / 2);
            float size = Mathf.Lerp(1f, 0.65f, tLerp);
            trans.localScale = new Vector3(1, 1, 1) * size;
        }
    }


    //------------------------- ui event ---------------
    public void OnDrag(PointerEventData data)
    {
        Vector3 delta = data.delta;
        float xMove = delta.y;
        if (leftTrans.position.y + xMove >= (center.position.y + Screen.height / 4))
            xMove = (center.position.y + Screen.height / 4) - leftTrans.position.y;
        if (rightTrans.position.y + xMove <= (center.position.y - Screen.height / 4))
            xMove = (center.position.y - Screen.width / 4) - rightTrans.position.y;

        contant.Translate(xMove * Vector3.up);
        FindSelectObj();
        ScaleTheSize();
        if (moveToCenter != null) StopCoroutine(moveToCenter);
    }

    public void OnDrag(float xMove=0.1f)
    {
        if (leftTrans.position.y + xMove >= (center.position.y + Screen.height / 4))
            xMove = (center.position.y + Screen.height / 4) - leftTrans.position.y;
        if (rightTrans.position.y + xMove <= (center.position.y - Screen.width / 4))
            xMove = (center.position.y - Screen.width / 4) - rightTrans.position.y;

        contant.Translate(xMove * Vector3.up);
        FindSelectObj();
        ScaleTheSize();
        if (moveToCenter != null) StopCoroutine(moveToCenter);
    }

    public void OnEndDrag(PointerEventData data)
    {
        moveToCenter = StartCoroutine(MoveToSelect());
    }
}
