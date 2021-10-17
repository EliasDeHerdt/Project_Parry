using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_PrimaryWeaponTemplate = null;
    [SerializeField] private GameObject m_PrimaryWeaponSocket = null;

    private PlayerCharacter m_Character = null;
    private BasicWeapon m_PrimaryWeapon = null;
    public enum AttackEnum { none, parry, swing, armorBreaker };
    protected AttackEnum m_CurrentAttackMode;

    protected bool m_PrepareBeforeSwing = false;

    public BasicWeapon CurrentWeapon
    {
        get { return m_PrimaryWeapon; }
    }

    public AttackEnum CurrentAttackMode
    {
        get { return m_CurrentAttackMode; }
        set { m_CurrentAttackMode = value; }
    }

    public bool PrepareBeforeSwing
    {
        get { return m_PrepareBeforeSwing; }
        set { m_PrepareBeforeSwing = value; }
    }
    
    private void Awake()
    {
        m_Character = GetComponent<PlayerCharacter>();
        //Spawn Weapon
        if (m_PrimaryWeaponTemplate != null && m_PrimaryWeaponSocket != null)
        {
            var WeaponObject = Instantiate(m_PrimaryWeaponTemplate, m_PrimaryWeaponSocket.transform, true);
            WeaponObject.transform.localPosition = Vector3.zero;
            WeaponObject.transform.localRotation = Quaternion.identity;
            m_PrimaryWeapon = WeaponObject.GetComponent<BasicWeapon>();
            m_PrimaryWeapon.Owner = this.gameObject;
        }
    }

    public bool Attack()
    {
        if (!m_PrimaryWeapon.IsAttacking)
        {
            m_PrimaryWeapon.ResetVariables();
            m_PrimaryWeapon.IsAttacking = true;

            m_CurrentAttackMode = AttackEnum.swing;
            return true;
        }
        return false;
    }

    public void Parry()
    {
        if (!m_PrimaryWeapon.IsAttacking)
        {
            m_PrimaryWeapon.ResetVariables();
            m_PrimaryWeapon.ResetSwordVisuals();
            m_PrimaryWeapon.IsAttacking = true;
            m_CurrentAttackMode = AttackEnum.parry;
        }
    }

    public void ArmorPiercer()
    {
        m_PrimaryWeapon.ResetVariables();
        m_PrimaryWeapon.ResetSwordVisuals();
        m_PrimaryWeapon.IsAttacking = true;
        m_CurrentAttackMode = AttackEnum.armorBreaker;
    }

    public void ParryHit()
    {
        if (m_Character != null)
        {
            m_Character.ParryHit();
            m_PrimaryWeapon.ResetSwordVisuals();
            m_PrimaryWeapon.ResetVariables();
        }
    }
}
