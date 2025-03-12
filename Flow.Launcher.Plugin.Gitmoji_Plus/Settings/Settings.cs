namespace Flow.Launcher.Plugin.Gitmoji_Plus.Settings;

public class Settings {

    public enum CopyAction {
        Emoji, Code
    }
    
    public CopyAction PropertyToCopy { get; set; }
    
}