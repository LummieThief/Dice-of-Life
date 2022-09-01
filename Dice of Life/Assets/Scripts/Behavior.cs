using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior : MonoBehaviour
{
    public virtual void OnRolledUp() { }
    public virtual void OnRolledOut() { }
    public virtual void OnRolledDown() { }
}
