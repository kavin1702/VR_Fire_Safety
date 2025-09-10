using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class OutlineEffect : MonoBehaviour
{
    private Material originalMaterial;
    private Material outlineMaterial;
    private Renderer rend;

    public Color outlineColor = Color.yellow;
    public float outlineWidth = 2f;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // Save the original material
        originalMaterial = rend.material;

        // Create a new outline material (uses Unity's Unlit/Color shader)
        Shader outlineShader = Shader.Find("Unlit/Color");
        outlineMaterial = new Material(outlineShader);
        outlineMaterial.color = outlineColor;
    }

    public void EnableOutline()
    {
        rend.materials = new Material[] { originalMaterial, outlineMaterial };
        outlineMaterial.SetFloat("_Outline", outlineWidth);
    }

    public void DisableOutline()
    {
        rend.material = originalMaterial;
    }
}
