using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media; // CompositionTarget을 위해 추가
using System.Windows.Threading;

namespace nanoGrow
{
    public partial class CharacterWindow : Window
    {
        private MainWindow _mainWindow;
        private PetStatus _petStatus;

        private DispatcherTimer _decisionTimer;
        private Random _random = new Random();

        private Point _targetPosition;
        private const double Speed = 100;
        private DateTime _lastRenderTime; // 마지막 렌더링 시간 저장

        public CharacterWindow(MainWindow main)
        {
            InitializeComponent();
            this._mainWindow = main;
            this._petStatus = main.CurrentStatus;
            this.DataContext = _petStatus;

            InitializeMovement();
        }

        private void InitializeMovement()
        {
            _decisionTimer = new DispatcherTimer();
            _decisionTimer.Interval = TimeSpan.FromSeconds(5);
            _decisionTimer.Tick += DecisionTimer_Tick;
            _decisionTimer.Start();
        }

        // 목표 지점을 정하는 타이머
        private void DecisionTimer_Tick(object sender, EventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double maxX = screenWidth - this.Width;
            double maxY = screenHeight - this.Height;
            _targetPosition = new Point(_random.NextDouble() * maxX, _random.NextDouble() * maxY);

            UpdateMoveState();

            // 렌더링 이벤트 구독 시작
            CompositionTarget.Rendering += UpdateFrame;
            _lastRenderTime = DateTime.Now;
        }

        // 화면 주사율에 맞춰 계속 호출되는 함수 (기존 RenderTimer_Tick 대체)
        private void UpdateFrame(object sender, EventArgs e)
        {
            if (_petStatus.CurrentState != PetState.MovingLeft && _petStatus.CurrentState != PetState.MovingRight)
            {
                CompositionTarget.Rendering -= UpdateFrame; // 이동이 아니면 이벤트 구독 취소
                return;
            }

            // 프레임 간 시간차(DeltaTime) 계산
            var now = DateTime.Now;
            var deltaTime = (now - _lastRenderTime).TotalSeconds;
            _lastRenderTime = now;

            Point currentPosition = new Point(this.Left, this.Top);
            Vector direction = _targetPosition - currentPosition;

            if (direction.Length < 5)
            {
                _petStatus.CurrentState = PetState.Idle; // 도착 시 Idle로 변경
                CompositionTarget.Rendering -= UpdateFrame; // 이벤트 구독 취소
                return;
            }

            direction.Normalize();
            double moveAmount = Speed * deltaTime; // 프레임 속도에 관계없이 일정한 속도로 이동

            Point newPosition = currentPosition + (direction * moveAmount);
            this.Left = newPosition.X;
            this.Top = newPosition.Y;
        }

        // ... 나머지 코드는 이전과 거의 동일 ...
        private void UpdateMoveState()
        {
            if (_targetPosition.X < this.Left)
            {
                _petStatus.CurrentState = PetState.MovingLeft;
            }
            else
            {
                _petStatus.CurrentState = PetState.MovingRight;
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _petStatus.CurrentState = PetState.Flying;
                _decisionTimer.Stop();
                CompositionTarget.Rendering -= UpdateFrame; // 렌더링 중지
                this.DragMove();
                _decisionTimer.Start();
            }
        }
        private void ReturnToDashboard()
        {
            _petStatus.CurrentState = PetState.Idle;
            _mainWindow.Show();
            this.Hide();
        }
        private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e) { ReturnToDashboard(); }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) { ReturnToDashboard(); }
    }
}