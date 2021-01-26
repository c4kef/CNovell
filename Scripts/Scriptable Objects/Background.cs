using UnityEngine;

namespace CNovell.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Новый фон", menuName = "CNovell/Задний фон")]
    public class Background : ScriptableObject
    {
        
        [SerializeField] private string m_name;
        [SerializeField] private Sprite m_background;
        [SerializeField] private bool m_isCG;

        #region getters

        public string GetName() => m_name; 
        public Sprite GetBackground() => m_background; 
        public bool GetIsCG() => m_isCG; 

        #endregion
    }
}