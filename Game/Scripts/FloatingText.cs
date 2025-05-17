using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float floatSpeed = 1f;

    private TextMeshPro textMesh;
    private Color originalColor;
    private float fadeTimer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;
        fadeTimer = lifetime;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);
        fadeTimer -= Time.deltaTime;
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Clamp01(fadeTimer / lifetime));
    }

    private void Start()
{
    Vector3 pos = transform.position;
    pos.z = -1f; // или -5f, если камера на -10
    transform.position = pos;
}

}