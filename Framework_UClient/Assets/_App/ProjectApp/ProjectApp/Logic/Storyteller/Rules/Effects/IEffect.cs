using Storyteller.Core;

namespace Storyteller.Rules
{
    public interface IEffect
    {
        string Id { get; }
        EffectType Type { get; }
        void Apply(StoryContext context, ComicPanel panel, PanelState state);
        string GetDescription();
    }
    
    [System.Serializable]
    public abstract class BaseEffect : IEffect
    {
        public abstract string Id { get; }
        public abstract EffectType Type { get; }
        public abstract void Apply(StoryContext context, ComicPanel panel, PanelState state);
        
        public virtual string GetDescription()
        {
            return $"Effect: {Type}";
        }
    }
}