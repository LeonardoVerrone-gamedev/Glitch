using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait Target", story: "Checks if [Target] is closest than [DistanceOffset] to [Self]", category: "Action", id: "3c2820fb7b207878d7f5bd7d67e143cb")]
public partial class WaitTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> DistanceOffset;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<bool> isPlayerNear;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        isPlayerNear.Value = false;
        if(Vector2.Distance(Target.Value.transform.position, Self.Value.transform.position) <= DistanceOffset.Value){
            isPlayerNear.Value = true;
            return Status.Success;
        }
        return Status.Running;
    }
}

