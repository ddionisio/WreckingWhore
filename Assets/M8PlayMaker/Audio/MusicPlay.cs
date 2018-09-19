using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Audio")]
    public class MusicPlay : FsmStateAction {
        [RequiredField]
        public FsmString music;

        public FsmBool immediate;

        public FsmBool wait;

        [HutongGames.PlayMaker.Tooltip("The event to call after music ends. Make sure to set wait = true. If this is set, this action will wait until music has finished. Make sure the given music is not set to loop!")]
        public FsmEvent onFinishEvent;

        public override void Reset() {
            music = null;
            immediate.Value = true;
            wait = null;
            onFinishEvent = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            MusicManager.instance.Play(music.Value, immediate.Value);

            if(wait.Value) {
                MusicManager.instance.musicFinishCallback += OnMusicFinish;
            }
            else {
                Finish();
            }
        }

        // Code that runs when exiting the state.
        public override void OnExit() {
            if(!FsmEvent.IsNullOrEmpty(onFinishEvent)) {
                MusicManager.instance.musicFinishCallback -= OnMusicFinish;
            }
        }

        void OnMusicFinish(string name) {
            if(name == music.Value) {
                if(!FsmEvent.IsNullOrEmpty(onFinishEvent)) {
                    Fsm.Event(onFinishEvent);
                }
                else {
                    Finish();
                }
            }
        }
    }
}