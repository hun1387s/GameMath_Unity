using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrossProductEditor : CommonEditor, IUpdateSceneGUI
{
    public Vector3 vec1;
    public Vector3 vec2;
    public Vector3 vec1xvec2;

    SerializedObject obj;
    SerializedProperty propVec1;
    SerializedProperty propVec2;
    SerializedProperty propVec1xVec2;

    GUIStyle guiStyle = new GUIStyle();

    [MenuItem("Tools/Cross Product")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CrossProductEditor), true, "Cross Product");
    }

    void SetDefaultValues()
    {
        vec1 = new Vector3(0f, 1f, 0f);
        vec2 = new Vector3(1f, 0f, 0f);
    }


    private void OnEnable()
    {
        if (vec1 == Vector3.zero && vec2 == Vector3.zero)
            SetDefaultValues();

        obj = new SerializedObject(this);
        propVec1 = obj.FindProperty("vec1");
        propVec2 = obj.FindProperty("vec2");
        propVec1xVec2 = obj.FindProperty("vec1xvec2");

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

        DrawBlockGUI("vec1", propVec1);
        DrawBlockGUI("vec2", propVec2);
        DrawBlockGUI("vec1xvec2", propVec1xVec2);

        if (obj.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Reset Value"))
        {
            SetDefaultValues();
        }
    }

    public void SceneGUI(SceneView view)
    {
        Vector3 v1 = Handles.PositionHandle(vec1, Quaternion.identity);
        Vector3 v2 = Handles.PositionHandle(vec2, Quaternion.identity);

        Handles.color = Color.blue;
        Vector3 v1xv2 = CrossProduct(v1, v2);
        Handles.DrawSolidDisc(v1xv2, Vector3.forward, 0.05f);

        if (vec1 != v1 || vec2 != v2)
        {
            Undo.RecordObject(this, "Tool Move");

            vec1 = v1;
            vec2 = v2;
            vec1xvec2 = v1xv2;

            RepaintOnGUI();
        }

        DrawLineGUI(v1, "vec1", Color.green);
        DrawLineGUI(v2, "vec2", Color.red);
        DrawLineGUI(v1xv2, "vec1 X vec2", Color.blue);
    }

    void DrawLineGUI(Vector3 pos, string text, Color color)
    {
        Handles.color = color;
        Handles.Label(pos, text, guiStyle);

        Handles.DrawAAPolyLine(3f, pos, Vector3.zero);
    }

    void RepaintOnGUI()
    {
        Repaint();
    }

    //Vector3 CrossProduct(Vector3 p, Vector3 q)
    //{
    //    float x = p.y * q.z - p.z * q.y;
    //    float y = p.z * q.x - p.x * q.z;
    //    float z = p.x * q.y - p.y * q.x;

    //    return new Vector3(x, y, z);
    //}

    Vector3 CrossProduct(Vector3 p, Vector3 q)
    {
        Matrix4x4 m = new Matrix4x4();

        m[0, 0] = 0;
        m[0, 1] = q.z;
        m[0, 2] = -q.y;

        m[1, 0] = -q.z;
        m[1, 1] = 0;
        m[1, 2] = q.x;

        m[2, 0] = q.y;
        m[2, 1] = -q.x;
        m[2, 2] = 0;

        return m * p;
    }
}
