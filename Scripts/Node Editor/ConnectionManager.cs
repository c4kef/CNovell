using UnityEngine;
using UnityEditor;
using CNovell.Nodes;

namespace CNovell.Editor
{

#if UNITY_EDITOR
    public class ConnectionManager
    {
        
        private BaseNode m_selectedLeftNode;
        private BaseNode m_selectedRightNode;
        private int m_selectedOutput;

        
        private GUIStyle m_inputStyle;
        private GUIStyle m_outputStyle;

        public Color m_Color;

        #region getters

        public BaseNode GetSelectedLeftNode() => m_selectedLeftNode; 
        public BaseNode GetSelectedRightNode() => m_selectedRightNode; 
        public int GetSelectedOutput() => m_selectedOutput; 
        public GUIStyle GetInputStyle() => m_inputStyle; 
        public GUIStyle GetOutputStyle() => m_outputStyle; 

        #endregion

        #region setters

        public void SetSelectedRightNode(BaseNode node) => m_selectedRightNode = node;

        #endregion
        
        public ConnectionManager()
        {
            m_inputStyle = new GUIStyle();
            m_inputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
            m_inputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
            m_inputStyle.border = new RectOffset(4, 4, 12, 12);
            
            m_outputStyle = new GUIStyle();
            m_outputStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
            m_outputStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
            m_outputStyle.border = new RectOffset(4, 4, 12, 12);
            m_Color = Color.white;
        }

        public void Update()
        {
            DrawBezierToMouse();
        }
        
        public void CreateConnection()
        {
            
            Undo.RecordObject(m_selectedLeftNode, "Создать соединение");
            Undo.RecordObject(m_selectedRightNode, "Создать соединение");
            
            if (m_selectedLeftNode.m_outputs[m_selectedOutput] != -1)
                RemoveConnection(m_selectedLeftNode, m_selectedOutput);

            m_selectedLeftNode.m_outputs[m_selectedOutput] = m_selectedRightNode.GetNodeID(); 
            m_selectedRightNode.m_inputs.Add(m_selectedLeftNode.GetNodeID()); 
        }
        
        public void CreateLinearConnection(BaseNode leftNode, BaseNode rightNode)
        {
            Debug.Assert(leftNode.m_outputs.Count == 1, "CNovell: Не пытайтесь создать линейный вывод для нелинейного узла");
            
            m_selectedLeftNode = leftNode;
            m_selectedRightNode = rightNode;
            m_selectedOutput = 0;

            CreateConnection();
            ClearConnectionSelection();
        }
        
        public void RemoveConnection(BaseNode leftNode, int outputIndex)
        {
            NodeManager nodeManager = NodeEditor.GetNodeManager();
            BaseNode rightNode = nodeManager.FindNode(leftNode.m_outputs[outputIndex]);

            Undo.RecordObject(leftNode, "Удалить соединение");
            Undo.RecordObject(rightNode, "Удалить соединение");

            leftNode.m_outputs[outputIndex] = -1; 
            rightNode.m_inputs.Remove(leftNode.GetNodeID()); 
        }
        
        public void ClearConnectionSelection()
        {
            m_selectedLeftNode = null;
            m_selectedRightNode = null;
        }
        
        public void OnClickInput(BaseNode node)
        {
            m_selectedRightNode = node;

            if (m_selectedLeftNode != null)
            {
                if (m_selectedRightNode != m_selectedLeftNode)
                    CreateConnection(); 

                ClearConnectionSelection(); 
            }
        }
        
        public void OnClickOutput(BaseNode node, int outputIndex)
        {
            m_selectedLeftNode = node;
            m_selectedOutput = outputIndex;
            
            if (m_selectedRightNode != null)
            {
                if (m_selectedLeftNode != m_selectedRightNode)
                    CreateConnection(); 

                ClearConnectionSelection(); 
            }
        }
        
        private void DrawBezierToMouse()
        {
            Color colour = m_Color; 

            if (m_selectedLeftNode != null)
            {
                Vector3 startPos = m_selectedLeftNode.m_outputPoints[m_selectedOutput].center;
                Vector3 endPos = NodeEditor.GetMousePosition();
                Vector3 startTangent = startPos + Vector3.right * 50;
                Vector3 endTangent = endPos + Vector3.left * 50;
                
                if (m_selectedLeftNode is ConditionNode)
                {
                    if (m_selectedOutput == 0)
                        colour = Color.green; 
                    else
                        colour = Color.red; 
                }
                
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 2);
            }
            else if (m_selectedRightNode != null)
            {
                Vector3 startPos = NodeEditor.GetMousePosition();
                Vector3 endPos = m_selectedRightNode.m_inputPoint.center;
                Vector3 startTangent = startPos + Vector3.right * 50;
                Vector3 endTangent = endPos + Vector3.left * 50;

                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, colour, null, 2);
            }
        }
    }
#endif
}