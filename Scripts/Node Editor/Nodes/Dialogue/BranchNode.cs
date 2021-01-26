using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Editor;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class BranchNode : BaseNode
	{
		[SerializeField] private List<string> m_branches;
		[SerializeField] private float m_timeWait;

		#region getters

		public List<string> GetBranches() => m_branches; 
		public float GetTimeWait() => m_timeWait; 

		#endregion

#if UNITY_EDITOR
		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Ветка выборов";

			m_rectangle.width = 340;

			m_branches = new List<string>(); 

			
			AddBranch();
			AddBranch();
		}
		
		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			BranchNode branchNode = node as BranchNode;

			
			m_branches = new List<string>();
			for (int i = 0; i < branchNode.m_branches.Count; i++)
				m_branches.Add(branchNode.m_branches[i]);
		}

		protected override void DrawNodeWindow(int id)
		{
			EditorGUILayout.LabelField("Ожидание ответа (S)");
			m_timeWait = EditorGUILayout.Slider(m_timeWait, 0, 5);
			
			for (int i = 0; i < m_branches.Count; i++)
			{
				EditorGUILayout.LabelField($"Вариант ({(i + 1)})");
				m_branches[i] = EditorGUILayout.TextField(m_branches[i]);
			}
			
			Rect buttonRect = new Rect(m_rectangle.width - 48, 0, 21, 16);
			buttonRect.y = GUILayoutUtility.GetLastRect().y + buttonRect.height + 4;

			
			if (GUI.Button(buttonRect, "+"))
				AddBranch();

			buttonRect.x += 22;

			
			if (GUI.Button(buttonRect, "-"))
				RemoveBranch();

			
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				m_rectangle.height = lastRect.y + lastRect.height + 24;
			}

			base.DrawNodeWindow(id);
		}
		
		private void AddBranch()
		{
			
			m_branches.Add("");
			AddOutputPoint();
		}
		
		private void RemoveBranch()
		{
			if (m_branches.Count <= 2)
				return;

			int removalIndex = m_branches.Count - 1;

			m_branches.RemoveAt(removalIndex);

			if (m_outputs[removalIndex] != -1)
				NodeEditor.GetConnectionManager().RemoveConnection(this, removalIndex);
			
			m_outputPoints.RemoveAt(removalIndex);
			m_outputs.RemoveAt(removalIndex);
		}
#endif
	}
}