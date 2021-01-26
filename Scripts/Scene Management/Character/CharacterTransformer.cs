﻿using System.Collections;
using UnityEngine;

namespace CNovell.SceneManagement
{
	public class CharacterTransformer
	{
		public void SetCharacterPosition(GameObject character, Vector2 position)
		{
			Vector2 desiredPosition = GetScreenPosition(position);

			
			RectTransform characterTransform = character.GetComponent<RectTransform>();
			characterTransform.anchoredPosition = desiredPosition;
		}

		public IEnumerator LerpCharacterPosition(GameObject character, Vector2 position, float lerpTime)
		{
			RectTransform characterTransform = character.GetComponent<RectTransform>();
			Vector2 currentPosition = characterTransform.anchoredPosition;
			Vector2 desiredPosition = GetScreenPosition(position);

			float elapsedTime = 0;

			while (elapsedTime < lerpTime)
			{
				float percentage = elapsedTime / lerpTime;

				float positionX = Mathf.Lerp(currentPosition.x, desiredPosition.x, percentage);
				float positionY = Mathf.Lerp(currentPosition.y, desiredPosition.y, percentage);

				Vector3 newPosition = new Vector3(positionX, positionY, 0);
				characterTransform.anchoredPosition = newPosition;

				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}

		private Vector2 GetScreenPosition(Vector2 position)
		{
			CharacterManager characterManager = SceneManager.GetInstance().GetCharacterManager();
			
			RectTransform panelTransform = characterManager.GetBackgroundPanel();
			float screenExtentX = panelTransform.rect.width * 0.5f;
			float screenExtentY = panelTransform.rect.height * 0.5f;
			
			float xScalar = (position.x * 2 - 100) * 0.01f;
			float yScalar = (position.y * 2 - 100) * 0.01f;

			return new Vector2(screenExtentX * xScalar, screenExtentY * yScalar);
		}

		public void SetCharacterScale(GameObject character, Vector2 scale)
		{
			Vector3 desiredScale = new Vector3(scale.x, scale.y, 1);
			character.transform.localScale = desiredScale;
		}

		public IEnumerator LerpCharacterScale(GameObject character, Vector2 scale, float lerpTime)
		{
			Vector2 currentScale = character.transform.localScale;
			Vector2 desiredScale = new Vector3(scale.x, scale.y);
			float elapsedTime = 0;

			while (elapsedTime < lerpTime)
			{
				float percentage = elapsedTime / lerpTime;

				float scaleX = Mathf.Lerp(currentScale.x, desiredScale.x, percentage);
				float scaleY = Mathf.Lerp(currentScale.y, desiredScale.y, percentage);

				character.transform.localScale = new Vector3(scaleX, scaleY, 1);

				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
	}
}