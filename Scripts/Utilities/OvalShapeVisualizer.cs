using UnityEngine;

namespace Utilities
{
    public class OvalShapeVisualizer : MonoBehaviour
    {
        public float width = 2f;
        public float height = 1f;
        public int numSegments = 50;

        private void OnDrawGizmos()
        {
            DrawOval();
        }

        private void DrawOval()
        {
            Gizmos.color = Color.yellow;

            Vector3[] vertices = new Vector3[numSegments + 1];

            for (int i = 0; i < numSegments; i++)
            {
                float angle = (Mathf.PI * 2 * i) / numSegments;
                vertices[i] = new Vector3(Mathf.Sin(angle) * width, 0, Mathf.Cos(angle) * height);
            }
            vertices[numSegments] = vertices[0];

            for (int i = 0; i < numSegments; i++)
            {
                Gizmos.DrawLine(transform.position + vertices[i], transform.position + vertices[i + 1]);
            }
        }
    }
}