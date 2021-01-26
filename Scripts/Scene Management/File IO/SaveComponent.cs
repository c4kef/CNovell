using UnityEngine;
using CNovell.SceneManagement;

namespace CNovell.Components
{
	public class SaveComponent : MonoBehaviour
	{
		[Header("Префаб слот для сохранения")]
		[SerializeField] private GameObject m_saveSlotPrefab;

		[Header("Сохранить UI элемент(ы)")]
		[SerializeField] private GameObject m_savePanel;
		[SerializeField] private RectTransform m_saveContent;

		[Header("Настройки")]
		[SerializeField] private int m_numberOfSaves;

		#region getters

		public GameObject GetSaveSlotPrefab() => m_saveSlotPrefab;
		public GameObject GetSavePanel() => m_savePanel;
		public RectTransform GetSaveContent() => m_saveContent;
		public int GetNumberOfSaves() => m_numberOfSaves;

		#endregion

		public void OpenSaveMenu() => SceneManager.GetInstance().GetSaveManager().UpdateSaveLoadSlots();

		public void OpenLoadMenu() => SceneManager.GetInstance().GetSaveManager().UpdateSaveLoadSlots(true);
	}
}