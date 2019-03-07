using GoogleARCore;
using System.Collections.Generic;
using UnityEngine;

public class ARSurface : MonoBehaviour
{
    private static int s_PlaneCount = 0;

    private DetectedPlane m_DetectedPlane;

    // Keep previous frame's mesh polygon to avoid mesh update every frame.
    private List<Vector3> m_PreviousFrameMeshVertices = new List<Vector3>();
    private List<Vector3> m_MeshVertices = new List<Vector3>();
    private Vector3 m_PlaneCenter = new Vector3();

    private List<int> m_MeshIndices = new List<int>();

    private Mesh m_Mesh;

    private MeshRenderer m_MeshRenderer;

    public void Awake()
    {
        m_Mesh = GetComponent<MeshFilter>().mesh;
        m_MeshRenderer = GetComponent<UnityEngine.MeshRenderer>();
    }

    public void Update()
    {
        if (m_DetectedPlane == null || m_DetectedPlane.SubsumedBy != null)
        {
            return;
        }

        _UpdateMeshIfNeeded();
    }

    public void Initialize(DetectedPlane plane)
    {
        m_DetectedPlane = plane;

        Update();
    }

    private void _UpdateMeshIfNeeded()
    {
        m_DetectedPlane.GetBoundaryPolygon(m_MeshVertices);

        if (_AreVerticesListsEqual(m_PreviousFrameMeshVertices, m_MeshVertices))
        {
            return;
        }

        m_PreviousFrameMeshVertices.Clear();
        m_PreviousFrameMeshVertices.AddRange(m_MeshVertices);

        m_PlaneCenter = m_DetectedPlane.CenterPose.position;

        Vector3 planeNormal = m_DetectedPlane.CenterPose.rotation * Vector3.up;

        int planePolygonCount = m_MeshVertices.Count;

        const float featherLength = 0.2f;

        const float featherScale = 0.2f;

        for (int i = 0; i < planePolygonCount; ++i)
        {
            Vector3 v = m_MeshVertices[i];

            Vector3 d = v - m_PlaneCenter;

            float scale = 1.0f - Mathf.Min(featherLength / d.magnitude, featherScale);
            m_MeshVertices.Add((scale * d) + m_PlaneCenter);
        }

        m_MeshIndices.Clear();
        int firstOuterVertex = 0;
        int firstInnerVertex = planePolygonCount;

        for (int i = 0; i < planePolygonCount - 2; ++i)
        {
            m_MeshIndices.Add(firstInnerVertex);
            m_MeshIndices.Add(firstInnerVertex + i + 1);
            m_MeshIndices.Add(firstInnerVertex + i + 2);
        }

        for (int i = 0; i < planePolygonCount; ++i)
        {
            int outerVertex1 = firstOuterVertex + i;
            int outerVertex2 = firstOuterVertex + ((i + 1) % planePolygonCount);
            int innerVertex1 = firstInnerVertex + i;
            int innerVertex2 = firstInnerVertex + ((i + 1) % planePolygonCount);

            m_MeshIndices.Add(outerVertex1);
            m_MeshIndices.Add(outerVertex2);
            m_MeshIndices.Add(innerVertex1);

            m_MeshIndices.Add(innerVertex1);
            m_MeshIndices.Add(outerVertex2);
            m_MeshIndices.Add(innerVertex2);
        }

        m_Mesh.Clear();
        m_Mesh.SetVertices(m_MeshVertices);
        m_Mesh.SetTriangles(m_MeshIndices, 0);
    }

    private bool _AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
    {
        if (firstList.Count != secondList.Count)
        {
            return false;
        }

        for (int i = 0; i < firstList.Count; i++)
        {
            if (firstList[i] != secondList[i])
            {
                return false;
            }
        }

        return true;
    }
}