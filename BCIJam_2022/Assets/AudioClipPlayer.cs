using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour {
	[SerializeField] AudioSource o_AudioSource;

	[SerializeField] ClipAndVolume[] audioClips;

	public void PlayAudioClip(int clipID) {
		o_AudioSource.PlayOneShot(audioClips[clipID].clip, audioClips[clipID].volume);
	}

	[System.Serializable]
	struct ClipAndVolume {
		public AudioClip clip;
		public float volume;
	}
}
