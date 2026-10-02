using System;
using System.Collections.Generic;
using UnityEngine;

namespace KroesSupermarketMod.CustomScripts
{
    internal class MeshColliderVisualizer : MonoBehaviour
    {
        private MeshCollider meshCollider;
        private GameObject lineChildContainer;
        private List<LineRenderer> lineRenderers = new List<LineRenderer>();
        private List<Edge> meshEdges = new List<Edge>();
        private bool showLines = false;

        private struct Edge
        {
            public int v1;
            public int v2;

            public Edge(int a, int b)
            {
                v1 = Math.Min(a, b);
                v2 = Math.Max(a, b);
            }

            public override bool Equals(object obj)
            {
                return obj is Edge other && v1 == other.v1 && v2 == other.v2;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (v1 * 397) ^ v2;
                }
            }
        }

        private void Awake()
        {
            meshCollider = GetComponent<MeshCollider>();

            if (meshCollider == null || meshCollider.sharedMesh == null)
            {
                Debug.LogWarning($"[MeshColliderVisualizer] No MeshCollider or sharedMesh found on {gameObject.name}");
                return;
            }

            // Create a dedicated child GameObject for the wireframe segments
            lineChildContainer = new GameObject("MeshColliderOutlineContainer");
            lineChildContainer.transform.SetParent(transform, false);

            // Extract unique edges from the mesh
            ExtractMeshEdges(meshCollider.sharedMesh);

            // Create a LineRenderer segment for each unique edge
            Material lineMat = new Material(Shader.Find("Sprites/Default")) { color = Color.green };

            foreach (var edge in meshEdges)
            {
                GameObject segmentObj = new GameObject("EdgeSegment");
                segmentObj.transform.SetParent(lineChildContainer.transform, false);

                LineRenderer lr = segmentObj.AddComponent<LineRenderer>();
                lr.useWorldSpace = true;
                lr.startWidth = 0.03f;
                lr.endWidth = 0.03f;
                lr.positionCount = 2;
                lr.material = lineMat;
                lr.startColor = Color.green;
                lr.endColor = Color.green;
                lr.enabled = false;

                lineRenderers.Add(lr);
            }
        }

        private void ExtractMeshEdges(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            HashSet<Edge> uniqueEdges = new HashSet<Edge>();

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int i0 = triangles[i];
                int i1 = triangles[i + 1];
                int i2 = triangles[i + 2];

                uniqueEdges.Add(new Edge(i0, i1));
                uniqueEdges.Add(new Edge(i1, i2));
                uniqueEdges.Add(new Edge(i2, i0));
            }

            meshEdges.AddRange(uniqueEdges);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                showLines = !showLines;
                SetLinesEnabled(showLines);
            }
        }

        private void LateUpdate()
        {
            if (showLines)
            {
                UpdateOutline();
            }
        }

        private void SetLinesEnabled(bool enabled)
        {
            foreach (var lr in lineRenderers)
            {
                if (lr != null)
                {
                    lr.enabled = enabled;
                }
            }
        }

        private void UpdateOutline()
        {
            if (meshCollider == null || meshCollider.sharedMesh == null) return;

            Vector3[] vertices = meshCollider.sharedMesh.vertices;

            for (int i = 0; i < meshEdges.Count; i++)
            {
                Edge edge = meshEdges[i];
                LineRenderer lr = lineRenderers[i];

                Vector3 worldV1 = transform.TransformPoint(vertices[edge.v1]);
                Vector3 worldV2 = transform.TransformPoint(vertices[edge.v2]);

                lr.SetPosition(0, worldV1);
                lr.SetPosition(1, worldV2);
            }
        }

        private void OnDestroy()
        {
            if (lineChildContainer != null)
            {
                Destroy(lineChildContainer);
            }
        }
    }
}