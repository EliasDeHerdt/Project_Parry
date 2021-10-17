using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    //include the UnityEngine.UI namespace
    [SerializeField] Image m_HealthBar = null;
    [SerializeField] Text m_HealthText = null;
    [SerializeField] Image m_FlowBar = null;
    [SerializeField] Text m_FlowText = null;
    [SerializeField] Text m_EndText = null;
    [SerializeField] Text m_Speech = null;

    private PlayerCharacter m_Player = null;
    private Health m_PlayerHealth = null;
    private GameOver m_GameOver = null; 

    private float m_PlayerFlowPercentage;
    private float m_PlayerhealthPercentage;
    public string SetSpeech
    {
        set { m_Speech.text = value; }
    }

    private void Start()
    {
        m_Player = FindObjectOfType<PlayerCharacter>();

        if (m_Player != null)
        {
            m_PlayerHealth = m_Player.GetComponent<Health>();
            m_GameOver = GameObject.Find("World").GetComponent<GameOver>();
        }
    }

    void Update()
    {
        UpdateVariables();
        SyncData();
    }

    void UpdateVariables()
    {
        m_PlayerFlowPercentage = m_Player.FlowPercentage;
        m_PlayerhealthPercentage = m_PlayerHealth.HealthPercentage;
    }

    void SyncData()
    {
        //health
        if (m_HealthBar && m_HealthText && m_FlowBar && m_FlowText && m_PlayerHealth)
        {
            m_HealthBar.transform.localScale = new Vector3(m_PlayerhealthPercentage, 1f, 1f);
            m_FlowBar.transform.localScale = new Vector3(m_PlayerFlowPercentage, 1f, 1f);
            m_HealthText.text = m_PlayerhealthPercentage * 100 + "%";
            m_FlowText.text = m_PlayerFlowPercentage * 100 + "%";
        }

        if (m_GameOver && m_GameOver.PlayerDead && m_EndText)
            m_EndText.text = "You Died";

        if (m_GameOver && m_GameOver.PlayerWin && m_EndText)
            m_EndText.text = "You Win";
    }
}
