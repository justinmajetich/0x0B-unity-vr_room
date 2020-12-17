using UnityEngine;

public class DoorBehavior : MonoBehaviour
{
    Animator m_Animator;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void Open() {
        QuestDebug.Instance.Log("Opening door...");
        m_Animator.SetBool("openDoor", true);
    }

    public void Close() {
        QuestDebug.Instance.Log("Closing door...");
        m_Animator.SetBool("openDoor", false);
    }
}
