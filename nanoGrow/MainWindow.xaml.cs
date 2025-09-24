using nanoGrow;
using System;
using System.Collections.Generic;
using System.IO; // File.Exists를 위해 추가
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
            LoadedAnimations = App.UserSettings;
            CurrentStatus = new PetStatus();
            UpdateCharacterImage();
            this.DataContext = CurrentStatus;
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


        // ... (UpdateTimerInterval, GameTimer_Tick, AnimationTimer_Tick은 이전과 동일) ...

        private void UpdateCharacterImage()
        {
            // .ToLower()를 제거하여 "MovingLeft"처럼 대소문자를 유지합니다.
            string stateKey = CurrentStatus.CurrentState.ToString();
            List<string> imagePathsToAnimate;

            if (LoadedAnimations.AnimationPaths.TryGetValue(stateKey, out List<string> customPaths) && customPaths.Count > 0 && !string.IsNullOrEmpty(customPaths[0]))
            {
                imagePathsToAnimate = customPaths;
            }
            else
            {
                switch (stateKey)
                {
                    // 키 이름을 대소문자까지 정확하게 맞춰줍니다.
                    case "MovingLeft":
                        imagePathsToAnimate = new List<string> { "/Images/move_left1.png", "/Images/move_left2.png" };
                        break;
                    case "MovingRight":
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

        // ... (SetTemporaryState 및 Feed, Clean, Wash, Sleep, Play 버튼 클릭 이벤트는 이전과 동일) ...

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            _characterWindow.Show();
            this.Hide();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            _settingWindow = new SettingWindow();
            _settingWindow.ShowDialog();
            LoadedAnimations = App.UserSettings;
            UpdateCharacterImage();
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
    }
}