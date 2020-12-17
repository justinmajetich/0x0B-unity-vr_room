using UnityEngine;
using TMPro;

public class LetterKey : MonoBehaviour, IUsable
{
    public Keyboard keyboard;
    string character;

    void Start()
    {
        character = GetComponentInChildren<TMP_Text>().text;
        keyboard = transform.parent.parent.GetComponent<Keyboard>();
    }

    public void Use() {
        QuestDebug.Instance.Log("Key is being used!");
        keyboard.Input(character);
    }
}
