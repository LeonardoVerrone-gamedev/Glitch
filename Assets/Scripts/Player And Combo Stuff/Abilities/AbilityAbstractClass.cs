using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public abstract void Activate();
    public abstract void Deactivate();
    public abstract float GetManaCost();
    public abstract bool isActive();
}
