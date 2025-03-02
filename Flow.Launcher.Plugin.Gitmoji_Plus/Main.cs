using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using Flow.Launcher.Plugin.Gitmoji_Plus.Helper;

namespace Flow.Launcher.Plugin.Gitmoji_Plus;

public class Gitmoji {
    public string Emoji { get; set; }
    public string Entity { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Name { get; set; }
    public string Example { get; set; }
    public string Icon { get; set; }
}

[SupportedOSPlatform("windows")]
public class Main : IPlugin, IContextMenu {
    
    private PluginInitContext _context;
    private List<Gitmoji> _gitmojis;
    
    private string _appIconPath = "Images/icon.png";
    private string _iconsFolder = "Icons";
    private string _gitmojiJsonFile = "Data/gitmojis.json";

    public void Init(PluginInitContext context) {
        _context = context;
        _appIconPath = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _appIconPath);
        _iconsFolder = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _iconsFolder);
        _gitmojiJsonFile = Path.Combine(context.CurrentPluginMetadata.PluginDirectory, _gitmojiJsonFile);
        LoadGitmojisFromJson();
    }

    private void LoadGitmojisFromJson() {
        if (File.Exists(_gitmojiJsonFile)) {
            var json = File.ReadAllText(_gitmojiJsonFile);
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            _gitmojis = JsonSerializer.Deserialize<List<Gitmoji>>(json, options);
        }
        else {
            _gitmojis = new List<Gitmoji>();
            _context.API.ShowMsg(
                "gitmoji plus plugin 😜",
                "couldn't initialize plugin because gitmojis.json was not found 🐛 please reinstall plugin or open issue on github if problem persists 😣",
                "Error"
            );
        }
    }

    public List<Result> Query(Query query) {
        if (_gitmojis == null || _gitmojis.Count == 0) {
            return new List<Result> {
                new Result {
                    Title = "gitmoji plus plugin error 😣",
                    SubTitle = "couldn't initialize plugin 🐛 please reinstall plugin or open issue on github if problem persists 😣",
                    IcoPath = _appIconPath
                }
            };
        }

        var results = new List<Result>();
        foreach (var gitmoji in _gitmojis) {
            var comparableItems = GetGitmojiComparableItems(gitmoji);
            if (comparableItems.Any(item => Match(query.Search, item))) {
                results.Add(new Result {
                    Title = $"[ {gitmoji.Code} ] {gitmoji.Description}",
                    SubTitle = gitmoji.Example,
                    IcoPath = Path.Combine(_iconsFolder, gitmoji.Icon),
                    ContextData = gitmoji,
                    Action = _ => {
                        ClipboardHelper.CopyTextToClipboard(gitmoji.Emoji, _context);
                        return true;
                    }
                });
            }
        }

        return results;
    }

    public List<Result> LoadContextMenus(Result selectedResult) {
        Gitmoji selectedGitmoji = selectedResult.ContextData as Gitmoji;
        return new List<Result> {
            new Result {
                Title = "copy emoji",
                SubTitle = selectedGitmoji.Emoji,
                IcoPath = Path.Combine(_iconsFolder, selectedGitmoji.Icon),
                Action = _ => {
                    ClipboardHelper.CopyTextToClipboard(selectedGitmoji.Emoji, _context);
                    return true;
                }
            },
            new Result {
                Title = "copy code",
                SubTitle = selectedGitmoji.Code,
                IcoPath = Path.Combine(_iconsFolder, selectedGitmoji.Icon),
                Action = _ => {
                    ClipboardHelper.CopyTextToClipboard(selectedGitmoji.Code, _context);
                    return true;
                }
            },
            new Result {
                Title = "copy description",
                SubTitle = selectedGitmoji.Description,
                IcoPath = Path.Combine(_iconsFolder, selectedGitmoji.Icon),
                Action = _ => {
                    ClipboardHelper.CopyTextToClipboard(selectedGitmoji.Description, _context);
                    return true;
                }
            },
            new Result {
                Title = "copy example commit message",
                SubTitle = selectedGitmoji.Example,
                IcoPath = Path.Combine(_iconsFolder, selectedGitmoji.Icon),
                Action = _ => {
                    ClipboardHelper.CopyTextToClipboard(selectedGitmoji.Example, _context);
                    return true;
                }
            },
            new Result {
                Title = "open gitmoji.dev",
                SubTitle = "visit official gitmoji.dev website",
                IcoPath = _appIconPath,
                Action = _ => {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo {
                        FileName = "https://gitmoji.dev/",
                        UseShellExecute = true
                    });
                    return true;
                }
            }
        };
    }

    private List<string> GetGitmojiComparableItems(Gitmoji gitmoji) {
        return new List<string> { gitmoji.Code, gitmoji.Name, gitmoji.Description, gitmoji.Emoji, gitmoji.Example };
    }

    private bool Match(string query, string item) {
        return string.IsNullOrEmpty(query) || item.Contains(query, StringComparison.OrdinalIgnoreCase);
    }
}