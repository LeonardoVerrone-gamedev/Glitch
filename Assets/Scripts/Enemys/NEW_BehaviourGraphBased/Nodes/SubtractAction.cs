using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Subtract", story: "Get [value] and subtract [X]", category: "Action", id: "711d94de76a413c2b95f5354cf908f65")]
public partial class SubtractAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Value;
    [SerializeReference] public BlackboardVariable<int> X;
    protected override Status OnStart()
    {
        Value.Value -= X.Value;
        return Status.Success;
    }
}

