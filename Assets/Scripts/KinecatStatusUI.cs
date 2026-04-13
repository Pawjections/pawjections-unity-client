using UnityEngine;
using TMPro;

public class KinecatStatusUI : MonoBehaviour
{
    public KinecatReceiver receiver;
    public TMP_Text statusText;

    private void Update()
    {
        if (receiver.detected)
        {
            statusText.text = "Kinecat Detected";
            statusText.color = Color.green;
        }
        else
        {
            statusText.text = "Kinecat not detected";
            statusText.color = Color.red;
        }
    }
}