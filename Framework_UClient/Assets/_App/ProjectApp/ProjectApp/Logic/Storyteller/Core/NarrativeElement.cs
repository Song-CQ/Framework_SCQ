using System;
using System.Collections.Generic;

namespace Storyteller.Core
{
    public enum ElementType { Character, Scene, Action, Object, Trait }
    
    [System.Serializable]
    public class NarrativeElement
    {
        public string Id;
        public string Name;
        public ElementType Type;
        public Dictionary<string, string> Properties = new();
        
        public NarrativeElement() { }
        
        public NarrativeElement(string id, string name, ElementType type)
        {
            Id = id;
            Name = name;
            Type = type;
        }
    }
    
    [System.Serializable]
    public class Character : NarrativeElement
    {
        public List<string> Traits = new();
        public string CurrentState = "normal";
        
        public Character() : base() { }
        public Character(string id, string name) : base(id, name, ElementType.Character) { }
        
        public bool HasTrait(string traitId) => Traits.Contains(traitId);
        
        public void AddTrait(string traitId)
        {
            if (!Traits.Contains(traitId))
                Traits.Add(traitId);
        }
    }
    
    [System.Serializable]
    public class Scene : NarrativeElement
    {
        public Scene() : base() { }
        public Scene(string id, string name) : base(id, name, ElementType.Scene) { }
    }
    
    [System.Serializable]
    public class StoryAction : NarrativeElement
    {
        public StoryAction() : base() { }
        public StoryAction(string id, string name) : base(id, name, ElementType.Action) { }
    }
}