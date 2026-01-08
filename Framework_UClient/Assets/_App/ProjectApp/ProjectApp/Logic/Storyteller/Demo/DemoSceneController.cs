using UnityEngine;
using UnityEngine.UI;
using Storyteller.Core;
using Storyteller.Engine;

namespace Storyteller.Demo
{
    public class DemoSceneController : MonoBehaviour
    {
        [Header("UI References")]
        public Text storyText;
        public Text resultText;
        public Button testButton;
        public Button resetButton;
        
        [Header("Element Buttons")]
        public Button vampireButton;
        public Button princessButton;
        public Button castleButton;
        public Button gardenButton;
        public Button proposeButton;
        public Button biteButton;
        public Button kissButton;
        
        [Header("Panel Selection")]
        public Toggle panel1Toggle;
        public Toggle panel2Toggle;
        
        private int currentPanelIndex = 0;
        
        void Start()
        {
            // 初始化UI
            UpdateStoryText();
            resultText.text = "点击元素添加到故事中，然后测试";
            
            // 按钮事件
            testButton.onClick.AddListener(TestStory);
            resetButton.onClick.AddListener(ResetStory);
            
            // 面板选择
            panel1Toggle.onValueChanged.AddListener((isOn) => { if (isOn) currentPanelIndex = 0; });
            panel2Toggle.onValueChanged.AddListener((isOn) => { if (isOn) currentPanelIndex = 1; });
            
            // 元素按钮
            vampireButton.onClick.AddListener(() => AddCharacter("vampire"));
            princessButton.onClick.AddListener(() => AddCharacter("princess"));
            castleButton.onClick.AddListener(() => AddScene("castle"));
            gardenButton.onClick.AddListener(() => AddScene("garden"));
            proposeButton.onClick.AddListener(() => AddAction("propose"));
            biteButton.onClick.AddListener(() => AddAction("bite"));
            kissButton.onClick.AddListener(() => AddAction("kiss"));
            
            // 订阅游戏事件
            GameManager.Instance.OnStoryEvent += OnStoryEvent;
            GameManager.Instance.OnValidationResult += OnValidationResult;
        }
        
        void AddCharacter(string characterId)
        {
            var character = GameManager.Instance.AvailableCharacters
                .Find(c => c.Id == characterId);
            
            if (character != null)
            {
                GameManager.Instance.AddElementToPanel(character, currentPanelIndex);
                UpdateStoryText();
            }
        }
        
        void AddScene(string sceneId)
        {
            var scene = GameManager.Instance.AvailableScenes
                .Find(s => s.Id == sceneId);
            
            if (scene != null)
            {
                GameManager.Instance.AddElementToPanel(scene, currentPanelIndex);
                UpdateStoryText();
            }
        }
        
        void AddAction(string actionId)
        {
            var action = GameManager.Instance.AvailableActions
                .Find(a => a.Id == actionId);
            
            if (action != null)
            {
                GameManager.Instance.AddElementToPanel(action, currentPanelIndex);
                UpdateStoryText();
            }
        }
        
        void TestStory()
        {
            GameManager.Instance.TestCurrentStory();
        }
        
        void ResetStory()
        {
            GameManager.Instance.ResetStory();
            UpdateStoryText();
            resultText.text = "故事已重置";
        }
        
        void UpdateStoryText()
        {
            storyText.text = GameManager.Instance.GetCurrentStoryText();
        }
        
        void OnStoryEvent(string eventType)
        {
            resultText.text += $"\n触发事件: {eventType}";
        }
        
        void OnValidationResult(bool success, string message)
        {
            if (success)
            {
                resultText.text = $"<color=green>✓ {message}</color>";
            }
            else
            {
                resultText.text = $"<color=red>✗ {message}</color>";
            }
        }
        
        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStoryEvent -= OnStoryEvent;
                GameManager.Instance.OnValidationResult -= OnValidationResult;
            }
        }
    }
}