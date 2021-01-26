using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class CharacterNode : BaseNode
    {
        
        private string[] m_toggle = { "Начало", "Конец" };
        [SerializeField] private int m_toggleSelection = 0;
        [SerializeField] private Character m_character;
        [SerializeField] private Sprite m_sprite;
        [SerializeField] private int m_spriteSelection = 0;
        [SerializeField] private float m_xPosition = 50; 
        [SerializeField] private float m_fadeTime = 0.5f; 
        [SerializeField] private bool m_isInverted = false; 
        [SerializeField] private bool m_waitForFinish = true; 

        #region getters

        public int GetToggleSelection() => m_toggleSelection; 
        public Character GetCharacter() => m_character; 
        public Sprite GetSprite() => m_sprite; 
        public float GetXPosition() => m_xPosition; 
        public float GetFadeTime() => m_fadeTime; 
        public bool GetIsInverted() => m_isInverted; 
        public bool GetWaitForFinish() => m_waitForFinish; 

        #endregion

#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Персонаж";

            m_rectangle.width = 340;

            AddOutputPoint(); 
        }

        public override void Copy(BaseNode node, Vector2 position)
        {
            base.Copy(node, position);

            CharacterNode characterNode = node as CharacterNode;

            
            m_toggleSelection = characterNode.m_toggleSelection;

            
            m_character = characterNode.m_character;
            m_spriteSelection = characterNode.m_spriteSelection;

            
            m_xPosition = characterNode.m_xPosition;
            m_fadeTime = characterNode.m_fadeTime;
            m_isInverted = characterNode.m_isInverted;
        }

        protected override void DrawNodeWindow(int id)
        {
            DrawSpriteBackground();
            
            m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);

            DrawCharacterObjectField(); 

            if (m_character != null)
            {
                if (DrawSpritePopup())
                {
                    DrawAlignmentSlider();
                    DrawFadeTimeSlider();
                    DrawInvertToggle();
                    DrawWaitForFinishToggle();
                }
                
                if (m_sprite != null)
                    EditorGUILayout.EndVertical();
            }
            
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
        
        private bool DrawSpritePopup()
        {
            List<Sprite> sprites = m_character.GetSprites();
            
            if (sprites.Count == 0)
            {
                Debug.LogError("CNovell: У персонажа нет спрайтов! Если вам нужен закадровый персонаж, нет необходимости в пунктах \"Начало\"/\"Конец\".");
                m_sprite = null;
                return false;
            }
            
            string[] spriteNames = new string[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite sprite = sprites[i];
                
                if (sprite == null)
                {
                    Debug.LogError($"CNovell: У персонажа не найден спрайт, индекс {i}!");
                    return false;
                }
                else
                    spriteNames[i] = sprite.name;
            }

            EditorGUILayout.LabelField("Спрайт");
            m_spriteSelection = EditorGUILayout.Popup(m_spriteSelection, spriteNames);
            m_sprite = sprites[m_spriteSelection];

            m_spriteSelection = Mathf.Clamp(m_spriteSelection, 0, sprites.Count - 1);
            return true;
        }

        private void DrawAlignmentSlider()
        {
            if (m_toggleSelection == 0)
            {
                EditorGUILayout.LabelField("X Позиция (%)");
                m_xPosition = EditorGUILayout.Slider(m_xPosition, 0, 100);
            }
        }

        private void DrawFadeTimeSlider()
        {
            EditorGUILayout.LabelField("Время скрытия");
            m_fadeTime = EditorGUILayout.Slider(m_fadeTime, 0.1f, 5);
        }
        
        private void DrawInvertToggle()
        {
            if (m_toggleSelection == 0)
            {
                EditorGUILayout.LabelField("Инвертировать");
                Rect toggleRect = GUILayoutUtility.GetLastRect();
                toggleRect.x = toggleRect.width - 8;
                m_isInverted = EditorGUI.Toggle(toggleRect, m_isInverted);
            }
        }

        private void DrawWaitForFinishToggle()
        {
            
            EditorGUILayout.LabelField("Ждать завершения?");
            Rect toggleRect = GUILayoutUtility.GetLastRect();
            toggleRect.x = toggleRect.width - 8;
            m_waitForFinish = EditorGUI.Toggle(toggleRect, m_waitForFinish);
        }
#endif
    }
}