using UnityEngine;

namespace KroesSupermarketMod.CustomScripts
{
    internal class BoxColliderVisualizer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private Renderer targetRenderer;
        private GameObject lineChildContainer;
        private bool showLines = false;

        private static readonly Vector3[] BoxVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),   new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),  new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),  new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),    new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),    new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f),  new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f)
        };

        private void Awake()
        {
            // Create a dedicated child GameObject for the LineRenderer
            lineChildContainer = new GameObject("SelectionOutlineContainer");
            lineChildContainer.transform.SetParent(transform, false);

            lineRenderer = lineChildContainer.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = true;
            lineRenderer.startWidth = 0.03f;
            lineRenderer.endWidth = 0.03f;
            lineRenderer.positionCount = BoxVertices.Length;

            // Simple bright green material
            Material lineMat = new Material(Shader.Find("Sprites/Default"));
            lineMat.color = Color.green;
            lineRenderer.material = lineMat;
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;

            targetRenderer = GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                showLines = !showLines;
                lineRenderer.enabled = showLines;
            }
        }

        private void LateUpdate()
        {
            if (showLines) UpdateOutline();
        }

        private void UpdateOutline()
        {
            if (lineRenderer == null) return;

            Vector3 center;
            Vector3 size;

            if (targetRenderer != null)
            {
                Bounds bounds = targetRenderer.bounds;
                center = bounds.center;
                size = bounds.size;
            }
            else
            {
                center = transform.position;
                size = transform.lossyScale;
            }

            for (int i = 0; i < BoxVertices.Length; i++)
            {
                Vector3 localPoint = Vector3.Scale(BoxVertices[i], size);
                Vector3 worldPoint = center + transform.rotation * localPoint;
                lineRenderer.SetPosition(i, worldPoint);
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
