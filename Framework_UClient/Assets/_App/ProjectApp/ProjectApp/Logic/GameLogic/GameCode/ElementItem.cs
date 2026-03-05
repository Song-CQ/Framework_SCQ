using System;
using DG.Tweening;
using FutureCore;
using ProjectApp;
using ProjectApp.Data;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


namespace ProjectApp
{
    [Serializable]
    public struct ElementData
    {
        public int X;
        public int Y;
        public ElementType Type;
        public int data1;
        public int data2;
        public int data3;



        // Unity��ҪĬ�Ϲ��캯��
        public ElementData(ElementType type = ElementType.Fixed_None)
        {
            X = 0;
            Y = 0;

            data1 = 0;
            data2 = 0;
            data3 = 0;

            Type = type;
        }
        public ElementData Set(ElementData data)
        {
            X = data.X;
            Y = data.Y;
            Type = data.Type;

            data1 = data.data1;
            data2 = data.data2;
            data3 = data.data3;
            return this;
        }


        public ElementData SetType(ElementType type)
        {
            Type = type;
            return this;
        }

        // �ṩ�޸�����ķ�����������ʵ����
        public ElementData SetPot(int x, int y)
        {
            X = x;
            Y = y;
            return this;
        }


        // ʵ�� IEquatable
        public bool Equals(ElementData other)
        {
            return X == other.X && Y == other.Y && Type == other.Type;
        }



        // ���������
        public static bool operator ==(ElementData left, ElementData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ElementData left, ElementData right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]:{2}", X, Y, Type);
        }

