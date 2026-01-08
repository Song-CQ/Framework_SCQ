using System;
using System.Collections.Generic;

namespace Storyteller.Core
{
    [System.Serializable]
    public class StoryContext
    {
        public string ChapterTitle;
        public List<StoryEvent> EventHistory = new();
        public HashSet<string> ActiveTags = new();
        public float CurrentTime;
        
        public Dictionary<string, List<string>> Relationships = new();
        
        public void AddEvent(StoryEvent storyEvent)
        {
            EventHistory.Add(storyEvent);
            CurrentTime = DateTime.Now.Ticks;
        }
        
        public void AddTag(string tag)
        {
            ActiveTags.Add(tag);
        }
        
        public void RemoveTag(string tag)
        {
            ActiveTags.Remove(tag);
        }
        
        public bool HasEvent(string eventType)
        {
            return EventHistory.Exists(e => e.Type == eventType);
        }
    }
    
    [System.Serializable]
    public class StoryEvent
    {
        public string Type;
        public int PanelIndex;
        public float Timestamp;
        public Dictionary<string, object> Data = new();
        
        public override string ToString()
        {
            return $"[{PanelIndex}] {Type} - {Timestamp}";
        }
    }
}