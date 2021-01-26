using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CNovell.Nodes;
using CNovell.Components;
using CNovell.ScriptableObjects;

namespace CNovell.SceneManagement
{
	public class DialogueManager
	{
		private readonly DialogueComponent m_dialogueComponent;
		
		private readonly GameObject m_dialogueBox;
		private readonly Text m_speaker;
		private readonly Text m_dialogue;

		private IEnumerator m_typewriteEvent;

		private string m_currentDialogue;
		private float m_textSpeed = 1.0f;
		private float m_autoSpeed;
		private bool m_isTyping = false;
		private bool m_isProceedAllowed = false;
		private bool m_isAutoEnabled = false;

		#region setters

		public void SetDialogueSpeed(float speed) => m_textSpeed = speed; 
		public void SetAutoSpeed(float speed) => m_autoSpeed = speed; 

		#endregion

		public DialogueManager(DialogueComponent dialogueComponent)
		{
			m_dialogueComponent = dialogueComponent;
			
			m_dialogueBox = dialogueComponent.GetDialogueBox();
			m_speaker = dialogueComponent.GetSpeaker();
			m_dialogue = dialogueComponent.GetDialogue();
		}
		
		public void SetDialogue(Character? character, Sprite sprite, string dialogue, CharacterManager characterManager = null)
		{
			
			m_currentDialogue = dialogue;
			m_speaker.text = character?.GetName() ?? "";
			m_dialogueComponent.StopAllCoroutines();
			m_dialogueComponent.StartCoroutine(m_typewriteEvent = TypewriteText());
			
			if (characterManager != null && character != null)
			{
				GameObject characterObject = characterManager.TryGetCharacter(character);

				if (sprite != null)
				{
					if (characterObject != null)
						characterManager.SetSprite(characterObject, sprite);
					else
						Debug.LogWarning("CNovell: Не пытайтесь изменить спрайт персонажа, которого нет в сцене.");
				}

				if (characterObject)
					characterManager.HighlightSpeakingCharacter(character);
			}
		}
		
		private IEnumerator TypewriteText()
		{
			m_dialogue.text = "";
			m_isTyping = true; 

			for (int i = 0; i < m_currentDialogue.Length; i++)
			{
				m_dialogue.text += m_currentDialogue[i]; 
				yield return new WaitForSeconds((1 - m_textSpeed) * 0.1f);
			}
			
			m_dialogue.GetComponentInParent<VerticalLayoutGroup>().padding = new RectOffset(10, 0, 10, 0);

			m_isProceedAllowed = true;
			m_isTyping = false; 
			
			if (m_isAutoEnabled)
				m_dialogueComponent.StartCoroutine(WaitForAuto());
		}
		
		public void SkipTypewrite()
		{
			
			m_dialogueComponent.StopCoroutine(m_typewriteEvent);
			m_dialogue.text = m_currentDialogue;

			m_isProceedAllowed = true;
			m_isTyping = false;
		}

		public void ToggleDialogueBox()
		{
			ClearDialogueBox();
			m_dialogueBox.SetActive(!m_dialogueBox.activeSelf);
		}

		public void ToggleDialogueBox(bool toggle)
		{
			ClearDialogueBox();
			m_dialogueBox.SetActive(toggle);
		}
		
		private void ClearDialogueBox()
		{
			m_speaker.text = "";
			m_dialogue.text = "";
		}

		#region SceneManager-reliant functions
		public void ProceedDialogue()
		{
			if (m_isAutoEnabled)
				return; 

			if (m_isTyping)
				SkipTypewrite();
			
			else if (m_isProceedAllowed)
			{
				m_isProceedAllowed = false;

				SceneManager sceneManager = SceneManager.GetInstance();
				if (sceneManager == null)
					Debug.LogError("CNovell: Нужен SceneManager для продолжения диалога!");
				else
					sceneManager.NextNode(); 
			}
		}

		public void ToggleAuto()
		{
			m_isAutoEnabled = !m_isAutoEnabled; 

			SceneManager sceneManager = SceneManager.GetInstance();
			if (sceneManager == null)
				Debug.LogError("CNovell: Нужен SceneManager для использования функции \"Автоматически\"!");
			else if (sceneManager.GetCurrentNode() is DialogueNode && m_isAutoEnabled && m_isTyping == false)
				m_dialogueComponent.StartCoroutine(WaitForAuto()); 
		}

		private IEnumerator WaitForAuto()
		{
			float waitTime = m_dialogue.text.Length * ((1.1f - m_autoSpeed) * 0.1f);
			yield return new WaitForSeconds(waitTime); 

			if (m_isAutoEnabled)
			{
				SceneManager sceneManager = SceneManager.GetInstance();
				if (sceneManager != null)
					sceneManager.NextNode();
			}
		}
		#endregion
	}
}