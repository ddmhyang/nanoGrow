using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace nanoGrow
{
    // 각 애니메이션 상태의 데이터를 담는 기본 클래스
    public class BaseStateViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // 이미지 2개를 사용하는 상태를 위한 뷰모델
    public class StandardStateViewModel : BaseStateViewModel
    {
        private string _path1;
        private string _path2;
        public string Path1 { get { return _path1; } set { _path1 = value; OnPropertyChanged(); } }
        public string Path2 { get { return _path2; } set { _path2 = value; OnPropertyChanged(); } }
    }

    // 이미지 4개를 사용하는 'Move' 상태를 위한 뷰모델
    public class MoveStateViewModel : BaseStateViewModel
    {
        private string _leftPath1, _leftPath2, _rightPath1, _rightPath2;
        public string LeftPath1 { get { return _leftPath1; } set { _leftPath1 = value; OnPropertyChanged(); } }
        public string LeftPath2 { get { return _leftPath2; } set { _leftPath2 = value; OnPropertyChanged(); } }
        public string RightPath1 { get { return _rightPath1; } set { _rightPath1 = value; OnPropertyChanged(); } }
        public string RightPath2 { get { return _rightPath2; } set { _rightPath2 = value; OnPropertyChanged(); } }
    }

    // 이미지 1개를 사용하는 'Background' 상태를 위한 뷰모델
    public class BackgroundStateViewModel : BaseStateViewModel
    {
        private string _path;
        public string Path { get { return _path; } set { _path = value; OnPropertyChanged(); } }
    }

    // 설정 창 전체를 관리하는 메인 뷰모델
    public class SettingViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<BaseStateViewModel> AllStates { get; set; }
        private BaseStateViewModel _selectedState;
        public BaseStateViewModel SelectedState
        {
            get { return _selectedState; }
            set { _selectedState = value; OnPropertyChanged(); }
        }

        public SettingViewModel()
        {
            AllStates = new ObservableCollection<BaseStateViewModel>();
            var loadedSettings = App.UserSettings;

            // 2개짜리 표준 상태들
            var standardKeys = new[] { "idle", "fly", "sleep", "eat", "play", "wash" };
            foreach (var key in standardKeys)
            {
                var state = new StandardStateViewModel { Name = key };
                if (loadedSettings.AnimationPaths[key].Count >= 2)
                {
                    state.Path1 = loadedSettings.AnimationPaths[key][0];
                    state.Path2 = loadedSettings.AnimationPaths[key][1];
                }
                AllStates.Add(state);
            }

            // 4개짜리 Move 상태
            var moveState = new MoveStateViewModel { Name = "move" };
            if (loadedSettings.AnimationPaths["movingleft"].Count >= 2)
            {
                moveState.LeftPath1 = loadedSettings.AnimationPaths["movingleft"][0];
                moveState.LeftPath2 = loadedSettings.AnimationPaths["movingleft"][1];
            }
            if (loadedSettings.AnimationPaths["movingright"].Count >= 2)
            {
                moveState.RightPath1 = loadedSettings.AnimationPaths["movingright"][0];
                moveState.RightPath2 = loadedSettings.AnimationPaths["movingright"][1];
            }
            AllStates.Add(moveState);

            // 1개짜리 Background 상태
            var bgState = new BackgroundStateViewModel { Name = "background" };
            if (loadedSettings.AnimationPaths["background"].Count >= 1)
            {
                bgState.Path = loadedSettings.AnimationPaths["background"][0];
            }
            AllStates.Add(bgState);

            SelectedState = AllStates.FirstOrDefault();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}