using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Editor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
	[System.Serializable]
	public class DialogueNode : BaseNode
	{
		[SerializeField] private Character m_character;

		[SerializeField] private Sprite m_sprite;
		[SerializeField] private int m_spriteSelection;

		[SerializeField] private AudioClip m_characterAudio;
		[SerializeField] private int m_audioSelection;

		[SerializeField] private string m_dialogue = "";

		private int m_newDialogueID;
		private bool m_newDialogueCreated = false;

		#region getters

		public Character GetCharacter() => m_character; 
		public Sprite GetSprite() => m_sprite; 
		public AudioClip GetCharacterAudio() => m_characterAudio; 
		public string GetDialogue() => m_dialogue; 

		#endregion

		public void SetSprite(Sprite m_sprite) => this.m_sprite = m_sprite;
		public void SetAudio(AudioClip m_characterAudio) => this.m_characterAudio = m_characterAudio;

#if UNITY_EDITOR
		public override void Init(Vector2 position)
		{
			base.Init(position);

			m_title = "Диалоги";

			m_rectangle.width = 340;

			AddOutputPoint(); 
		}
		
		public override void Copy(BaseNode node, Vector2 position)
		{
			base.Copy(node, position);

			DialogueNode dialogueNode = node as DialogueNode;

			m_character = dialogueNode.m_character;
			m_spriteSelection = dialogueNode.m_spriteSelection; 
			m_audioSelection = dialogueNode.m_audioSelection; 

			m_dialogue = dialogueNode.m_dialogue;
		}
		
		protected override void DrawNodeWindow(int id)
		{
			DrawSpriteBackground();
			DrawCharacterObjectField(); 
			
			if (m_character != null)
			{
				DrawSpritePopup(); 
				DrawAudioPopup(); 

				if (m_sprite != null)
					EditorGUILayout.EndVertical();

			}
			DrawDialogueTextArea(); 
			
			if (GUI.Button(new Rect(m_rectangle.width - 40, 0, 20, 15), "+"))
				CreateNextDialogue();

			if (Event.current.type == EventType.Repaint)
			{
				Rect lastRect = GUILayoutUtility.GetLastRect();
				m_rectangle.height = lastRect.y + lastRect.height + 4;
			}

			base.DrawNodeWindow(id);
		}
		
		private void DrawSpriteBackground()
		{
			if (m_sprite == null)
				return; 
			
			float aspectRatio = m_sprite.rect.width / m_sprite.rect.height;
			float spriteWidth = m_rectangle.width;
			float spriteHeight = spriteWidth / aspectRatio;
			
			GUI.color = new Color(1, 1, 1, 0.45f);
			GUI.DrawTexture(new Rect(72, 16, spriteWidth, spriteHeight), m_sprite.texture);
			GUI.color = Color.white;
			
			EditorGUILayout.BeginVertical(GUILayout.Width(m_rectangle.width * 0.5f));
		}
		
		private void DrawCharacterObjectField()
		{
			EditorGUILayout.LabelField("Персонаж");
			m_character = EditorGUILayout.ObjectField(m_character, typeof(Character), false) as Character;

			if (m_character == null)
				m_sprite = null; 
		}
		
		private void DrawSpritePopup()
		{
			List<Sprite> sprites = m_character.GetSprites();
			string[] spriteNames = new string[sprites.Count + 1];
			spriteNames[0] = "Отсутствует"; 
			
			for (int i = 0; i < sprites.Count; i++)
			{
				Sprite sprite = sprites[i];
				
				if (sprite == null)
					Debug.LogError($"CNovell: у персонажа отсутствует спрайт по индексу {i}!");
				else
					spriteNames[i + 1] = sprite.name;
			}
			
			EditorGUILayout.LabelField("Изменение спрайта");
			m_spriteSelection = EditorGUILayout.Popup(m_spriteSelection, spriteNames);
			
			if (m_spriteSelection == 0)
				m_sprite = null;
			else
				m_sprite = sprites[m_spriteSelection - 1];

			m_spriteSelection = Mathf.Clamp(m_spriteSelection, 0, sprites.Count);
		}
		
		private void DrawAudioPopup()
		{
			
			List<AudioClip> characterAudio = m_character.GetAudioClips();
			string[] audioNames = new string[characterAudio.Count + 1];
			audioNames[0] = "Отсутствует"; 
			
			for (int i = 0; i < characterAudio.Count; i++)
			{
				AudioClip audio = characterAudio[i];

				if (audio == null)
					Debug.LogError($"CNovell: у персонажа отсутствует спрайт по индексу {i}!");
				else
					audioNames[i + 1] = audio.name;
			}
			
			EditorGUILayout.LabelField("Звуки персонажа");
			m_audioSelection = EditorGUILayout.Popup(m_audioSelection, audioNames);
			
			m_audioSelection = Mathf.Clamp(m_audioSelection, 0, characterAudio.Count);

			if (m_audioSelection == 0 || characterAudio[m_audioSelection - 1] == null)
				m_characterAudio = null; 
			else
				m_characterAudio = characterAudio[m_audioSelection - 1];
		}
		
		private void DrawDialogueTextArea()
		{
			
			EditorGUILayout.LabelField("Диалоги");
			
			GUI.SetNextControlName($"Диалог {m_nodeID}");
			m_dialogue = EditorGUILayout.TextArea(m_dialogue, GUILayout.MaxHeight(42));
			
			if (m_newDialogueCreated)
			{
				EditorGUI.FocusTextInControl($"Диалог {m_newDialogueID}");
				m_newDialogueCreated = false;
			}

			if (m_dialogue.Length > 0)
			{
				
				if (m_dialogue[m_dialogue.Length - 1] == '\t')
				{
					m_dialogue = m_dialogue.Remove(m_dialogue.Length - 1); 
					CreateNextDialogue(); 
				}
			}
		}
		
		private void CreateNextDialogue()
		{
			NodeManager nodeManager = NodeEditor.GetNodeManager();
			ConnectionManager connectionManager = NodeEditor.GetConnectionManager();

			DialogueNode dialogueNode = nodeManager.AddNode(typeof(DialogueNode)) as DialogueNode;
			connectionManager.CreateLinearConnection(this, dialogueNode);

			dialogueNode.m_rectangle = m_rectangle;
			dialogueNode.m_rectangle.x += m_rectangle.width + 48;

			dialogueNode.m_character = m_character; 
			
			m_newDialogueID = dialogueNode.m_nodeID;
			m_newDialogueCreated = true;
		}

#endif
	}
}