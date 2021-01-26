using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
	public class BackgroundNode : BaseNode
	{
		private string[] m_toggle = { "Начало", "Конец" };
		[SerializeField] private int m_toggleSelection = 0;

		[SerializeField] private Background m_background; 

		[SerializeField] private Color m_fadeColour = Color.black;
		[SerializeField] private float m_fadeTime = 0.5f;
		[SerializeField] private bool m_waitForFinish = true;

		#region getters

		public int GetToggleSelection() => m_toggleSelection; 
		public Background GetBackground() => m_background; 
		public Color GetFadeColour() => m_fadeColour;
		public float GetFadeTime() => m_fadeTime; 
		public bool GetWaitForFinish() => m_waitForFinish; 

		#endregion

#if UNITY_EDITOR
		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Задний фон";

			m_rectangle.width = 170;

			AddOutputPoint(); 
		}
		
		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			BackgroundNode backgroundNode = node as BackgroundNode;
			
			m_toggleSelection = backgroundNode.m_toggleSelection;
			
			m_background = backgroundNode.m_background;
			
			m_fadeColour = backgroundNode.m_fadeColour;
			m_fadeTime = backgroundNode.m_fadeTime;
			m_waitForFinish = backgroundNode.m_waitForFinish;
		}
		
		protected override void DrawNodeWindow(int id)
		{
			m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);
			
			if (m_toggleSelection == 0)
			{
				EditorGUILayout.LabelField("Задний фон");
				m_background = EditorGUILayout.ObjectField(m_background, typeof(Background), false) as Background;
				
				if (m_background != null)
				{
					GUILayoutOption[] options = { GUILayout.Width(160), GUILayout.Height(90) };
					GUILayout.Box(m_background.GetBackground().texture, options);
				}
			}

			EditorGUILayout.LabelField("Цвет исчезания");
			m_fadeColour = EditorGUILayout.ColorField(m_fadeColour);

			EditorGUILayout.LabelField("Время исчезания");
			m_fadeTime = EditorGUILayout.Slider(m_fadeTime, 0.1f, 5);
			
			if (m_toggleSelection == 0)
			{
				EditorGUILayout.LabelField("Ждать завершения?");
				Rect toggleRect = GUILayoutUtility.GetLastRect();
				toggleRect.x = m_rectangle.width - 20;

				m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);
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