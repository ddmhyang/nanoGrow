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