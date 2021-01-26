using System.Collections.Generic;
using UnityEngine;
using CNovell.Components;

namespace CNovell.ScriptableObjects
{
	[System.Serializable]
	public class SaveData
	{
		[SerializeField] private List<SaveCharacterInfo> characterInfo;
		[SerializeField] private int m_pageIndex;
		[SerializeField] private int m_nodeIndex;
		[SerializeField] private string m_nameOfBackground;
		[SerializeField] private string m_dateTime;
		[SerializeField] private string m_nameScene;

		#region getters

		public List<SaveCharacterInfo> GetCharacters() => characterInfo;
		public int GetPageIndex() => m_pageIndex;
		public int GetNodeIndex() => m_nodeIndex;
		public string GetNameOfBackground() => m_nameOfBackground;
		public string GetDateTime() => m_dateTime;
		public string GetNameScene() => m_nameScene;

		#endregion

		#region setters

		public void SetCharacters(List<SaveCharacterInfo> characters) => characterInfo = characters;
		public void SetPageIndex(int pageIndex) => m_pageIndex = pageIndex;
		public void SetNodeIndex(int nodeIndex) => m_nodeIndex = nodeIndex;
		public void SetNameOfBackground(string nameOfBackground) => m_nameOfBackground = nameOfBackground;
		public void SetDateTime(string dateTime) => m_dateTime = dateTime;
		public void SetNameScene(string nameScene) => m_nameScene = nameScene;

		#endregion
	}
}