using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using CNovell.Components;
using CNovell.ScriptableObjects;
using CNovell.Nodes;

namespace CNovell.SceneManagement
{
    public class SaveManager
    {
        private SceneManager m_sceneManager;

        private readonly GameObject m_saveSlotPrefab;
        private readonly GameObject m_savePanel;
        private readonly RectTransform m_saveContent;

        private GameData m_gameData;
        private SaveData m_currentData;

        private List<GameObject> m_saveSlots = new List<GameObject>();

        public SaveManager(SceneManager sceneManager, SaveComponent saveComponent)
        {
            m_currentData = new SaveData();
            m_sceneManager = sceneManager;
            m_saveSlotPrefab = saveComponent.GetSaveSlotPrefab();
            m_savePanel = saveComponent.GetSavePanel();
            m_saveContent = saveComponent.GetSaveContent();

            int numberOfSaves = saveComponent.GetNumberOfSaves();

            m_gameData = LoadGameData();

            if (m_gameData == null)
                m_gameData = new GameData(numberOfSaves);

            InitSaveLoadSlots(numberOfSaves);
        }

        public void InitSaveLoadSlots(int numberOfSaves)
        {
            for (int i = 0; i < numberOfSaves; i++)
            {
                GameObject saveSlotObject = UnityEngine.Object.Instantiate(m_saveSlotPrefab, m_saveContent);
                m_saveSlots.Add(saveSlotObject);
            }

            UpdateSaveLoadSlots();
        }

        public void UpdateSaveLoadSlots(bool isLoad = false)
        {
            List<SaveData> saveDatas = m_gameData.GetSaves();

            for (int i = 0; i < m_saveSlots.Count; i++)
            {
                GameObject saveSlotObject = m_saveSlots[i];
                SaveData saveData = saveDatas[i];
                SaveSlot saveSlot = saveSlotObject.GetComponent<SaveSlot>();

                Text scene = saveSlot.GetSlotScene();
                scene.text = $"Слот {i + 1} - ";

                if (saveData != null)
                {
                    scene.text += saveData.GetNameScene();
                    Text dateTime = saveSlot.GetDate();
                    dateTime.text = saveData.GetDateTime();
                }

                Button saveButton = saveSlotObject.GetComponent<Button>();
                int saveIndex = i;
                saveButton.onClick.RemoveAllListeners();
                saveButton.onClick.AddListener(() => m_savePanel.SetActive(false));

                if (isLoad)
                {
                    saveButton.onClick.AddListener(() => Load(saveIndex));

                    if (saveData == null)
                        saveButton.interactable = false;
                }
                else
                    saveButton.onClick.AddListener(() => Save(saveIndex));
            }
        }

        public void Save(int saveIndex)
        {
            List<SaveCharacterInfo> characters = m_sceneManager.GetCharacterManager().GetCharactersInfo();

            m_currentData.SetCharacters(characters);
            m_currentData.SetPageIndex(m_sceneManager.GetCurrentScene().GetPages().FindIndex(page => page == m_sceneManager.GetCurrentScene().GetCurrentPage()));
            m_currentData.SetNodeIndex(m_sceneManager.GetCurrentScene().GetPages()[m_currentData.GetPageIndex()].GetNodes().FindIndex(node => node == m_sceneManager.GetCurrentNode()));
            m_currentData.SetNameOfBackground(m_sceneManager.GetCurrentBackground().GetName());
            m_currentData.SetNameScene(m_sceneManager.GetCurrentScene().name);

            DateTime now = DateTime.Now;
            string dateTime = now.ToShortDateString() + "   " + now.ToShortTimeString();
            m_currentData.SetDateTime(dateTime);

            m_gameData.SetSave(m_currentData, saveIndex);
            m_currentData = new SaveData();

            File.WriteAllText($@"{Application.persistentDataPath}\cnovell.c4ke", JsonUtility.ToJson(m_gameData));
        }

        public void Load(int saveIndex)
        {
            GameData gameData = JsonUtility.FromJson<GameData>(File.ReadAllText($@"{Application.persistentDataPath}\cnovell.c4ke"));
            SaveData saveData = gameData.GetSaves()[saveIndex];
            for (int page = 0; page <= saveData.GetPageIndex(); page++)
            {
                m_sceneManager.NewPage(page);
                foreach (var character in m_sceneManager.GetCurrentScene().GetPages()[page].GetNodesCharacter())
                    if (saveData.GetCharacters().Find(charFind => charFind.m_nameCharacter == character.GetCharacter().GetName()) != null)
                        m_sceneManager.LoadNode(character);

                BackgroundNode node = m_sceneManager.GetCurrentScene().GetPages()[page].GetNodesBackground().Find(bg => bg.GetBackground().GetName() == saveData.GetNameOfBackground());
                if (node != null)
                    m_sceneManager.LoadNode(node);
            }

            m_sceneManager.JumpToNode(m_sceneManager.GetCurrentScene().GetPages()[saveData.GetPageIndex()].GetNodes()[saveData.GetNodeIndex()]);
            m_sceneManager.GetCharacterManager().SetCharactersInfos(saveData.GetCharacters());
        }

        private void CreateGameData() => File.WriteAllText($@"{Application.persistentDataPath}\cnovell.c4ke", JsonUtility.ToJson(m_gameData));

        private GameData LoadGameData()
        {
            if (!File.Exists($@"{Application.persistentDataPath}\cnovell.c4ke"))
                CreateGameData();

            return JsonUtility.FromJson<GameData>(File.ReadAllText($@"{Application.persistentDataPath}\cnovell.c4ke"));
        }
    }
}