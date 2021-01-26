using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CNovell.SceneManagement;

namespace CNovell.Components
{
    public class DialogueComponent : MonoBehaviour
    {
        [Header("Элемент(ы) пользовательского интерфейса диалога(ов)")]
        [Tooltip("Подключи родительскую панель, она используется для включения/отключения диалогового окна при необходимости.")]
        [SerializeField] private GameObject m_dialogueBox;
        [Tooltip("Вставьте текстовое поле, в котором отображается имя говорящего персонажа")]
        [SerializeField] private Text m_speaker;
        [Tooltip("Вставьте текстовое поле, в котором отображается диалог")]
        [SerializeField] private Text m_dialogue;

        private DialogueManager m_dialogueManager;

        #region getters

        public GameObject GetDialogueBox() => m_dialogueBox; 
        public Text GetSpeaker() => m_speaker; 
        public Text GetDialogue() => m_dialogue; 

        public DialogueManager GetDialogueManager()
        {
            if (m_dialogueManager == null)
                m_dialogueManager = new DialogueManager(this);

            return m_dialogueManager;
        }

        #endregion
        
        public void ProceedDialogue()
        {
            m_dialogueManager.ProceedDialogue();
        }
        
        public void ToggleAuto()
        {
            m_dialogueManager.ToggleAuto();
        }
    }
}