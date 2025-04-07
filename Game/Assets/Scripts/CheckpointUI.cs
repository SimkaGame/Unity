using UnityEngine;
using UnityEngine.UI;

public class CheckpointUI : MonoBehaviour
{
    public GameObject checkpointText;
    public float displayTime = 2f;

    public void ShowMessage()
    {
        if (checkpointText != null)
        {
            checkpointText.SetActive(true);
            CancelInvoke();
            Invoke("HideMessage", displayTime);
        }
    }

    void HideMessage()
    {
        checkpointText.SetActive(false);
    }
}
