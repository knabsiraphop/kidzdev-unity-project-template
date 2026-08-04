using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// Optional confirm-before-download gate for <see cref="GameBootstrap"/>. Leave
    /// unassigned on GameBootstrap to auto-proceed with remote content downloads
    /// (template baseline). Subclass this in a project that needs explicit consent
    /// before large downloads start (e.g. a popup asking "Download 40MB?").
    /// </summary>
    public abstract class BootstrapDownloadConfirmGate : MonoBehaviour
    {
        public abstract UniTask<bool> ConfirmAsync(long totalBytes);
    }
}
