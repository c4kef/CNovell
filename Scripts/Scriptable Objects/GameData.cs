using System.Collections.Generic;
using UnityEngine;

namespace CNovell.ScriptableObjects
{
	[System.Serializable]
	public class GameData
	{
		[SerializeField] private List<SaveData> m_saves = new List<SaveData>();

		#region getters

		public List<SaveData> GetSaves() => m_saves; 

		#endregion

		public GameData(int numberOfSaves)
		{
			for (int i = 0; i < numberOfSaves; i++)
				m_saves.Add(null);
		}

		public void SetSave(SaveData saveData, int saveSlot) => m_saves[saveSlot] = saveData;
	}
}