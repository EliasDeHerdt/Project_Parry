using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private BasicCharacter m_Player = null;
    [SerializeField] private BasicCharacter m_WinTarget = null;
    [SerializeField] private float m_ResetDelay = 2f;

    private float m_Timer;

    public bool PlayerDead
    {
        get { return m_Player.IsDead; }
    }

    public bool PlayerWin
    {
        get { return m_WinTarget.IsDead; }
    }

    private void Update()
    {
        if (m_Player != null && m_Player.IsDead || m_WinTarget != null && m_WinTarget.IsDead)
        {
            m_Timer += Time.deltaTime;
            TriggerGameOver();
        }
    }

    void TriggerGameOver()
    {
        if (m_Timer >= m_ResetDelay)
        {
            //include the namespave UnityEngine.SceneManagement
            SceneManager.LoadScene(0);
            m_ResetDelay = 0f;
        }
    }
}

