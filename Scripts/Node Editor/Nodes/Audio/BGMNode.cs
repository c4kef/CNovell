using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CNovell.Nodes
{
    [System.Serializable]
    public class BGMNode : BaseNode
    {
        private string[] m_toggle = { "Установка", "Пауза", "Продолжение" };
        [SerializeField] private int m_toggleSelection = 0;

        [SerializeField] private AudioClip m_bgmAudio; 
        [SerializeField] private List<AudioClip> m_ambientAudio; 

        #region getters

        public int GetToggleSelection() => m_toggleSelection; 
        public AudioClip GetBGM() => m_bgmAudio; 
        public List<AudioClip> GetAmbientAudio() => m_ambientAudio; 

        #endregion

#if UNITY_EDITOR
        public override void Init(Vector2 position)
        {
            base.Init(position);

            m_title = "Музыка заднего плана";

            m_rectangle.width = 200;
            m_rectangle.height = 118;

            AddOutputPoint(); 
            
            m_ambientAudio = new List<AudioClip>();
            AddAmbientAudio(); 
        }
        
        public override void Copy(BaseNode node, Vector2 position)
        {
            base.Copy(node, position);

            BGMNode bgmNode = node as BGMNode;

            m_toggleSelection = bgmNode.m_toggleSelection;

            m_bgmAudio = bgmNode.m_bgmAudio;

            m_ambientAudio = new List<AudioClip>();
            for (int i = 0; i < bgmNode.m_ambientAudio.Count; i++)
                m_ambientAudio.Add(bgmNode.m_ambientAudio[i]);
        }
        
        protected override void DrawNodeWindow(int id)
        {
            m_toggleSelection = EditorGUILayout.Popup(m_toggleSelection, m_toggle);

            if (m_toggleSelection == 0)
            {
                EditorGUILayout.LabelField("Фоновая музыка");
                m_bgmAudio = EditorGUILayout.ObjectField(m_bgmAudio, typeof(AudioClip), false) as AudioClip;

                EditorGUILayout.LabelField("Эмбиентные треки");
                for (int i = 0; i < m_ambientAudio.Count; i++)
                    m_ambientAudio[i] = EditorGUILayout.ObjectField(m_ambientAudio[i], typeof(AudioClip), false) as AudioClip;

                EditorGUILayout.LabelField("");
                Rect buttonRect = new Rect(m_rectangle.width - 48, 0, 21, 16);
                buttonRect.y = GUILayoutUtility.GetLastRect().y;
                
                if (GUI.Button(buttonRect, "+"))
                    AddAmbientAudio();

                buttonRect.x += 22;

                if (GUI.Button(buttonRect, "-"))
                    RemoveAmbientAudio();
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                Rect lastRect = GUILayoutUtility.GetLastRect();
                m_rectangle.height = lastRect.y + lastRect.height + 4;
            }

            base.DrawNodeWindow(id);
        }
        
        private void AddAmbientAudio()
        {
            
            m_ambientAudio.Add(null);
            m_rectangle.height += 18;
        }
        
        private void RemoveAmbientAudio()
        {
            if (m_ambientAudio.Count > 1)
            {
                m_ambientAudio.RemoveAt(m_ambientAudio.Count - 1);
                m_rectangle.height -= 18;
            }
        }

#endif
    }
}