using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float minIntensity = 9f;
    [SerializeField] private float maxIntensity = 12f;
    [SerializeField] private float flickerSpeed = 10f;

    private Light2D fireLight;

    private void Awake()
    {
        fireLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        fireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(Time.time * flickerSpeed, 0f));
    }
}