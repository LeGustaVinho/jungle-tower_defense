using Sirenix.OdinInspector;

namespace Jungle.Scripts.Core
{
    using UnityEngine;

    public class GridDrawer : MonoBehaviour
    {
        public int width = 10;
        public int height = 10;
        public float cellSize = 1f;
        public Color color;
        public Material lineMaterial;

        [Button]
        public void DrawGrid()
        {
            Vector3 startPosition = transform.position - new Vector3((width * cellSize / 2 + (cellSize/2)), 0, (height * cellSize / 2) + (cellSize/2));

            for (int x = 0; x <= width; x++)
            {
                Vector3 start = startPosition + new Vector3(x * cellSize, 0, 0);
                Vector3 end = start + new Vector3(0, 0, height * cellSize);
                CreateLine(start, end);
            }

            for (int z = 0; z <= height; z++)
            {
                Vector3 start = startPosition + new Vector3(0, 0, z * cellSize);
                Vector3 end = start + new Vector3(width * cellSize, 0, 0);
                CreateLine(start, end);
            }
        }

        private void CreateLine(Vector3 start, Vector3 end)
        {
            GameObject line = new GameObject("Line");
            line.transform.position = start;
            line.transform.parent = transform;
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }
}