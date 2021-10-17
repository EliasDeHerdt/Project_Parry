using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.UIElements;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    [SerializeField] string m_TextToShow = "enter text";
    [SerializeField] float m_Duration = 2f;
    [SerializeField] HUD m_HUD = null;

    private float m_Timer = 0f;
    private bool m_Active = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Friendly" && m_HUD)
            return;

        m_Active = true;
        m_HUD.SetSpeech = m_TextToShow;
    }

    void Update()
    {
        if (m_Active)
        {
            m_Timer += Time.deltaTime;
            if (m_Timer >= m_Duration)
            {
                m_HUD.SetSpeech = "";
                Destroy(gameObject);
            }
        }
    }
}
