using System;
using Cysharp.Threading.Tasks;
using KidzDev.Unity.AddressablesToolkit;
using UnityEngine;
using UnityEngine.UI;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// Optional progress bridge for <see cref="GameBootstrap"/>. Drives a plain
    /// filled <see cref="UnityEngine.UI.Image"/> loading bar directly, and
    /// raises a UniTask-returning event for anything else (e.g. an awaited
    /// tween) — subscribe via code only, there is no Inspector wiring. Leave
    /// unassigned on GameBootstrap for a UI-less Bootstrap scene.
    /// </summary>
    public sealed class BootstrapProgressReporter : MonoBehaviour, IProgress<DownloadProgress>
    {
        [Tooltip("Optional. Image.type must be Filled. Drives fillAmount (0..1) directly during remote content download.")]
        [SerializeField] private Image progressFillImage;

        public event Func<float, UniTask> PercentChangedAsync;

        public void Report(DownloadProgress value)
        {
            if (progressFillImage != null)
                progressFillImage.fillAmount = value.Percent;

            var handler = PercentChangedAsync;
            if (handler == null)
                return;

            foreach (var d in handler.GetInvocationList())
                ((Func<float, UniTask>)d).Invoke(value.Percent).Forget();
        }
    }
}
