using UnityEngine;
using CNovell.SceneManagement;

namespace CNovell.Components
{
	public class CharacterComponent : MonoBehaviour
	{
		
		[Header("Префаб персонажа")]
		[SerializeField] private GameObject m_characterPrefab;

		
		[Header("Элементы пользовательского интерфейса персонажа(ей)")]
		[SerializeField] private RectTransform m_backgroundPanel;
		[SerializeField] private RectTransform m_foregroundPanel;

		private CharacterManager m_characterManager;

		#region getters

		public GameObject GetCharacterPrefab() => m_characterPrefab;
		public RectTransform GetBackgroundPanel() => m_backgroundPanel;
		public RectTransform GetForegroundPanel() => m_foregroundPanel;

		public CharacterManager GetCharacterManager()
		{
			if (m_characterManager == null)
				m_characterManager = new CharacterManager(this);

			return m_characterManager;
		}

		#endregion
	}
}