using System;
using Storyteller.Core;

namespace Storyteller.Rules.Conditions
{
    [System.Serializable]
    public class SceneHasPropertyCondition : BaseCondition
    {
        public override string Id => $"scene_prop_{PropertyKey}_{DesiredValue}";
        public override ConditionType Type => ConditionType.SceneHasProperty;
        
        public string PropertyKey;
        public string DesiredValue;
        
        public override bool Evaluate(StoryContext context, ComicPanel panel, PanelState previousState)
        {
            if (panel.Scene == null) return false;
            
            if (!panel.Scene.Properties.TryGetValue(PropertyKey, out var actualValue))
                return false;
            
            return actualValue == DesiredValue;
        }
        
        public override string GetDescription()
        {
            return $"Scene has property {PropertyKey} = {DesiredValue}";
        }
    }
}