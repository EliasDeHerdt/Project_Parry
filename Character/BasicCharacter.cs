using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCharacter : MonoBehaviour
{
    protected Health m_Health;
    protected MovementBehaviour m_MovementBehaviour;

    protected bool m_IsDead = false;
    protected bool m_IsArmored = false;
    protected bool m_CanTakeDamage = true;


    //only used on enemies, so gets instatiated there.
    protected Transform m_HealthBar;
    protected bool m_ShowHealthBar = false;

    public bool IsArmored
    {
        get { return m_IsArmored; }
        set { m_IsArmored = value; }
    }

    public bool CanTakeDamage
    {
        get { return m_CanTakeDamage; }
        set { m_CanTakeDamage = value; }
    }

    public bool IsDead
    {
        get { return m_IsDead; }
        set { m_IsDead = value; }
    }

    public bool ShowHealthBar
    {
        get { return m_ShowHealthBar; }
        set { m_ShowHealthBar = value; }
    }

    public Vector3 Velocity
    {
        get { return m_MovementBehaviour.DesiredMovementDirection; }
    }

    public float MovementMultiplier
    {
        get { return m_MovementBehaviour.MovementSpeedMultiplier; }
    }

    protected virtual void Awake()
    {
        m_Health = transform.GetComponent<Health>();
        m_MovementBehaviour = GetComponent<MovementBehaviour>();
    }

    protected virtual void Update()
    {
        if (m_IsDead)
            m_MovementBehaviour.DesiredMovementDirection = Vector3.zero;
    }
}
