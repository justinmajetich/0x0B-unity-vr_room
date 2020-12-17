using UnityEngine;

public class ConsoleControls : MonoBehaviour
{
    [SerializeField] DoorBehavior glassDoor;

    void Start()
    {
        if (!glassDoor) { glassDoor = GameObject.Find("GlassDoor").GetComponent<DoorBehavior>(); }
    }

    public void Use() {
        glassDoor.Open();
    }
}
