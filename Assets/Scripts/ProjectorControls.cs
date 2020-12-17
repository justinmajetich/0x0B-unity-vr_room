using UnityEngine;

public class ProjectorControls : MonoBehaviour, IUsable
{
    GameObject m_ParticleSystem;

    void Start()
    {
        m_ParticleSystem = transform.Find("ParticleSystem").gameObject;
    }

    public void Use() {
        m_ParticleSystem.SetActive(!m_ParticleSystem.activeSelf);
    }
}
