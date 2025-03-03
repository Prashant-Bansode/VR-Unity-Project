using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderGizmoRuntime : MonoBehaviour
{
    // Color of the wireframe box
    public Color gizmoColor = Color.green;
    
    // A simple material for drawing (cached for performance)
    private static Material _lineMaterial;

    void CreateLineMaterial()
    {
        if (_lineMaterial == null)
        {
            // Unity has a built-in shader that is useful for drawing simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnRenderObject()
    {
        CreateLineMaterial();
        _lineMaterial.SetPass(0);

        // Set our transform's matrix as the current matrix
        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(gizmoColor);

        // Get the BoxCollider and its parameters
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 center = box.center;
        Vector3 halfSize = box.size * 0.5f;

        // Compute the 8 corners of the BoxCollider in local space
        Vector3[] corners = new Vector3[8];
        corners[0] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        corners[1] = center + new Vector3( halfSize.x, -halfSize.y, -halfSize.z);
        corners[2] = center + new Vector3( halfSize.x, -halfSize.y,  halfSize.z);
        corners[3] = center + new Vector3(-halfSize.x, -halfSize.y,  halfSize.z);
        corners[4] = center + new Vector3(-halfSize.x,  halfSize.y, -halfSize.z);
        corners[5] = center + new Vector3( halfSize.x,  halfSize.y, -halfSize.z);
        corners[6] = center + new Vector3( halfSize.x,  halfSize.y,  halfSize.z);
        corners[7] = center + new Vector3(-halfSize.x,  halfSize.y,  halfSize.z);

        // Draw bottom rectangle
        DrawLine(corners[0], corners[1]);
        DrawLine(corners[1], corners[2]);
        DrawLine(corners[2], corners[3]);
        DrawLine(corners[3], corners[0]);

        // Draw top rectangle
        DrawLine(corners[4], corners[5]);
        DrawLine(corners[5], corners[6]);
        DrawLine(corners[6], corners[7]);
        DrawLine(corners[7], corners[4]);

        // Draw vertical edges
        DrawLine(corners[0], corners[4]);
        DrawLine(corners[1], corners[5]);
        DrawLine(corners[2], corners[6]);
        DrawLine(corners[3], corners[7]);

        GL.End();
        GL.PopMatrix();
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GL.Vertex(start);
        GL.Vertex(end);
    }
}
