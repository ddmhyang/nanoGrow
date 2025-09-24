using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nanoGrow
{
    public enum PetState
    {
        Idle,
        Eating,
        Cleaning,
        Washing,
        Sleeping,
        Playing,
        Flying,
        MovingLeft,
        MovingRight
    }

    public class PetStatus : INotifyPropertyChanged
    {
        private int _hunger;
        private int _cleanliness;
        private int _wash;
        private int _sleep;
        private int _play;
        private int _ageInDays;
        private PetState _currentState;
        private string _currentImagePath;

        public int Hunger { get { return _hunger; } set { _hunger = value; OnPropertyChanged(); } }
        public int Cleanliness { get { return _cleanliness; } set { _cleanliness = value; OnPropertyChanged(); } }
        public int Wash { get { return _wash; } set { _wash = value; OnPropertyChanged(); } }
        public int Sleep { get { return _sleep; } set { _sleep = value; OnPropertyChanged(); } }
        public int Play { get { return _play; } set { _play = value; OnPropertyChanged(); } }
        public int AgeInDays { get { return _ageInDays; } set { _ageInDays = value; OnPropertyChanged(); } }
        public PetState CurrentState { get { return _currentState; } set { _currentState = value; OnPropertyChanged(); } }
        public string CurrentImagePath { get { return _currentImagePath; } set { _currentImagePath = value; OnPropertyChanged(); } }

        public PetStatus()
        {
            Hunger = 80;
            Cleanliness = 60;
            Wash = 90;
            Sleep = 70;
            Play = 50;
            AgeInDays = 0;
            CurrentState = PetState.Idle;
            CurrentImagePath = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}