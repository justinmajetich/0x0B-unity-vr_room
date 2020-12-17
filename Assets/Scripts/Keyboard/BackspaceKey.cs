using UnityEngine;

public class BackspaceKey : MonoBehaviour, IUsable
{
    public Keyboard keyboard;

    void Start()
    {
        keyboard = transform.parent.parent.GetComponent<Keyboard>();
    }

    public void Use() {
        keyboard.Backspace();
    }
}