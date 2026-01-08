using System;
using Storyteller.Core;

namespace Storyteller.Rules.Conditions
{
    [System.Serializable]
    public class CharacterHasTraitCondition : BaseCondition
    {
        public override string Id => $"char_has_trait_{TraitId}";
        public override ConditionType Type => ConditionType.CharacterHasTrait;
        
        public string TraitId;
        public string CharacterSelector = "current";
        
        public override bool Evaluate(StoryContext context, ComicPanel panel, PanelState previousState)
        {
            var character = GetCharacter(CharacterSelector, panel);
            if (character == null) return false;
            
            return character.HasTrait(TraitId) || 
                   (previousState?.ActiveTraits.Contains(TraitId) ?? false);
        }
        
        public override string GetDescription()
        {
            return $"{CharacterSelector} character has trait: {TraitId}";
        }
    }
}