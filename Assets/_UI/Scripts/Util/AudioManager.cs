using System;
using NaughtyAttributes;
using Random = UnityEngine.Random;

namespace UI {
    public class AudioManager : Singleton<AudioManager> {
        public AudioPlayer MusicPlayer;
        public AudioPlayer SfxPlayer;
        public AudioPlayer VoicePlayer;

        [ReorderableList]
        public string[] MusicList;
        
        
        public void PlayRandomMusic() {
            var randomPath = this.MusicList[Random.Range(0, this.MusicList.Length)];
            this.MusicPlayer.Play(randomPath);
        }
        private  bool musicIsOn = false;
        public  void VideoPlayStart() {
            if (this.MusicPlayer.Vol < 1) {
                musicIsOn = false;
                return;
            }
            musicIsOn = true;
            this.MusicPlayer.Vol = 0;
        }

        public  void VideoPlayCancel() {
            if (musicIsOn) {
                this.MusicPlayer.Vol = 1;
                PlayRandomMusic();
            }
        }
    }
}