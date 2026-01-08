using System;
using System.Collections.Generic;
using Storyteller.Core;

namespace Storyteller.Rules.Effects
{
    [System.Serializable]
    public class TriggerStoryEventEffect : BaseEffect
    {
        public override string Id => $"trigger_event_{EventType}";
        public override EffectType Type => EffectType.TriggerStoryEvent;
        
        public string EventType;
        public Dictionary<string, string> EventData = new();
        
        public override void Apply(StoryContext context, ComicPanel panel, PanelState state)
        {
            var storyEvent = new StoryEvent
            {
                Type = EventType,
                PanelIndex = panel.PanelIndex,
                Timestamp = DateTime.Now.Ticks,
                Data = new Dictionary<string, object>()
            };
            
            // 复制事件数据
            foreach (var kvp in EventData)
            {
                storyEvent.Data[kvp.Key] = kvp.Value;
            }
            
            // 添加动态数据
            if (panel.Character != null)
                storyEvent.Data["character_id"] = panel.Character.Id;
            
            if (panel.Scene != null)
                storyEvent.Data["scene_id"] = panel.Scene.Id;
            
            // 添加到上下文和面板状态
            context.AddEvent(storyEvent);
            state.Events.Add(storyEvent);
            
            // Unity中触发事件
            UnityEngine.Debug.Log($"Story Event: {EventType} at panel {panel.PanelIndex}");
        }
        
        public override string GetDescription()
        {
            return $"Trigger event: {EventType}";
        }
    }
}