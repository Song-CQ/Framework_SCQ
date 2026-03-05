using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectApp
{
    /// <summary>
    /// ��Ϸ��Ϣ����
    /// </summary>
    public static class GameMsg
    {
        private static uint _counter = 1000;
        /// <summary>
        /// ���һ��Ԫ��
        /// </summary>
        public static readonly uint Player_ClickElement = ++_counter;
        /// <summary>
        /// ����һ��Ԫ�ص���һ��Ԫ��
        /// </summary>
        public static readonly uint Player_SwipeElementToElement = ++_counter;
        /// <summary>
        /// ��һ��Ԫ���ϻ���
        /// </summary>
        public static readonly uint Player_SwipeElement = ++_counter;
        /// <summary>
        /// ���һ���������
        /// </summary>
        public static readonly uint Player_ClickExternalPropItem = ++_counter;

        
        

        /// <summary>
        /// ѡ��һ��Ԫ��
        /// </summary>
        public static readonly uint SelectElement = ++_counter;

        /// <summary>
        /// ȡ��ѡ��Ԫ��
        /// </summary>
        public static readonly uint DeselectElement = ++_counter; // 1002

        /// <summary>
        /// ��������Ԫ��
        /// </summary>
        public static readonly uint SwapElements = ++_counter;

        /// <summary>
        /// Ԫ��ƥ��ɹ�
        /// </summary>
        public static readonly uint MatchElements = ++_counter;

        /// <summary>
        /// Ԫ������
        /// </summary>
        public static readonly uint ClearElements = ++_counter;

        /// <summary>
        /// ������Ԫ��
        /// </summary>
        public static readonly uint GenerateElements = ++_counter;

        /// <summary>
        /// Ԫ������
        /// </summary>
        public static readonly uint ElementsFall = ++_counter;

        /// <summary>
        ///  �ı�Ԫ�ص�����
        /// </summary>
        public static readonly uint RestElements = ++_counter;
        /// <summary>
        ///  �ı�ȫ��Ԫ�ص�����
        /// </summary>
        public static readonly uint RestAllElements = ++_counter;


        /// <summary>
        /// ����Ԫ���л�Ԫ������
        /// </summary>
        public static readonly uint ChangeElementType = ++_counter;
        /// <summary>
        /// �������
        /// </summary>
        public static readonly uint ActivateProp = ++_counter;
        /// <summary>
        /// ����˫�ص���
        /// </summary>
        public static readonly uint ActivateTwoProp = ++_counter;

        /// <summary>
        /// ʹ���������
        /// </summary>
        public static readonly uint UseExternalProp = ++_counter;
        
        /// <summary>
        /// ȡ��ʹ���������
        /// </summary>
        public static readonly uint CancelExternalProp = ++_counter;
        /// <summary>
        /// �����������
        /// </summary>
        public static readonly uint CostExternalProp = ++_counter;






        /// <summary>
        /// ��Ϸ��������
        /// </summary>
        public static readonly uint ScoreUpdated = ++_counter;

        /// <summary>
        /// ��Ϸ����
        /// </summary>
        public static readonly uint GameOver = ++_counter;
        /// <summary>
        /// ��Ϸ��ʼ
        /// </summary>
        public static readonly uint GameStart = ++_counter;

        /// <summary>
        /// ��Ϸʤ��
        /// </summary>
        public static readonly uint GameWin = ++_counter;

    }

}
