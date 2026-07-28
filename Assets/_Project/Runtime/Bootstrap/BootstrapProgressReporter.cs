using System;
using KidzDev.Unity.AddressablesToolkit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// Optional progress bridge for <see cref="GameBootstrap"/>. Drives a plain
    /// filled <see cref="UnityEngine.UI.Image"/> loading bar directly for the
    /// common case, and also raises a UnityEvent for anything fancier a project
    /// wants to hook up in the Inspector. Leave unassigned on GameBootstrap for
    /// a UI-less Bootstrap scene.
    /// </summary>
    public sealed class BootstrapProgressReporter : MonoBehaviour, IProgress<DownloadProgress>
    {
        [Tooltip("Optional. Image.type must be Filled. Drives fillAmount (0..1) directly during remote content download.")]
        [SerializeField] private Image progressFillImage;

        [Tooltip("Optional. Fires for anything beyond a plain fill image (e.g. a percent label).")]
        [SerializeField] private UnityEvent<float> onPercentChanged;

        public void Report(DownloadProgress value)
        {
            if (progressFillImage != null)
                progressFillImage.fillAmount = value.Percent;

            onPercentChanged?.Invoke(value.Percent);
        }
    }
}
