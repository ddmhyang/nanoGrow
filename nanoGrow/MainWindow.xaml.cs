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

        // ... (나머지 변수 선언은 이전과 동일) ...
        private CharacterWindow _characterWindow;
        private SettingWindow? _settingWindow;
        private DispatcherTimer _gameTimer;
        private DispatcherTimer _animationTimer;
        private DispatcherTimer? _actionStateTimer;
        private int _currentFrameIndex = 0;
        private DateTime _lastTickTime;


        public MainWindow()
        {
            InitializeComponent();

            LoadedAnimations = App.UserSettings;
            CurrentStatus = new PetStatus();

            this.DataContext = CurrentStatus;

            // 생성자에서 첫 이미지를 바로 설정
            UpdateCharacterImage();

            _characterWindow = new CharacterWindow(this);
            _lastTickTime = DateTime.Now;

            InitializeGameTimer();
            InitializeAnimationTimer();
        }

        // ## 이 함수가 매우 단순해졌습니다! ##
        private void UpdateCharacterImage()
        {
            PetState stateToAnimate = CurrentStatus.CurrentState;

            // MainWindow에서는 이동 관련 애니메이션을 보여주지 않음
            if (stateToAnimate == PetState.MovingLeft || stateToAnimate == PetState.MovingRight || stateToAnimate == PetState.Flying)
            {
                stateToAnimate = PetState.Idle;
            }

            // AnimationData에게 현재 상태에 맞는 이미지 목록을 요청
            List<string> imagePathsToAnimate = LoadedAnimations.GetPathsForState(stateToAnimate);

            // 받은 목록으로 애니메이션 실행
            if (imagePathsToAnimate.Count > 0)
            {
                _currentFrameIndex = (_currentFrameIndex + 1) % imagePathsToAnimate.Count;
                CurrentStatus.CurrentImagePath = imagePathsToAnimate[_currentFrameIndex];
            }
        }

        // ... (나머지 코드는 모두 이전과 동일합니다) ...
        #region Other Methods
        public void OnReturnedToDashboard()
        {
            _animationTimer.Start();
            CurrentStatus.CurrentState = PetState.Idle;
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
                if (_actionStateTimer != null) _actionStateTimer.Stop();
                CurrentStatus.CurrentState = PetState.Idle;
            };
        }

        private void UpdateTimerInterval()
        {
            double totalHours = 12 + CurrentStatus.AgeInDays;
            totalHours = Math.Min(totalHours, 48);
            double secondsPerTick = (totalHours * 3600) / 100;
            _gameTimer.Interval = TimeSpan.FromSeconds(secondsPerTick);
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
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

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            UpdateCharacterImage();
        }

        private void SetTemporaryState(PetState state)
        {
            CurrentStatus.CurrentState = state;
            _actionStateTimer?.Stop();
            _actionStateTimer?.Start();
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

        private void PinButton_Click(object sender, RoutedEventArgs e)
        {
            // _animationTimer.Stop(); // 더 이상 타이머를 멈출 필요가 없습니다.
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
        #endregion
    }
}