using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace nanoGrow
{
    public class BaseStateViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; } = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class StandardStateViewModel : BaseStateViewModel
    {
        private string? _path1;
        private string? _path2;
        public string? Path1 { get { return _path1; } set { _path1 = value; OnPropertyChanged(); } }
        public string? Path2 { get { return _path2; } set { _path2 = value; OnPropertyChanged(); } }
    }

    public class BackgroundStateViewModel : BaseStateViewModel
    {
        private string? _path;
        public string? Path { get { return _path; } set { _path = value; OnPropertyChanged(); } }
    }

    public class SettingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<BaseStateViewModel> AllStates { get; set; }
        private BaseStateViewModel? _selectedState;
        public BaseStateViewModel? SelectedState
        {
            get { return _selectedState; }
            set { _selectedState = value; OnPropertyChanged(); }
        }

        public SettingViewModel()
        {
            AllStates = new ObservableCollection<BaseStateViewModel>();
            var loadedSettings = App.UserSettings;

            var standardKeys = new[] { "Idle", "Eating", "Cleaning", "Washing", "Sleeping", "Playing", "Flying", "MovingLeft", "MovingRight" };
            foreach (var key in standardKeys)
            {
                var state = new StandardStateViewModel { Name = key };
                if (loadedSettings.AnimationPaths.TryGetValue(key, out var paths) && paths.Count >= 2)
                {
                    state.Path1 = paths[0];
                    state.Path2 = paths[1];
                }
                AllStates.Add(state);
            }

            var bgState = new BackgroundStateViewModel { Name = "Background" };
            if (loadedSettings.AnimationPaths.TryGetValue("Background", out var bgPaths) && bgPaths.Count >= 1)
            {
                bgState.Path = bgPaths[0];
            }
            AllStates.Add(bgState);

            SelectedState = AllStates.FirstOrDefault();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}