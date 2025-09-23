using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get TargetPosition based on Target", story: "Set [TargetPosition] to [Target] current position", category: "Action", id: "16377293cb270ed50cb017de1863fc47")]
public partial class GetTargetPositionBasedOnTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<GameObject> Self;

    [SerializeReference] public BlackboardVariable<float> PlusOffsetX;
    [SerializeReference] public BlackboardVariable<float> PlusOffsetY;

    protected override Status OnStart()
    {

        if(TargetPosition.Value.x < Self.Value.transform.position.x){
            PlusOffsetX.Value = -PlusOffsetX.Value;
        }
        if(TargetPosition.Value.y < Self.Value.transform.position.y){
            PlusOffsetY.Value = -PlusOffsetY.Value;
        }
        TargetPosition.Value = new Vector3(Target.Value.transform.position.x + PlusOffsetX.Value, Target.Value.transform.position.y + PlusOffsetY.Value, 0f);
        
        return Status.Success;
    }
}

