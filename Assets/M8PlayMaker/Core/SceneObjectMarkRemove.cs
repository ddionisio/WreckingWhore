using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayMaker {
    [ActionCategory("Mate Scene")]
    [HutongGames.PlayMaker.Tooltip("This is for use with SceneSerializer. Mark object as removed, ensuring that it will not be on the scene the next time it is loaded.")]
    public class SceneObjectMarkRemove : FSMActionComponentBase<SceneSerializer> {

        public override void OnEnter() {
            base.OnEnter();

            mComp.MarkRemove();

            Finish();
        }
    }
}
