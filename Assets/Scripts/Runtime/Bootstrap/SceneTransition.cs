using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using KidzDev.Unity.AddressablesToolkit;
using KidzDev.Unity.UIOverlay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KidzGame.Bootstrap
{
    /// <summary>
    /// Fade-to-black-and-back scrim. Not Bootstrap-specific — reusable by any Feature that needs the
    /// same fade + work pattern later.
    /// </summary>
    public static class SceneTransition
    {
        /// <summary>Builds a fresh fade-to-black <see cref="FadeScrim"/> with the given durations. Caller
        /// owns the full sequence: await <see cref="FadeScrim.FadeInAsync"/>, do work, await
        /// <see cref="FadeScrim.FadeOutAsync"/>.</summary>
        public static FadeScrim CreateFade(float fadeInDuration, float fadeOutDuration) =>
            new FadeScrim(new AsymmetricFadeTransition(fadeInDuration, fadeOutDuration), Color.black);

        /// <summary>Convenience wrapper: fade to black, load one Addressable scene, fade back.</summary>
        public static async UniTask LoadAsync(
            object sceneKey,
            float fadeInDuration,
            float fadeOutDuration,
            CancellationToken ct,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool activateOnLoad = true)
        {
            var fade = CreateFade(fadeInDuration, fadeOutDuration);
            try
            {
                var fadeInStart = Time.realtimeSinceStartup;
                await fade.FadeInAsync(ct);
                Debug.Log($"[SceneTransition] Fade-in took {Time.realtimeSinceStartup - fadeInStart:0.###}s (configured {fadeInDuration}s).");

                await SceneLoader.LoadAsync(sceneKey, mode, activateOnLoad, ct);
                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);
            }
            finally
            {
                var fadeOutStart = Time.realtimeSinceStartup;
                await fade.FadeOutAsync(CancellationToken.None);
                Debug.Log($"[SceneTransition] Fade-out took {Time.realtimeSinceStartup - fadeOutStart:0.###}s (configured {fadeOutDuration}s).");
            }
        }
    }
}
