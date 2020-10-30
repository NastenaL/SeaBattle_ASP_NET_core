namespace SeaBattleASP.Models
{
    using SeaBattleASP.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class Game
    {
        public int Id { get; set; }

        public PlayingField PlayingField { get; set; }

        public GameState State { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public bool IsPl1Turn { get; set; }


        public void StartGame()
        {
            this.State = GameState.Started;

            Random gen = new Random();
            this.IsPl1Turn = gen.Next(100) < 50 ? true : false;
        }

        public void EndGame()
        {
            this.State = GameState.Finished;
        }

        public void MakeStep(Ship ship, MovementType movementType, List<DeckCell> EnemyShipsDecks)
        {
            List<DeckCell> selectedShip = new List<DeckCell>();
            
            foreach(Deck deckCell in ship.Decks)
            {
                selectedShip.Add(this.PlayingField.ShipsDeckCells.Find(s => s.Deck == deckCell));
            }

            var t = selectedShip;
            if (movementType == MovementType.Fire)
            {
                ship.Fire(EnemyShipsDecks, selectedShip);
            }
            else if(movementType == MovementType.Repair)
            {
                ship.Repair(selectedShip);
            }
            else if(movementType == MovementType.Move)
            {
                ship.Move(selectedShip);
            }
        }
    }
}
