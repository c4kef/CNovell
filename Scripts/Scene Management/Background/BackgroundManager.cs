using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CNovell.Components;

namespace CNovell.SceneManagement
{
	public class BackgroundManager
	{
		private readonly BackgroundComponent m_backgroundComponent;
		
		private readonly Image m_imageBackground;
		private readonly Image m_colourBackground;

		#region getters

		public Sprite GetBackground() => m_imageBackground.sprite; 

		#endregion

		public BackgroundManager(BackgroundComponent backgroundComponent)
		{
			m_backgroundComponent = backgroundComponent;
			m_imageBackground = backgroundComponent.GetImageBackground();
			m_colourBackground = backgroundComponent.GetColourBackground();
		}
		
		public void EnterBackground(Sprite background, Color fadeColour, float fadeInTime = 0.5f, bool nextNode = false, bool waitForFinish = false)
		{
			m_colourBackground.color = fadeColour; 
			m_backgroundComponent.StartCoroutine(FadeIn(background, fadeInTime, nextNode, waitForFinish));
		}
		
		public void ExitBackground(Color fadeColour, float fadeOutTime = 0.5f, bool nextNode = false)
		{
			m_colourBackground.color = fadeColour; 
			m_backgroundComponent.StartCoroutine(FadeOut(fadeOutTime, nextNode)); 
		}
		
		public void SetBackground(Sprite background)
		{
			m_imageBackground.sprite = background;
		}
		
		private IEnumerator FadeIn(Sprite background, float fadeInTime = 0.5f, bool nextNode = false, bool waitForFinish = false)
		{
			if (m_imageBackground.sprite != null)
				Debug.LogWarning("CNovell: Скрой Фон посредством \"Конца\" перед переходом на другой фон!");

			m_imageBackground.sprite = background;

			SceneManager sceneManager = SceneManager.GetInstance();

			if (sceneManager != null && !waitForFinish && nextNode)
				sceneManager.NextNode();

			float elapsedTime = 0.0f;

			while (elapsedTime < fadeInTime)
			{
				float percentage = elapsedTime / fadeInTime;
				m_imageBackground.color = new Color(1, 1, 1, percentage);
				
				elapsedTime += Time.deltaTime;

				yield return null;
			}
			
			if (sceneManager != null && waitForFinish && nextNode)
				sceneManager.NextNode();
		}
		
		private IEnumerator FadeOut(float fadeOutTime = 0.5f, bool nextNode = false)
		{
			float elapsedTime = 0.0f;

			while (elapsedTime < fadeOutTime)
			{
				float percentage = 1 - (elapsedTime / fadeOutTime);
				m_imageBackground.color = new Color(1, 1, 1, percentage);

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			m_imageBackground.sprite = null; 

			SceneManager sceneManager = SceneManager.GetInstance();
			if (sceneManager != null && nextNode)
				sceneManager.NextNode();
		}
	}
}