        /// <summary>
        /// ���ÿձ�� ����xy���� �����������
        /// </summary>
        public void SetEmpty()
        {
            Type = ElementType.Fixed_Empty;
            data1 = 0;
            data2 = 0;
            data3 = 0;
        }
    }

    //Ԫ����
    [Serializable]
    public class ElementItem : IRaycast3D
    {
        public Transform Transform { get; private set; }

        public ElementData Data { get; private set; }

        private SpriteRenderer icon;

        public GameObject selectGo;

        public Vector3 Pos
        {
            get => pos;
            set
            {
                pos = value;
                Transform.localPosition = pos;
            }
        }
        private Vector3 pos;


        public Collider Collider { get; private set; }

       
        public bool isSelect;

        private bool _active = false;
        public bool Active
        {
            get => _active;
        }

        private Transform changeTrf;

        private SpriteRenderer[] changeIcons = new SpriteRenderer[3];

        public ElementItem() { }

        public ElementItem(Transform transform)
        {
            Transform = transform;
            Collider = Transform.GetComponent<BoxCollider>();
            icon = Transform.Find("icon").GetComponent<SpriteRenderer>();
            selectGo = Transform.Find("selectGo").gameObject;
            Transform.gameObject.AddComponent<DebugElementItem>().Init(this);

            changeTrf = Transform.Find("changeIcon");
            changeIcons[0] = changeTrf.GetChild(0).GetComponent<SpriteRenderer>();
            changeIcons[1] = changeTrf.GetChild(1).GetComponent<SpriteRenderer>();
            changeIcons[2] = changeTrf.GetChild(2).GetComponent<SpriteRenderer>();


            _active = Transform.gameObject.activeSelf;
            SetActive(true);


        }


        public void Init(ElementData _data)
        {

            SetData(_data);
            RefreshView();

            // ���ó�ʼ����
            UpdateIconLayout();

        }

        public void SetData(ElementData _data)
        {
            Data = _data;
        }

        public void SetEmpty()
        {
            var temp  = Data;
            temp.SetEmpty();
            Data = temp;
        }

        public void SetType(ElementType type)
        {
            var temp  = Data;
            temp.SetEmpty();
            temp.Type =  type;
        
            Data = temp;
        }

        public void SetActive(bool value)
        {
            if (value != _active)
            {
                Transform.SetActive(value);
                _active = value;
            }

        }


        public void RefreshView()
        {
            if (ElementTool.CheckType_HasIcon(Data.Type))
            {
                icon.sprite = GameTool.GetSprite(Data.Type);
            }

            if (Data.Type == ElementType.Item_Special)
            {
                changeTrf.gameObject.SetActive(true);

                changeIcons[0].sprite = GameTool.GetSprite((ElementType)Data.data1);
                changeIcons[1].sprite = GameTool.GetSprite((ElementType)Data.data2);
                changeIcons[2].sprite = GameTool.GetSprite((ElementType)Data.data3);

            }
            else
            {
                changeTrf.gameObject.SetActive(false);
            }

            selectGo.SetActive(isSelect);

        }

        public void Release()
        {
            SetSelect(false);
            SetActive(false);

        }

        public void SetSelect(bool v)
        {
            if (v == isSelect) return;
            isSelect = v;

            selectGo.SetActive(isSelect);

        }

        /// <summary>
        /// ���
        /// </summary>
        /// <param name="hitPoint"></param>
        public void Raycast_OnClick(Vector3 hitPoint)
        {
            GameTool.GameCore.ClickElementItem(this);
            selectGo.SetActive(isSelect);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="hitPoint"></param>
        public void Raycast_OnSwipe(Vector3 startPoint, Vector3 endPoint, IRaycast3D raycast3D_end)
        {
            ElementItem endItem = (raycast3D_end as ElementItem);

            if (endItem != this)
            {
                GameTool.GameCore.SwipeItemToItem(this, endItem);
            }
            else
            {
                Vector2 dir =  endPoint - startPoint;
                GameTool.GameCore.SwipeElementItem(this, dir.normalized);
            }



        }


        public void StopAllDOTween()
        {
            switchSequenceL.Pause();
            switchSequenceR.Pause();         

            UpdateIconLayout();

        }

        public void Dispose()
        {
            // ����DOTween����
            switchSequenceR?.Kill();
            switchSequenceR = null;
            switchSequenceL?.Kill();
            switchSequenceL = null;

            Transform = null;
            Collider = null;
            icon = null;
            selectGo = null;

            changeTrf = null;
            changeIcons = null;
        }




        #region �л�����

        //[Header("��������")]
        public  float switchDuration = 0.3f;
        private float centerScale = 0.7f;
        private float sideScale = 0.4f;
        private Vector3 topLeftPosition = new Vector3(-0.357f, 0.345f, -0.1f);
        private Vector3 centerPosition = new Vector3(0f, 0f, -0.2f);
        private Vector3 bottomRightPosition = new Vector3(0.397f, -0.345f, -0.1f);



        // �л�����һ��ͼ��
        public void SwitchToNext()
        {
            
            UpdateIconLayout();
            // ������һ������

            PerformSwitchAnimation(true);
        }

        // �л�����һ��ͼ��
        public void SwitchToPrevious()
        {

            UpdateIconLayout();
            // ������һ������

            PerformSwitchAnimation(false);
        }


        private Sequence switchSequenceR;
        private Sequence switchSequenceL;
        private void PerformSwitchAnimation(bool isR)
        {
            // ֹͣ�������ж���
            switchSequenceR?.Pause();
            switchSequenceL?.Pause();
            if (isR)
            {
                if (switchSequenceR == null)
                {
                    // ִ�л�������
                    Sequence sequence = DOTween.Sequence();

                    // ����ͼ���ƶ����м�
                    sequence.Join(changeIcons[1].transform
                        .DOLocalMove(centerPosition, switchDuration));
                    sequence.Join(changeIcons[1].transform
                        .DOScale(centerScale, switchDuration));

                    // �м�ͼ���ƶ�������
                    sequence.Join(changeIcons[0].transform
                        .DOLocalMove(bottomRightPosition, switchDuration));
                    sequence.Join(changeIcons[0].transform
                        .DOScale(sideScale, switchDuration));

                    // ����ͼ���ƶ������ϣ����ѭ����
                    sequence.Join(changeIcons[2].transform
                        .DOLocalMove(topLeftPosition, switchDuration));
                    sequence.Join(changeIcons[2].transform
                        .DOScale(sideScale, switchDuration));

                    //sequence.OnStart(() => Debug.Log("1������ʼ"));
                    //sequence.OnUpdate(() => Debug.Log("1����������..."));
                    sequence.onComplete = () =>
                    {
                        Debug.Log("1�������");
                        UpdateIconLayout();
                    };

                    // ������ʾ˳��
                    sequence.SetEase(Ease.OutCubic);
                    switchSequenceR = sequence;

                }
                else
                {
                    switchSequenceR.Restart();
                }
            }
            else
            {
                if (switchSequenceL == null)
                {
                    // ִ�л�������
                    Sequence sequence = DOTween.Sequence().SetAutoKill(false);

                    // ����ͼ���ƶ����м�
                    sequence.Join(changeIcons[2].transform
                        .DOLocalMove(centerPosition, switchDuration));
                    sequence.Join(changeIcons[2].transform
                        .DOScale(centerScale, switchDuration));

                    // �м�ͼ���ƶ�������
                    sequence.Join(changeIcons[0].transform
                        .DOLocalMove(topLeftPosition , switchDuration));
                    sequence.Join(changeIcons[0].transform
                        .DOScale(sideScale, switchDuration));

                    // ����ͼ���ƶ������£����ѭ����
                    sequence.Join(changeIcons[1].transform
                        .DOLocalMove(bottomRightPosition,switchDuration));
                    sequence.Join(changeIcons[1].transform
                        .DOScale(sideScale, switchDuration));


                    sequence.OnStart(() => Debug.Log("2������ʼ"));
                    sequence.OnUpdate(() => Debug.Log("2����������..."));
                    sequence.onComplete = () =>
                    {
                        Debug.Log("2�������");
                        UpdateIconLayout();
                    };

                    // ������ʾ˳��
                    sequence.SetEase(Ease.OutCubic);
                    switchSequenceL = sequence;
                }
                else
                {
                    switchSequenceL.Restart();
                }
            }

        }

        // ����ͼ�겼�֣��޶�����
        private void UpdateIconLayout()
        {

            // ����λ�úʹ�С
            changeIcons[1].transform.localPosition = topLeftPosition;
            changeIcons[1].transform.localScale = Vector3.one * sideScale;

            changeIcons[0].transform.localPosition = centerPosition;
            changeIcons[0].transform.localScale = Vector3.one * centerScale;

            changeIcons[2].transform.localPosition = bottomRightPosition;
            changeIcons[2].transform.localScale = Vector3.one * sideScale;

            changeIcons[0].sprite = GameTool.GetSprite((ElementType)Data.data1);
            changeIcons[1].sprite = GameTool.GetSprite((ElementType)Data.data2);
            changeIcons[2].sprite = GameTool.GetSprite((ElementType)Data.data3);
        }

        #endregion


        
    }


}



