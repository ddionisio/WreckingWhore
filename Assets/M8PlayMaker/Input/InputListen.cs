using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Input")]
    [HutongGames.PlayMaker.Tooltip("Wait for an input response from given player's input action.  Note: Refer to the generated InputAction.cs for action reference.")]
    public class InputListen : FsmStateAction {
        public FsmInt player;
        public FsmInt action;

        public FsmEvent onPressEvent;
        public FsmEvent onReleaseEvent;

        public override void Reset() {
            player = null;
            action = null;
            onPressEvent = null;
            onReleaseEvent = null;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            Main.instance.input.AddButtonCall(player.Value, action.Value, OnInput);
            //Finish();
        }

        public override void OnExit() {
            Main.instance.input.RemoveButtonCall(player.Value, action.Value, OnInput);
        }

        void OnInput(InputManager.Info data) {
            if(data.state == InputManager.State.Pressed) {
                Fsm.Event(onPressEvent);
            }
            else if(data.state == InputManager.State.Released) {
                Fsm.Event(onReleaseEvent);
            }
        }

        public override string ErrorCheck() {
            if(FsmEvent.IsNullOrEmpty(onPressEvent) &&
                FsmEvent.IsNullOrEmpty(onReleaseEvent))
                return "Action sends no events!";
            return "";
        }
    }
}