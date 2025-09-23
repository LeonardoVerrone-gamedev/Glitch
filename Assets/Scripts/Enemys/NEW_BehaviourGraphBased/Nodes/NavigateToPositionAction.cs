using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate To Position", story: "[Self] navigates to [TargetPosition] at [speed]", category: "Action/Navigation", id: "7c889c5f1f36d31da16e903c34a0b13c")]
public partial class NavigateToPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<float> Speed;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if(Vector3.Distance(TargetPosition.Value, Self.Value.transform.position) > 0.15f){
            Vector3 direction = (TargetPosition.Value - Self.Value.transform.position).normalized;
            Self.Value.transform.position += direction * Speed.Value * Time.deltaTime;
            return Status.Running;
        }
        return Status.Success;
    }
}

