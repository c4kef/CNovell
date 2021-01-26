using UnityEngine;
using UnityEngine.UI;
using CNovell.SceneManagement;

namespace CNovell.Components
{
	public class BackgroundComponent : MonoBehaviour
	{
		
		[Header("Фоновые изображения")]
		[Tooltip("Подключи сюда изображение, которое будет отображать фоновый спрайт")]
		[SerializeField] private Image m_imageBackground;
		[Tooltip("Подключи изображение, которое будет действовать как затухающий цвет.")]
		[SerializeField] private Image m_colourBackground;

		private BackgroundManager m_backgroundManager;

		#region getters

		public Image GetImageBackground() => m_imageBackground; 
		public Image GetColourBackground() => m_colourBackground; 

		public BackgroundManager GetBackgroundManager()
		{
			if (m_backgroundManager == null)
				m_backgroundManager = new BackgroundManager(this);

			return m_backgroundManager;
		}

		#endregion
	}
}