using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/AnimatedSpriteMaterialOverride")]
public class tk2dAnimatedSpriteMaterialOverride : tk2dAnimatedSprite {
    public Material material;

    private Material mMaterialInst;

    protected override void UpdateMaterial() {
        if(material == null)
            base.UpdateMaterial();
        else {
            if(mMaterialInst == null) {
                mMaterialInst = Object.Instantiate(material) as Material;

#if UNITY_EDITOR
                mMaterialInst.hideFlags = HideFlags.DontSave;
#endif
            }

            renderer.material = mMaterialInst;
        }
    }
}
