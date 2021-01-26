using UnityEngine;
using UnityEngine.Audio;
using CNovell.SceneManagement;

namespace CNovell.Components
{
	public class AudioComponent : MonoBehaviour
	{
		[Header("Аудио микшер")]
		[Tooltip("Подключи микшер CNovell, находящийся в \"CNovell\\Audio\"")]
		[SerializeField] private AudioMixer m_audioMixer;
		
		[Header("Группы микшеров")]
		[Tooltip("Подключи группу микшеров фоновой музыки из микшера CNovell.")]
		[SerializeField] private AudioMixerGroup m_BGM;
		[Tooltip("Подключи группу микшеров окружения из микшера CNovell.")]
		[SerializeField] private AudioMixerGroup m_ambience;
		[Tooltip("Подключи группу микшеров звуковых эффектов, которая находится в микшере CNovell.")]
		[SerializeField] private AudioMixerGroup m_SFX;
		[Tooltip("Подключи группу голосового микшера, найденную в микшере CNovell")]
		[SerializeField] private AudioMixerGroup m_voice;

		private AudioManager m_audioManager;

		#region getters

		public AudioMixer GetAudioMixer() => m_audioMixer;
		public AudioMixerGroup GetBGM() => m_BGM; 
		public AudioMixerGroup GetAmbience() => m_ambience; 
		public AudioMixerGroup GetSFX() => m_SFX; 
		public AudioMixerGroup GetVoice() => m_voice; 

		public AudioManager GetAudioManager()
		{
			if (m_audioManager == null)
				m_audioManager = new AudioManager(this);

			return m_audioManager;
		}

		#endregion
	}
}