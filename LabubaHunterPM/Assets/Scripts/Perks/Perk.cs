using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perk : ScriptableObject
{
    public virtual string Type { get; }
    public virtual string Name { get => Type; }
    public Sprite Icon;
}
