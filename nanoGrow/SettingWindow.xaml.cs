using Microsoft.Win32;
using nanoGrow;
using System;
using System.Reflection;
using System.Windows;

namespace nanoGrow
{
    public partial class SettingWindow : Window
    {
        private SettingViewModel _viewModel;

        public SettingWindow()
        {
            InitializeComponent();
            _viewModel = new SettingViewModel();
            this.DataContext = _viewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsManager = new SettingsManager();
            var animationData = new AnimationData();

            foreach (var state in _viewModel.AllStates)
            {
                if (state is StandardStateViewModel standard)
                {
                    animationData.AnimationPaths[standard.Name].Clear();
                    animationData.AnimationPaths[standard.Name].Add(standard.Path1);
                    animationData.AnimationPaths[standard.Name].Add(standard.Path2);
                }
                else if (state is MoveStateViewModel move)
                {
                    animationData.AnimationPaths["movingleft"].Clear();
                    animationData.AnimationPaths["movingleft"].Add(move.LeftPath1);
                    animationData.AnimationPaths["movingleft"].Add(move.LeftPath2);
                    animationData.AnimationPaths["movingright"].Clear();
                    animationData.AnimationPaths["movingright"].Add(move.RightPath1);
                    animationData.AnimationPaths["movingright"].Add(move.RightPath2);
                }
                else if (state is BackgroundStateViewModel bg)
                {
                    animationData.AnimationPaths["background"].Clear();
                    animationData.AnimationPaths["background"].Add(bg.Path);
                }
            }

            settingsManager.SaveSettings(animationData);
            App.UserSettings = animationData;
            MessageBox.Show("설정이 저장되었습니다!");
            this.Close();
        }

        // 모든 '찾아보기...' 버튼이 이 하나의 함수를 사용
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedState == null) return;

            // 버튼의 Tag 속성에 저장된 속성 이름(Path1, LeftPath2 등)을 가져옴
            string propertyName = (sender as FrameworkElement).Tag.ToString();

            string filePath = OpenImageFile();
            if (filePath != null)
            {
                // Reflection을 사용해 이름으로 속성 값을 동적으로 설정
                _viewModel.SelectedState.GetType().GetProperty(propertyName)?.SetValue(_viewModel.SelectedState, filePath);
            }
        }

        private string OpenImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}