using System.Configuration;
using System.Data;
using System.Net.WebSockets;
using System.Windows;

namespace ClientUNO
{
    // сделать наконец игровое поле
    // подключить бд
    // добавить музыку
    // убрать мусор/ оптимизировать код/ добавить вывод в консоль на сервере при любых действиях
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

            new MainWindow().Show();
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
