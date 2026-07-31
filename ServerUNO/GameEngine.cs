using Microsoft.VisualBasic;
using Shared_Library;
using System;
using System.Reflection;

namespace Server
{
    public class GameEngine
    {

        Random random = new Random();
        public List<Player> players;
        public int currentPlayer;
        public int direction = 1;
        public Deck deck;
        public Card lastCard;
        public List<Card> DiscardPile { get; set; } // сброс
        public ColorCard currentColor;
        Player winner = null;
        public bool isEndGame = false;


        public GameEngine()
        {
            players = new List<Player>();
            deck = new Deck();
            lastCard = new Card();
            DiscardPile = new List<Card>();
            isEndGame = false;
        }
        public void StartGame()
        {
            isEndGame = false;
            deck.CreateDeck();

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < players.Count; j++)
                {
                    DrawCard(players[j]);
                }
            }

            lastCard = deck.GiveCard();
            if (lastCard.Color == ColorCard.Black)
                currentColor = (ColorCard)random.Next(Enum.GetValues(typeof(ColorCard)).Length - 1);
            currentPlayer = random.Next(players.Count);
        }
        public void TakeCard(int player) // взять карту
        {
            if (player == currentPlayer)
            {
                DrawCard(players[currentPlayer]);

                NextPlayer();

                ApplyAction(players[currentPlayer]);
            }
            else throw new Exception("It's not your move.");
        }
        public void PlayCard(int player, Card card, ColorCard? chosenColor = null) // сыграть карту
        {
            if (player != currentPlayer)
                throw new Exception("It's not your move.");

            if (!CheckTurn(card, chosenColor))
                throw new Exception("Uncorrected card.");


            players[currentPlayer].PlayerCards.Remove(card);

            DiscardPile.Add(lastCard);
            lastCard = card;
            if(players[currentPlayer].PlayerCards.Count == 0)
            {
                EndGame(players[currentPlayer]);
                isEndGame = true;
            }

            NextPlayer();

            ApplyAction(players[currentPlayer]);
        }
        private void ApplyAction(Player player)
        {
            switch (player.Action)
            {
                case PlayerAction.Skip:
                    NextPlayer();
                    player.Action = PlayerAction.None;
                    break;

                case PlayerAction.Two:
                    for (int i = 0; i < 2; i++)
                        DrawCard(players[currentPlayer]);
                    player.Action = PlayerAction.None;
                    break;


                case PlayerAction.Four:
                    for (int i = 0; i < 4; i++)
                        DrawCard(players[currentPlayer]);
                    player.Action = PlayerAction.None;
                    break;
            }
        }
        public void PushUno(Player player)
        {
            player.UnoPressed = true;

            if (player != players[currentPlayer])
            {
                for (int i = 0; i < 2; i++)
                {
                    DrawCard(player);
                }
            }
        }
        public bool CheckTurn(Card card, ColorCard? chosenColor = null)
        {
            // Черные карты
            if (card.Color == ColorCard.Black)
            {
                if (!chosenColor.HasValue)
                    return false;

                currentColor = chosenColor.Value;

                if (card.Type == TypeCard.Wild)
                    return true;

                if (card.Type == TypeCard.WildDrawFour)
                {
                    players[GetNextPlayerIndex()].Action = PlayerAction.Four;
                    return true;
                }

                return false;
            }

            if (lastCard.Color == ColorCard.Black)
            {
                if (card.Color != currentColor)
                    return false;
            }
            else
            {
                if (card.Color != lastCard.Color)
                {
                    if (card.Type != lastCard.Type)
                        return false;


                    if (card.Type == TypeCard.Number &&
                        card.Value != lastCard.Value)
                    {
                        return false;
                    }
                }
            }

            switch (card.Type)
            {
                case TypeCard.Skip:
                    players[GetNextPlayerIndex()].Action = PlayerAction.Skip;
                    break;


                case TypeCard.Reverse:
                    direction *= -1;
                    if (players.Count == 2)
                        players[GetNextPlayerIndex()].Action = PlayerAction.Skip;
                    break;


                case TypeCard.DrawTwo:
                    players[GetNextPlayerIndex()].Action = PlayerAction.Two;
                    break;
            }


            return true;
        }
        public void DrawCard(Player player) //выдать карты игроку
        {
            Card card = new Card();
            try
            {
                player.PlayerCards.Add(deck.GiveCard());
            }
            catch (Exception e)
            {
                if (e.Message == "Deck is empty")
                {
                    foreach (Card temp in DiscardPile)
                    {
                        deck.Cards.Add(temp);
                    }
                    deck.Cards = deck.Cards.OrderBy(x => random.Next()).ToList();

                    if (deck.Cards.Count == 0)
                    {
                        EndGame(null);
                        isEndGame = true;
                    }
                        
                }
                else

                    Console.WriteLine(e.Message);
            }
        }
        private int GetNextPlayerIndex() // получение индекса сл игрока
        {
            int index = currentPlayer + direction;

            if (index < 0)
                index = players.Count - 1;

            if (index >= players.Count)
                index = 0;

            return index;
        }
        public void NextPlayer() // переход к сл игроку
        {
            currentPlayer += direction;
            if (currentPlayer < 0)
                currentPlayer = players.Count - 1;
            else if (currentPlayer >= players.Count)
                currentPlayer = 0;
        }
        public void EndGame(Player winner) // конец игры 
        {
            this.winner = winner;
            RatingPlayers(winner);
            for(int i = 0; i < players.Count; i++)
            {
                players[i].CountGames += 1;

                if(players[i] == winner )
                    players[i].Wins += 1;
            }
        }
        public void RatingPlayers(Player winner = null) // подсчет статистики
        {
            foreach (var player in players)
            {
                if (player == winner)
                {
                    player.Points = 0;
                    continue;
                }

                int score = 0;

                foreach (var card in player.PlayerCards)
                {
                    score += GetScore(card);
                }

                player.Points = score;
            }
        }
        public int GetScore(Card card) // получение номинала карты
        {
            switch (card.Type)
            {
                case TypeCard.Number:
                    return (int)card.Value;

                case TypeCard.Skip:
                case TypeCard.Reverse:
                case TypeCard.DrawTwo:
                    return 20;

                case TypeCard.Wild:
                case TypeCard.WildDrawFour:
                    return 50;

                default:
                    return 0;
            }
        }
    }
}



