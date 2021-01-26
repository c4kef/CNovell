using UnityEngine;
using UnityEngine.UI;
using CNovell.ScriptableObjects;
using System.Collections.Generic;

namespace CNovell.Components
{
	[System.Serializable]
	public class SaveCharacterInfo
	{
		[SerializeField]
		public string m_nameCharacter;
		[SerializeField]
		public int m_indexSprite;
		[SerializeField]
		public Vector3 m_rTransform_pos;
		[SerializeField]
		public Quaternion m_rTransform_rot;
	}

	[System.Serializable]
	public class CharacterInfo : MonoBehaviour
	{
		[SerializeField] private Character m_character;
		[SerializeField] private Image m_image;
		[SerializeField] private RectTransform m_rTransform;

        private void Awake()
        {
			m_image = GetComponent<Image>();
			m_rTransform = GetComponent<RectTransform>();
        }

        #region getters

        public Character GetCharacter() => m_character; 
		public Image GetImage() => m_image; 
		public RectTransform GetRectTransform() => m_rTransform; 
		public SaveCharacterInfo GetSaveData()
        {
			SaveCharacterInfo save = new SaveCharacterInfo();
			save.m_rTransform_pos = m_rTransform.position;
			save.m_rTransform_rot = m_rTransform.rotation;
			save.m_nameCharacter = m_character.GetName();
			save.m_indexSprite = m_character.GetSprites().FindIndex(sprite => sprite == m_image.sprite);
			return save;
		}

		#endregion

		#region setters

		public void SetCharacter(Character character) => m_character = character; 
		public void SetImage(Image image) => m_image = image; 
		public void SetRectTransform(RectTransform transform) => m_rTransform = transform; 
		public void SetSaveData(SaveCharacterInfo save)
		{
			GetComponent<RectTransform>().position = save.m_rTransform_pos;
			GetComponent<Image>().sprite = m_character.GetSprites()[save.m_indexSprite];
		}

		#endregion
	}
}