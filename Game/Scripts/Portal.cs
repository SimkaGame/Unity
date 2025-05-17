using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Portal : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private bool requireCondition;
    [SerializeField] private bool canEnter = true;
    [SerializeField] private AudioSource portalSound;
    [SerializeField] private Animator transitionAnimator;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Image darkImage;
    [SerializeField] private Canvas mainCanvas;

    private bool canTrigger = true; // Управление триггером

    private void Awake()
    {
        if (fadeImage)
            fadeImage.gameObject.SetActive(false);
        if (darkImage)
            darkImage.color = Color.clear;
        if (!transitionAnimator || !fadeImage)
            enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && (!requireCondition || canEnter) && canTrigger)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player && !player.IsTeleporting)
            {
                Debug.Log($"Portal triggered at: {transform.position}, player at: {other.transform.position}");
                player.IsTeleporting = true;
                canTrigger = false;
                StartCoroutine(Transition(player));
            }
        }
    }

    private IEnumerator Transition(PlayerController player)
    {
        float duration = 8.5f;
        float elapsed = 0f;

        if (portalSound)
            portalSound.Play();
        if (transitionAnimator && fadeImage)
        {
            fadeImage.gameObject.SetActive(true);
            transitionAnimator.SetTrigger("FadeOut");
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (darkImage)
                darkImage.color = new Color(0, 0, 0, elapsed / duration);
            yield return null;
        }

        if (darkImage)
            darkImage.color = Color.clear;
        if (fadeImage)
            fadeImage.gameObject.SetActive(false);
        if (mainCanvas)
            mainCanvas.enabled = false;
        if (player)
            player.IsTeleporting = false;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
            yield return null;

        // Блокируем триггер на 5 секунд после загрузки
        StartCoroutine(BlockTriggerForSpawn());
    }

    // Блокировка триггера на 5 секунд
    public IEnumerator BlockTriggerForSpawn()
    {
        canTrigger = false;
        yield return new WaitForSeconds(5f);
        canTrigger = true;
        Debug.Log($"Portal trigger re-enabled at: {transform.position}");
    }

    public void SetCanEnter(bool value)
    {
        canEnter = value;
    }
}