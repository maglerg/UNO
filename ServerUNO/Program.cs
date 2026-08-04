using Npgsql;
using Server;
using ServerUNO.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerUNO
{
    class Program
    {
        // если один из игроков вышел то игра заканчивается и игрок возвращается в лобби комнаты + появляется сообщение что игрок отключился
        //при броске дикой карты 4 не понятно какой цвет выбал игрок тк видна только надпись что игрок берет 4 карты
        // доделать интерфейс с окном игры (имена и норм количество карт)
        // в лобби баг если хост игры выходит нет обозначения нового хоста и нет кнопки старт
        // сделать настройки для игры 
        //после одной игры если нажать на ввернуться в лобби и запустить игру заново. откроется  2 окна игры
        // если игрок нажимает уно самому себе то он берет 2 карты XD
        static async Task Main(string[] args)
        {
            AccountManager accountManager = new AccountManager();
            RoomManager roomManager = new RoomManager(accountManager);

            ServerManager server = new ServerManager(roomManager);
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Server is shutting down");
            };
            await server.Start();
        }
    }
}
