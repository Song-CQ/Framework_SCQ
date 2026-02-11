/****************************************************
    文件: GameUI.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameUI界面
*****************************************************/
using ConsoleE;
using FutureCore;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectApp
{
    public class GameUI : BaseUI
    {
        #region 控件常量
        //框架自动创建请勿在此处修改内容
        private const string ui_BeesTrf_Key = "ui_BeesTrf";
        private const string ui_BetterBeesTrf_Key = "ui_BetterBeesTrf";
        private const string ui_currentScore_Key = "ui_currentScore";
        private const string ui_scoreChangeText_Key = "ui_scoreChangeText";
        private const string ui_PropList_Key = "ui_PropList";
        private const string ui_TipsText_Key = "ui_TipsText";

        #endregion
        private GameUICtrl uiCtrl;
        private GameModel model;
        private UGUIEntity u_Entity;

        private UI_List ui_PropList;
        private TextMeshProUGUI ui_TipsText;
        private List<Transform> beesTars;
        private Queue<BetterBeeFlight> betterBeesFlightQueue;
        private BetterBeeFlight LastBetterBeesFlight;

        private EliminateGameCore core;

        public GameUI(GameUICtrl ctrl) : base(ctrl)
        {
            uiName = UIConst.GameUI;
            this.uiCtrl = ctrl;
        }

        protected override void SetUIInfo(UIInfo uiInfo)
        {
            uiInfo.packageName = "Game";
            uiInfo.assetName = "Game_Plane";
            uiInfo.layerType = UILayerType.Normal;
            uiInfo.isNeedOpenAnim = false;
            uiInfo.isNeedCloseAnim = false;
            uiInfo.isNeedUIMask = false;
            uiInfo.isTickUpdate = true;
        }

        #region 生命周期
        protected override void OnInit()
        {
            //model = moduleMgr.GetModel(ModelConst.GameModel) as GameModel;
        }

        protected override void OnClose()
        {
        }

        private class PropData
        {
            public ExternalProp type;
            public int Sum;

        }

        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            scoreText = GetComponent<TextMeshProUGUI>(ui_currentScore_Key);
            scoreChangeText = GetComponent<TextMeshProUGUI>(ui_scoreChangeText_Key);

            ui_PropList = GetComponent<UI_List>(ui_PropList_Key);
            ui_PropList.updateItemData = UpdataItemData;

            ui_TipsText = GetComponent<TextMeshProUGUI>(ui_TipsText_Key);
            ui_TipsText.SetActive(false);

            beesTars = new List<Transform>();
            foreach (Transform item in GetComponent<Transform>(ui_BeesTrf_Key).GetComponentsInChildren<Transform>(true))
            {
                beesTars.Add(item);
                item.SetActive(false);
            }

            betterBeesFlightQueue = new Queue<BetterBeeFlight>();
            foreach (BetterBeeFlight item in GetComponent<Transform>(ui_BetterBeesTrf_Key).GetComponentsInChildren<BetterBeeFlight>(true))
            {
                betterBeesFlightQueue.Enqueue(item);
                item.SetActive(false);
            }


            List<ItemData> datas = new List<ItemData>();
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Undo });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Vertical });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Horizontal });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Wild });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Hammer });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.AddScore });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.Swipe });
            datas.Add(new ItemData() { IntData = (int)ExternalProp.AllRandom });

            ui_PropList.SetData(datas);

            core = GameTool.GameCore;


        }


        private void UpdataItemData(BaseUIList_Item item, ItemData data)
        {
            ExternalPropUIList_Item externalPropItem = item as ExternalPropUIList_Item;
            externalPropItem.SetClickCallback(OnClickPropItem);


        }

        private void OnClickPropItem(BaseUIList_Item item, ItemData data)
        {
            ExternalProp type = (ExternalProp)data.IntData;
            core.ClickExternalPropItem(type);



        }

        protected override void OnOpenBefore(object args)
        {

        }

        protected override void OnOpen(object args)
        {
            core.AddListener(GameMsg.Player_ClickExternalPropItem, OnClickExternalPropItem);
            core.AddListener(GameMsg.UseExternalProp, OnUseExternalProp);

            UICtrlDispatcher.Instance.AddListener(GameMsg.ScoreUpdated, OnScoreUpdated);
            UICtrlDispatcher.Instance.AddListener(GameMsg.GameWin, OnGame);

        }

        private void OnGame(object obj)
        {
            ui_TipsText.text = "胜利";
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (isAnimating)
            {
                UpdateScoreAnimation();
            }
        }



        #region  分数动画

        private void OnScoreUpdated(object obj)
        {
            object[] objects = obj as object[];
            int oldScore = (int)objects[0];
            int currScore = (int)objects[1];

            StartScoreAnimation(oldScore, currScore);

            StartBetterBeesAnimation(currScore);
        }

        private void StartBetterBeesAnimation(float currScore)
        {
            int sum = (int)(beesTars.Count * (currScore / core.TargetScore));
            if (sum == betterBeesSum)
            {
                return;
            }

            Transform tartrf = beesTars[sum - 1];

            if (betterBeesFlightQueue.Count > 0)
            {
                var item = betterBeesFlightQueue.Dequeue();
                LastBetterBeesFlight = item;
                item.SetActive(true);
                item.AddCB(() =>
                {
                    RefreshBeesTars(sum);
                    betterBeesFlightQueue.Enqueue(item);         
                });

                item.FlyToHive(tartrf);
            }else
            {
                LastBetterBeesFlight.AddCB(()=>RefreshBeesTars(sum));
            }
        }

        private void RefreshBeesTars(int sum)
        {
            for (int i = 0; i < beesTars.Count; i++)
            {
                beesTars[i].SetActive(i < sum);
            }
        }

        private int betterBeesSum;

        private int currentDisplayScore;
        private int targetScore;
        private float animationProgress = 0f;
        private bool isAnimating = false;
        private float animationTime = 0.5f;
        private int animationStartValue;

        private TextMeshProUGUI scoreText;
        private TextMeshProUGUI scoreChangeText; // 用于显示分数变化
        private UnityEngine.AudioSource scoreSound; // 音效

        private void StartScoreAnimation(int startValue, int endValue)
        {
            animationStartValue = startValue;
            targetScore = endValue;
            currentDisplayScore = startValue;
            animationProgress = 0f;
            isAnimating = true;

            // 显示分数变化文字
            if (scoreChangeText != null)
            {
                int difference = endValue - startValue;
                scoreChangeText.text = (difference > 0 ? "+" : "") + difference.ToString();
                scoreChangeText.gameObject.SetActive(true);

                // 设置颜色
                scoreChangeText.color = difference > 0 ?
                    new Color(0.2f, 0.8f, 0.2f) : // 绿色表示增加
                    new Color(0.8f, 0.2f, 0.2f);  // 红色表示减少
            }

            // 播放音效
            if (scoreSound != null)
            {
                scoreSound.pitch = 1f + Mathf.Clamp(Mathf.Abs(endValue - startValue) * 0.001f, 0f, 0.5f);
                scoreSound.Play();
            }
        }



        private void UpdateScoreAnimation()
        {
            animationProgress += Time.deltaTime / animationTime;

            // 使用缓动函数使动画更自然
            float t = EaseOutCubic(Mathf.Clamp01(animationProgress));

            // 插值计算当前显示分数
            currentDisplayScore = Mathf.RoundToInt(Mathf.Lerp(animationStartValue, targetScore, t));

            // 更新UI
            if (scoreText != null)
            {
                scoreText.text = FormatNumber(currentDisplayScore);

                // 动画期间的视觉效果
                if (animationProgress < 1f)
                {
                    // 轻微的颜色变化
                    float pulse = Mathf.Sin(animationProgress * Mathf.PI * 4f) * 0.3f + 0.7f;
                    scoreText.color = new Color(pulse, pulse, 1f);

                    // 缩放效果
                    float scale = 1f + Mathf.Sin(animationProgress * Mathf.PI * 8f) * 0.05f;
                    scoreText.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }

            // 隐藏分数变化文字
            if (scoreChangeText != null && animationProgress > 0.3f)
            {
                scoreChangeText.gameObject.SetActive(false);
            }

            // 动画结束
            if (animationProgress >= 1f)
            {
                isAnimating = false;
                currentDisplayScore = targetScore;

                if (scoreText != null)
                {
                    scoreText.text = FormatNumber(targetScore);
                    scoreText.color = Color.white;
                    scoreText.transform.localScale = Vector3.one;
                }
            }
        }

        // 缓动函数 - 三次缓出
        private float EaseOutCubic(float t)
        {
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        // 格式化数字（添加千位分隔符等）
        private string FormatNumber(int number)
        {
            return number.ToString("N0");
        }

        // 您也可以添加一个立即完成动画的方法
        public void CompleteAnimationImmediately()
        {
            if (isAnimating)
            {
                isAnimating = false;
                currentDisplayScore = targetScore;

                if (scoreText != null)
                {
                    scoreText.text = FormatNumber(targetScore);
                    scoreText.color = Color.white;
                    scoreText.transform.localScale = Vector3.one;
                }

                if (scoreChangeText != null)
                {
                    scoreChangeText.gameObject.SetActive(false);
                }
            }
        }

        #endregion




        private void OnUseExternalProp(object obj)
        {
            ui_TipsText.SetActive(false);
        }

        private void OnClickExternalPropItem(object obj)
        {
            ui_TipsText.SetActive(true);


        }

        protected override void OnHide()
        {
        }

        protected override void OnDisplay(object args)
        {
        }
        #endregion

        #region 消息
        protected override void AddListener()
        {
            //modelDispatcher.AddListener(ModelMsg.XXX, OnXXX);
        }
        protected override void RemoveListener()
        {
            //modelDispatcher.RemoveListener(ModelMsg.XXX, OnXXX);
        }
        #endregion

    }
}