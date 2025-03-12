using System.Windows.Controls;

namespace Flow.Launcher.Plugin.Gitmoji_Plus.Settings;

public partial class PluginSettings : UserControl
{
    
    private readonly PluginInitContext _context;
    private readonly Settings _settings;

    public PluginSettings(PluginInitContext context, Settings settings)
    {
        InitializeComponent();
        _context = context;
        _settings = settings;
        LoadSettings();
    }

    private void LoadSettings()
    {
        CopyActionComboBox.SelectedIndex = _settings.PropertyToCopy switch
        {
            Settings.CopyAction.Emoji => 0,
            Settings.CopyAction.Code => 1,
            _ => 0
        };
    }
    
    private void CopyActionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        _settings.PropertyToCopy = CopyActionComboBox.SelectedIndex switch
        {
            0 => Settings.CopyAction.Emoji,
            1 => Settings.CopyAction.Code,
            _ => Settings.CopyAction.Emoji
        };
        
        _context.API.SaveSettingJsonStorage<Settings>();
    }
}