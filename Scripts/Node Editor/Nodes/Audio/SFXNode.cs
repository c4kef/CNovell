using UnityEngine;
using UnityEditor;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class SFXNode : BaseNode
	{
		[SerializeField] private AudioClip m_sfxAudio; 

		[SerializeField] private bool m_waitForFinish = false;

		#region getters

		public AudioClip GetSFX() => m_sfxAudio;
		public bool GetWaitForFinish() => m_waitForFinish;

		#endregion

#if UNITY_EDITOR

		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Музыкальные спецэффекты (SFX)";

			m_rectangle.width = 270;
			m_rectangle.height = 84;

			AddOutputPoint(); 
		}
		
		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			SFXNode sfxNode = node as SFXNode;

			m_sfxAudio = sfxNode.m_sfxAudio;
		}
		
		protected override void DrawNodeWindow(int id)
		{
			
			EditorGUILayout.LabelField("Звук");
			m_sfxAudio = EditorGUILayout.ObjectField(m_sfxAudio, typeof(AudioClip), false) as AudioClip;

			
			EditorGUILayout.LabelField("Ждать завершения?");
			Rect toggleRect = GUILayoutUtility.GetLastRect();
			toggleRect.x = m_rectangle.width - 18;
			m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);

			base.DrawNodeWindow(id);
		}
#endif
	}
}