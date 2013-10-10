using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

    public UISlider hp;
    public UISlider pow;

    public NGUIColorBlink hpBlinker;
    public NGUIColorBlink powBlinker;

    public Stats bossStats;
    public UISlider bossHP;

    private Player mPlayer;

    void Awake() {

    }

    // Use this for initialization
    void Start() {
        hpBlinker.enabled = false;
        powBlinker.enabled = false;

        mPlayer = Player.instance;
        mPlayer.stats.statCallback += OnStatChange;

        if(bossStats && bossHP) {
            bossStats.statCallback += OnBossStatChange;
        }
    }

    void OnBossStatChange(Stats stat, Stats.Type which, float delta) {
        if(delta != 0.0f) {
            switch(which) {
                case Stats.Type.HP:
                    bossHP.value = stat.hp / stat.maxHP;
                    break;
            }
        }
    }

    void OnStatChange(Stats stat, Stats.Type which, float delta) {
        if(delta != 0.0f) {
            switch(which) {
                case Stats.Type.HP:
                    hp.value = stat.hp / stat.maxHP;

                    hpBlinker.enabled = hp.value <= 0.3f;
                    break;

                case Stats.Type.Power:
                    pow.value = stat.power / stat.maxPower;

                    powBlinker.enabled = stat.power <= mPlayer.powerAttackConsume + mPlayer.powerAttackConsume * 0.25f;
                    break;
            }
        }
    }
}
