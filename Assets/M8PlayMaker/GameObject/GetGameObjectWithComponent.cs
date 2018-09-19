using UnityEngine;
using HutongGames.PlayMaker;

namespace M8.PlayerMaker.GameObjectLib {
    [ActionCategory("Mate GameObject")]
    [HutongGames.PlayMaker.Tooltip("Gets the game object that has the given component. If the source has the component, that is returned, otherwise will try to find the first game object within the hierarchy that has the component. If nothing is found, output will be set to none.")]
    public class GetGameObjectWithComponent : FsmStateAction {
        [RequiredField]
        public FsmOwnerDefault source;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject output;

        [UIHint(UIHint.ScriptComponent)]
        public FsmString component;

        public bool checkInactive;

        public bool everyFrame;

        public override void Reset() {
            source = null;
            component = null;
            checkInactive = false;
            everyFrame = false;
        }

        // Code that runs on entering the state.
        public override void OnEnter() {
            DoIt();

            if(!everyFrame)
                Finish();
        }

        // Code that runs every frame.
        public override void OnUpdate() {
            DoIt();
        }

        void DoIt() {
            Component comp = null;
            GameObject sourceGO = source.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : source.GameObject.Value;
            if(sourceGO != null) {
                comp = FindCompRecursive(sourceGO);
            }

            output.Value = comp != null ? comp.gameObject : null;
        }

        Component FindCompRecursive(GameObject go) {
            Component comp = go.GetComponent(component.Value);

            if(comp == null) {
                for(int i = 0, max = go.transform.childCount; i < max; i++) {
                    GameObject childGO = go.transform.GetChild(i).gameObject;
                    if(childGO.activeSelf || checkInactive) {
                        comp = FindCompRecursive(childGO);
                        if(comp != null)
                            return comp;
                    }
                }
            }

            return comp;
        }
    }
}
