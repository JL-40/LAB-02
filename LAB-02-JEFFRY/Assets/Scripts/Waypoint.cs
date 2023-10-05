using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint
{
    [SerializeField] public Vector3 position;

    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
    }

    public Vector3 GetPosition { get { return position; } }

    public Waypoint()
    {
        position = Vector3.zero;
    }
}
