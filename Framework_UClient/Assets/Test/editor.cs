using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class editor : EditorWindow
{

    [MenuItem("GameObject/[Window]/test", false, -100)]
    private static void GoOpenFrameworkWin()
    {
        CreateWindow<editor>("test");
    }
    private void OnEnable()
    {
        m_HorizontalSplitterPercent = 0.4f;
        m_VerticalSplitterPercentRight = 0.7f;
        m_VerticalSplitterPercentLeft = 0.85f;
        m_Position = new Rect(0,0,900, 520);


        m_HorizontalSplitterRect = new Rect(
               (int)(m_Position.x + m_Position.width * m_HorizontalSplitterPercent),
               m_Position.y,
               k_SplitterWidth,
               m_Position.height);
        m_VerticalSplitterRectRight = new Rect(
            m_HorizontalSplitterRect.x,
            (int)(m_Position.y + m_HorizontalSplitterRect.height * m_VerticalSplitterPercentRight),
            (m_Position.width - m_HorizontalSplitterRect.width) - k_SplitterWidth,
            k_SplitterWidth);
        m_VerticalSplitterRectLeft = new Rect(
            m_Position.x,
            (int)(m_Position.y + m_HorizontalSplitterRect.height * m_VerticalSplitterPercentLeft),
            (m_HorizontalSplitterRect.width) - k_SplitterWidth,
            k_SplitterWidth);

        Show();
        Focus();
    }
    const float k_SplitterWidth = 3f;
    private void OnGUI()
    {
        HandleHorizontalResize();
        HandleVerticalResize();



        //Left half
        var bundleTreeRect = new Rect(
            m_Position.x,
            m_Position.y,
            m_HorizontalSplitterRect.x,
            m_VerticalSplitterRectLeft.y - m_Position.y);

        TestOnGUI(bundleTreeRect);
        TestOnGUI(new Rect(
            bundleTreeRect.x,
            bundleTreeRect.y + bundleTreeRect.height + k_SplitterWidth,
            bundleTreeRect.width,
            m_Position.height - bundleTreeRect.height - k_SplitterWidth * 2));


        //Right half.
        float panelLeft = m_HorizontalSplitterRect.x + k_SplitterWidth;
        float panelWidth = m_VerticalSplitterRectRight.width - k_SplitterWidth * 2;
        float searchHeight = 20f;
        float panelTop = m_Position.y + searchHeight;
        float panelHeight = m_VerticalSplitterRectRight.y - panelTop;
        TestOnGUI(new Rect(panelLeft, m_Position.y, panelWidth, searchHeight));
        TestOnGUI(new Rect(
            panelLeft,
            panelTop,
            panelWidth,
            panelHeight));
        TestOnGUI(new Rect(
            panelLeft,
            panelTop + panelHeight + k_SplitterWidth,
            panelWidth,
            (m_Position.height - panelHeight) - k_SplitterWidth * 2));

    }



    bool m_ResizingHorizontalSplitter = false;
    bool m_ResizingVerticalSplitterRight = false;
    bool m_ResizingVerticalSplitterLeft = false;
    Rect m_HorizontalSplitterRect, m_VerticalSplitterRectRight, m_VerticalSplitterRectLeft;
    [SerializeField]
    float m_HorizontalSplitterPercent;
    [SerializeField]
    float m_VerticalSplitterPercentRight;
    [SerializeField]
    float m_VerticalSplitterPercentLeft;
    Rect m_Position;
    private void HandleHorizontalResize()
    {
        m_HorizontalSplitterRect.x = (int)(m_Position.width * m_HorizontalSplitterPercent);
        m_HorizontalSplitterRect.height = m_Position.height;

        EditorGUIUtility.AddCursorRect(m_HorizontalSplitterRect, MouseCursor.ResizeHorizontal);
        if (Event.current.type == EventType.MouseDown && m_HorizontalSplitterRect.Contains(Event.current.mousePosition))
            m_ResizingHorizontalSplitter = true;

        if (m_ResizingHorizontalSplitter)
        {
            m_HorizontalSplitterPercent = Mathf.Clamp(Event.current.mousePosition.x / m_Position.width, 0.1f, 0.9f);
            m_HorizontalSplitterRect.x = (int)(this.maxSize.x * m_HorizontalSplitterPercent);

        }

        if (Event.current.type == EventType.MouseUp)
            m_ResizingHorizontalSplitter = false;
    }
    internal static void DrawOutline(Rect rect, float size)
    {
        Color color = new Color(0.6f, 0.6f, 0.6f, 1.333f);
        if (EditorGUIUtility.isProSkin)
        {
            color.r = 0.12f;
            color.g = 0.12f;
            color.b = 0.12f;
        }

        if (Event.current.type != EventType.Repaint)
            return;

        Color orgColor = GUI.color;
        GUI.color = GUI.color * color;
        GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.yMax - size, rect.width, size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.x, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - size, rect.y + 1, size, rect.height - 2 * size), EditorGUIUtility.whiteTexture);

        GUI.color = orgColor;
    }

    Vector2 m_Dimensions = new Vector2(0, 0);
    const float k_ScrollbarPadding = 16f;
    const float k_BorderSize = 1f;
    public void TestOnGUI(Rect fullPos)
    {
        DrawOutline(fullPos, 1f);

        Rect pos = new Rect(fullPos.x + k_BorderSize, fullPos.y + k_BorderSize, fullPos.width - 2 * k_BorderSize, fullPos.height - 2 * k_BorderSize);


        //if (m_Dimensions.y == 0 || m_Dimensions.x != pos.width - k_ScrollbarPadding)
        //{
        //    //recalculate height.
        //    m_Dimensions.x = pos.width - k_ScrollbarPadding;
        //    m_Dimensions.y = 0;
        //    foreach (var message in m_Messages)
        //    {
        //        m_Dimensions.y += m_Style[0].CalcHeight(new GUIContent(message.message), m_Dimensions.x);
        //    }
        //}

        //m_ScrollPosition = GUI.BeginScrollView(pos, m_ScrollPosition, new Rect(0, 0, m_Dimensions.x, m_Dimensions.y));
        //int counter = 0;
        //float runningHeight = 0.0f;
        //foreach (var message in m_Messages)
        //{
        //    int index = counter % 2;
        //    var content = new GUIContent(message.message);
        //    float height = m_Style[index].CalcHeight(content, m_Dimensions.x);

        //    GUI.Box(new Rect(0, runningHeight, m_Dimensions.x, height), content, m_Style[index]);
        //    GUI.DrawTexture(new Rect(0, runningHeight, 32f, 32f), message.icon);
        //    //TODO - cleanup formatting issues and switch to HelpBox
        //    //EditorGUI.HelpBox(new Rect(0, runningHeight, m_dimensions.x, height), message.message, (MessageType)message.severity);

        //    counter++;
        //    runningHeight += height;
        //}
        GUI.EndScrollView();
    }

    private void HandleVerticalResize()
    {
        m_VerticalSplitterRectRight.x = m_HorizontalSplitterRect.x;
        m_VerticalSplitterRectRight.y = (int)(m_HorizontalSplitterRect.height * m_VerticalSplitterPercentRight);
        m_VerticalSplitterRectRight.width = m_Position.width - m_HorizontalSplitterRect.x;
        m_VerticalSplitterRectLeft.y = (int)(m_HorizontalSplitterRect.height * m_VerticalSplitterPercentLeft);
        m_VerticalSplitterRectLeft.width = m_VerticalSplitterRectRight.width;


        EditorGUIUtility.AddCursorRect(m_VerticalSplitterRectRight, MouseCursor.ResizeVertical);
        if (Event.current.type == EventType.MouseDown && m_VerticalSplitterRectRight.Contains(Event.current.mousePosition))
            m_ResizingVerticalSplitterRight = true;

        EditorGUIUtility.AddCursorRect(m_VerticalSplitterRectLeft, MouseCursor.ResizeVertical);
        if (Event.current.type == EventType.MouseDown && m_VerticalSplitterRectLeft.Contains(Event.current.mousePosition))
            m_ResizingVerticalSplitterLeft = true;


        if (m_ResizingVerticalSplitterRight)
        {
            m_VerticalSplitterPercentRight = Mathf.Clamp(Event.current.mousePosition.y / m_HorizontalSplitterRect.height, 0.2f, 0.98f);
            m_VerticalSplitterRectRight.y = (int)(m_HorizontalSplitterRect.height * m_VerticalSplitterPercentRight);
        }
        else if (m_ResizingVerticalSplitterLeft)
        {
            m_VerticalSplitterPercentLeft = Mathf.Clamp(Event.current.mousePosition.y / m_HorizontalSplitterRect.height, 0.25f, 0.98f);
            m_VerticalSplitterRectLeft.y = (int)(m_HorizontalSplitterRect.height * m_VerticalSplitterPercentLeft);
        }


        if (Event.current.type == EventType.MouseUp)
        {
            m_ResizingVerticalSplitterRight = false;
            m_ResizingVerticalSplitterLeft = false;
        }
    }

}
