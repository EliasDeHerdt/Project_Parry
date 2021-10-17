using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] protected float m_DestroyTime = 1f;

    private float m_Timer;
    void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_DestroyTime)
            Destroy(gameObject);
    }
}
