using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int m_StartHealth = 10;
    [SerializeField] private Color m_FlickerColor = Color.white;
    [SerializeField] private float m_FlickerDuration = 0.1f;
    [SerializeField] private float m_TimeBeforeKill = 2f;
    [SerializeField] protected AudioSource m_AudioSource;
    [SerializeField] protected AudioClip m_DamageSound;
    [SerializeField] protected AudioClip m_DeathSound;
    [SerializeField] protected AudioClip m_HealSound;

    private float m_Timer;
    private Color m_StartColor;
    private BasicCharacter m_Owner;
    private float m_CurrentHealth = 0;
    private Material m_AttachedMaterial;

    public float HealthPercentage
    {
        get
        {
            return ((float)m_CurrentHealth) / m_StartHealth;
        }
    }

    void Awake()
    {
        m_CurrentHealth = m_StartHealth;
    }

    private void Start()
    {
        m_Owner = GetComponent<BasicCharacter>();

        Renderer renderer = transform.GetComponentInChildren<Renderer>();
        if (renderer)
        {
            m_AttachedMaterial = renderer.material;

            if (m_AttachedMaterial)
                m_StartColor = m_AttachedMaterial.GetColor("_Color");
        }
    }

    private void Update()
    {
        if (m_Owner.IsDead)
        {
            m_Timer += Time.deltaTime;
            Kill();
        }
    }

    public void Heal(float amount)
    {
        if (!m_Owner.IsDead)
        {
            m_CurrentHealth += amount;

            if (m_CurrentHealth > m_StartHealth)
                m_CurrentHealth = m_StartHealth;

            if (m_HealSound && m_AudioSource){

                m_AudioSource.clip = m_HealSound;
                m_AudioSource.Play();
            }
        }
    }

    public void Damage(float amount)
    {
        if (!m_Owner.IsDead)
        {
            m_CurrentHealth -= amount;

            if (m_AttachedMaterial)
            {
                m_AttachedMaterial.SetColor("_Color", m_FlickerColor);
                Invoke("ResetColor", m_FlickerDuration);
            }

            if (m_CurrentHealth <= 0){
                if (m_AudioSource)
                    m_AudioSource.clip = m_DeathSound;
                m_Owner.IsDead = true;
            }
            else if (m_AudioSource)
                m_AudioSource.clip = m_DamageSound;

            if (m_AudioSource)
                m_AudioSource.Play();
        }
    }

    public void ResetColor()
    {
        if (!m_AttachedMaterial)
            return;

        m_AttachedMaterial.SetColor("_Color", m_StartColor);
    }

    void Kill()
    {
        if (m_Timer >= m_TimeBeforeKill)
        {
            Destroy(gameObject);
        }
    }
}
