using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathManager))]
public class PathManagerEditor : Editor
{
    [SerializeField] PathManager pathManager;

    [SerializeField] List<Waypoint> thePath;
    List<int> toDelete;

    Waypoint selectedPoint = null;
    bool doRepaint = true;

    private void OnSceneGUI()
    {
        thePath = pathManager.GetPath();
        DrawPath(thePath);
    }

    private void OnEnable()
    {
        pathManager = target as PathManager;
        toDelete = new List<int>();
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();
        thePath = pathManager.GetPath();

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Path");

        DrawGUIForPoints();

        // Button for adding a point to the path
        if (GUILayout.Button("Add Point to Path"))
        {
            pathManager.CreateAddPoint();
        }

        EditorGUILayout.EndVertical();
        SceneView.RepaintAll();
    }

    void DrawGUIForPoints()
    {
        if (thePath != null && thePath.Count > 0)
        {
            for (int i = 0; i < thePath.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                Waypoint p = thePath[i];

                Color c = GUI.color;
                if (selectedPoint == p)
                {
                    GUI.color = Color.green;
                }

                Vector3 oldPosition = p.GetPosition;
                Vector3 newPosition = EditorGUILayout.Vector3Field("", oldPosition);

                if(EditorGUI.EndChangeCheck())
                {
                    p.SetPosition(newPosition);
                }

                // the Delete button
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    toDelete.Add(i); // add the index to our delete list
                }

                GUI.color = c;
                EditorGUILayout.EndHorizontal();
            }
        }
        if (toDelete.Count > 0)
        {
            foreach (int i in toDelete)
            {
                thePath.RemoveAt(i); // remove the path
            }
            toDelete.Clear(); // Clear the delete list for the next time
        }
    }

    public void DrawPath(List<Waypoint> path)
    {
        if (path != null)
        {
            int current = 0;
            foreach (Waypoint wp in path)
            {
                // draw current point
                doRepaint = DrawPoint(wp);
                int next = (current + 1) % path.Count;
                Waypoint wpNext = path[next];

                DrawPathLine(wp, wpNext);

                // advance counter
                current++;
            }
        }
        if (doRepaint)
        {
            Repaint();
        }
    }

    public void DrawPathLine(Waypoint p1, Waypoint p2)
    {
        // draw a line between current and next point
        Color c = Handles.color;
        Handles.color = Color.gray;
        Handles.DrawLine(p1.GetPosition, p2.GetPosition);
        Handles.color = c;
    }

    public bool DrawPoint(Waypoint p)
    {
        bool isChanged = false;
        if (selectedPoint == p)
        {
            Color c = Handles.color;
            Handles.color = Color.green;

            EditorGUI.BeginChangeCheck();
            Vector3 oldPosition = p.GetPosition;
            Vector3 newPosition = Handles.PositionHandle(oldPosition, Quaternion.identity);

            float handleSize = HandleUtility.GetHandleSize(newPosition);

            Handles.SphereHandleCap(-1, newPosition, Quaternion.identity, 0.25f * handleSize, EventType.Repaint);
            if (EditorGUI.EndChangeCheck())
            {
                p.SetPosition(newPosition);
            }

            Handles.color = c;
        }
        else
        {
            Vector3 currentPosition = p.GetPosition;
            float handleSize = HandleUtility.GetHandleSize(currentPosition);
            if (Handles.Button(currentPosition, Quaternion.identity, 0.25f * handleSize, 0.25f * handleSize, Handles.SphereHandleCap))
            {
                isChanged = true;
                selectedPoint = p;
            }
        }
        return isChanged;
    }
}
