using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check Distance to Target", story: "Checks the Distance to [Target] and assign", category: "Action", id: "cb30047851cc61ad7ccd820758d3df7e")]
public partial class CheckDistanceToTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<float> distanceToTarget;
    [SerializeReference] public BlackboardVariable<float> minDistanceToProced;
    [SerializeReference] public BlackboardVariable<bool> bothAxes;
    [SerializeReference] public BlackboardVariable<string> AxisToCheck;

    protected override Status OnUpdate()
    {
        if (bothAxes.Value == true)
        {
            distanceToTarget.Value = Vector2.Distance(Self.Value.transform.position, Target.Value.transform.position);
        }
        else
        {
            if (AxisToCheck.Value == "X")
            {
                distanceToTarget.Value = Mathf.Abs(Self.Value.transform.position.x - Target.Value.transform.position.x);
            }

            if (AxisToCheck.Value == "Y")
            {
                distanceToTarget.Value = Mathf.Abs(Self.Value.transform.position.y - Target.Value.transform.position.y);
            }
        }

        if (distanceToTarget.Value <= minDistanceToProced.Value)
        {
            return Status.Success;
        }
        return Status.Running;
    }
}

