using System.Threading;
using Cysharp.Threading.Tasks;
using KidzDev.Unity.AddressablesToolkit;
using UnityEngine;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// The only MonoBehaviour allowed in the first scene. Owns Addressables
    /// initialization and hands off the first real scene load — every other
    /// entry point into the game must go through this, never a raw
    /// SceneManager/Addressables call. See ARCHITECTURE.md.
    /// </summary>
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Tooltip("Addressable key of the first real scene to load once Addressables is ready.")]
        [SerializeField] private string firstSceneAddress = "MainMenu";

        [Tooltip("Optional. Leave unassigned for a UI-less Bootstrap scene (template baseline). " +
                 "Assign a BootstrapProgressReporter to drive a loading-bar UI during remote content download.")]
        [SerializeField] private BootstrapProgressReporter progressReporter;

        [Tooltip("Optional. Leave unassigned to auto-proceed with remote content downloads (template baseline). " +
                 "Assign a project-specific BootstrapDownloadConfirmGate subclass to require consent first.")]
        [SerializeField] private BootstrapDownloadConfirmGate downloadConfirmGate;

        [Tooltip("Fade-to-black duration before the load starts. <= 0 makes it instant.")]
        [SerializeField] private float sceneFadeInDuration = 1f;

        [Tooltip("Fade-from-black duration after the new scene settles. <= 0 makes it instant.")]
        [SerializeField] private float sceneFadeOutDuration = 1f;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
        }

        private void Start()
        {
            BootAsync(_cts.Token).Forget();
        }

        private async UniTaskVoid BootAsync(CancellationToken ct)
        {
            bool ready = await InitializeAddressablesAsync(ct);
            if (!ready)
            {
                Debug.LogError("[GameBootstrap] AddressablesService failed to initialize; aborting first scene load.");
                return;
            }

            await SceneTransition.LoadAsync(firstSceneAddress, sceneFadeInDuration, sceneFadeOutDuration, ct);
        }

        private UniTask<bool> InitializeAddressablesAsync(CancellationToken ct)
        {
            var confirm = downloadConfirmGate != null
                ? (RemoteContentUpdater.ConfirmDownload)downloadConfirmGate.ConfirmAsync
                : null;

            return AddressablesService.InitializeAsync(progress: progressReporter, confirm: confirm, ct: ct);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}
