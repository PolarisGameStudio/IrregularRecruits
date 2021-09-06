#if UNITY_ANDROID
using Google.Play.AppUpdate;
using Google.Play.Common;
#endif
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UpdateManagement
{
    // based on https://developer.android.com/guide/playcore/in-app-updates/unity?authuser=1
    public class AutoUpdateManager : Singleton<AutoUpdateManager>
    {
#if UNITY_ANDROID 

        private AppUpdateManager UpdateManager;
        private PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> AppUpdateInfoOperation;
        private AppUpdateInfo AppUpdateInfoResult;
        public GameObject Background;
        public GameObject AskUserForUpdateGO;
        public Button UpdateButton;
        public TextMeshProUGUI UpdateStatusText;
        public TextMeshProUGUI DescriptionText;

        private void Awake()
        {
            UpdateManager = new AppUpdateManager();

            StartCoroutine(CheckForUpdate());
        }

        IEnumerator CheckForUpdate()
        {
            AppUpdateInfoOperation =
              UpdateManager.GetAppUpdateInfo();

            // Wait until the asynchronous operation completes.
            yield return AppUpdateInfoOperation;

            if (AppUpdateInfoOperation.IsSuccessful)
            {
                AppUpdateInfoResult = AppUpdateInfoOperation.GetResult();
                // Check AppUpdateInfo's UpdateAvailability, UpdatePriority,
                // IsUpdateTypeAllowed(), etc. and decide whether to ask the user
                // to start an in-app update.

                if (AppUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable)
                    AskUserToUpdate();
                else
                {
                    UpdateStatusText.text = "Latest version";
                    Background.SetActive(false);
                }
            }
            else
            {
                // Log appUpdateInfoOperation.Error.
                UpdateStatusText.text = "UNABLE TO RETRIEVE UPDATE STATUS";
                Background.SetActive(false);
            }
        }

        private void AskUserToUpdate()
        {
            UpdateStatusText.text = "NEW UPDATE AVAILABLE";

            Background.SetActive(true);
            AskUserForUpdateGO.SetActive(true);
            UpdateButton.onClick.AddListener(() => StartCoroutine(StartImmediateUpdate()));
        }

        IEnumerator StartImmediateUpdate()
        {
            //ui lock 

            AskUserForUpdateGO.SetActive(false);

            DescriptionText.text = "Update in progress...";

            // Creates an AppUpdateOptions defining an immediate in-app
            // update flow and its parameters.
            var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();

            // Creates an AppUpdateRequest that can be used to monitor the
            // requested in-app update flow.
            var startUpdateRequest = UpdateManager.StartUpdate(
              // The result returned by PlayAsyncOperation.GetResult().
              AppUpdateInfoResult,
              // The AppUpdateOptions created defining the requested in-app update
              // and its parameters.
              appUpdateOptions);
            yield return startUpdateRequest;

            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (for
            // example, by logging result.Error or by displaying a message to the user).

            UpdateStatusText.text = "Update failed";
            Background.SetActive(false);

        }
        

#endif
    }

}