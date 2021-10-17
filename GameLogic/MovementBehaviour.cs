using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
    //Variables
    //makes var editable in the inspect screen
    [SerializeField] protected float m_MovementSpeed = 1.0f;
    [SerializeField] protected float m_RotationSpeed = 1.0f;
    
    protected Rigidbody m_RigidBody;
    protected Vector3 m_DesiredMovementDirection = Vector3.zero;
    protected Vector3 m_DesiredLookatPoint = Vector3.zero;
    protected float m_MovementSpeedMultiplier = 1.0f;
    protected bool m_LockOnTarget = false;

    public Vector3 DesiredMovementDirection
    {
        get { return m_DesiredMovementDirection; }
        set { m_DesiredMovementDirection = value; }
    }
    public Vector3 DesiredLookatPoint
    {
        get { return m_DesiredLookatPoint; }
        set { m_DesiredLookatPoint = value; }
    }

    public float MovementSpeedMultiplier
    {
        get { return m_MovementSpeedMultiplier; }
        set { m_MovementSpeedMultiplier = value; }
    }

    public bool LockOnTarget
    {
        get { return m_LockOnTarget; }
        set { m_LockOnTarget = value; }
    }

    //Functions
    protected virtual void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        Quaternion.LookRotation(m_RigidBody.transform.forward);
    }

    protected virtual void FixedUpdate()
    {
        float delta = Time.deltaTime;

        HandleRotation(delta);
        HandleMovement(delta);
        CheckMultiplier();
    }

    protected virtual void HandleMovement(float delta)
    {
        Vector3 movement = m_DesiredMovementDirection.normalized;
        movement *= m_MovementSpeed * m_MovementSpeedMultiplier * delta;

        //rigidbody's y is needed for realistic gravity
        m_RigidBody.velocity = new Vector3( movement.x, m_RigidBody.velocity.y, movement.z);
    }

    protected virtual void HandleRotation(float delta)
    {
        if (!m_LockOnTarget)
        {
            Vector3 lookVector;
            if (m_DesiredLookatPoint == Vector3.zero)
                lookVector = transform.forward;
            else
                lookVector = m_DesiredLookatPoint - m_RigidBody.position;
            lookVector.y = 0;
            lookVector.Normalize();

            Quaternion rotation = Quaternion.LookRotation(lookVector);
            m_RigidBody.rotation = Quaternion.Lerp(m_RigidBody.rotation, rotation, m_RotationSpeed*delta);
        } else if (m_DesiredLookatPoint != Vector3.zero)
        {
            Vector3 lookVector = m_DesiredLookatPoint - m_RigidBody.position;
            lookVector.y = 0;
            lookVector.Normalize();

            Quaternion rotation = Quaternion.LookRotation(lookVector);
            m_RigidBody.rotation = Quaternion.Lerp(m_RigidBody.rotation, rotation, m_RotationSpeed*delta);
        }
    }

    protected void CheckMultiplier()
    {
        if (m_MovementSpeedMultiplier > 1f)
            m_MovementSpeedMultiplier *= 0.8f;
        else if (m_MovementSpeedMultiplier < 1f)
            m_MovementSpeedMultiplier = 1;
    }
}