using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CNovell.Nodes;
using CNovell.ScriptableObjects;

namespace CNovell.Editor
{

#if UNITY_EDITOR
    public class NodeEditor : EditorWindow
    {
        private static Scene m_scene;
        private Scene m_previousScene;

        private static NodeManager m_nodeManager;
        private static ConnectionManager m_connectionManager;

        private static Vector2 m_mousePosition;

        private Vector2 m_offset;
        private Vector2 m_drag;

        #region getters

        public static Scene GetScene() => m_scene; 
        public static NodeManager GetNodeManager() => m_nodeManager; 
        public static ConnectionManager GetConnectionManager() => m_connectionManager; 
        public static Vector2 GetMousePosition() => m_mousePosition; 

        #endregion

        #region enums

        enum MouseButton
        {
            LeftClick,
            RightClick,
            ScrollWheel
        }

        #endregion

        [MenuItem("Window/CNovell/Редактор сцен")]
        public static void Init()
        {
            NodeEditor window = GetWindow<NodeEditor>();
            window.titleContent = new GUIContent("Редактор сцен");
            window.minSize = new Vector2(600.0f, 300.0f);
            window.wantsMouseMove = true;
            window.Show();
        }

        private void OnEnable()
        {
            m_nodeManager = new NodeManager();
            m_connectionManager = new ConnectionManager();

            m_scene = m_previousScene;

        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            DrawSceneObjectField();

            if (m_scene == null)
                return;

            DrawPageToolbar();
            EditorGUILayout.EndHorizontal();

            if (m_scene != m_previousScene)
            {
                m_scene.Init();
                m_previousScene = m_scene;
            }

            m_mousePosition = Event.current.mousePosition;
            m_nodeManager.Update();
            m_connectionManager.Update();

            DrawContent();
            DrawLogo();

            ProcessEvents();

            EditorUtility.SetDirty(m_scene);
            Repaint();
        }

        private void ProcessContextMenu(bool newNodeOnly = false)
        {
            GenericMenu contextMenu = new GenericMenu();

            #region in-built CNovell nodes

            contextMenu.AddItem(new GUIContent("Создать/Аудио/Фоновая музыка"), false, () => m_nodeManager.AddNode(typeof(BGMNode)));
            contextMenu.AddItem(new GUIContent("Создать/Аудио/Звуковые эффекты"), false, () => m_nodeManager.AddNode(typeof(SFXNode)));

            contextMenu.AddItem(new GUIContent("Создать/Фон/Фон"), false, () => m_nodeManager.AddNode(typeof(BackgroundNode)));

            contextMenu.AddItem(new GUIContent("Создать/Персонаж/Размер"), false, () => m_nodeManager.AddNode(typeof(CharacterScaleNode)));
            contextMenu.AddItem(new GUIContent("Создать/Персонаж/Движение"), false, () => m_nodeManager.AddNode(typeof(CharacterTranslateNode)));
            contextMenu.AddSeparator("Создать/Персонаж/");
            contextMenu.AddItem(new GUIContent("Создать/Персонаж/Персонаж"), false, () => m_nodeManager.AddNode(typeof(CharacterNode)));

            contextMenu.AddItem(new GUIContent("Создать/Диалог/Ветка выборов"), false, () => m_nodeManager.AddNode(typeof(BranchNode)));
            contextMenu.AddItem(new GUIContent("Создать/Диалог/Диалог"), false, () => m_nodeManager.AddNode(typeof(DialogueNode)));
            contextMenu.AddSeparator("Создать/Диалог/");
            contextMenu.AddItem(new GUIContent("Создать/Диалог/Диалоги"), false, () => m_nodeManager.AddNode(typeof(DialogueBoxNode)));

            contextMenu.AddItem(new GUIContent("Создать/Утилиты/Задержка"), false, () => m_nodeManager.AddNode(typeof(DelayNode)));
            contextMenu.AddItem(new GUIContent("Создать/Утилиты/Страница"), false, () => m_nodeManager.AddNode(typeof(PageNode)));

            contextMenu.AddItem(new GUIContent("Создать/Переменные/Состояние"), false, () => m_nodeManager.AddNode(typeof(ConditionNode)));
            contextMenu.AddItem(new GUIContent("Создать/Переменные/Изменение"), false, () => m_nodeManager.AddNode(typeof(ModifyNode)));

            contextMenu.AddItem(new GUIContent("Создать/Конец"), false, () => m_nodeManager.AddNode(typeof(EndNode)));

            #endregion

            #region copy/paste/delete nodes

            contextMenu.AddDisabledItem(new GUIContent("Скопировать"));
            if (m_nodeManager.GetClipboard() != null && !newNodeOnly)
                contextMenu.AddItem(new GUIContent("Вставить"), false, m_nodeManager.PasteNode);
            else
                contextMenu.AddDisabledItem(new GUIContent("Вставить"));
            contextMenu.AddDisabledItem(new GUIContent("Удалить"));

            #endregion

            #region add/remove pages

            contextMenu.AddSeparator("");
            if (!newNodeOnly)
                contextMenu.AddItem(new GUIContent("Новая страница"), false, m_scene.NewPage);
            else
                contextMenu.AddDisabledItem(new GUIContent("Новая страница"));

            if (m_scene.GetPages().Count > 1 && !newNodeOnly)
                contextMenu.AddItem(new GUIContent("Удалить страницу"), false, m_scene.DeletePage);
            else
                contextMenu.AddDisabledItem(new GUIContent("Удалить страницу"));

            #endregion

            contextMenu.ShowAsContext();
        }

