using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CNovell.Components;
using CNovell.ScriptableObjects;

namespace CNovell.SceneManagement
{
	public class CharacterManager
	{
		private readonly CharacterComponent m_characterComponent;
		private CharacterTransformer m_characterTransformer;

		private readonly GameObject m_characterPrefab;
		private readonly RectTransform m_backgroundPanel;
		private readonly RectTransform m_foregroundPanel;

		private List<GameObject> m_characters;

		#region getters

		public CharacterTransformer GetCharacterTransformer() => m_characterTransformer; 
		public RectTransform GetBackgroundPanel() => m_backgroundPanel; 
		public List<GameObject> GetCharacters() => m_characters; 
		public List<SaveCharacterInfo> GetCharactersInfo()
		{
			List<SaveCharacterInfo> infos = new List<SaveCharacterInfo>();
			foreach (var character in m_characters)
				infos.Add(character.GetComponent<Components.CharacterInfo>().GetSaveData());
			return infos;
		}

		#endregion
		
		public CharacterManager(CharacterComponent characterComponent)
		{
			m_characterComponent = characterComponent;

			
			m_characterPrefab = characterComponent.GetCharacterPrefab();
			m_backgroundPanel = characterComponent.GetBackgroundPanel();
			m_foregroundPanel = characterComponent.GetForegroundPanel();

			
			m_characterTransformer = new CharacterTransformer();
			m_characters = new List<GameObject>(); 
		}

		public void EnterCharacter(Character character, Sprite sprite, float xPosition, float fadeInTime = 0.5f, bool nextNode = false, bool waitForFinish = false, bool isInvert = false)
		{
			
			if (TryGetCharacter(character) != null)
			{
				Debug.LogWarning("CNovell: Не пытайтесь ввести два одинаковых символа.");

				SceneManager sceneManager = SceneManager.GetInstance();
				if (sceneManager != null && nextNode)
					sceneManager.NextNode();
				return;
			}
			
			GameObject characterObject = Object.Instantiate(m_characterPrefab, m_backgroundPanel);
			
			if (isInvert)
				m_characterTransformer.SetCharacterScale(characterObject, new Vector2(-1, 1));
			
			SetSprite(characterObject, sprite);
			
			m_characterTransformer.SetCharacterPosition(characterObject, new Vector2(xPosition, 0));
			
			Components.CharacterInfo characterInfo = characterObject.GetComponent<Components.CharacterInfo>();
			characterInfo.SetCharacter(character);

			m_characters.Add(characterObject);

			
			m_characterComponent.StartCoroutine(FadeIn(characterObject, fadeInTime, nextNode, waitForFinish));
		}
		
		public void ExitCharacter(Character character, Sprite sprite, float fadeOutTime = 0.5f, bool nextNode = false, bool waitForFinish = false)
		{
			GameObject characterObject = TryGetCharacter(character);

			if (characterObject == null)
			{
				Debug.LogWarning("CNovell: Не пытайтесь выйти из персонажа, которого нет в сцене.");

				SceneManager sceneManager = SceneManager.GetInstance();
				if (sceneManager != null && nextNode)
					sceneManager.NextNode();
				return;
			}

			SetSprite(characterObject, sprite); 
			
			m_characterComponent.StartCoroutine(FadeOut(characterObject, fadeOutTime, nextNode, waitForFinish));
		}
		
		public GameObject TryGetCharacter(Character character)
		{
			for (int i = 0; i < m_characters.Count; i++)
			{
				GameObject existingCharacter = m_characters[i];
				Components.CharacterInfo characterInfo = existingCharacter.GetComponent<Components.CharacterInfo>();

				if (characterInfo.GetCharacter() == character)
					return existingCharacter; 
			}

			return null; 
		}
		
		public void SetSprite(GameObject character, Sprite sprite)
		{
			Image characterImage = character.GetComponent<Image>();
			characterImage.sprite = sprite;
			characterImage.preserveAspect = true;
		}

		public void HighlightSpeakingCharacter(Character speakingCharacter)
		{
			for (int i = 0; i < m_characters.Count; i++)
			{
				GameObject character = m_characters[i];
				character.transform.SetParent(m_backgroundPanel.transform);
				character.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f);
			}

			GameObject characterToHighlight = TryGetCharacter(speakingCharacter);
			characterToHighlight.transform.SetParent(m_foregroundPanel.transform);
			characterToHighlight.GetComponent<Image>().color = new Color(1, 1, 1);
		}

		public void SetCharacters(List<GameObject> characters)
		{
			for (int i = 0; i < m_characters.Count; i++)
				Object.Destroy(m_characters[i]);

			m_characters.Clear();

			for (int i = 0; i < characters.Count; i++)
			{
				GameObject character = Object.Instantiate(characters[i], m_backgroundPanel);
				m_characters.Add(character);
			}
		}
		
		public void SetCharactersInfos(List<SaveCharacterInfo> characters)
		{
			for (int i = 0; i < m_characters.Count; i++)
				m_characters[i].GetComponent<Components.CharacterInfo>().SetSaveData(characters[i]);
		}

		private IEnumerator FadeIn(GameObject character, float fadeInTime = 0.5f, bool nextNode = false, bool waitForFinish = false)
		{
			SceneManager sceneManager = SceneManager.GetInstance();
			
			if (sceneManager != null && nextNode && !waitForFinish)
				sceneManager.NextNode();

			float elapsedTime = 0.0f;

			while (elapsedTime < fadeInTime)
			{
				float percentage = elapsedTime / fadeInTime;
				character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);

				elapsedTime += Time.deltaTime; 
				yield return null;
			}

			if (sceneManager != null && nextNode && waitForFinish)
				sceneManager.NextNode();
		}

		private IEnumerator FadeOut(GameObject character, float fadeOutTime = 0.5f, bool nextNode = false, bool waitForFinish = false)
		{
			SceneManager sceneManager = SceneManager.GetInstance();
			
			if (sceneManager != null && nextNode && !waitForFinish)
				sceneManager.NextNode();

			float elapsedTime = 0.0f;

			while (elapsedTime < fadeOutTime)
			{
				float percentage = 1 - (elapsedTime / fadeOutTime);
				character.GetComponent<Image>().color = new Color(1, 1, 1, percentage);

				elapsedTime += Time.deltaTime; 
				yield return null;
			}

			m_characters.Remove(character);
			Object.Destroy(character);

			if (sceneManager != null && nextNode && waitForFinish)
				sceneManager.NextNode();
		}
	}
}