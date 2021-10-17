using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoldierCharacter : BasicCharacter
{
    [SerializeField] protected GameObject m_PlayerTarget;
    [SerializeField] protected float m_DetectionRadius = 10f;
    [SerializeField] protected float m_FollowRadius = 20f;
    [SerializeField] protected float m_AttackRadius = 1.5f; //The radius the enemy will move towards to attack the player
    [SerializeField] protected float m_ChargeAttackRadius = 2f; //The radis the enemy will start his timer for attacks
    [SerializeField] protected float m_MinAttackInterval = 1f;
    [SerializeField] protected float m_MaxAttackInterval = 4f;
    protected float m_ArriveBuffer = 1f;

    protected float m_Timer;
    protected bool m_EnemyDetected;
    protected Vector3 m_SpawnLocation;
    protected Quaternion m_SpawnRotation;
    protected float m_AttackInterval;
    protected float m_DistanceToPlayer;
    protected float m_DistanceToSpawn;
    protected float m_DistancePlayerToSpawn;
    protected AttackBehaviour m_AttackBehaviour;

    //Functions
    protected override void Awake()
    {
        base.Awake();
        m_HealthBar = transform.Find("HealthBar");
    }

    protected void Start()
    {
        m_AttackBehaviour = transform.GetComponent<AttackBehaviour>();
        m_AttackBehaviour.PrepareBeforeSwing = true;

        m_SpawnLocation = transform.position;
        m_SpawnRotation = transform.rotation;

        m_AttackInterval = Random.Range(m_MinAttackInterval, m_MaxAttackInterval);
    }

    protected override void Update()
    {
        base.Update();
        HandleHealthBar();

        if (!m_IsDead)
        {
            m_DistanceToSpawn = Vector3.Distance(transform.position, m_SpawnLocation);
            m_DistanceToPlayer = Vector3.Distance(m_PlayerTarget.transform.position, transform.position);
            m_DistancePlayerToSpawn = Vector3.Distance(m_PlayerTarget.transform.position, m_SpawnLocation);
            HandleEnemyDetection();
            HandleMovementInput();
        }
        m_ShowHealthBar = false;
    }

    private void FixedUpdate()
    {
        float delta = Time.deltaTime;

        if (!m_IsDead)
        {
            HandleAttackInput(delta);
        }
    }

    void HandleEnemyDetection()
    {
        m_EnemyDetected = false;
        m_MovementBehaviour.LockOnTarget = false;
        if (m_DistanceToPlayer <= m_DetectionRadius)
        {
            m_EnemyDetected = true;
            m_MovementBehaviour.LockOnTarget = true;
        }
    }

    void HandleHealthBar()
    {
        if (!m_HealthBar)
            return;

        m_HealthBar.localScale = new Vector3(0f, m_HealthBar.localScale.y, m_HealthBar.localScale.z);
        if (m_ShowHealthBar)
        {
            m_HealthBar.localScale = new Vector3(m_Health.HealthPercentage, m_HealthBar.localScale.y, m_HealthBar.localScale.z);
        }
    }

    void HandleMovementInput()
    {
        if (m_MovementBehaviour == null || m_PlayerTarget == null)
            return;

        //movement
        if (m_EnemyDetected && m_DistancePlayerToSpawn <= m_FollowRadius && m_DistanceToPlayer > m_AttackRadius)
            m_MovementBehaviour.DesiredMovementDirection = m_PlayerTarget.transform.position - transform.position;
        else if ((!m_EnemyDetected || m_DistancePlayerToSpawn > m_FollowRadius) 
            && m_DistanceToSpawn > m_ArriveBuffer)
            m_MovementBehaviour.DesiredMovementDirection = m_SpawnLocation - transform.position;
        else
            m_MovementBehaviour.DesiredMovementDirection = Vector3.zero;

       //rotation
        if (m_EnemyDetected && m_DistancePlayerToSpawn <= m_FollowRadius)
            m_MovementBehaviour.DesiredLookatPoint = m_PlayerTarget.transform.position;
        else if ((!m_EnemyDetected || m_DistancePlayerToSpawn > m_FollowRadius) && m_DistanceToSpawn > m_ArriveBuffer)
            m_MovementBehaviour.DesiredLookatPoint = m_SpawnLocation;
        else
            m_MovementBehaviour.DesiredLookatPoint = m_SpawnRotation.eulerAngles;
    }
    protected virtual void HandleAttackInput(float delta)
    {
        if (m_DistanceToPlayer <= m_ChargeAttackRadius)
        {
            m_Timer += delta;
            if (m_Timer >= m_AttackInterval)
            {
                m_Timer = 0f;
                m_AttackBehaviour.Attack();
                m_AttackInterval = Random.Range(m_MinAttackInterval, m_MaxAttackInterval);
            }
        }
    }
}
