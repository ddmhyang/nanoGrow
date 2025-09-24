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
            // ## 여기가 핵심! 새 객체 대신 기존 설정을 불러옵니다. ##
            var animationData = settingsManager.LoadSettings();

            // ViewModel의 현재 데이터를 실제 저장용 데이터 형식으로 변환하여 덮어씁니다.
            foreach (var state in _viewModel.AllStates)
            {
                if (state is StandardStateViewModel standard)
                {
                    animationData.AnimationPaths[standard.Name].Clear();
                    animationData.AnimationPaths[standard.Name].Add(standard.Path1);
                    animationData.AnimationPaths[standard.Name].Add(standard.Path2);
                }
                else if (state is BackgroundStateViewModel bg)
                {
                    animationData.AnimationPaths["Background"].Clear();
                    animationData.AnimationPaths["Background"].Add(bg.Path);
                }
            }

            settingsManager.SaveSettings(animationData);
            App.UserSettings = animationData;
            MessageBox.Show("설정이 저장되었습니다!");
            this.Close();
        }

        // ... Browse_Click 및 OpenImageFile 함수는 이전과 동일 ...
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedState == null) return;

            string propertyName = (sender as FrameworkElement)?.Tag?.ToString() ?? "";
            if (string.IsNullOrEmpty(propertyName)) return;

            string? filePath = OpenImageFile();
            if (filePath != null)
            {
                _viewModel.SelectedState.GetType().GetProperty(propertyName)?.SetValue(_viewModel.SelectedState, filePath);
            }
        }

        private string? OpenImageFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}