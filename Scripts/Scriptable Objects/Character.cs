using System.Collections.Generic;
using UnityEngine;

namespace CNovell.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Новенький персонаж", menuName = "CNovell/Персонаж")]
    [System.Serializable]
    public class Character : ScriptableObject
    {
        
        [SerializeField] private string m_name;
        [SerializeField] private List<Sprite> m_sprites = new List<Sprite>();
        [SerializeField] private List<AudioClip> m_audioClips = new List<AudioClip>();

        #region getters

        public string GetName() => m_name; 
        public List<Sprite> GetSprites() => m_sprites; 
        public List<AudioClip> GetAudioClips() => m_audioClips; 

        #endregion

        #region setters

        public void SetName(string name) => m_name = name;
        public void SetSprites(List<Sprite> sprites) => m_sprites = sprites;
        public void SetAudioClips(List<AudioClip> audios) => m_audioClips = audios;

        #endregion
    }
}