using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;
    private string lastCheckpointScene;
    private readonly Vector3 spawnOffset = Vector3.zero; // Смещение убрано

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position, AudioSource audioSource, string sceneName = "")
    {
        lastCheckpointPosition = position;
        lastCheckpointScene = string.IsNullOrEmpty(sceneName) ? SceneManager.GetActiveScene().name : sceneName;
        Debug.Log($"Checkpoint set at: {position} in scene: {lastCheckpointScene}");

        if (audioSource?.clip)
        {
            GameObject soundObject = new GameObject("CheckpointSound");
            soundObject.transform.position = position;
            AudioSource tempAudio = soundObject.AddComponent<AudioSource>();
            tempAudio.clip = audioSource.clip;
            tempAudio.volume = audioSource.volume;
            tempAudio.spatialBlend = audioSource.spatialBlend;
            tempAudio.Play();
            Destroy(soundObject, audioSource.clip.length);
        }

        foreach (Checkpoint checkpoint in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(checkpoint.transform.position, position) > 0.1f)
                checkpoint.Deactivate();
        }
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition + spawnOffset; // Теперь без смещения
    }

    public string GetLastCheckpointScene()
    {
        return lastCheckpointScene;
    }

    public void ResetCheckpoint()
    {
        lastCheckpointPosition = Vector3.zero;
        lastCheckpointScene = "";
    }
}