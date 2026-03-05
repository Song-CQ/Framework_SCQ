using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{

    public enum ElementType
    {
        Item_A = 1,    // ��ɫ����Ԫ��
        Item_B, // ��ɫ����Ԫ��
        Item_C,   // ��ɫ����Ԫ��
        Item_D,  // ��ɫ����Ԫ��

        Item_Special,// �ɱ任Ԫ��


        //// ���л�������Ԫ��
        //Prop_Rocket,           // �����ϣ��������л���
        //RocketBombCombo,       // ���ը�����
        //DoubleRocket,          // ˫��������л�����
        //CrossRocket,           // ʮ�ֻ��
        //MegaBomb,              // ����ը�������л�ģʽ��



        Dummy_CanMatche = 100,// վλ ����С���������ƥ������

        //����
        Prop_Horizontal,      // ������
        Prop_Vertical,        // ������
        Prop_Wild,             // ��ɫը��/����Ԫ��   
        Prop_Bomb,                 // ը��



        Dummy_CanClickEvent = 200, // ռλ ����С�Ľ��ܵ��


        //��������
        Dummy_CanDown = 900,//ռλ ������Ĳ�������


        //���ɵ��
        Fixed_Empty = 1000, // ��λ���
        Fixed_None = 2000,//�÷��񱻽��÷��� һ���ǵ��� ������ܻ��п��ƻ�����

    }
    public static class ElementTool
    {
        

        /// <summary>
        /// ��Ԫ���Ƿ�Ҫ��ʾͼƬ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool CheckType_HasIcon(ElementType type)
        {
            switch (type)
            {
                case ElementType.Dummy_CanMatche:
                case ElementType.Dummy_CanClickEvent:
                case ElementType.Dummy_CanDown:
                case ElementType.Fixed_Empty:
                case ElementType.Fixed_None:
                    return false;
            }
            return true;
        }
        /// <summary>
        /// ��Ԫ���Ƿ������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckType_FillEmpty(ElementType type)
        {

            if (type >= ElementType.Dummy_CanDown)
            {
                return false;
            }
            return true;


        }

        /// <summary>
        /// ��Ԫ�� ����ͨ���Ƿ�ͨ ���� ��Ԫ�ؾ�����
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>

        public static bool CheckType_UpEmpty(ElementType type)
        {
            if (type == ElementType.Fixed_Empty)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// ��Ԫ���Ƿ� �� ƥ��
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckType_CanMatches(ElementType type)
        {
            if (type < ElementType.Dummy_CanMatche)
            {
                return true;
            }

            return false;

        }

        /// <summary>
        /// ��Ԫ���Ƿ���� ����¼�
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool CheckType_ClickEvent(ElementType type)
        {
            if (type < ElementType.Dummy_CanClickEvent)
            {
                return true;
            }
            return false;
        }

        public static ElementType GetTypeToElementData(ElementData data)
        {
            if (data.Type == ElementType.Item_Special)
            {
                return (ElementType)data.data1;
            }
            return data.Type;
        }

        public static bool CheckType_IsProp(ElementType type)
        {
            if (type >= ElementType.Prop_Horizontal && type <= ElementType.Prop_Bomb)
            {
                return true;
            }

            return false;

        }


        
    }
}
