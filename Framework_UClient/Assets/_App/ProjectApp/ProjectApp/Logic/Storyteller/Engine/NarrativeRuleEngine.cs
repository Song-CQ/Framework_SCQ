using System;
using System.Collections.Generic;
using System.Linq;
using Storyteller.Core;
using Storyteller.Rules;

namespace Storyteller.Engine
{
    public class RuleEvaluationResult
    {
        public bool IsValid = true;
        public string ErrorMessage;
        public List<string> TriggeredRules = new();
        public List<StoryEvent> GeneratedEvents = new();
        public List<IEffect> ExecutedEffects = new();
        public Exception Exception;
    }
    
    public class NarrativeRuleEngine
    {
        private List<NarrativeRule> _rules = new();
        private StoryContext _context;
        
        public NarrativeRuleEngine(StoryContext context)
        {
            _context = context;
            LoadDefaultRules();
        }
        
        private void LoadDefaultRules()
        {
            _context.AddTag("vampire");
            _context.AddTag("royalty");
            // 吸血鬼规则
            _rules.Add(new NarrativeRule
            {
                Id = "vampire_sun_death",
                Description = "吸血鬼在阳光下会死亡",
                Priority = 100,
                Tags = new List<string> { "vampire", "death" },
                Conditions = new List<ICondition>
                {
                    new Storyteller.Rules.Conditions.CharacterHasTraitCondition
                    {
                        TraitId = "vampire",
                        CharacterSelector = "current"
                    },
                    new Storyteller.Rules.Conditions.SceneHasPropertyCondition
                    {
                        PropertyKey = "lighting",
                        DesiredValue = "sunlight"
                    }
                },
                Effects = new List<IEffect>
                {
                    new Storyteller.Rules.Effects.ChangeCharacterStateEffect
                    {
                        CharacterSelector = "current",
                        NewState = "dead"
                    },
                    new Storyteller.Rules.Effects.TriggerStoryEventEffect
                    {
                        EventType = "vampire_turns_to_dust",
                        EventData = new Dictionary<string, string>
                        {
                            ["message"] = "吸血鬼在阳光下化为了灰烬！"
                        }
                    }
                }
            });
            
            // 爱情规则
            _rules.Add(new NarrativeRule
            {
                Id = "love_proposal",
                Description = "相爱的两人可以结婚",
                Priority = 50,
                Tags = new List<string> { "love", "marriage" },
                Conditions = new List<ICondition>
                {
                    new Storyteller.Rules.Conditions.CharacterHasTraitCondition
                    {
                        TraitId = "in_love",
                        CharacterSelector = "current"
                    },
                    new Storyteller.Rules.Conditions.CharacterHasTraitCondition
                    {
                        TraitId = "in_love",
                        CharacterSelector = "target"
                    }
                },
                Effects = new List<IEffect>
                {
                    new Storyteller.Rules.Effects.TriggerStoryEventEffect
                    {
                        EventType = "marriage",
                        EventData = new Dictionary<string, string>
                        {
                            ["message"] = "他们结婚了！从此幸福地生活在一起。"
                        }
                    }
                }
            });
        }
        
        public void AddRule(NarrativeRule rule)
        {
            _rules.Add(rule);
        }
        
        public RuleEvaluationResult EvaluatePanel(ComicPanel panel, PanelState previousState = null)
        {
            var result = new RuleEvaluationResult();
            
            try
            {
                // 获取适用的规则（按优先级排序）
                var applicableRules = _rules
                    .Where(r => r.IsEnabled && r.IsRelevant(_context))
                    .OrderByDescending(r => r.Priority)
                    .ToList();
                
                foreach (var rule in applicableRules)
                {
                    bool allConditionsMet = true;
                    
                    // 评估所有条件
                    foreach (var condition in rule.Conditions)
                    {
                        if (!condition.Evaluate(_context, panel, previousState))
                        {
                            allConditionsMet = false;
                            break;
                        }
                    }
                    
                    // 如果条件都满足，执行效果
                    if (allConditionsMet)
                    {
                        result.TriggeredRules.Add(rule.Id);
                        
                        foreach (var effect in rule.Effects)
                        {
                            effect.Apply(_context, panel, panel.State);
                            result.ExecutedEffects.Add(effect);
                        }
                        
                        // 添加规则生成的事件
                        result.GeneratedEvents.AddRange(panel.State.Events);
                    }
                }
                
                result.IsValid = true;
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"规则引擎错误: {ex.Message}";
                result.Exception = ex;
            }
            
            return result;
        }
        
        public List<NarrativeRule> GetAllRules()
        {
            return _rules;
        }
    }
}