public class DebugElementItem : MonoBehaviour
{
    public ElementItem elementItem;

    [SerializeField]
    public ElementData Data;
    public bool isSelect;

    public ElementType newType = ElementType.Prop_Bomb;


    public System.Collections.Generic.List<string> InfoText = new System.Collections.Generic.List<string>();
    string oldname;

    [Button("����������")]
    public void SetData()
    {
        Data.SetType(newType);
        elementItem.SetData(Data);

        GameTool.GameCore.Data.SetElementData(Data);
        elementItem.RefreshView();
    }

    public void Init(ElementItem _elementItem)
    {
        elementItem = _elementItem;
        oldname = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);

        InfoText.Add(oldname);
    }

    [Button("next")]
    public void SwitchToNext()
        {

        if (elementItem == null) return;
        elementItem.SwitchToNext();

    }
    [Button("previou")]
    public void SwitchToPrevious()
        {
        if (elementItem == null) return;
        elementItem.SwitchToPrevious();

    }

    public void Update()
    {
        if (elementItem == null) return;

        Data = elementItem.Data;
        isSelect = elementItem.isSelect;
        //elementItem.selectGo.SetActive(isSelect);
        transform.name = string.Format("{0}_{1} : {2}", Data.X, Data.Y, Data.Type);
        if (transform.name != oldname)
        {
            oldname = transform.name;

            InfoText.Add(oldname);
        }


    }
}

