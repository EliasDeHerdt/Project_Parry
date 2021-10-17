using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject m_Owner = null;

    private BasicCharacter m_Character = null;
    private Animator m_Animator = null;

    private void Awake()
    {
        m_Character = m_Owner.GetComponent<BasicCharacter>();
        m_Animator = transform.GetComponent<Animator>();
    }

    private void Update()
    {
        if (m_Character != null && !m_Character.IsDead)
        {
            HandleMovementAnimation();
        }
        else
        {
            m_Animator.SetBool(IS_DEAD_PARAMAETER, m_Character.IsDead);
        }
    }

    const string IS_DEAD_PARAMAETER = "IsDead";
    const string IS_MOVING_PARAMETER = "IsMoving";
    void HandleMovementAnimation()
    {
        if (m_Animator == null)
            return;
        
        bool CheckForMovement = (m_Character.Velocity != Vector3.zero);
        bool CheckForDodge = (m_Character.MovementMultiplier > 1);

        m_Animator.SetBool(IS_MOVING_PARAMETER, CheckForMovement);
    }
}
