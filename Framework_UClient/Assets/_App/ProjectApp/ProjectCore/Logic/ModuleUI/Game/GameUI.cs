/****************************************************
    文件: GameUI.cs
    作者: Clear
    日期: 2023/11/23 20:49:21
    类型: MVC_AutoCread
    功能: GameUI界面
*****************************************************/
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
        private const string ui_SoceMask_Key = "ui_SoceMask";
        private const string ui_currentScore_Key = "ui_currentScore";
        private const string ui_scoreChangeText_Key = "ui_scoreChangeText";
        private const string ui_TaskMask_Key = "ui_TaskMask";
        private const string ui_propIcon_Key = "ui_propIcon";
        private const string ui_ProgressText_Key = "ui_ProgressText";
        private const string ui_tastText_Key = "ui_tastText";
        private const string ui_PropList_Key = "ui_PropList";
        private const string ui_TipsText_Key = "ui_TipsText";

        #endregion
        private GameUICtrl uiCtrl;
        private GameModel model;
        private UGUIEntity u_Entity;

        private UI_List ui_PropList;
        private TextMeshProUGUI ui_TipsText;
        
        private RectTransform ui_SoceMask;


        private List<Transform> beesTars;
        private Queue<BetterBeeFlight> betterBeesFlightQueue;
        private BetterBeeFlight LastBetterBeesFlight;

        private EliminateGameCore core;
        private List<ItemData> propDataList = new List<ItemData>();


        private ExternalProp_PlayerData externalProp_PlayerData;



        private RectTransform ui_TaskMask;
        private TextMeshProUGUI ui_TastText;
        private TextMeshProUGUI ui_ProgressText;
        private Image ui_propIconImg;

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





        protected override void OnBind()
        {
            u_Entity = uiEntity as UGUIEntity;

            scoreText = GetComponent<TextMeshProUGUI>(ui_currentScore_Key);
            scoreChangeText = GetComponent<TextMeshProUGUI>(ui_scoreChangeText_Key);

            ui_PropList = GetComponent<UI_List>(ui_PropList_Key);
            ui_PropList.updateItemData = UpdataItemData;

            ui_TipsText = GetComponent<TextMeshProUGUI>(ui_TipsText_Key);
            ui_TipsText.transform.parent.SetActive(false);

            ui_SoceMask = GetComponent<RectTransform>(ui_SoceMask_Key);
            
            ui_TaskMask = GetComponent<RectTransform>(ui_TaskMask_Key);
            ui_TastText = GetComponent<TextMeshProUGUI>(ui_tastText_Key);
            ui_propIconImg = GetComponent<Image>(ui_propIcon_Key);


            beesTars = new List<Transform>();
            Transform trf = GetComponent<Transform>(ui_BeesTrf_Key);
            foreach (Transform item in trf.GetComponentsInChildren<Transform>(true))
            {
                if (trf == item) continue;

                beesTars.Add(item);
                item.SetActive(false);
            }

            int cocr = 0;
            UIEventListener uIEvent = UIEventListener.GetEventListener(trf);
            uIEvent.PointerClick_Event += (e) =>
            {
                cocr += 1000;
                StartBetterBeesAnimation(cocr);
            };


            betterBeesFlightQueue = new Queue<BetterBeeFlight>();
            foreach (BetterBeeFlight item in GetComponent<Transform>(ui_BetterBeesTrf_Key).GetComponentsInChildren<BetterBeeFlight>(true))
            {
                betterBeesFlightQueue.Enqueue(item);
                item.SetActive(false);
            }




            core = GameTool.GameCore;

            externalProp_PlayerData = PlayerDataMgr.Instance.GetData<ExternalProp_PlayerData>();

            foreach (var item in externalProp_PlayerData.allExternalProp)
            {
                propDataList.Add(new PropData() { type = item.Key, Sum = item.Value });
            }

            ui_PropList.SetData(propDataList);

        }


        private void UpdataItemData(BaseUIList_Item item, ItemData data)
        {
            ExternalPropUIList_Item externalPropItem = item as ExternalPropUIList_Item;
            externalPropItem.SetClickCallback(OnClickPropItem);


        }

        private void OnClickPropItem(BaseUIList_Item item, ItemData data)
        {
            PropData propData = (data as PropData);
            core.ClickExternalPropItem(item as ExternalPropUIList_Item);



        }

        protected override void OnOpenBefore(object args)
        {
            core.AddListener(GameMsg.CostExternalProp, OnConsumeExternalProp);
            core.AddListener(GameMsg.GameStart, RestUI);

            UICtrlDispatcher.Instance.AddListener(GameMsg.ScoreUpdated, OnScoreUpdated);
            UICtrlDispatcher.Instance.AddListener(GameMsg.GameWin, OnGameWin);
        }



        protected override void OnClose()
        {
            core.RemoveListener(GameMsg.CostExternalProp, OnConsumeExternalProp);
            core.RemoveListener(GameMsg.GameStart, RestUI);

            UICtrlDispatcher.Instance.RemoveListener(GameMsg.ScoreUpdated, OnScoreUpdated);
            UICtrlDispatcher.Instance.RemoveListener(GameMsg.GameWin, OnGameWin);


        }

        private void OnConsumeExternalProp(object obj)
        {
            object[] objects = obj as object[];
            ExternalProp propType = (ExternalProp)objects[0];
            List<Vector2Int> list = objects[1] as List<Vector2Int>;



            foreach (PropData item in propDataList)
            {
                if (item.type == propType)
                {
                    if (!externalProp_PlayerData.allExternalProp.ContainsKey(propType))
                    {
                        externalProp_PlayerData.allExternalProp[propType] = 0;
                    }
                    item.Sum = externalProp_PlayerData.allExternalProp[propType];
                }
            }

            RefershPropList();


        }

        private void RefershPropList()
        {
            ui_PropList.RefreshCurrentShowItems();

        }

        protected override void OnOpen(object args)
        {
            RestUI();

        }

        private void RestUI(object args = null)
        {
            scoreText.text = "0";
            scoreChangeText.text = "";

            ui_SoceMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GetMaskHeight(0));
            ui_TaskMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GetMaskHeight(0));

        }

        private void OnGameWin(object obj)
        {
            ui_TipsText.text = "胜利";
            //ui_TipsText.transform.parent.SetActive(true);
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


        }

        private void StartBetterBeesAnimation(float currScore)
        {
            int sum = (int)(beesTars.Count * (currScore / core.TargetScore));
            if (sum == betterBeesSum)
            {
                return;
            }
            sum = sum > beesTars.Count ? beesTars.Count : sum;

            Transform tartrf = beesTars[sum - 1];

            if (betterBeesFlightQueue.Count > 0)
            {
                var item = betterBeesFlightQueue.Dequeue();
                LastBetterBeesFlight = item;
                item.SetActive(true);
                item.AddCB(() =>
                {
                    RefreshBeesTars(sum);
                    item.SetActive(false);
                    betterBeesFlightQueue.Enqueue(item);
                });

                item.FlyToHive(tartrf);
            }
            else
            {
                LastBetterBeesFlight.AddCB(() => RefreshBeesTars(sum));
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
        private float animationTime = 0.8f;
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

            StartBetterBeesAnimation(targetScore);

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
                    // scoreText.color = new Color(pulse, pulse, 1f);

                    // 缩放效果
                    float scale = 1f + Mathf.Sin(animationProgress * Mathf.PI * 8f) * 0.05f;
                    scoreText.transform.localScale = new Vector3(scale, scale, 1f);
                }
            }
            if (ui_TaskMask)
            {
                ui_TaskMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, GetMaskHeight(currentDisplayScore * 1f / core.TargetScore));
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

        #region 任务

        private void UpdataTask()
        {
            ui_TastText.text = model.currQuest.description;
            ui_ProgressText.text = model.currQuest.goals[0].GetProgressText();
            ui_propIconImg.sprite = GameTool.GetSprite(ExternalProp.Horizontal);
        }

        private int GetMaskHeight(float val)
        {
            return (int)(140 / val);
        }

        #endregion

    }
}