using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class CharacterTranslateNode : BaseNode
	{
		[SerializeField] private Character m_character;
		[SerializeField] private float m_xPosition = 50;
		[SerializeField] private bool m_isLerp = false;
		[SerializeField] private float m_lerpTime = 0.5f;
		#region getters

		public Character GetCharacter() => m_character; 
		public float GetXPosition() => m_xPosition; 
		public bool GetIsLerp() => m_isLerp; 
		public float GetLerpTime() => m_lerpTime; 

		#endregion

#if UNITY_EDITOR

		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Перемещение персонажа";

			m_rectangle.width = 190;

			AddOutputPoint(); 
		}

		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			CharacterTranslateNode characterMoveNode = node as CharacterTranslateNode;

			
			m_character = characterMoveNode.m_character;
			m_xPosition = characterMoveNode.m_xPosition;
			m_isLerp = characterMoveNode.m_isLerp;
			m_lerpTime = characterMoveNode.m_lerpTime;
		}
		
		protected override void DrawNodeWindow(int id)
		{
			EditorGUILayout.LabelField("Персонаж");
			m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;
			
			EditorGUILayout.LabelField("X Позиция (%)");
			m_xPosition = EditorGUILayout.Slider(m_xPosition, 0, 100);
			
			EditorGUILayout.LabelField("Гладко переместить?");
			Rect toggleRect = GUILayoutUtility.GetLastRect();
			toggleRect.x = m_rectangle.width - 18;
			m_isLerp = EditorGUI.Toggle(toggleRect, m_isLerp);

			if (m_isLerp)
			{
				EditorGUILayout.LabelField("Скорость");
				m_lerpTime = EditorGUILayout.Slider(m_lerpTime, 0, 3);
			}
			
			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				m_rectangle.height = lastRect.y + lastRect.height + 4;
			}

			base.DrawNodeWindow(id);
		}
#endif
	}
}