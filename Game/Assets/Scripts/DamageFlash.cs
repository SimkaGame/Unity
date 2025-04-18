using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageFlash : MonoBehaviour
{
    [Header("Цвет вспышки")]
    [SerializeField] private Color flashColor = Color.red;

    [Header("Длительность эффекта")]
    [SerializeField] private float flashDuration = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void PlayFlash()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = flashColor;

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(flashColor, originalColor, t / flashDuration);
            yield return null;
        }

        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }
}
