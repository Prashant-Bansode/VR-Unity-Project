using UnityEngine;
using TMPro;

public class TireAttachmentZone : MonoBehaviour
{
    [Header("Tire Pressure Settings")]
    [Tooltip("Tire pressure value in PSI")]
    public float tirePressure = 32.0f;

    [Header("UI Settings")]
    [Tooltip("TextMeshPro UI element to display tire pressure info")]
    public TMP_Text tirePressureText;
    [Tooltip("Initial instruction image to disable when gauge is grabbed.")]
    public GameObject TyreInstructionImage;
    [Tooltip("Initial instruction text to disable when gauge is grabbed.")]
    public TMP_Text tyreInstructionText;
    public TMP_Text CongratulationText;


    [Header("Gizmo Settings")]
    [Tooltip("Color when gauge is not attached (default: red)")]
    public Color notAttachedColor = Color.red;
    [Tooltip("Color when gauge is attached (default: green)")]
    public Color attachedColor = Color.green;

    // Internal flag to check if the gauge is attached
    private bool gaugeAttached = false;

    // Cache BoxCollider reference
    private BoxCollider boxCollider;

    // Static material for runtime gizmo drawing
    private static Material _lineMaterial;

    void Start()
    {
        // Get the BoxCollider attached to this GameObject
        boxCollider = GetComponent<BoxCollider>();

        // Initially hide the tire pressure text
        if (tirePressureText != null)
            tirePressureText.gameObject.SetActive(false);
    }

    // When a collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the tire gauge (by tag)
        if (!gaugeAttached && other.CompareTag("TireGauge"))
        {
            gaugeAttached = true;
            Debug.Log("Tire gauge detected in attachment zone.");
            
            TyreInstructionImage.SetActive(false);
        
            tyreInstructionText.gameObject.SetActive(false);

            // Snap the gauge to the attachment zone
            SnapGauge(other.gameObject);

            // Display the tire pressure info permanently (text remains enabled)
            if (tirePressureText != null)
            {
                tirePressureText.text = "Tire Pressure: " + tirePressure.ToString("F1") + " PSI";
                tirePressureText.gameObject.SetActive(true);
                CongratulationText.gameObject.SetActive(true);
            }
        }
    }

    // Snap the gauge to the attachment zone's position and rotation
    private void SnapGauge(GameObject gauge)
    {
        gauge.transform.position = transform.position;
        gauge.transform.rotation = transform.rotation;
    }

    // Draw a runtime wireframe box around the BoxCollider visible in both Editor and Game view
    void OnRenderObject()
    {
        if (boxCollider == null)
            return;

        // Choose color based on whether gauge is attached
        Color gizmoColor = gaugeAttached ? attachedColor : notAttachedColor;

        CreateLineMaterial();
        _lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(gizmoColor);

        Vector3 center = boxCollider.center;
        Vector3 halfSize = boxCollider.size * 0.5f;

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

    // Create a simple material for line drawing if not already created
    void CreateLineMaterial()
    {
        if (_lineMaterial == null)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            _lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            _lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Helper method to draw a line between two points
    void DrawLine(Vector3 start, Vector3 end)
    {
        GL.Vertex(start);
        GL.Vertex(end);
    }
}
