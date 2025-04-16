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

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;

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
        // Проверка на ненулевую позицию чекпоинта
        if (lastCheckpointPosition == Vector3.zero)
        {
            return GameObject.FindGameObjectWithTag("Player").transform.position + spawnOffset;
        }
        return lastCheckpointPosition + spawnOffset;
    }
}