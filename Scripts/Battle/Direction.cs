using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Direction
{

    public enum Type { North, South, East, West, NorthEast, SouthEast, NorthWest, SouthWest };
    public Type type = Type.North;

    public static Type[] AllTypes = { Type.North, Type.NorthEast, Type.East, Type.SouthEast, Type.South, Type.SouthWest, Type.West, Type.NorthWest };

    public Direction(Type type)
    {
        this.type = type;

    }

    public bool IsEqual(Direction other_direction)
    {
        return this.type == other_direction.type;
    }

    public Vector3 GetVector()
    {
        Vector3 finalDirection;
        switch (type)
        {
            case Type.South:
                finalDirection = new Vector3(0, -1, 0);
                break;
            case Type.West:
                finalDirection = new Vector3(-1, 0, 0);
                break;
            case Type.East:
                finalDirection = new Vector3(1, 0, 0);
                break;
            case Type.North:
                finalDirection = new Vector3(0, 1, 0);
                break;
            case Type.SouthEast:
                finalDirection = new Vector3(1, -1, 0);
                break;
            case Type.SouthWest:
                finalDirection = new Vector3(-1, -1, 0);
                break;
            case Type.NorthEast:
                finalDirection = new Vector3(1, 1, 0);
                break;
            case Type.NorthWest:
                finalDirection = new Vector3(-1, 1, 0);
                break;
            default:
                finalDirection = new Vector3(0, 0, 0);
                break;
        }
        return finalDirection.normalized;
    }

    public float GetRotation()
    {
        float z_rotation = 0;
        switch (type)
        {
            case Type.South:
                z_rotation = 180;
                break;
            case Type.West:
                z_rotation = 90;
                break;
            case Type.East:
                z_rotation = 270;
                break;
            case Type.North:
                z_rotation = 0;
                break;
            case Type.SouthEast:
                z_rotation = 225;
                break;
            case Type.SouthWest:
                z_rotation = 135;
                break;
            case Type.NorthEast:
                z_rotation = 315;
                break;
            case Type.NorthWest:
                z_rotation = 45;
                break;
        }
        return z_rotation;
    }


    // return a facing (direction) object for whichever facing is closest to the direction vector
    public static Direction ClosestDirection(Vector3 direction)
    {
        float dist = Mathf.Infinity;
        Direction best_direction = null;

        foreach (Type t in AllTypes)
        {
            Direction tmp = new Direction(t);
            float t_dist = Vector3.Distance(direction.normalized, tmp.GetVector());
            if (t_dist < dist)
            {
                dist = t_dist;
                best_direction = tmp;
            }
        }
        return best_direction;
    }

    // returns TRUE if second unit is closer to the front line (further down the direction they are moving)
    public bool GetFrontlineUnit(old_Battalion first, old_Battalion second)
    {
        Vector3 dir = this.GetVector();

        float first_score = Vector3.Dot(first.transform.position, dir);
        float second_score = Vector3.Dot(second.transform.position, dir);
        if (first_score >= second_score)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override string ToString()
    {
        return this.type.ToString();
    }
    
}
