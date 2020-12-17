using UnityEngine;

public class EnterKey : MonoBehaviour, IUsable
{
    public Keyboard keyboard;

    void Start()
    {
        keyboard = transform.parent.parent.GetComponent<Keyboard>();
    }

    public void Use() {
        keyboard.Enter();
    }
}
