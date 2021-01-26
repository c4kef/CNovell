using UnityEngine;
using UnityEngine.UI;

namespace CNovell.Components
{
    public class SaveSlot : MonoBehaviour
    {
        [SerializeField] private Text m_scene;
        [SerializeField] private Text m_date;

        #region getters

        public Text GetSlotScene() => m_scene; 
        public Text GetDate() => m_date; 

        #endregion
    }
}