using UnityEngine;

public class SkyParallax : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.95f;
    [SerializeField] private float textureParallaxSpeed = 0.5f;
    [SerializeField] private Vector2 textureScale = new Vector2(2f, 2f);
    [SerializeField] private Material skyMaterial;
    [SerializeField] private float quadScaleMultiplier = 4f;

    private Transform cameraTransform;
    private Vector3 initialQuadPos;
    private Vector2 initialTextureOffset;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        initialQuadPos = transform.position;
        initialTextureOffset = skyMaterial.mainTextureOffset;
        skyMaterial.mainTextureScale = textureScale;

        float screenHeight = Camera.main.orthographicSize * 2;
        transform.localScale = new Vector3(screenHeight * Camera.main.aspect * quadScaleMultiplier, screenHeight * quadScaleMultiplier, 1);
    }

    private void LateUpdate()
    {
        Vector3 quadPos = initialQuadPos;
        quadPos.x += (cameraTransform.position.x - initialQuadPos.x) * parallaxSpeed;
        quadPos.y += (cameraTransform.position.y - initialQuadPos.y) * parallaxSpeed;
        transform.position = quadPos;

        Vector2 textureOffset = initialTextureOffset;
        textureOffset.x += (cameraTransform.position.x - initialQuadPos.x) * textureParallaxSpeed;
        textureOffset.y += (cameraTransform.position.y - initialQuadPos.y) * textureParallaxSpeed;
        skyMaterial.mainTextureOffset = textureOffset;
    }
}