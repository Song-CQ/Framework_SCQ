using System;
using Storyteller.Core;

namespace Storyteller.Rules.Effects
{
    [System.Serializable]
    public class ChangeCharacterStateEffect : BaseEffect
    {
        public override string Id => $"change_state_{CharacterSelector}_{NewState}";
        public override EffectType Type => EffectType.ChangeCharacterState;
        
        public string CharacterSelector = "current";
        public string NewState;
        
        public override void Apply(StoryContext context, ComicPanel panel, PanelState state)
        {
            var character = GetCharacter(CharacterSelector, panel);
            if (character == null) return;
            
            var oldState = character.CurrentState;
            character.CurrentState = NewState;
            
            state.SetVariable($"{character.Id}_state", NewState);
            
            // 记录状态变更事件
            context.AddEvent(new StoryEvent
            {
                Type = "character_state_changed",
                PanelIndex = panel.PanelIndex,
                Timestamp = DateTime.Now.Ticks,
                Data = new System.Collections.Generic.Dictionary<string, object>
                {
                    ["character_id"] = character.Id,
                    ["old_state"] = oldState,
                    ["new_state"] = NewState
                }
            });
        }
        
        private Character GetCharacter(string selector, ComicPanel panel)
        {
            return selector switch
            {
                "current" => panel.Character,
                "target" => panel.TargetCharacter,
                _ => panel.Character
            };
        }
        
        public override string GetDescription()
        {
            return $"Change {CharacterSelector} character state to {NewState}";
        }
    }
}