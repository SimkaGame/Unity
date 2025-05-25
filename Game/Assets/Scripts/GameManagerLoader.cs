using UnityEngine;

public class GameManagerLoader : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject checkpointManagerPrefab;

    private void Awake()
    {
        if (!GameManager.Instance && gameManagerPrefab)
            Instantiate(gameManagerPrefab);

        if (!CheckpointManager.Instance && checkpointManagerPrefab)
            Instantiate(checkpointManagerPrefab);
    }

    private void Start() => Destroy(gameObject);
}