        private void ProcessEvents()
        {
            Event current = Event.current;
            m_drag = Vector2.zero;

            switch (current.type)
            {
                case EventType.MouseDown: 
                    if (current.button == (int)MouseButton.LeftClick)
                    {
                        m_connectionManager.ClearConnectionSelection();
                        GUI.FocusControl(null);
                    }

                    if (current.button == (int)MouseButton.RightClick)
                        ProcessContextMenu();

                    break;

                case EventType.MouseDrag:
                    if (current.button == (int)MouseButton.ScrollWheel)
                        DragNodes(current.delta); 
                    if (current.button == (int)MouseButton.LeftClick && current.alt)
                        DragNodes(current.delta); 
                    break;
            }
        }

        private void DrawSceneObjectField()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(256));
            EditorGUIUtility.labelWidth = 96;

            m_scene = EditorGUILayout.ObjectField("Текущая сцена ", m_scene, typeof(Scene), true) as Scene;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator(); 
        }

        private void DrawPageToolbar()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(48));
            string[] pages = new string[m_scene.GetPages().Count];
            for (int i = 0; i < pages.Length; i++)
                pages[i] = $"Страница {(i + 1)}";

            int currentPage = m_scene.GetCurrentPageID();
            GUILayoutOption maxWidth = GUILayout.MaxWidth(Screen.width - 272);
            m_scene.SetCurrentPage(GUILayout.Toolbar(currentPage, pages, maxWidth));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawContent()
        {
            Rect contentRect = GUILayoutUtility.GetLastRect();
            contentRect.y += contentRect.height;
            contentRect.height = Screen.height - 56;
            GUI.BeginScrollView(contentRect, new Vector2(0, 0), contentRect);

            DrawGrid(20, Color.grey, 0.2f);
            DrawGrid(100, Color.grey, 0.4f);

            BeginWindows();
            m_nodeManager.DrawNodes();
            EndWindows();

            GUI.EndScrollView();
        }

        private void DrawLogo()
        {
            float xPosLogo = Screen.width - 80;
            float yPosLogo = Screen.height - 80;
            float xPosText = xPosLogo - 130;
            float yPosText = yPosLogo + 40;

            GUI.Label(new Rect(xPosText, yPosText, 300, 20), "CNovell - разработчик C4ke#0002");
        }

        private void DrawGrid(float spacing, Color colour, float transparency)
        {
            int widthDivs = Mathf.CeilToInt(Screen.width / spacing);
            int heightDivs = Mathf.CeilToInt(Screen.height / spacing);

            Handles.BeginGUI();
            Handles.color = new Color(colour.r, colour.g, colour.b, transparency);

            m_offset += m_drag * 0.5f;
            Vector3 newOffset = new Vector3(m_offset.x % spacing, m_offset.y % spacing, 0);

            for (int i = 0; i <= widthDivs; i++)
                Handles.DrawLine(new Vector3(spacing * i, -spacing, 0) + newOffset,
                                 new Vector3(spacing * i, Screen.height + spacing, 0f) + newOffset);

            for (int j = 0; j <= heightDivs; j++)
                Handles.DrawLine(new Vector3(-spacing, spacing * j, 0) + newOffset,
                                 new Vector3(Screen.width + spacing, spacing * j, 0f) + newOffset);

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        void DragNodes(Vector2 delta)
        {
            List<BaseNode> nodes = m_nodeManager.GetNodes();
            m_drag = delta;

            for (int i = 0; i < nodes.Count; i++)
                nodes[i].Drag(delta);
        }

        private void GetMinMaxXY(out float minX, out float maxX, out float minY, out float maxY)
        {
            List<BaseNode> nodes = m_nodeManager.GetNodes();
            minX = Mathf.Infinity;
            maxX = Mathf.NegativeInfinity;
            minY = Mathf.Infinity;
            maxY = Mathf.NegativeInfinity;

            for (int i = 0; i < nodes.Count; i++)
            {
                Rect nodeRect = nodes[i].GetNodeRect();

                if (minX > nodeRect.x)
                    minX = nodeRect.x;
                if (maxX < nodeRect.x + nodeRect.width)
                    maxX = nodeRect.x + nodeRect.width;
                if (minY > nodeRect.y)
                    minY = nodeRect.y;
                if (maxY < nodeRect.y + nodeRect.height)
                    maxY = nodeRect.y + nodeRect.height;
            }
        }
    }

#endif
}