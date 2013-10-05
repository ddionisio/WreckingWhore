using UnityEngine;
using System.Collections;

public class PlatformerGravityController : GravityController {

    protected override void ApplyUp() {
        if(orientUp) {
            Vector2 tup = transform.up;
            Vector2 toUp = up;
            float side = M8.MathUtil.CheckSideSign(tup, toUp);
            Vector3 angles = transform.eulerAngles;
            mRotateTo = Quaternion.Euler(angles.x, angles.y, angles.z + side * Vector2.Angle(tup, toUp));

            //transform.up = up;

            if(!mIsOrienting) {
                StartCoroutine(OrientUp());
            }
        }
    }
}
