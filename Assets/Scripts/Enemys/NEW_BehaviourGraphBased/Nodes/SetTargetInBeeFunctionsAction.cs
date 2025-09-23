using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Set Target in BeeFunctions", story: "Set [Target] in [BeeFunctios]", category: "Action", id: "05896cf0ccf457fb44fa3612d4281bf1")]
public partial class SetTargetInBeeFunctionsAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<BeeFunctions> BeeFunctios;

    protected override Status OnStart()
    {
        BeeFunctios.Value.SetPlayer(Target.Value);
        return Status.Success;
    }
}

