using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrintBehavior : Behavior
{
    public override void OnRolledUp() { Debug.Log("up"); }
    public override void OnRolledOut() { Debug.Log("out"); }
    public override void OnRolledDown() { Debug.Log("down"); }
}
