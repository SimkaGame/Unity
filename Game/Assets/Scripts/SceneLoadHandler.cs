using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    private bool isFirstGameLaunch = true;
    [SerializeField] private string firstLevelScene = "Level_1";
    [SerializeField] private string mainMenuScene = "MainMenu";
    public static bool isTeleportTransition = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);

        isFirstGameLaunch = !(PlayerPrefs.HasKey("LastCheckpointPosition") && PlayerPrefs.HasKey("LastCheckpointScene"));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuScene)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player || CheckpointManager.Instance == null)
            return;

        PlayerController pc = player.GetComponent<PlayerController>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();

        if (pc) pc.IsTeleporting = false;
        if (rb) rb.simulated = true;
        if (sr) sr.enabled = true;

        bool isFirstLevel = scene.name == firstLevelScene;

        if (isFirstLevel && isFirstGameLaunch)
        {
            CheckpointManager.Instance.SetCheckpoint(player.transform.position, null, scene.name);
            isFirstGameLaunch = false;
        }
        else if (isTeleportTransition)
        {
            Portal spawnPortal = GameObject.FindGameObjectWithTag("SpawnPortal")?.GetComponent<Portal>() ??
                                 Object.FindFirstObjectByType<Portal>();

            Vector3 spawnPos = spawnPortal ? spawnPortal.transform.position + new Vector3(0.5f, 0.5f, 0) : player.transform.position;
            CheckpointManager.Instance.SetCheckpoint(spawnPos, spawnPortal?.GetComponent<AudioSource>(), scene.name);
            player.transform.position = spawnPos;
        }
        else if (CheckpointManager.Instance.GetLastCheckpointScene() == scene.name)
        {
            player.transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
        }
        else
        {
            CheckpointManager.Instance.SetCheckpoint(player.transform.position, null, scene.name);
        }

        isTeleportTransition = false;
    }

    public static void SetTeleportTransition()
    {
        isTeleportTransition = true;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
