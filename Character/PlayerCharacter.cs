using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class PlayerCharacter : BasicCharacter
{
    //Variables
    [SerializeField] protected int m_MaxFlow = 100;
    [SerializeField] protected int m_FlowGainOnParry = 12;
    [SerializeField] protected GameObject m_ParryEffect = null;
    [SerializeField] protected GameObject m_HealEffect = null;

    protected int m_Flow;
    protected bool m_ParryPressed;
    protected CameraController m_CController;
    protected AttackBehaviour m_AttackBehaviour;

    protected int m_DashCost = 10;
    protected int m_HealCost = 25;
    protected int m_ArmorPiercerCost = 60;

    protected List<Collider> m_Targets = new List<Collider>();
    protected float m_TargetCheckRadius = 10f;
    protected int m_CurrentTarget = 0;

    public int Flow
    {
        get { return m_Flow; }
    }

    public float FlowPercentage
    {
        get { return (float)m_Flow / m_MaxFlow; }
    }

    //Functions
    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        m_CController = CameraController.singleton;
        m_AttackBehaviour = transform.GetComponent<AttackBehaviour>();
    }

    protected override void Update() 
    {
        base.Update();
        if (m_CController != null)
        {
            m_CController.mouseX = Input.GetAxis("Mouse X");
            m_CController.mouseY = Input.GetAxis("Mouse Y");
        }
        
        if (!m_IsDead)
        {
            HandleMovementInput();
            HandleCameraLockOn();

            //Abilities
            Heal();
            ArmorPiercer();
        }
    }

    private void FixedUpdate()
    {
        if (!m_IsDead)
        {
            HandleAttackInput();
        }
    }

    void HandleMovementInput()
    {
        if (m_MovementBehaviour == null || m_CController == null)
            return;

        if (Dash())
            return;

        Transform cameraTransform = m_CController.GetCameraTransform();

        //movement
        float horizontalMovement = Input.GetAxis("MovementHorizontal");
        float verticalMovement = Input.GetAxis("MovementVertical");

        Vector3 movement = horizontalMovement * cameraTransform.right + verticalMovement * cameraTransform.forward;
        movement.y = 0;

        m_MovementBehaviour.DesiredMovementDirection = movement;

        if (horizontalMovement == 0 && verticalMovement == 0)
            return;

        //rotation
        Vector3 lookDirection = new Vector3(movement.x, 0, movement.z);
        m_MovementBehaviour.DesiredLookatPoint = transform.position + lookDirection;
    }

    void HandleCameraLockOn()
    {
        //Lock-On Input
        bool LockOn = Input.GetKeyDown("left shift");
        float Unlock = Input.GetAxis("Unlock");   

        //Fill the targetlist with every target in the given radius
        if (LockOn){

            ++m_CurrentTarget;
            m_Targets.Clear();
            m_Targets.AddRange(Physics.OverlapSphere(transform.position, m_TargetCheckRadius, LayerMask.GetMask("Enemy")));
            if (m_CurrentTarget >= m_Targets.Count)
                m_CurrentTarget = 0;
        }

        //unlock the camera
        if (Unlock > 0)
        {
            m_Targets.Clear();
            m_CController.LookAtTarget = false;
            m_MovementBehaviour.LockOnTarget = false;
        }

        //if the a target dies, and we are still locked on, remove him from the list.
        //if there are still more targets, return to the first target in the list.
        if (m_MovementBehaviour.LockOnTarget == true && m_Targets[m_CurrentTarget].gameObject.GetComponent<BasicCharacter>().IsDead)
        {
            m_Targets.RemoveAt(m_CurrentTarget);
            if (m_CurrentTarget >= m_Targets.Count)
                m_CurrentTarget = 0;
        }
        
        //if the targetlist contains any target, tell the movementbehaviour & the cameracontroller to lock on the target.
        //if false, free the camera and movementbehaviour
        if (m_Targets.Count > 0)
        {
            m_CController.LookAtTarget = true;
            m_MovementBehaviour.LockOnTarget = true;
            m_MovementBehaviour.DesiredLookatPoint = m_Targets[m_CurrentTarget].transform.position;
            m_Targets[m_CurrentTarget].GetComponent<BasicCharacter>().ShowHealthBar = true;
        } 
        else
        {
            m_CController.LookAtTarget = false;
            m_MovementBehaviour.LockOnTarget = false;
        }
    }

    void HandleAttackInput()
    {
        if (Input.GetAxis("Parry") > 0f && !m_ParryPressed)
        {
            m_AttackBehaviour.Parry();
            m_ParryPressed = true;
        } 
        else if (Input.GetAxis("Parry") <= 0f)
        {
            m_ParryPressed = false;
        }
        if (Input.GetAxis("Attack") > 0f)
        {
            m_AttackBehaviour.Attack();
        }
    }

    bool Dash()
    {
        if (Input.GetKeyDown("space") && m_Flow >= m_DashCost && m_MovementBehaviour.MovementSpeedMultiplier == 1f)
        {
            m_Flow -= m_DashCost;
            if (m_MovementBehaviour.DesiredMovementDirection == Vector3.zero)
                m_MovementBehaviour.DesiredMovementDirection = transform.forward * -1;
            m_MovementBehaviour.MovementSpeedMultiplier = 6f;
            return true;
        }
        else if (m_MovementBehaviour.MovementSpeedMultiplier > 1f)
            return true;
        return false;
    }

    void Heal()
    {
        if ((Input.GetKeyDown("&") || Input.GetKeyDown("1")) && m_Flow >= m_HealCost)
        {
            m_Flow -= m_HealCost;
            m_Health.Heal(2.5f);
            if (m_HealEffect)
                Instantiate(m_HealEffect, transform.position, m_HealEffect.transform.rotation);
        }
    }

    void ArmorPiercer()
    {
        if ((Input.GetKeyDown("@") || Input.GetKeyDown("2")) && m_Targets.Count > 0 && m_Flow >= m_ArmorPiercerCost)
        {
            m_Flow -= m_ArmorPiercerCost;
            m_AttackBehaviour.ArmorPiercer();
        }
    }

    //gets called whenever the player hits a parry (call is done by the target hitting the player)
    public void ParryHit()
    {
        //Gain flow
        m_Flow += m_FlowGainOnParry;
        if (m_Flow > m_MaxFlow)
            m_Flow = m_MaxFlow;

        Vector3 fxLocation = new Vector3(0f, 1f, 0.5f);
        if (m_ParryEffect)
            Instantiate(m_ParryEffect, transform.position + fxLocation, transform.rotation);
    }
}
