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
        int winner = -1;


        public GameEngine()
        {
            players = new List<Player>();
            deck = new Deck();
            lastCard = new Card();
            DiscardPile = new List<Card>();
        }
        public void StartGame()
        {
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
                while (CheckAction())
                {
                    NextPlayer();
                }
            }
            else throw new Exception("Player is not active");
        }
        public void PlayCard(int player, Card card, ColorCard? chosenColor = null) // сыграть карту
        {
            if (player == currentPlayer)
            {
                if (CheckTurn(card, chosenColor))
                {
                    players[currentPlayer].PlayerCards.Remove(card);
                    DiscardPile.Add(lastCard);
                    lastCard = card;

                    //проверка уно

                    if (players[player].PlayerCards.Count == 0)
                        EndGame(player);

                    NextPlayer();
                    while (CheckAction())
                    {
                        NextPlayer();
                    }
                }
                else throw new Exception("Uncorrected card or color is not selected");

            }
            else throw new Exception("Player is not active");
        }

        public void PushUno(int indexPush) // нажать кнопку уно
        {
            if (indexPush != currentPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    DrawCard(players[indexPush]);
                }
            }
        }

        public bool CheckAction() // проверка + выполнение условий action
        {
            if (players[currentPlayer].Action != PlayerAction.None)
            {
                if (players[currentPlayer].Action == PlayerAction.Two)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        DrawCard(players[currentPlayer]);
                    }
                    players[currentPlayer].Action = PlayerAction.None;
                    return true;
                }
                else if (players[currentPlayer].Action == PlayerAction.Four)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        DrawCard(players[currentPlayer]);
                    }
                    players[currentPlayer].Action = PlayerAction.None;
                    return true;
                }
                else if (players[currentPlayer].Action == PlayerAction.Skip)
                {
                    players[currentPlayer].Action = PlayerAction.None;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        public bool CheckTurn(Card card, ColorCard? chosenColor = null) // применение карты + выбранного цвета
        {
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
                else
                    return false;
            }

            else if (lastCard.Color == ColorCard.Black)
            {
                if (card.Color == currentColor)
                    return true;
                else return false;
            }
            if (card.Color == lastCard.Color)
                return true;
            if (card.Type == lastCard.Type)
            {
                if (card.Type == TypeCard.Number)
                {
                    if (card.Value != CardValue.None && card.Value == lastCard.Value)
                        return true;
                    else return false;
                }
                if (card.Type == TypeCard.Skip)
                {
                    players[GetNextPlayerIndex()].Action = PlayerAction.Skip;
                    return true;
                }
                if (card.Type == TypeCard.Reverse)
                {
                    direction *= -1;
                    return true;
                }
                if (card.Type == TypeCard.DrawTwo)
                {
                    players[GetNextPlayerIndex()].Action = PlayerAction.Two;
                    return true;
                }
                return false;
            }
            else return false;
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
                        EndGame();
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
        public void EndGame(int winner = -1) // конец игры 
        {
            this.winner = winner;
            List<Player> rating = RatingPlayers(players[winner]);
            //дальше если нужны данные для бд их можно взять отсюда
        }
        public List<Player> RatingPlayers(Player winner = null) // подсчет статистики
        {
            int score = 0;
            List<(Player player, int score)> rating = new();
            List<Player> list = new List<Player>();

            foreach (var player in players)
            {
                score = 0;
                if (player == winner)
                {
                    rating.Add((player, 0));
                    continue;
                }

                foreach (var card in player.PlayerCards)
                    score += GetScore(card);

                rating.Add((player, score));
            }
            rating.Sort((a, b) => a.score.CompareTo(b.score));
            for (int i = 0; i < rating.Count; i++)
            {
                list.Add(rating[i].player);
            }

            return list;
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



