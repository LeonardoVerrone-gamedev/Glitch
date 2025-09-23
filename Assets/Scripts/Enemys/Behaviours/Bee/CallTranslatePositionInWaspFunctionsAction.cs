using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Call TranslatePosition in WaspFunctions", story: "Call translatePosition in [WaspFunctions]", category: "Action", id: "4588abf01c7c8982ed5699e94910275f")]
public partial class CallTranslatePositionInWaspFunctionsAction : Action
{
    [SerializeReference] public BlackboardVariable<BeeFunctions> WaspFunctions;

    protected override Status OnStart()
    {
        WaspFunctions.Value.TranslateCurrentPosition();
        return Status.Success;
    }
}

