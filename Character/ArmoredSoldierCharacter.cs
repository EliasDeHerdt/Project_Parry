using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ArmoredSoldierCharacter : SoldierCharacter
{
    [SerializeField] protected int m_FlurryAmount = 3;

    protected int m_CurrentFlurry = 0;
    protected bool m_AttackStarted = false;

    protected override void Awake()
    {
        base.Awake();
        m_IsArmored = true;
    }

    protected override void HandleAttackInput(float delta)
    {
        if (m_DistanceToPlayer <= m_ChargeAttackRadius || m_AttackStarted)
        {
            m_Timer += delta;
            if (m_Timer >= m_AttackInterval || m_AttackStarted)
            {
                m_Timer = 0f;
                m_AttackStarted = true;
                if (m_CurrentFlurry != m_FlurryAmount){
                    if (m_AttackBehaviour.Attack())
                        ++m_CurrentFlurry;
                }
                else {
                    m_CurrentFlurry = 0;
                    m_AttackStarted = false;
                    m_AttackInterval = Random.Range(m_MinAttackInterval, m_MaxAttackInterval);
                }
            }
        }
    }
}
