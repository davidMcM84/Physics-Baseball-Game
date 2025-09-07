using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physics_Baseball_Game.Models
{
    internal abstract class BallPlayer
    {
        public Guid Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public PlayerPosition Position { get; set; }
        public PlayerRole Role { get; set; }
        public BatHand BattingHand { get; set; }
        public ThrowHand ThrowingHand { get; set; }
        public PhysicalAttributes Attributes { get; set; }

        protected BallPlayer(
            string firstName, string lastName, int age, PlayerPosition position, PlayerRole role, BatHand battingHand,
            ThrowHand throwingHand, PhysicalAttributes attributes)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Position = position;
            Role = role;
            BattingHand = battingHand;
            ThrowingHand = throwingHand;
            Attributes = attributes;
        }
    }

    internal enum PlayerPosition
    {
        Pitcher,
        Catcher,
        FirstBase,
        SecondBase,
        ThirdBase,
        Shortstop,
        LeftField,
        CenterField,
        RightField,
        DesignatedHitter
    }

    internal enum PlayerRole
    {
        Batter,
        StartingPitcher,
        Reliever,
        Closer
    }

    internal enum BatHand
    {
        Left,
        Right,
        Switch
    }

    internal enum ThrowHand
    {
        Left,
        Right,
        Switch
    }

    internal readonly record struct PhysicalAttributes(
        double HeightInInches,
        double WeightInPounds,
        double SprintSpeed,
        double Acceleration,
        double BatSpeed,
        double ArmStrength,
        double ReactionTime
        );
}
