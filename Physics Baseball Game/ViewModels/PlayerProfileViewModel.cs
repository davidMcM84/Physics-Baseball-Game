using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Physics_Baseball_Game.Commands;
using Physics_Baseball_Game.Models;

namespace Physics_Baseball_Game.ViewModels
{
    internal sealed class PlayerProfileViewModel : ViewModelBase
    {
        private readonly BallPlayer _player;
        private readonly Snapshot _original;

        // Editable identity properties
        private string _firstName;
        private string _lastName;
        private int _age;
        private PlayerPosition _position;
        private PlayerRole _role;
        private BatHand _batHand;
        private ThrowHand _throwHand;

        // Editable physical attributes
        private double _height;
        private double _weight;
        private double _sprintSpeed;
        private double _acceleration;
        private double _batSpeed;
        private double _armStrength;
        private double _reactionTime;

        public PlayerProfileViewModel(BallPlayer player)
        {
            _player = player;
            _original = Snapshot.Take(player);

            _firstName = player.FirstName;
            _lastName = player.LastName;
            _age = player.Age;
            _position = player.Position;
            _role = player.Role;
            _batHand = player.BattingHand;
            _throwHand = player.ThrowingHand;

            _height = player.Attributes.HeightInInches;
            _weight = player.Attributes.WeightInPounds;
            _sprintSpeed = player.Attributes.SprintSpeed;
            _acceleration = player.Attributes.Acceleration;
            _batSpeed = player.Attributes.BatSpeed;
            _armStrength = player.Attributes.ArmStrength;
            _reactionTime = player.Attributes.ReactionTime;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        public Guid Id => _player.Id;

        public string FirstName { get => _firstName; set { if (SetProperty(ref _firstName, value)) RaiseCommands(); } }
        public string LastName { get => _lastName; set { if (SetProperty(ref _lastName, value)) RaiseCommands(); } }
        public int Age { get => _age; set { if (SetProperty(ref _age, value)) RaiseCommands(); } }
        public PlayerPosition Position { get => _position; set { if (SetProperty(ref _position, value)) RaiseCommands(); } }
        public PlayerRole Role { get => _role; set { if (SetProperty(ref _role, value)) RaiseCommands(); } }
        public BatHand BattingHand { get => _batHand; set { if (SetProperty(ref _batHand, value)) RaiseCommands(); } }
        public ThrowHand ThrowingHand { get => _throwHand; set { if (SetProperty(ref _throwHand, value)) RaiseCommands(); } }

        public double HeightInInches { get => _height; set { if (SetProperty(ref _height, value)) RaiseCommands(); } }
        public double WeightInPounds { get => _weight; set { if (SetProperty(ref _weight, value)) RaiseCommands(); } }
        public double SprintSpeed { get => _sprintSpeed; set { if (SetProperty(ref _sprintSpeed, value)) RaiseCommands(); } }
        public double Acceleration { get => _acceleration; set { if (SetProperty(ref _acceleration, value)) RaiseCommands(); } }
        public double BatSpeed { get => _batSpeed; set { if (SetProperty(ref _batSpeed, value)) RaiseCommands(); } }
        public double ArmStrength { get => _armStrength; set { if (SetProperty(ref _armStrength, value)) RaiseCommands(); } }
        public double ReactionTime { get => _reactionTime; set { if (SetProperty(ref _reactionTime, value)) RaiseCommands(); } }

        // Enumerations for ComboBoxes
        public ObservableCollection<PlayerPosition> Positions { get; } =
            new(Enum.GetValues(typeof(PlayerPosition)).Cast<PlayerPosition>());
        public ObservableCollection<PlayerRole> Roles { get; } =
            new(Enum.GetValues(typeof(PlayerRole)).Cast<PlayerRole>());
        public ObservableCollection<BatHand> BatHands { get; } =
            new(Enum.GetValues(typeof(BatHand)).Cast<BatHand>());
        public ObservableCollection<ThrowHand> ThrowHands { get; } =
            new(Enum.GetValues(typeof(ThrowHand)).Cast<ThrowHand>());

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? CloseRequested;
        public event Action<BallPlayer>? Saved;

        private bool CanSave()
        {
            return !_original.Equals(Snapshot.TakeEditable(this));
        }

        private void Save()
        {
            _player.FirstName = FirstName;
            _player.LastName = LastName;
            _player.Age = Age;
            _player.Position = Position;
            _player.Role = Role;
            _player.BattingHand = BattingHand;
            _player.ThrowingHand = ThrowingHand;
            _player.Attributes = new PhysicalAttributes(
                HeightInInches,
                WeightInPounds,
                SprintSpeed,
                Acceleration,
                BatSpeed,
                ArmStrength,
                ReactionTime);

            Saved?.Invoke(_player);
            CloseRequested?.Invoke();
        }

        private void Cancel()
        {
            // Revert
            FirstName = _original.FirstName;
            LastName = _original.LastName;
            Age = _original.Age;
            Position = _original.Position;
            Role = _original.Role;
            BattingHand = _original.BatHand;
            ThrowingHand = _original.ThrowHand;
            HeightInInches = _original.HeightInInches;
            WeightInPounds = _original.WeightInPounds;
            SprintSpeed = _original.SprintSpeed;
            Acceleration = _original.Acceleration;
            BatSpeed = _original.BatSpeed;
            ArmStrength = _original.ArmStrength;
            ReactionTime = _original.ReactionTime;

            CloseRequested?.Invoke();
        }

        private void RaiseCommands()
        {
            (SaveCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private readonly record struct Snapshot(
            string FirstName,
            string LastName,
            int Age,
            PlayerPosition Position,
            PlayerRole Role,
            BatHand BatHand,
            ThrowHand ThrowHand,
            double HeightInInches,
            double WeightInPounds,
            double SprintSpeed,
            double Acceleration,
            double BatSpeed,
            double ArmStrength,
            double ReactionTime)
        {
            public static Snapshot Take(BallPlayer p) =>
                new(p.FirstName, p.LastName, p.Age, p.Position, p.Role,
                    p.BattingHand, p.ThrowingHand,
                    p.Attributes.HeightInInches, p.Attributes.WeightInPounds, p.Attributes.SprintSpeed,
                    p.Attributes.Acceleration, p.Attributes.BatSpeed, p.Attributes.ArmStrength, p.Attributes.ReactionTime);

            public static Snapshot TakeEditable(PlayerProfileViewModel vm) =>
                new(vm.FirstName, vm.LastName, vm.Age, vm.Position, vm.Role,
                    vm.BattingHand, vm.ThrowingHand,
                    vm.HeightInInches, vm.WeightInPounds, vm.SprintSpeed,
                    vm.Acceleration, vm.BatSpeed, vm.ArmStrength, vm.ReactionTime);
        }
    }
}