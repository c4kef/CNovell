using System.Collections.Generic;
using UnityEngine;
using CNovell.Editor;
using CNovell.Nodes;

namespace CNovell.ScriptableObjects
{
    [System.Serializable]
    public class Page : ScriptableObject
    {
        [SerializeField] private int m_currentID = 0;
        [SerializeField] private List<BaseNode> m_nodes = new List<BaseNode>();

        #region getters

        public List<BaseNode> GetNodes() => m_nodes;
        public List<CharacterNode> GetNodesCharacter()
        {
            List<CharacterNode> characters = new List<CharacterNode>();
            foreach (var node in m_nodes)
                if (node as CharacterNode != null)
                    if ((node as CharacterNode).GetToggleSelection() != 1)
                        characters.Add(node as CharacterNode);

            return characters;
        }

        public List<BackgroundNode> GetNodesBackground()
        {
            List<BackgroundNode> backgrounds = new List<BackgroundNode>();
            foreach (var node in m_nodes)
                if (node as BackgroundNode != null)
                    if ((node as BackgroundNode).GetToggleSelection() != 1)
                        backgrounds.Add(node as BackgroundNode);

            return backgrounds;
        }

        public List<DialogueNode> GetNodesDialogue()
        {
            List<DialogueNode> dialogues = new List<DialogueNode>();
            foreach (var node in m_nodes)
                if (node as DialogueNode != null)
                    dialogues.Add(node as DialogueNode);

            return dialogues;
        }
        public int GetCurrentNodeID() => m_currentID; 

        #endregion

        #region setters

        public void SetCurrentNodeID(int currentID) => m_currentID = currentID; 

        #endregion

#if UNITY_EDITOR
        public void Init()
        {
            BaseNode startNode = NodeEditor.GetNodeManager().AddNode(typeof(StartNode));
            m_nodes.Add(startNode); 
        }
#endif
    }
}