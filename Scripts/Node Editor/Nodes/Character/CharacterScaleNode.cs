using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class CharacterScaleNode : BaseNode
	{
		[SerializeField] private Character m_character;
		[SerializeField] private Vector2 m_scale = new Vector2(1, 1);
		[SerializeField] private bool m_isLerp = false;
		[SerializeField] private float m_lerpTime = 0.5f;

		#region getters

		public Character GetCharacter() => m_character; 
		public Vector2 GetScale() => m_scale; 
		public bool GetIsLerp() => m_isLerp; 
		public float GetLerpTime() => m_lerpTime; 

		#endregion

#if UNITY_EDITOR
		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Размер персонажа";

			m_rectangle.width = 190;

			AddOutputPoint(); 
		}

		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			CharacterScaleNode characterScaleNode = node as CharacterScaleNode;

			
			m_character = characterScaleNode.m_character;
			m_scale = characterScaleNode.m_scale;
			m_isLerp = characterScaleNode.m_isLerp;
			m_lerpTime = characterScaleNode.m_lerpTime;
		}

		protected override void DrawNodeWindow(int id)
		{
			
			EditorGUILayout.LabelField("Персонаж");
			m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;
			
			m_scale = EditorGUILayout.Vector2Field("Размер", m_scale);
			
			EditorGUILayout.LabelField("Гладко уменьшать?");
			Rect toggleRect = GUILayoutUtility.GetLastRect();
			toggleRect.x = m_rectangle.width - 18;
			m_isLerp = EditorGUI.Toggle(toggleRect, m_isLerp);

			
			if (m_isLerp)
			{
				EditorGUILayout.LabelField("Скорость");
				m_lerpTime = EditorGUILayout.Slider(m_lerpTime, 0.1f, 5);
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