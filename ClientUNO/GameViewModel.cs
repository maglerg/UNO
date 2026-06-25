using Shared_Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;

namespace ClientUNO
{
    class GameViewModel
    {
        private GameState state { get; set; }
        private readonly GameClient client;

        public void ProcessMessage(ServerMessage msg)
        {
            switch (msg)
            {
                case ErrorMessage errorMsg:
                    throw new ArgumentException(errorMsg.TextError);
                // в интерфейсе должно быть окно где можно ловить хавать сообщения
                case GameStateMessage gsMsg:
                    state.Update(gsMsg);
                    //обновить окно интерфейса
                    break;

                default:
                    throw new ArgumentException("Unknown message type");
            }
        }
        public bool CanDrawCard =>
        state.IsMyTurn;

        public bool CanPlayCard(Card card)
        {
            // проверка логики как на сервере
        }

        public async Task PlayCard(Card card)
        {
            if (CanPlayCard)
            {
                // отправляем сообщение что мы хотим сыграть эту карту
                // обновялем интерфейс
            }
            // пишем что не можем сыграть эту карту

        }
    }
}
