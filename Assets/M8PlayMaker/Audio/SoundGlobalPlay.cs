using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Audio")]
    public class SoundGlobalPlay : FsmStateAction {
        [RequiredField]
        public FsmString sound;

        public FsmBool wait;

        [HutongGames.PlayMaker.Tooltip("If set, wait for sound to end before finishing, then enter event. Make sure to set wait to true")]
        public FsmEvent onEndEvent;

        public override void Reset() {
            sound = null;
            wait = null;
            onEndEvent = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            if(wait.Value)
                SoundPlayerGlobal.instance.Play(sound.Value, OnSoundEnd);
            else {
                SoundPlayerGlobal.instance.Play(sound.Value);

                Finish();
            }
        }

        // Code that runs when exiting the state.
        public override void OnExit() {

        }

        void OnSoundEnd(object param) {
            if(!FsmEvent.IsNullOrEmpty(onEndEvent))
                Fsm.Event(onEndEvent);
            else
                Finish();
        }
    }
}
