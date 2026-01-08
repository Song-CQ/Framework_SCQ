using System;
using System.Collections.Generic;
using UnityEngine;
using Storyteller.Core;
using Storyteller.Engine;

namespace Storyteller.Engine
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        private NarrativeRuleEngine _ruleEngine;
        private StoryValidator _validator;
        private StoryContext _storyContext;
        
        public List<ComicPanel> CurrentPanels { get; } = new();
        public string CurrentChapterTitle = "吸血鬼求婚";
        
        // 可用元素池
        public List<Character> AvailableCharacters = new();
        public List<Scene> AvailableScenes = new();
        public List<StoryAction> AvailableActions = new();
        
        // 事件
        public event Action<string> OnStoryEvent;
        public event Action<bool, string> OnValidationResult;
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Initialize()
        {
            _storyContext = new StoryContext();
            _ruleEngine = new NarrativeRuleEngine(_storyContext);
            _validator = new StoryValidator(_ruleEngine);
            
            CreateDemoElements();
            ResetStory();
        }
        
        void CreateDemoElements()
        {
            
            // 创建角色
            var vampire = new Character("vampire", "吸血鬼");
            vampire.AddTrait("vampire");
            vampire.AddTrait("undead");
            
            var princess = new Character("princess", "公主");
            princess.AddTrait("human");
            princess.AddTrait("royalty");
            
            AvailableCharacters.Add(vampire);
            AvailableCharacters.Add(princess);
            
            // 创建场景
            var castle = new Scene("castle", "城堡");
            castle.Properties["lighting"] = "candlelight";
            
            var garden = new Scene("garden", "花园");
            garden.Properties["lighting"] = "sunlight";
            
            AvailableScenes.Add(castle);
            AvailableScenes.Add(garden);
            
            // 创建动作
            AvailableActions.Add(new StoryAction("propose", "求婚"));
            AvailableActions.Add(new StoryAction("bite", "咬"));
            AvailableActions.Add(new StoryAction("kiss", "亲吻"));
        }
        
        public void ResetStory()
        {
            CurrentPanels.Clear();
            _storyContext = new StoryContext();
            
            // 创建初始面板
            CurrentPanels.Add(new ComicPanel(0));
            CurrentPanels.Add(new ComicPanel(1));
        }
        
        public bool AddElementToPanel(NarrativeElement element, int panelIndex)
        {
            if (panelIndex >= CurrentPanels.Count)
                CurrentPanels.Add(new ComicPanel(panelIndex));
            
            var panel = CurrentPanels[panelIndex];
            
            switch (element.Type)
            {
                case ElementType.Character:
                    if (panel.Character == null)
                    {
                        panel.Character = element as Character;
                        return true;
                    }
                    else if (panel.TargetCharacter == null)
                    {
                        panel.TargetCharacter = element as Character;
                        return true;
                    }
                    break;
                    
                case ElementType.Scene:
                    panel.Scene = element as Scene;
                    return true;
                    
                case ElementType.Action:
                    panel.Action = element as StoryAction;
                    return true;
            }
            
            return false;
        }
        
        public void RemoveElementFromPanel(ElementType type, int panelIndex)
        {
            if (panelIndex >= CurrentPanels.Count) return;
            
            var panel = CurrentPanels[panelIndex];
            switch (type)
            {
                case ElementType.Character:
                    panel.Character = null;
                    break;
                case ElementType.Scene:
                    panel.Scene = null;
                    break;
                case ElementType.Action:
                    panel.Action = null;
                    break;
            }
        }
        
        public void TestCurrentStory()
        {
            var result = _validator.ValidateStory(CurrentPanels, CurrentChapterTitle);
            
            if (result.IsValid)
            {
                Debug.Log("✓ 故事验证通过！");
                OnValidationResult?.Invoke(true, "故事验证通过！");
                
                // 播放故事事件
                foreach (var storyEvent in result.AllEvents)
                {
                    Debug.Log($"事件: {storyEvent.Type}");
                    OnStoryEvent?.Invoke(storyEvent.Type);
                }
            }
            else
            {
                Debug.LogError($"✗ 故事验证失败: {result.ErrorMessage}");
                OnValidationResult?.Invoke(false, result.ErrorMessage);
            }
        }
        
        public string GetCurrentStoryText()
        {
            var text = $"章节: {CurrentChapterTitle}\n\n";
            
            for (int i = 0; i < CurrentPanels.Count; i++)
            {
                var panel = CurrentPanels[i];
                if (panel.IsValid())
                {
                    text += $"第{i + 1}幕: ";
                    text += $"{panel.Character?.Name} ";
                    text += $"在{panel.Scene?.Name} ";
                    text += $"{panel.Action?.Name} ";
                    
                    if (panel.TargetCharacter != null)
                        text += $"{panel.TargetCharacter.Name}";
                    
                    text += "\n";
                }
            }
            
            return text;
        }
    }
}