using System.Configuration;
using System.Data;
using System.Net.WebSockets;
using System.Windows;

namespace ClientUNO
{
    // подключить бд
    // добавить музыку
    // убрать мусор/ оптимизировать код/ добавить вывод в консоль на сервере при любых действиях
    // добавить таймер при ходе игрока
    // добавить сортировку карт по цветам и значения в руке 
    // добавить эфекты при ходе игрока. (сейчас просто при блоке игрок инорируется и вся логика просиходит на серверер. сам игрок не понимает почему не ходит)
    public partial class App : Application
    {
        public static AppSession Session { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var socket = ConnectToServer();

            var client = new GameClient(socket);
            var model = new GameViewModel();

            client.MessageReceived += model.ProcessMessage;

            Session = new AppSession(client, model);

            Task.Run(() => client.ReceiveLoop());
        }
        public static ClientWebSocket ConnectToServer()
        {
            var socket = new ClientWebSocket();

            var uri = new Uri("ws://localhost:5000/ws/");

            socket.ConnectAsync(uri, CancellationToken.None).GetAwaiter().GetResult();

            return socket;
        }
    }

}
