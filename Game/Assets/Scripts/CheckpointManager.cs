using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 lastCheckpointPosition = Vector3.zero;
    private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

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

    public void SetCheckpoint(Vector3 position, AudioSource audioSource)
    {
        lastCheckpointPosition = position;

        if (audioSource != null && audioSource.clip != null)
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

        Checkpoint[] checkpoints = FindObjectsByType<Checkpoint>(FindObjectsSortMode.None);
        foreach (var checkpoint in checkpoints)
        {
            if (Vector3.Distance(checkpoint.transform.position, position) > 0.1f)
            {
                checkpoint.Deactivate();
            }
        }
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition + spawnOffset;
    }
}