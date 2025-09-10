using System;

namespace Physics_Baseball_Game.Models
{
    internal sealed class BasicPlayer : BallPlayer
    {
        public BasicPlayer(
            string firstName,
            string lastName,
            int age,
            PlayerPosition position,
            PlayerRole role,
            BatHand batHand,
            ThrowHand throwHand,
            PhysicalAttributes attributes,
            List<Pitch> pitches)
            : base(firstName, lastName, age, position, role, batHand, throwHand, attributes, pitches)
        {
        }
    }
}