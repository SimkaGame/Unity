using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float lifetime = 2f;
    public float floatSpeed = 1f;

    private TextMeshPro textMesh;
    private Color originalColor;
    private float fadeTimer;

    private void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Destroy(gameObject);
            return;
        }

        originalColor = textMesh.color;
        fadeTimer = lifetime;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        fadeTimer -= Time.deltaTime;
        float alpha = fadeTimer / lifetime;

        Color newColor = originalColor;
        newColor.a = Mathf.Clamp01(alpha);
        textMesh.color = newColor;
    }
}