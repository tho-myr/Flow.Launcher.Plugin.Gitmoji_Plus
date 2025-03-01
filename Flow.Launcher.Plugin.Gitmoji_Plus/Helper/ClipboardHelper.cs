using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Path = System.IO.Path;

namespace Flow.Launcher.Plugin.Gitmoji_Plus.Helper;

[SupportedOSPlatform("windows")]
public static class ClipboardHelper
{
    private const int RetryCount = 5;
    private const int RetryDelay = 200;
    
    public static async void CopyTextToClipboard(string text, PluginInitContext context)
    {
        if (text is not null)
        {
            var exception = await RetryActionOnSTAThreadAsync(() =>
            {
                Clipboard.SetText(text);
            });
            if (exception == null) return;
            context.API.ShowMsg(
                "failed to copy to clipboard \ud83d\ude16", 
                "see flow launcher logs for more details", 
                Path.Combine(context.CurrentPluginMetadata.PluginDirectory, "Images/icon.png")
            );
            context.API.LogException(nameof(ClipboardHelper), "gitmoji plus plugin failed to copy to clipboard", exception);
        } else {
            context.API.ShowMsg("failed to copy to clipboard because text was null \ud83d\ude16");
        }
    }
    
    private static async Task<Exception> RetryActionOnSTAThreadAsync(Action action)
    {
        for (var i = 0; i < RetryCount; i++)
        {
            try {
                await StartSTATaskAsync(action);
                break;
            }
            catch (Exception e) {
                if (i == RetryCount - 1)
                {
                    return e;
                }
                await Task.Delay(RetryDelay);
            }
        }
        return null;
    }
    
    private static Task StartSTATaskAsync(Action action)
    {
        var taskCompletionSource = new TaskCompletionSource();
        Thread thread = new(() =>
        {
            try {
                action();
                taskCompletionSource.TrySetResult();
            } catch (Exception e) {
                taskCompletionSource.SetException(e);
            }
        })
        {
            IsBackground = true,
            Priority = ThreadPriority.Normal
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return taskCompletionSource.Task;
    }
}