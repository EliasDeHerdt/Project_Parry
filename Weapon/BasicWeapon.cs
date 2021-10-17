using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BasicWeapon : MonoBehaviour
{
    [SerializeField] protected float m_Damage = 1f;
    [SerializeField] protected float m_SwingStart = 0.25f;
    [SerializeField] protected float m_SwingDuration = 0.75f;
    [SerializeField] protected AudioSource m_AudioSource;
    [SerializeField] protected AudioClip m_SwingSound1;
    [SerializeField] protected AudioClip m_SwingSound2;
    [SerializeField] protected AudioClip m_ParrySound;

    protected float m_ParryDuration = 0.2f;
    protected float m_ParryRecovery = 0.5f;
    protected float m_DamageMultiplier = 1f;

    protected GameObject m_Owner;
    protected AttackBehaviour m_OwnerAttacKbehaviour;

    protected Transform m_WeaponSocket;
    protected Transform m_SwingAroundSocket;

    protected Animator m_Animator;
    const string IS_SWINGING_PARAMETER = "IsSwinging";
    const string IS_SWINGING2_PARAMETER = "IsSwinging2";
    const string IS_PARRYING_PARAMETER = "IsParrying";
    const string IS_PREPARING_PARAMETER = "IsPreparing";
    const string ARMOR_BREAKER_PARAMETER = "ArmorBreaker";

    protected List<GameObject> m_TargetsHit = new List<GameObject>();

    protected float m_Timer = 0f;
    protected bool m_IsPrepared = false;
    protected bool m_IsAttacking = false;
    protected bool m_ParryActive = false;
    protected bool m_AudioPlayed = false;
    protected bool m_CanDealDamage = false;
    protected bool m_ArmorPiercing = false;

    public GameObject Owner
    {
        get { return m_Owner; }
        set {
            m_Owner = value;
            m_OwnerAttacKbehaviour = m_Owner.GetComponent<AttackBehaviour>();
            m_WeaponSocket = m_Owner.transform.Find("WeaponSocket");
            m_Animator = m_WeaponSocket.GetComponent<Animator>();
        }
    }

    public bool IsAttacking
    {
        get { return m_IsAttacking; }
        set { m_IsAttacking = value; }
    }

    public bool IsParrying
    {
        get { return m_ParryActive; }
    }

    public void Update()
    {
        if (m_Owner.GetComponent<BasicCharacter>().IsDead)
            Destroy(gameObject);
     
        float delta = Time.deltaTime;
        HandleAttack(delta);
    }

    void OnTriggerEnter(Collider other)
    {
        //make sure we only hit friendly or enemies
        if (other.tag != "Friendly" && other.tag != "Enemy")
            return;

        //make sure we only hit opposing team
        if (other.tag == m_Owner.tag)
            return;

        Health otherHealth = other.GetComponent<Health>();
        AttackBehaviour otherAttackBehaviour = other.GetComponent<AttackBehaviour>();

        //Make sure the other collider has a Health and AttackBehaviour component
        if (otherHealth == null || otherAttackBehaviour == null)
            return;

        bool isTargetAlreadyHit = m_TargetsHit.Contains(other.gameObject);
        bool otherIsArmored = other.GetComponent<BasicCharacter>().IsArmored;
        bool otherCanTakeDamage = other.GetComponent<BasicCharacter>().CanTakeDamage;

        //Check all needed conditions for an attack to hit. If false, check if a parry is hit.
        if (otherCanTakeDamage && m_CanDealDamage && !otherAttackBehaviour.CurrentWeapon.m_ParryActive && !isTargetAlreadyHit){

            m_TargetsHit.Add(other.gameObject);
            if (!otherIsArmored || m_ArmorPiercing)
                otherHealth.Damage(m_Damage * m_DamageMultiplier);
        }
        else if (otherAttackBehaviour.CurrentWeapon.m_ParryActive && !isTargetAlreadyHit){

            m_TargetsHit.Add(other.gameObject);
            otherAttackBehaviour.ParryHit();
        }
    }

    //Checks which attack should be issued
    void HandleAttack(float delta)
    {
        if (m_OwnerAttacKbehaviour.CurrentAttackMode == AttackBehaviour.AttackEnum.parry)
        {
            m_Timer += delta;
            MakeParry(delta);
        } 
        else if (m_OwnerAttacKbehaviour.CurrentAttackMode == AttackBehaviour.AttackEnum.swing)
        {
            m_Timer += delta;
            MakeSwing(delta);
        }
        else if (m_OwnerAttacKbehaviour.CurrentAttackMode == AttackBehaviour.AttackEnum.armorBreaker)
        {
            m_Timer += delta;
            MakeArmorPiercer(delta);
        }
    }

    //Used to block attacks and gain flow
    void MakeParry(float delta)
    {
        if (!m_IsPrepared){

            m_IsPrepared = true;
            m_ParryActive = true;
            m_Animator.SetBool(IS_PARRYING_PARAMETER, true);
            if (m_AudioSource && m_ParrySound){

                m_AudioSource.clip = m_ParrySound;
                m_AudioSource.Play();
            }
        }
        if (m_ParryActive && m_Timer >= m_ParryDuration){

            m_ParryActive = false;
        }
        if (m_Timer >= m_Animator.GetCurrentAnimatorStateInfo(0).length){

            ResetVariables();
            ResetSwordVisuals();
        }
    }

    //Basic attacks
    void MakeSwing(float delta)
    {
        if (!m_IsPrepared)
        {
            //check if the owner of the sword should do a prepare animation before swinging
                //if he doesn't need to prepare
            if (!m_OwnerAttacKbehaviour.PrepareBeforeSwing){
                m_IsPrepared = true;
                if (!m_Animator.GetBool(IS_SWINGING_PARAMETER)){
                    m_Animator.SetBool(IS_SWINGING_PARAMETER, true);
                    m_Animator.SetBool(IS_SWINGING2_PARAMETER, false);
                    if (m_AudioSource && m_SwingSound1 && !m_AudioPlayed){

                        m_AudioSource.clip = m_SwingSound1;
                        m_AudioSource.Play();
                        m_AudioPlayed = true;
                    }
                }
                else{
                    m_Animator.SetBool(IS_SWINGING_PARAMETER, false);
                    m_Animator.SetBool(IS_SWINGING2_PARAMETER, true);
                    if (m_AudioSource && m_SwingSound2 && !m_AudioPlayed){

                        m_AudioSource.clip = m_SwingSound2;
                        m_AudioSource.Play();
                        m_AudioPlayed = true;
                    }
                }

            }
                //if he needs to prepare
            else {
                //Because the animtor is set up correctly, this works without any major change to the code.
                //Although the prepare will only be done on the first attack.
                m_Animator.SetBool(IS_PREPARING_PARAMETER, true);
                if (m_Timer >= m_Animator.GetCurrentAnimatorStateInfo(0).length){
                    m_Timer = 0;
                    m_IsPrepared = true;
                    m_Animator.SetBool(IS_PREPARING_PARAMETER, false);
                    if (!m_Animator.GetBool(IS_SWINGING_PARAMETER))
                    {

                        m_Animator.SetBool(IS_SWINGING_PARAMETER, true);
                        m_Animator.SetBool(IS_SWINGING2_PARAMETER, false);
                        if (m_AudioSource && m_SwingSound1 && !m_AudioPlayed){

                            m_AudioSource.clip = m_SwingSound1;
                            m_AudioSource.Play();
                            m_AudioPlayed = true;
                        }
                    }
                    else{

                        m_Animator.SetBool(IS_SWINGING_PARAMETER, false);
                        m_Animator.SetBool(IS_SWINGING2_PARAMETER, true);
                        if (m_AudioSource && m_SwingSound2 && !m_AudioPlayed) {

                            m_AudioSource.clip = m_SwingSound2;
                            m_AudioSource.Play();
                            m_AudioPlayed = true;
                        }
                    }
                }
            }
        }
        else{
            if (m_Timer >= m_SwingStart){

                m_CanDealDamage = true;
            }
            if (m_Timer >= m_SwingStart + m_SwingDuration){

                m_CanDealDamage = false;
                m_IsAttacking = false;
                if (m_AudioSource && !m_AudioPlayed){

                    m_AudioSource.Play();
                    m_AudioPlayed = true;
                }
            }
            if (m_Timer >= m_Animator.GetCurrentAnimatorStateInfo(0).length){

                ResetVariables();
                ResetSwordVisuals();
            }
        }
    }

    //Ability Armor Piercer
    void MakeArmorPiercer(float delta)
    {
        if (!m_IsPrepared){

            m_Animator.SetBool(ARMOR_BREAKER_PARAMETER, true);

            m_ArmorPiercing = true;
            m_IsPrepared = true;
            m_DamageMultiplier = 2f;
        }
        if (m_Timer >= m_SwingStart){

            m_CanDealDamage = true;
        }
        if (m_Timer >= m_SwingStart + m_SwingDuration){

            m_CanDealDamage = false;
        }
        if (m_Timer >= m_Animator.GetCurrentAnimatorStateInfo(0).length){

            ResetVariables();
            ResetSwordVisuals();
        }
    }

    //Reset all animations
    public void ResetSwordVisuals()
    {
        //Reset Animations
        m_Animator.SetBool(IS_SWINGING_PARAMETER, false);
        m_Animator.SetBool(IS_SWINGING2_PARAMETER, false);
        m_Animator.SetBool(IS_PARRYING_PARAMETER, false);
        m_Animator.SetBool(IS_PREPARING_PARAMETER, false);
        m_Animator.SetBool(ARMOR_BREAKER_PARAMETER, false);
    }

    //Reset all variables used when attacking
    public void ResetVariables()
    {
        //reset list of targets hit
        m_TargetsHit.Clear();

        //reset timer and damage mulitplier
        m_Timer = 0f;
        m_DamageMultiplier = 1f;
        
        //reset all checks for attacks and the current attackmode
        m_IsPrepared = false;
        m_IsAttacking = false;
        m_ParryActive = false;
        m_AudioPlayed = false;
        m_CanDealDamage = false;
        m_ArmorPiercing = false;
        m_OwnerAttacKbehaviour.CurrentAttackMode = AttackBehaviour.AttackEnum.none;
    }
}
