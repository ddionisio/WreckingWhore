using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public const int playerIndex = 0;

    private Player mPlayer;
    private PlatformerController mPlatformer;
    private PlatformerSpriteController mPlatformerSpriteCtrl;
    
    private bool mInputEnabled;

    public Player player { get { return mPlayer; } }
    public PlatformerController platformer { get { return mPlatformer; } }
    public PlatformerSpriteController platformerSpriteControl { get { return mPlatformerSpriteCtrl; } }
    
    public bool inputEnabled {
        get { return mInputEnabled; }

        set {
            if(mInputEnabled != value) {
                InputManager input = Main.instance.input;

                if(input) {
                    mInputEnabled = value;

                    if(mInputEnabled) {
                        input.AddButtonCall(playerIndex, InputAction.Attack1, OnInputAttackPrimary);
                        input.AddButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
                        input.AddButtonCall(playerIndex, InputAction.Special, OnInputSpecial);
                    }
                    else {
                        input.RemoveButtonCall(playerIndex, InputAction.Attack1, OnInputAttackPrimary);
                        input.RemoveButtonCall(playerIndex, InputAction.Attack2, OnInputAttackSecondary);
                        input.RemoveButtonCall(playerIndex, InputAction.Special, OnInputSpecial);
                    }

                    mPlatformer.inputEnabled = mInputEnabled;
                }
            }
        }
    }

    void OnDestroy() {
        inputEnabled = false;
    }

    void Awake() {
        mPlayer = GetComponent<Player>();

        mPlatformer = GetComponent<PlatformerController>();
        mPlatformer.player = playerIndex;
        mPlatformer.moveInputX = InputAction.Horizontal;
        mPlatformer.moveInputY = InputAction.Vertical;
        mPlatformer.jumpInput = InputAction.Jump;

        mPlatformerSpriteCtrl = GetComponent<PlatformerSpriteController>();
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    void OnInputAttackPrimary(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
        }
    }

    void OnInputAttackSecondary(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
        }
    }

    void OnInputSpecial(InputManager.Info dat) {
        if(dat.state == InputManager.State.Pressed) {
        }
    }
}
