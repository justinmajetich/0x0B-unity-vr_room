using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockPanel : MonoBehaviour
{
    public Color32 unlockedPanelColor = Color.green;
    public Color32 lockedPanelColor = Color.green;
    public Color32 unlockedTextColor = Color.green;
    public Color32 lockedTextColor = Color.green;

    Image lockBG;
    TextMeshProUGUI lockText;

    void Start()
    {
        if (!lockBG) {lockBG = GetComponent<Image>(); }
        if (!lockText) {lockText = GetComponentInChildren<TextMeshProUGUI>(); }

        lockBG.color = lockedPanelColor;
        lockText.color = lockedTextColor;
    }

    public void Unlock() {
        lockBG.color = unlockedPanelColor;
        lockText.color = unlockedTextColor;
        lockText.text = "UNLOCKED";
    }
}
