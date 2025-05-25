using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetScene = "MainMenu";
    [SerializeField] private bool requireCondition;
    [SerializeField] private bool canEnter = true;
    [SerializeField] private bool completesLevel = false;
    [SerializeField] private int levelIndex = 1;
    [SerializeField] private AudioSource portalSound;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image darkImage;
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private float spawnCooldown = 0.1f;
    [SerializeField] private float transitionDuration = 3f;

    private bool canTrigger = false;
    private static bool isTransitioning = false;
    private BoxCollider2D triggerCollider;

    private void Awake()
    {
        isTransitioning = false;
        triggerCollider = GetComponent<BoxCollider2D>();

        if (fadeImage) fadeImage.gameObject.SetActive(false);
        if (darkImage) darkImage.color = Color.clear;
        if (string.IsNullOrEmpty(targetScene) || !SceneExists(targetScene)) targetScene = "MainMenu";
        if (completesLevel && levelIndex < 1) completesLevel = false;

        if (triggerCollider == null)
        {
            triggerCollider = gameObject.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
        }
        else if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
        }

        triggerCollider.enabled = false;
        StartCoroutine(EnableTriggerAfterSpawn());
    }

    private bool SceneExists(string name)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            if (System.IO.Path.GetFileNameWithoutExtension(path) == name)
                return true;
        }
        return false;
    }

    IEnumerator EnableTriggerAfterSpawn()
    {
        triggerCollider.enabled = false;
        canTrigger = false;
        yield return new WaitForSeconds(spawnCooldown);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            if (gameObject.scene.name == "Level_2")
            {
                while (triggerCollider.bounds.Contains(player.transform.position))
                    yield return null;
            }
            else
            {
                while (Vector3.Distance(player.transform.position, transform.position) < 1.5f ||
                       triggerCollider.bounds.Contains(player.transform.position))
                    yield return null;
            }
        }

        triggerCollider.enabled = true;
        canTrigger = true;
    }

    public void ResetTriggerCooldown()
    {
        StopAllCoroutines();
        triggerCollider.enabled = false;
        canTrigger = false;
        StartCoroutine(EnableTriggerAfterSpawn());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || (requireCondition && !canEnter) || !canTrigger || isTransitioning)
            return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null || player.IsTeleporting)
            return;

        isTransitioning = true;
        player.IsTeleporting = true;
        canTrigger = false;
        StartCoroutine(Transition(player));
    }

    IEnumerator Transition(PlayerController player)
    {
        float elapsed = 0f;
        portalSound?.Play();

        if (transitionAnimator && fadeImage)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.canvas.sortingOrder = 10;
            fadeImage.canvas.enabled = true;
            transitionAnimator.SetTrigger("FadeOut");
        }

        if (darkImage)
        {
            darkImage.canvas.sortingOrder = 5;
            darkImage.canvas.enabled = true;
        }

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            if (darkImage)
                darkImage.color = new Color(0, 0, 0, elapsed / transitionDuration);
            yield return null;
        }

        if (darkImage) darkImage.color = Color.clear;
        if (fadeImage) fadeImage.gameObject.SetActive(false);
        if (mainCanvas) mainCanvas.enabled = false;
        if (player) player.IsTeleporting = false;

        if (completesLevel && GameManager.Instance)
            GameManager.Instance.CompleteLevel(levelIndex);

        SceneLoadHandler.SetTeleportTransition();
        yield return SceneManager.LoadSceneAsync(targetScene);

        isTransitioning = false;
    }

    public void SetCanEnter(bool value)
    {
        canEnter = value;
    }
}
