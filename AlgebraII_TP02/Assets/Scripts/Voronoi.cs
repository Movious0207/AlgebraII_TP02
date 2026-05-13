using UnityEngine;
using System.Collections.Generic;
using CustomMath;
using UnityEditor.ShaderGraph;

public class Voronoi : MonoBehaviour
{
    [SerializeField] private GameObject[] interestPoints;
    [SerializeField] private GameObject player;

    public class points
    {
        public points (Vec3 position)
        {
            this.position = position;
        }
        public Vec3 position;
        public Color color;
        public List<MyPlane> planes = new List<MyPlane>();
    }

    public List<points> point = new List<points>();

    private MyPlane GetPlaneInBetween(Vec3 from, Vec3 to)
    {
        Vec3 middlePoint = from + (to - from) / 2;
        Vec3 planeNormal = from - middlePoint;

        return new MyPlane(planeNormal, middlePoint);
    }

    private bool IsRedundant(points point, MyPlane plane)
    {
        Vec3 planeClosestPoint = plane.ClosestPointOnPlane(point.position);

        for(int i = 0; i < point.planes.Count; i++)
        {
            if(point.planes[i] == plane)
            {
                continue;
            }
            if(!point.planes[i].GetSide(planeClosestPoint))
            {
                return true;
            }
        }
        return false;
    }

    private void DrawPlanes(MyPlane plane, Vec3 point, Color color)
    {
        Vec3 planeCenter = plane.ClosestPointOnPlane(point);
        Gizmos.color = color;

        Gizmos.DrawLine(planeCenter, planeCenter + plane.Normal);

        //draw tip
        Gizmos.DrawSphere(planeCenter, 0.05f);

        Gizmos.color = default;
    }

    void Start()
    {
        for (int i = 0; i < interestPoints.Length; i++)
        {
            point.Add(new points(new Vec3 (interestPoints[i].transform.position.x, interestPoints[i].transform.position.y, interestPoints[i].transform.position.z)));
        }
        for (int i = 0; i < interestPoints.Length; i++)
        {
            for (int j = 0; j < interestPoints.Length; j++)
            {
                if (i != j)
                {
                    point[i].planes.Add(GetPlaneInBetween(point[i].position,point[j].position));
                }
            }
        }
        for (int i = 0; i < interestPoints.Length; i++)
        {
            for (int j = 0; j < point[i].planes.Count; j++)
            {
                if (IsRedundant(point[i],point[i].planes[j]))
                {
                    point[i].planes.Remove(point[i].planes[j]);
                }
            }
        }
    }

    
    void Update()
    {
        Vec3 playerPos = new Vec3(player.transform.position);
        for(int i = 0; i < interestPoints.Length; i++)
        {
            bool isInside = true;
            for(int j = 0; j < point[i].planes.Count; j++)
            {
                if(!point[i].planes[j].GetSide(playerPos))
                {
                    isInside = false;
                }
            }
            if(isInside)
            {
                point[i].color = Color.green;
            }
            else
            {
                point[i].color = Color.red;
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < interestPoints.Length; i++)
        {
            for (int j = 0; j < point[i].planes.Count; j++)
            {
                DrawPlanes(point[i].planes[j], point[i].position, point[i].color);
            }
        }
    }
}
