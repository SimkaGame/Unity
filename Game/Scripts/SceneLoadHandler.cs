using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogWarning("Player not found in scene: " + scene.name);
            return;
        }

        if (CheckpointManager.Instance == null)
        {
            Debug.LogWarning("CheckpointManager is null, cannot set checkpoint.");
            return;
        }

        if (CheckpointManager.Instance.GetLastCheckpointScene() == scene.name)
        {
            player.transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
            Debug.Log($"Player spawned at checkpoint: {player.transform.position}");
        }
        else
        {
            Portal spawnPortal = GameObject.FindGameObjectWithTag("SpawnPortal")?.GetComponent<Portal>();
            if (spawnPortal == null)
            {
                spawnPortal = Object.FindFirstObjectByType<Portal>();
            }

            if (spawnPortal)
            {
                Vector3 spawnPosition = spawnPortal.transform.position;
                CheckpointManager.Instance.SetCheckpoint(spawnPosition, spawnPortal.GetComponent<AudioSource>(), scene.name);
                player.transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
                Debug.Log($"Player spawned at portal: {player.transform.position}");
                spawnPortal.StartCoroutine(spawnPortal.BlockTriggerForSpawn());
            }
            else
            {
                // Устанавливаем временный чекпоинт на позиции игрока, если портал не найден
                CheckpointManager.Instance.SetCheckpoint(player.transform.position, null, scene.name);
                Debug.Log($"No portal found, set checkpoint at player position: {player.transform.position}");
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}