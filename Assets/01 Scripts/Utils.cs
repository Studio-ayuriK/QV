using UnityEngine;

public class Utils {
    // scale is not half extents!!
    static public void DebugDrawCube(Vector3 position, Quaternion rotation, Vector3 scale) {
        Vector3 extents = scale * 0.5f;

        Vector3[] points = new Vector3[8];
        // positive z
        points[0] = new Vector3(-extents.x, extents.y, extents.z);
        points[1] = new Vector3(extents.x, extents.y, extents.z);
        points[2] = new Vector3(extents.x, -extents.y, extents.z);
        points[3] = new Vector3(-extents.x, -extents.y, extents.z);
        // negative z
        points[4] = new Vector3(-extents.x, extents.y, -extents.z);
        points[5] = new Vector3(extents.x, extents.y, -extents.z);
        points[6] = new Vector3(extents.x, -extents.y, -extents.z);
        points[7] = new Vector3(-extents.x, -extents.y, -extents.z);

        for (int i = 0; i < 8; ++i)
            points[i] = (rotation * points[i]) + position;

        Debug.DrawLine(points[0], points[1], Color.red, 3.0f);
        Debug.DrawLine(points[1], points[2], Color.red, 3.0f);
        Debug.DrawLine(points[2], points[3], Color.red, 3.0f);
        Debug.DrawLine(points[3], points[0], Color.red, 3.0f);

        Debug.DrawLine(points[4], points[5], Color.red, 3.0f);
        Debug.DrawLine(points[5], points[6], Color.red, 3.0f);
        Debug.DrawLine(points[6], points[7], Color.red, 3.0f);
        Debug.DrawLine(points[7], points[4], Color.red, 3.0f);

        Debug.DrawLine(points[0], points[4], Color.red, 3.0f);
        Debug.DrawLine(points[1], points[5], Color.red, 3.0f);
        Debug.DrawLine(points[2], points[6], Color.red, 3.0f);
        Debug.DrawLine(points[3], points[7], Color.red, 3.0f);
    }
}
