using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
//using System.Drawing;

public class DotProductEditor : EditorWindow
{
    public Vector3 pt0;
    public Vector3 pt1;
    public Vector3 ptC;

    SerializedObject obj;
    SerializedProperty propP0;
    SerializedProperty propP1;
    SerializedProperty propC;

    GUIStyle guiStyle = new GUIStyle();


    [MenuItem("Tools/Dot Product")]
    public static void ShowWindow()
    {
        DotProductEditor window = (DotProductEditor)GetWindow
            (typeof(DotProductEditor), true, "Dot Product");
        window.Show();
    }

    private void OnEnable()
    {
        if (pt0 == Vector3.zero && pt1 == Vector3.zero)
        {
            pt0 = new Vector3(0f, 1f, 0f);
            pt1 = new Vector3(.5f, .5f, 0f);
            ptC = Vector3.zero;
        }

        obj = new SerializedObject(this);
        propP0 = obj.FindProperty("pt0");
        propP1 = obj.FindProperty("pt1");
        propC = obj.FindProperty("ptC");

        guiStyle.fontSize = 25;
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.normal.textColor = Color.white;

        SceneView.duringSceneGui += SceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= SceneGUI;
    }

    private void OnGUI()
    {
        obj.Update();

        DrawBlockGUI("p0", propP0);
        DrawBlockGUI("p1", propP1);
        DrawBlockGUI("c", propC);

        if (obj.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }
    }

    void DrawBlockGUI(string lab, SerializedProperty prop)
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField(lab, GUILayout.Width(50));
        EditorGUILayout.PropertyField(prop, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }

    private void SceneGUI(SceneView view)
    {
        Handles.color = Color.red;
        Vector3 p0 = SetMovePoint(pt0);
        Handles.color = Color.green;
        Vector3 p1 = SetMovePoint(pt1);
        Handles.color = Color.white;
        Vector3 c = SetMovePoint(ptC);

        if (pt0 != p0 || pt1 != p1 || ptC != c)
        {
            pt0 = p0;
            pt1 = p1;
            ptC = c;

            Repaint();
        }

        DrawLabel(p0, p1, c);
    }

    Vector3 SetMovePoint(Vector3 pos)
    {
        float size = HandleUtility.GetHandleSize(Vector3.zero) * 0.15f;
        return Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.SphereHandleCap);
    }

    float DotProduct(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Vector3 a = (p0 - c).normalized;
        Vector3 b = (p1 - c).normalized;

        return (a.x * b.x) + (a.y * b.y) + (a.z * b.z);
    }

    void DrawLabel(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Handles.Label(c, DotProduct(p0, p1, c).ToString("F2"), guiStyle);
        Handles.color = Color.black;

        Vector3 cLeft = WorldRotation(p0, c, new Vector3(0f, 1f, 0f));
        Vector3 cRight = WorldRotation(p0, c, new Vector3(0f, -1f, 0f));


        Handles.DrawAAPolyLine(3f, p0, c);
        Handles.DrawAAPolyLine(3f, p1, c);
        Handles.DrawAAPolyLine(3f, c, cLeft);
        Handles.DrawAAPolyLine(3f, c, cRight);

    }

    Vector3 WorldRotation(Vector3 p, Vector3 c, Vector3 pos)
    {
    // R = C + HP
    // C: 기준점(central point), 중심이 되는 벡터
    // H: 회전(Quaternion rotation)을 의미하며, 특정 벡터의 방향을 기준으로 회전이 적용
    // 회전의 각도는 p0−c 방향의 아크탄젠트(atan) 값을 기반으로 계산
    // P: 벡터의 크기는 C와 새로운 벡터 R 사이의 거리와 같다
    // R: 변환된 최종 위치 벡터

        Vector2 dir = (p - c).normalized;
        float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.AngleAxis(ang, Vector3.forward);

        return c + rot * pos;
    }

}
