using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Globalization;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private Vector3 lastCheckpointPosition;
    private string lastCheckpointScene;
    private List<Vector3> activatedCheckpoints = new List<Vector3>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCheckpoint();
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

        string posData = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", position.x, position.y, position.z);
        PlayerPrefs.SetString("LastCheckpointPosition", posData);
        PlayerPrefs.SetString("LastCheckpointScene", lastCheckpointScene);
        PlayerPrefs.SetString("CurrentLevel", lastCheckpointScene);
        PlayerPrefs.Save();

        bool exists = false;
        foreach (var checkpoint in activatedCheckpoints)
        {
            if (Vector3.Distance(checkpoint, position) < 0.1f)
            {
                exists = true;
                break;
            }
        }
        if (!exists)
            activatedCheckpoints.Add(position);

        if (audioSource?.clip != null)
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

        foreach (var checkpoint in FindObjectsByType<Checkpoint>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(checkpoint.transform.position, position) > 0.1f)
                checkpoint.Deactivate();
        }
    }

    public Vector3 GetLastCheckpointPosition() => lastCheckpointPosition;

    public string GetLastCheckpointScene() => lastCheckpointScene;

    public bool IsCheckpointActivated(Vector3 position)
    {
        foreach (var checkpoint in activatedCheckpoints)
            if (Vector3.Distance(checkpoint, position) < 0.1f)
                return true;
        return false;
    }

    public void ResetCheckpoint()
    {
        lastCheckpointPosition = Vector3.zero;
        lastCheckpointScene = "";
        activatedCheckpoints.Clear();
        PlayerPrefs.DeleteKey("LastCheckpointPosition");
        PlayerPrefs.DeleteKey("LastCheckpointScene");
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();
    }

    private void LoadCheckpoint()
    {
        string posData = PlayerPrefs.GetString("LastCheckpointPosition", "");
        lastCheckpointScene = PlayerPrefs.GetString("LastCheckpointScene", "");
        if (!string.IsNullOrEmpty(posData))
        {
            string[] coords = posData.Split(',');
            if (coords.Length == 3 &&
                float.TryParse(coords[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(coords[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                float.TryParse(coords[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
            {
                lastCheckpointPosition = new Vector3(x, y, z);

                bool exists = false;
                foreach (var checkpoint in activatedCheckpoints)
                {
                    if (Vector3.Distance(checkpoint, lastCheckpointPosition) < 0.1f)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                    activatedCheckpoints.Add(lastCheckpointPosition);
            }
            else
            {
                lastCheckpointPosition = Vector3.zero;
                lastCheckpointScene = "";
            }
        }
        else
        {
            lastCheckpointPosition = Vector3.zero;
            lastCheckpointScene = "";
        }
    }
}
