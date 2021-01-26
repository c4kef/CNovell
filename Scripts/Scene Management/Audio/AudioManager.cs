using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using CNovell.Components;

namespace CNovell.SceneManagement
{
    public class AudioManager
    {
        
        private readonly AudioMixer m_audioMixer;
        private readonly AudioMixerGroup m_BGM;
        private readonly AudioMixerGroup m_ambience;
        private readonly AudioMixerGroup m_SFX;
        private readonly AudioMixerGroup m_voice;

        private GameObject m_bgmAudio;
        private GameObject m_ambientAudio;
        private GameObject m_sfxAudio;
        private GameObject m_voiceAudio;

        #region getters

        public AudioMixer GetAudioMixer() => m_audioMixer; 

        #endregion
        
        public AudioManager(AudioComponent audioComponent)
        {
            m_audioMixer = audioComponent.GetAudioMixer();
            m_BGM = audioComponent.GetBGM();
            m_ambience = audioComponent.GetAmbience();
            m_SFX = audioComponent.GetSFX();
            m_voice = audioComponent.GetVoice();
            
            m_bgmAudio = new GameObject("BGM");
            m_ambientAudio = new GameObject("Ambience");
            m_sfxAudio = new GameObject("SFX");
            m_voiceAudio = new GameObject("Voice");
            
            AudioSource bgmSource = m_bgmAudio.AddComponent<AudioSource>();
            AudioSource sfxSource = m_sfxAudio.AddComponent<AudioSource>();
            AudioSource voiceSource = m_voiceAudio.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = m_BGM;
            sfxSource.outputAudioMixerGroup = m_SFX;
            voiceSource.outputAudioMixerGroup = m_voice;
            bgmSource.loop = true; 
        }
        
        public void SetBGM(AudioClip bgmClip, List<AudioClip> ambientClips = null)
        {
            if (m_ambientAudio.GetComponents<AudioSource>().Length > 0)
                ClearBGM();

            AudioSource bgmAudio = m_bgmAudio.GetComponent<AudioSource>();
            bgmAudio.clip = bgmClip;
            bgmAudio.Play();

            if (ambientClips != null)
            {
                for (int i = 0; i < ambientClips.Count; i++)
                {
                    AudioSource ambienceSource = m_ambientAudio.AddComponent<AudioSource>(); 
                    ambienceSource.outputAudioMixerGroup = m_ambience;
                    ambienceSource.loop = true;

                    ambienceSource.clip = ambientClips[i];
                    ambienceSource.outputAudioMixerGroup = m_ambience;
                    ambienceSource.loop = true;

                    ambienceSource.Play(); 
                }
            }
        }
        
        public void ResumeBGM()
        {
            AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
            AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

            bgmSource.UnPause();

            for (int i = 0; i < ambientSources.Length; i++)
                ambientSources[i].UnPause();
        }
        
        public void PauseBGM()
        {
            AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
            AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

            bgmSource.Pause();

            for (int i = 0; i < ambientSources.Length; i++)
                ambientSources[i].Pause();
        }
        
        private void ClearBGM()
        {
            AudioSource bgmSource = m_bgmAudio.GetComponent<AudioSource>();
            AudioSource[] ambientSources = m_ambientAudio.GetComponents<AudioSource>();

            bgmSource.Stop();
            bgmSource.clip = null; 

            for (int i = 0; i < ambientSources.Length; i++)
                Object.Destroy(ambientSources[i]); 
        }

        public IEnumerator PlaySFX(AudioClip sfxClip, bool nextNode = false, bool waitForFinish = false)
        {
            m_sfxAudio.GetComponent<AudioSource>().PlayOneShot(sfxClip); 

            SceneManager sceneManager = SceneManager.GetInstance();

            
            if (sceneManager != null && !waitForFinish && nextNode)
                sceneManager.NextNode();

            yield return new WaitForSeconds(sfxClip.length); 

            
            if (sceneManager != null && waitForFinish && nextNode)
                sceneManager.NextNode();
        }
        public void PlayVoiceClip(AudioClip voiceClip)
        {
            m_voiceAudio.GetComponent<AudioSource>().PlayOneShot(voiceClip); 
        }
    }
}