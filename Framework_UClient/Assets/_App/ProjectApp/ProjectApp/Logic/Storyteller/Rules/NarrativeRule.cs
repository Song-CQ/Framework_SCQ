using System;
using System.Collections.Generic;
using Storyteller.Core;

namespace Storyteller.Rules
{
    [System.Serializable]
    public class NarrativeRule
    {
        public string Id;
        public string Description;
        public int Priority = 0;
        public List<string> Tags = new();
        public bool IsEnabled = true;
        
        // 序列化时需要特殊处理接口
        [UnityEngine.SerializeReference]
        public List<ICondition> Conditions = new();
        
        [UnityEngine.SerializeReference]
        public List<IEffect> Effects = new();
        
        public bool IsRelevant(StoryContext context)
        {
            foreach (var tag in Tags)
            {
                if (context.ActiveTags.Contains(tag))
                    return true;
            }
            return false;
        }
        
        public override string ToString()
        {
            return $"{Id}: {Description} (Priority: {Priority})";
        }
    }
    
    public enum ConditionType { CharacterHasTrait, SceneHasProperty, And, Or, Not }
    public enum EffectType { ChangeCharacterState, TriggerStoryEvent }
}