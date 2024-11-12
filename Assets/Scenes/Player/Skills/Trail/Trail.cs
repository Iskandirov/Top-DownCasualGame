using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Trail : SkillBaseMono
{
    public VisualEffect vfx;
    private void Start()
    {
        player = PlayerManager.instance;
    }
    private void FixedUpdate()
    {
        vfx.SetVector3("PlayerPosition", player.objTransform.position + new Vector3(0,5f,0));
    }
}
