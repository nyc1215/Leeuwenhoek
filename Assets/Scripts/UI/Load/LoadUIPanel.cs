using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Manager;
using UI.Util;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UI.Load
{
    public class LoadUIPanel : UIPanelUtil
    {
        private string _nextSceneToLoadAsync;
        private GProgressBar _progressBar;
        private GTextField _clickToContinueTextField;

        protected override void Awake()
        {
            base.Awake();
            _nextSceneToLoadAsync = MyGameManager.Instance.nextSceneToLoadAsync;
            _progressBar = UIRoot.GetChild("ProcessBar_Load").asProgress;
            _clickToContinueTextField = UIRoot.GetChild("Text_ClickToContinue").asTextField;
            Assert.IsNotNull(_progressBar);
            Assert.IsNotNull(_clickToContinueTextField);
        }

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(_nextSceneToLoadAsync))
            {
                _clickToContinueTextField.visible = false;
                StartCoroutine(IE_LoadSceneAsync());
            }
            else
            {
                Debug.LogWarning($"{_nextSceneToLoadAsync} is null or empty!!!");
            }
        }

        /// <summary>
        /// 异步加载场景，并且显示加载进度
        /// </summary>
        private IEnumerator IE_LoadSceneAsync()
        {
            yield return null;

            var asyncOperation = SceneManager.LoadSceneAsync(_nextSceneToLoadAsync);
            asyncOperation.allowSceneActivation = false;

            while (!asyncOperation.isDone)
            {
                var progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
                _progressBar.value = progress * 100;

                if (asyncOperation.progress >= 0.9f)
                {
                    _progressBar.value = 100;
                    _clickToContinueTextField.visible = true;
                    if (Input.anyKeyDown)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }
    }
}