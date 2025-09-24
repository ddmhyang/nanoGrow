using System.Windows;

namespace nanoGrow
{
    public partial class App : Application
    {
        // private set; -> set; 으로 변경
        public static AnimationData UserSettings { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settingsManager = new SettingsManager();
            UserSettings = settingsManager.LoadSettings();
        }
    }
}