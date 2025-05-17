using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadHandler : MonoBehaviour
{
    private bool isFirstGameLaunch = true; // Флаг для первого запуска игры

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

        // Проверяем, является ли текущая сцена первым уровнем
        bool isFirstLevel = scene.buildIndex == 0; // Или scene.name == "Level1", если используете имя

        if (isFirstLevel && isFirstGameLaunch)
        {
            // Первый запуск игры на первом уровне: спавним на начальной позиции
            CheckpointManager.Instance.SetCheckpoint(player.transform.position, null, scene.name);
            Debug.Log($"First game launch on first level, player stays at initial position: {player.transform.position}");
            isFirstGameLaunch = false; // Сбрасываем флаг после первого спавна
        }
        else if (CheckpointManager.Instance.GetLastCheckpointScene() == scene.name)
        {
            // Если есть чекпоинт в текущей сцене, спавним игрока на нем
            player.transform.position = CheckpointManager.Instance.GetLastCheckpointPosition();
            Debug.Log($"Player spawned at checkpoint: {player.transform.position}");
        }
        else
        {
            // Иначе спавним у портала (в т.ч. при возвращении на первый уровень)
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
                // Если портала нет, устанавливаем чекпоинт на текущей позиции игрока
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