using System;
using System.Collections.Generic;

namespace Storyteller.Core
{
    [System.Serializable]
    public class ComicPanel
    {
        public int PanelIndex;
        public Character Character;
        public Scene Scene;
        public StoryAction Action;
        public Character TargetCharacter;
        public NarrativeElement Object;
        
        [NonSerialized] public PanelState State = new();
        
        public ComicPanel() { }
        
        public ComicPanel(int index)
        {
            PanelIndex = index;
        }
        
        public bool IsValid()
        {
            return Character != null && Action != null;
        }
        
        public override string ToString()
        {
            return $"Panel {PanelIndex}: {Character?.Name} {Action?.Name} {TargetCharacter?.Name} in {Scene?.Name}";
        }
    }
    
    [System.Serializable]
    public class PanelState
    {
        public Dictionary<string, object> Variables = new();
        public List<string> ActiveTraits = new();
        public List<StoryEvent> Events = new();
        
        public T GetVariable<T>(string key)
        {
            return Variables.ContainsKey(key) ? (T)Variables[key] : default;
        }
        
        public void SetVariable(string key, object value)
        {
            Variables[key] = value;
        }
    }
}