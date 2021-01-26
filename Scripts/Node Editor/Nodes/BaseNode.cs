using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Editor;
using CNovell.ScriptableObjects;

namespace CNovell.Nodes
{
    public abstract class BaseNode : ScriptableObject
    {
        [SerializeField] protected int m_nodeID; 

        [SerializeField] protected Rect m_rectangle;
        [SerializeField] protected string m_title;

        public List<int> m_inputs;
        public Rect m_inputPoint;
        
        public List<int> m_outputs;
        public List<Rect> m_outputPoints;

        public Color m_Color;
        
        private static float staticHue = 0;

        #region getters

        public int GetNodeID() => m_nodeID; 
        public Rect GetNodeRect() => m_rectangle; 

        #endregion

#if UNITY_EDITOR
        public virtual void Init(Vector2 position)
        {
            Scene scene = NodeEditor.GetScene();
            
            m_nodeID = scene.GetCurrentNodeID();
            scene.SetCurrentNodeID(m_nodeID + 1);

            m_Color = Color.white;
            
            m_rectangle = new Rect(position.x, position.y, 50, 100);

            m_inputs = new List<int>();
            m_inputPoint = new Rect(0, 0, 10, 20);

            
            m_outputs = new List<int>();
            m_outputPoints = new List<Rect>();
        }
        
        public virtual void Copy(BaseNode node, Vector2 position)
        {
            Scene scene = NodeEditor.GetScene();

            m_nodeID = scene.GetCurrentNodeID();
            scene.SetCurrentNodeID(m_nodeID + 1);

            m_Color = node.m_Color;
            
            m_rectangle = new Rect(position.x, position.y, 50, 100);

            m_title = node.m_title; 

            m_rectangle.width = node.m_rectangle.width;
            m_rectangle.height = node.m_rectangle.height;

            m_inputs = new List<int>();
            m_inputPoint = new Rect(0, 0, 10, 20);
            
            m_outputs = new List<int>();
            m_outputPoints = new List<Rect>();

            for (int i = 0; i < node.m_outputPoints.Count; i++)
                AddOutputPoint();
        }
        
        public virtual void Draw()
        {
            DrawInputPoint();
            DrawOutputPoints();

            staticHue -= 0.0001f;
            if (staticHue < -1.0f) staticHue += 1.0f;
            for (int i = 0; i < 1920; i++)
            {
                float hue = staticHue + (1.0f / (float)1920) * i;
                if (hue < 0.0f) hue += 1.0f;
                m_Color = Color.HSVToRGB(hue, 1.0f, 1.0f);
            }
            
            DrawBeziers();

            int windowID = NodeEditor.GetNodeManager().GetNodes().IndexOf(this);
            m_rectangle = GUI.Window(windowID, m_rectangle, DrawNodeWindow, m_title);
        }
        
        protected virtual void DrawNodeWindow(int id)
        {
            if (!(this is StartNode))
            {
                ProcessEvents(Event.current);

                if (GUI.Button(new Rect(m_rectangle.width - 20, 0, 20, 15), "X"))
                    NodeEditor.GetNodeManager().RemoveNode(this);
            }

            GUI.DragWindow(); 
        }
        
        protected void AddOutputPoint()
        {
            m_outputPoints.Add(new Rect(0, 0, 10, 20));
            m_outputs.Add(-1); 
        }
        
        void DrawInputPoint()
        {
            if (this is StartNode)
                return; 

            float xOffset = m_rectangle.x - m_inputPoint.width;
            m_inputPoint.x = xOffset;
            
            float yOffset = m_rectangle.height * 0.5f;
            m_inputPoint.y = m_rectangle.y + yOffset - m_inputPoint.height * 0.5f;

            ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
            connectionManager.m_Color = m_Color;
            if (GUI.Button(m_inputPoint, "", connectionManager.GetInputStyle()))
                connectionManager.OnClickInput(this);
        }
        
        void DrawOutputPoints()
        {
            for (int i = 0; i < m_outputPoints.Count; i++)
            {
                Rect rectangle = m_outputPoints[i];
                
                float xOffset = m_rectangle.x + m_rectangle.width;
                rectangle.x = xOffset;
                
                float yOffset = m_rectangle.height * ((i + 1) / (float)(m_outputPoints.Count + 1));
                rectangle.y = m_rectangle.y + yOffset - rectangle.height * 0.5f;

                m_outputPoints[i] = rectangle;
                
                ConnectionManager connectionManager = NodeEditor.GetConnectionManager();
                connectionManager.m_Color = m_Color;
                if (GUI.Button(m_outputPoints[i], "", connectionManager.GetOutputStyle()))
                    connectionManager.OnClickOutput(this, i);
            }
        }
        
        void DrawBeziers()
        {
            for (int i = 0; i < m_outputs.Count; i++)
            {
                if (m_outputs[i] == -1)
                    continue; 

                BaseNode outputNode = NodeEditor.GetNodeManager().FindNode(m_outputs[i]);

                Vector3 startPos = m_outputPoints[i].center;
                Vector3 endPos = outputNode.m_inputPoint.center;
                Vector3 startTangent = startPos + Vector3.right * 50;
                Vector3 endTangent = endPos + Vector3.left * 50;

                Handles.color = m_Color;
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Handles.color, null, 2);

                
                if (Handles.Button((startPos + endPos) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
                    NodeEditor.GetConnectionManager().RemoveConnection(this, i);
            }
        }
        
        protected virtual void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu(); 

            genericMenu.AddDisabledItem(new GUIContent("Новый узелок"));

            
            genericMenu.AddItem(new GUIContent("Скопировать"), false, () => NodeEditor.GetNodeManager().CopyNode(this));
            genericMenu.AddDisabledItem(new GUIContent("Вставить"));
            genericMenu.AddItem(new GUIContent("Удалить"), false, () => NodeEditor.GetNodeManager().RemoveNode(this));

            genericMenu.ShowAsContext(); 
        }
        
        public void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ProcessContextMenu(); 
                        e.Use();
                    }
                    break;

                
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.Delete)
                        NodeEditor.GetNodeManager().RemoveNode(this); 
                    break;
            }
        }
        
        public void Drag(Vector2 translation)
        {
            m_rectangle.position += translation;
        }
#endif
    }
}