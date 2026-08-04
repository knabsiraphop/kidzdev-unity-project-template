using Cysharp.Threading.Tasks;
using KidzDev.Unity.Popup;
using KidzGame.Core;
using UnityEngine;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// <see cref="BootstrapDownloadConfirmGate"/> backed by the ecosystem's own
    /// <c>com.kidzdev.unity.popup</c> package — a <see cref="PopupManager"/> showing a
    /// <see cref="ConfirmPopup"/> prefab via <see cref="PopupRef.Resources"/> (no Addressables
    /// needed, since this fires before content is confirmed/downloaded).
    /// </summary>
    public sealed class BootstrapDownloadConfirmPopup : BootstrapDownloadConfirmGate
    {
        [Tooltip("Resources-folder path (no extension) to a prefab with a ConfirmPopup component " +
                 "(see com.kidzdev.unity.popup/Runtime/Popups/ConfirmPopup.cs).")]
        [SerializeField] private string confirmPopupResourcesPath = PopupKeys.ConfirmPopup;

        private PopupManager _popupManager;

        public override async UniTask<bool> ConfirmAsync(long totalBytes)
        {
            _popupManager ??= new PopupManager();

            var megabytes = totalBytes / (1024f * 1024f);
            var message = $"Download {megabytes:0.#} MB?";

            return await _popupManager.ShowAsync<bool>(PopupRef.Resources(confirmPopupResourcesPath), message);
        }

        private void OnDestroy()
        {
            _popupManager?.Dispose();
        }
    }
}
