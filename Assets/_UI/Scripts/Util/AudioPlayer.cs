using System;
using JetBrains.Annotations;
using UnityEngine;
using XPlugin.Update;

namespace UI {
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour {
        
        public string saveKey = "Vol";

        public float defalutVol = 1;

        private float _vol;

        public float Vol {
            get { return this._vol; }
            set {
                this._vol = value;
                this._source.volume = value;
//                if (value == 0) {
//                    this._source.Stop();
//                }
                PlayerPrefs.SetFloat(saveKey, value);
            }
        }

        private AudioSource _source;

        public AudioSource Source => this._source;

        private void Awake() {
            this._source = GetComponent<AudioSource>();
            this._vol = PlayerPrefs.GetFloat(saveKey, defalutVol);
            this._source.volume = this._vol;
        }

        public void Play(string clipPath) {
            Play(UResources.Load<AudioClip>(clipPath));
        }

        public void Play(AudioClip clip) {
            if (clip == null){
                return;
            }
            if (this.Vol > 0) {
                if (this._source.loop) {
                    this._source.Stop();
                    this._source.clip = clip;
                    this._source.Play();
                } else {
                    this._source.PlayOneShot(clip);
                }
            }
        }
    }
}