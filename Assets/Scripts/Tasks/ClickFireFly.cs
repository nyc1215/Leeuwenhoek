﻿using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using FairyGUI;
using Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tasks
{
    public class ClickFireFly : TaskUtil
    {
        private const int SuccessNum = 5;
        [Min(0.75f)] public float createFireFlyFreq = 4;

        private GTextField _title2;
        private GTextField _successText;
        private GGraph _successTextBg;

        private int _successNum;
        private WaitForSeconds _createFireFlyWaitForSeconds;

        private Coroutine _createFireFlyCoroutine;

        protected override void Awake()
        {
            TaskPanelName = "ClickFireFly";

            base.Awake();

            _successText = TaskUI.GetChild("Success_Text").asTextField;
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
            _title2 = TaskUI.GetChild("title2").asTextField;

            _createFireFlyWaitForSeconds = new WaitForSeconds(createFireFlyFreq);
        }

        protected override void InitTask()
        {
            base.InitTask();
            _successNum = 0;
            _successText.visible = false;
            _successTextBg.visible = false;
            _title2.SetVar("now", _successNum.ToString()).SetVar("all", SuccessNum.ToString()).FlushVars();
        }

        protected override void OpenTaskUI()
        {
            base.OpenTaskUI();
            if (_createFireFlyCoroutine != null)
            {
                StopCoroutine(_createFireFlyCoroutine);
                _createFireFlyCoroutine = null;
            }

            _createFireFlyCoroutine = StartCoroutine(CreateFireFly());
        }

        private IEnumerator CreateFireFly()
        {
            yield return null;

            while (!isSuccess && TaskWindow.isShowing)
            {
                var aInstanceOfFirefly = TaskWindow.AddChild(UIPackage.CreateObject("Game", "Com_FireFly").asCom).asCom;
                Debug.Log("aInstanceOfFirefly add in");
                aInstanceOfFirefly.xy = RandomPos(aInstanceOfFirefly);
                aInstanceOfFirefly.onClick.Add(() => { AInstanceOfFireflyBeClicked(aInstanceOfFirefly); });
                aInstanceOfFirefly.GetTransition("变透明").Play(() =>
                {
                    aInstanceOfFirefly.touchable = false;
                    aInstanceOfFirefly.onClick.Clear();
                    TaskWindow.RemoveChild(aInstanceOfFirefly);
                    aInstanceOfFirefly.Dispose();
                });
                yield return _createFireFlyWaitForSeconds;
            }
        }

        private void AInstanceOfFireflyBeClicked(GObject aInstanceOfFirefly)
        {
            if (aInstanceOfFirefly.alpha > 0)
            {
                _successNum++;
                _title2.SetVar("now", _successNum.ToString()).FlushVars();
                if (_successNum == SuccessNum)
                {
                    isSuccess = true;
                    ShowSuccessText();
                }

                aInstanceOfFirefly.asCom.GetTransition("变透明").Stop(false, false);
                TaskWindow.RemoveChild(aInstanceOfFirefly);
                aInstanceOfFirefly.Dispose();
            }
        }

        private static Vector2 RandomPos(GObject aInstanceOfFirefly)
        {
            return new Vector2(Random.Range(aInstanceOfFirefly.width / 2f, 800f - aInstanceOfFirefly.width / 2f),
                Random.Range(80f + aInstanceOfFirefly.height / 2f, 600f - aInstanceOfFirefly.height / 2f));
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;
        }

        public override void EndTask()
        {
            if (isSuccess)
            {
                MyGameNetWorkManager.Instance.AddGameProgress(addProgress);
                TaskWindow.Hide();
                _spriteRenderer.color = Color.white;
                MyGameManager.Instance.localPlayerController.nowTask = null;
                _gameUIPanel.ChangeTaskButtonVisible(false);
            }
            else
            {
                if (_createFireFlyCoroutine != null)
                {
                    StopCoroutine(_createFireFlyCoroutine);
                    _createFireFlyCoroutine = null;
                }
                TaskWindow.Hide();
            }

            MyGameManager.Instance.localPlayerController.OnEnable();
        }
    }
}