using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CallScratchAndStretchAnimation", story: "[Agent] [squashes]", category: "Action", id: "11b1c0d03ae1b23217a2c0aa99cfa9dd")]
public partial class CallScratchAndStretchAnimationAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> Agent;
    [SerializeReference] public BlackboardVariable<ScratchAndStretch> Squashes;
    [SerializeReference] public BlackboardVariable<String> actionName;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Squashes.Value.PlayStretchAnimation(actionName);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

