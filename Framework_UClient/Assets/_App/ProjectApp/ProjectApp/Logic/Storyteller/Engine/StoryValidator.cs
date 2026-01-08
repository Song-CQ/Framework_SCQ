using System;
using System.Collections.Generic;
using System.Linq;
using Storyteller.Core;

namespace Storyteller.Engine
{
    public class ValidationResult
    {
        public bool IsValid;
        public string ErrorMessage;
        public int ErrorPanelIndex = -1;
        public StoryContext StoryContext;
        public List<StoryEvent> AllEvents = new();
    }
    
    public class StoryValidator
    {
        private NarrativeRuleEngine _ruleEngine;
        
        public StoryValidator(NarrativeRuleEngine ruleEngine)
        {
            _ruleEngine = ruleEngine;
        }
        
        public ValidationResult ValidateStory(List<ComicPanel> panels, string chapterTitle)
        {
            var result = new ValidationResult();
            var context = new StoryContext
            {
                ChapterTitle = chapterTitle
            };
            
            PanelState previousState = null;
            
            for (int i = 0; i < panels.Count; i++)
            {
                var panel = panels[i];
                
                if (!panel.IsValid())
                {
                    result.IsValid = false;
                    result.ErrorPanelIndex = i;
                    result.ErrorMessage = $"面板 {i} 不完整";
                    return result;
                }
                
                // 评估面板
                var ruleResult = _ruleEngine.EvaluatePanel(panel, previousState);
                
                if (!ruleResult.IsValid)
                {
                    result.IsValid = false;
                    result.ErrorPanelIndex = i;
                    result.ErrorMessage = ruleResult.ErrorMessage;
                    return result;
                }
                
                // 收集事件
                result.AllEvents.AddRange(ruleResult.GeneratedEvents);
                previousState = panel.State;
            }
            
            // 检查故事是否符合标题
            if (!CheckStoryMatchesTitle(result.AllEvents, chapterTitle))
            {
                result.IsValid = false;
                result.ErrorMessage = "故事不符合题目要求";
                return result;
            }
            
            result.IsValid = true;
            result.StoryContext = context;
            return result;
        }
        
        private bool CheckStoryMatchesTitle(List<StoryEvent> events, string title)
        {
            if (title.Contains("吸血鬼"))
            {
                return events.Any(e => e.Type == "vampire_turns_to_dust");
            }
            else if (title.Contains("结婚") || title.Contains("婚礼"))
            {
                return events.Any(e => e.Type == "marriage");
            }
            
            return true; // 默认通过
        }
    }
}