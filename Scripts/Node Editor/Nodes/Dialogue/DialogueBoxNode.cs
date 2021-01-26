using UnityEngine;
using UnityEditor;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class DialogueBoxNode : BaseNode
	{
		
		private string[] m_toggle = { "Показать", "Скрыть" };
		[SerializeField] private int m_toggleSelection = 0;

		#region getters

		public int GetToggleSelection() => m_toggleSelection; 

		#endregion

#if UNITY_EDITOR
		
		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Диалоги";

			m_rectangle.width = 128;
			m_rectangle.height = 40;

			AddOutputPoint(); 
		}
		
		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			DialogueBoxNode dialogueBoxNode = node as DialogueBoxNode;
			
			m_toggleSelection = dialogueBoxNode.m_toggleSelection;
		}
		
		protected override void DrawNodeWindow(int id)
		{
			m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);

			base.DrawNodeWindow(id);
		}

#endif
	}
}