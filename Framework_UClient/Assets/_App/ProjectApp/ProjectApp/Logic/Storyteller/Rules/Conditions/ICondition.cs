using Storyteller.Core;

namespace Storyteller.Rules
{
    public interface ICondition
    {
        string Id { get; }
        ConditionType Type { get; }
        bool Evaluate(StoryContext context, ComicPanel panel, PanelState previousState);
        string GetDescription();
    }
    
    [System.Serializable]
    public abstract class BaseCondition : ICondition
    {
        public abstract string Id { get; }
        public abstract ConditionType Type { get; }
        public abstract bool Evaluate(StoryContext context, ComicPanel panel, PanelState previousState);
        
        public virtual string GetDescription()
        {
            return $"Condition: {Type}";
        }
        
        protected Character GetCharacter(string selector, ComicPanel panel)
        {
            return selector switch
            {
                "current" => panel.Character,
                "target" => panel.TargetCharacter,
                "any" => FindAnyCharacterInPanel(panel),
                _ => panel.Character
            };
        }
        
        private Character FindAnyCharacterInPanel(ComicPanel panel)
        {
            if (panel.Character != null) return panel.Character;
            if (panel.TargetCharacter != null) return panel.TargetCharacter;
            return null;
        }
    }
}