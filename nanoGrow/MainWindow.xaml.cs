using nanoGrow;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace nanoGrow
{
    public partial class MainWindow : Window
    {
        public PetStatus CurrentStatus { get; set; }
        public AnimationData LoadedAnimations { get; set; }

        private CharacterWindow _characterWindow;
        private SettingWindow _settingWindow;
        private DispatcherTimer _gameTimer;
        private DispatcherTimer _animationTimer;
        private DispatcherTimer _actionStateTimer;

        private int _currentFrameIndex = 0;
        private DateTime _lastTickTime;

        public MainWindow()
        {
            InitializeComponent();

            // ## 1. 데이터 로드를 가장 먼저 수행합니다. ##
            LoadedAnimations = App.UserSettings;
            CurrentStatus = new PetStatus();

            // ## 2. 첫 이미지를 미리 설정합니다. ##
            // LoadedAnimations 데이터와 CurrentStatus 데이터를 모두 사용해서 첫 이미지를 결정합니다.
            UpdateCharacterImage();

            // ## 3. 데이터 컨텍스트를 마지막에 연결합니다. ##
            this.DataContext = CurrentStatus;

            // --- 나머지 초기화 코드는 동일 ---
            _characterWindow = new CharacterWindow(this);
            _lastTickTime = DateTime.Now;

            InitializeGameTimer();
            InitializeAnimationTimer();
        }

        private void InitializeGameTimer()
        {
            _gameTimer = new DispatcherTimer();
            UpdateTimerInterval();
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void InitializeAnimationTimer()
        {
            _animationTimer = new DispatcherTimer();
            _animationTimer.Interval = TimeSpan.FromMilliseconds(500);
            _animationTimer.Tick += AnimationTimer_Tick;
            _animationTimer.Start();

            _actionStateTimer = new DispatcherTimer();
            _actionStateTimer.Interval = TimeSpan.FromSeconds(3);
            _actionStateTimer.Tick += (s, e) =>
            {
                CurrentStatus.CurrentState = PetState.Idle;
                _actionStateTimer.Stop();
            };
        }

        private void UpdateTimerInterval()
        {
            double totalHours = 12 + CurrentStatus.AgeInDays;
            totalHours = Math.Min(totalHours, 48);
            double secondsPerTick = (totalHours * 3600) / 100;
            _gameTimer.Interval = TimeSpan.FromSeconds(secondsPerTick);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            CurrentStatus.Hunger = Math.Max(0, CurrentStatus.Hunger - 1);
            CurrentStatus.Cleanliness = Math.Max(0, CurrentStatus.Cleanliness - 1);
            CurrentStatus.Wash = Math.Max(0, CurrentStatus.Wash - 1);
            CurrentStatus.Sleep = Math.Max(0, CurrentStatus.Sleep - 1);
            CurrentStatus.Play = Math.Max(0, CurrentStatus.Play - 1);

            if ((DateTime.Now - _lastTickTime).TotalDays >= 1)
            {
                CurrentStatus.AgeInDays++;
                _lastTickTime = DateTime.Now;
                UpdateTimerInterval();
            }
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            UpdateCharacterImage();
        }

        // MainWindow.xaml.cs 안의 UpdateCharacterImage 함수
        private void UpdateCharacterImage()
        {
            string stateKey = CurrentStatus.CurrentState.ToString().ToLower();
            List<string> imagePathsToAnimate;

            if (LoadedAnimations.AnimationPaths.TryGetValue(stateKey, out List<string> customPaths) && customPaths.Count > 0 && !string.IsNullOrEmpty(customPaths[0]))
            {
                imagePathsToAnimate = customPaths;
            }
            else
            {
                switch (stateKey)
                {
                    // ## 이 부분을 수정합니다 ##
                    case "movingleft":
                        imagePathsToAnimate = new List<string> { "/Images/move_left1.png", "/Images/move_left2.png" };
                        break;
                    case "movingright":
                        imagePathsToAnimate = new List<string> { "/Images/move_right1.png", "/Images/move_right2.png" };
                        break;
                    default:
                        imagePathsToAnimate = new List<string> { "/Images/pet_idle.png", "/Images/pet_idle2.png" };
                        break;
                }
            }

            if (imagePathsToAnimate != null && imagePathsToAnimate.Count > 0)
            {
                _currentFrameIndex = (_currentFrameIndex + 1) % imagePathsToAnimate.Count;
                CurrentStatus.CurrentImagePath = imagePathsToAnimate[_currentFrameIndex];
            }
        }


        private void SetTemporaryState(PetState state)
        {
            CurrentStatus.CurrentState = state;
            _actionStateTimer.Stop();
            _actionStateTimer.Start();
        }

        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus.Hunger = Math.Min(100, CurrentStatus.Hunger + 10);
            SetTemporaryState(PetState.Eating);
        }
        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus.Cleanliness = Math.Min(100, CurrentStatus.Cleanliness + 10);
            // 'Cleaning' 상태를 잠시 재생하도록 수정
            SetTemporaryState(PetState.Cleaning);
        }
        private void WashButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus.Wash = Math.Min(100, CurrentStatus.Wash + 10);
            SetTemporaryState(PetState.Washing);
        }
        private void SleepButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus.Sleep = Math.Min(100, CurrentStatus.Sleep + 10);
            SetTemporaryState(PetState.Sleeping);
        }
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentStatus.Play = Math.Min(100, CurrentStatus.Play + 10);
            SetTemporaryState(PetState.Playing);
        }

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            _characterWindow.Show();
            this.Hide();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _settingWindow = new SettingWindow();
            _settingWindow.ShowDialog();
        }
    }
}