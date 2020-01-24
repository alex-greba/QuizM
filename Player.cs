using DocumentFormat.OpenXml.ExtendedProperties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZ
{
    public class Player
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public int Points { get; set; }
        public int Sessions { get; set; }
        public double Performance { get; set; }
        public Player() { }
        public Player(int playerId, string name, int rating, int points = 0, int sessions = 0, double performance = 0)
        {
            PlayerId = playerId;
            Name = name;
            Rating = rating;
            Points = points;
            Sessions = sessions;
            Performance = performance;
        }

        public List<Player> readListPlayer()//читаем Файл с игроками - возвращаем список игроков
        {
            string playerListJson = File.ReadAllText("ТипаБазаДанных\\playerList.txt");//c:\Users\GAW\source\repos\QUIZ\bin
            return JsonConvert.DeserializeObject<List<Player>>(playerListJson);// --------------------------------------------- => RESULT
        }

        public void writeListPlayer(List<Player> playerList)//перезаписываем Всех игроков в Файле с игроками
        {
            string playerListJson = JsonConvert.SerializeObject(playerList);
            File.WriteAllText("ТипаБазаДанных\\playerList.txt", playerListJson);// ------------------------------------------- => RESULT
        }
        public void writeOnePlayer(Player playerOne)//дописываем в Файле Одного игрока
        {
            string playerOneJson = JsonConvert.SerializeObject(playerOne);
            string playerListJson = File.ReadAllText("ТипаБазаДанных\\playerList.txt");
            string playerListJsonTemp = playerListJson.Replace("]", "," + playerOneJson + "]");
            File.WriteAllText("ТипаБазаДанных\\playerList.txt", playerListJsonTemp);// ---------------------------------------- => RESULT
        }

        public Player registration()
        {
        reTry: Console.Clear();
            Console.Write("\n\n\n\n\n\n\n\n\t\t\t\t\t      LOGIN  >>>  ");
            string login = Console.ReadLine();//вводим логин
            if (login == "" || login.Length > 20)//некорректный логин(пустой или длинее 20 символов)
            {
                Console.WriteLine("\n\t\t\t\t\tLOGIN должен состоять хотя бы из одного символа..." +
                    "\n\t\t\t\t\tИ не быть длинее 20 символов...\n\n\t\t\t\t\tOk?"); Console.ReadKey();
                goto reTry;
            }
            List<Player> playerList = readListPlayer();//получаем список зарегистрированных игроков
            foreach (Player existingPlayer in playerList)
            {
                if (existingPlayer.Name == login)//введённый логин совпадает с существующим игроком -> вход по этому логину
                {
                    return existingPlayer;//вернули в меню существующего игрока -------------------------- => RESULT
                }
            }
            //Console.WriteLine("Кол-во уже сущ-х игроков: " + playerList.Count); Console.ReadKey();
            //Console.WriteLine("MAX значение рейтинга: " + playerList.Max(p => p.Rating)); Console.ReadKey();
            Player newPlayer = new Player(playerList.Last().PlayerId + 1, login, playerList.Max(p => p.Rating) + 1);//логин не нашли -> создаем нового игрока
            // (Id = id последнего в списке + 1; Логин; Rating = max значение рейтинга в списке + 1; Поумолчанию в конструкторе: points = 0, sessions = 0)
            playerList.Add(newPlayer);//добавляем в сисок (не в Файл) нового игрока
            writeOnePlayer(newPlayer);//дописываем в Файл нового игрока
            return newPlayer;//вернули в меню нового игрока ---------------------------------------------- => RESULT
        }

        public int calculateRating(Player currentPlayer)
        {
            List<Player> playerList = readListPlayer();
            int existingPlayerINDEX = playerList.FindIndex(p => p.PlayerId == currentPlayer.PlayerId);//находим индекс
            double outdatedPerformance = playerList[existingPlayerINDEX].Performance;//записываем старый КПД текущего игрока
            playerList[existingPlayerINDEX] = currentPlayer;//заменили существующего игрока текущим (тот же юзер с
                                                            //обновлёнными данными по игре, но с устаревшим значением рейтинга)

            if (playerList[existingPlayerINDEX].Performance > outdatedPerformance)//если КПД Увеличился
            {
                /*
                var list1 = from p in playerList where p.Performance < playerList[existingPlayerINDEX].Performance select p;
                var list2 = from p in playerList where p.Rating < playerList[existingPlayerINDEX].Rating select p;
                var listIntersect = list1.Intersect(list2);// пересечение списков
                */
                int countUp = 0;//количество позиций, на которое повысится рейтинг (уменьшится значение) у текущего (обрабатываемого) игрока
                foreach (Player existingPlayer in playerList.Where(existingPlayer =>              //игроки, рейтинг которых будет скорректирован
                     existingPlayer.Performance < playerList[existingPlayerINDEX].Performance &&
                     existingPlayer.Rating < playerList[existingPlayerINDEX].Rating))
                {
                    existingPlayer.Rating++;//понижаем рейтинг на одну позицию (увеличиваем значение) - корректируем в спискке
                    countUp++;//у скольки игроков понизили рейтинг - на столько вырастет рейтинг текущего (обрабатываемого) игрока
                }
                playerList[existingPlayerINDEX].Rating = playerList[existingPlayerINDEX].Rating - countUp;//повышаем рейтинг (уменьшаем значение)
                                                                                                          //текущего (обрабатываемого) игрока - корректируем в спискке ----- => RESULT
            }
            else if (playerList[existingPlayerINDEX].Performance < outdatedPerformance)//если КПД Уменьшился
            {
                int countDown = 0;//количество позиций, на которое понизится рейтинг (увеличится значение) у текущего (обрабатываемого) игрока
                foreach (Player existingPlayer in playerList.Where(existingPlayer =>              //игроки, рейтинг которых будет скорректирован
                    existingPlayer.Performance > playerList[existingPlayerINDEX].Performance &&
                    existingPlayer.Rating > playerList[existingPlayerINDEX].Rating))
                {
                    existingPlayer.Rating--;//повышаем рейтинг на одну позицию (уменьшаем значение) - корректируем в спискке
                    countDown++;//у скольки игроков повысили рейтинг - на столько упадет рейтинг текущего (обрабатываемого) игрока
                }
                playerList[existingPlayerINDEX].Rating = playerList[existingPlayerINDEX].Rating + countDown;//понижаем рейтинг (увеличиваем значение)
                                                                                                            //текущего (обрабатываемого) игрока - корректируем в спискке ----- => RESULT
            }
            //else{ если КПД неизменился - делать нечего}

            writeListPlayer(playerList);//обновляем весь список игроков новыми данными
            return playerList[existingPlayerINDEX].Rating;
        }

        public void top20(Player currentPlayer)
        {
            Console.Clear();
            Console.WriteLine("\t      +--------+----------------------+\n" +
                String.Format("\t   {0,-2} | {1,6} | {2,-20} |",
                    "",
                    "МЕСТО",
                    "       ИГРОК") +
                    "\n\t      +--------+----------------------+");
            string str;
            foreach (Player existingPlayer in readListPlayer().//для тех, у кого рейтинг <= 20 и Вас, если не попали в ТОР
                Where(existingPlayer =>
                existingPlayer.Rating <= 20 || existingPlayer.PlayerId == currentPlayer.PlayerId).
                OrderBy(existingPlayer => existingPlayer.Rating))
            {
                str = String.Format("\t   {0,-2} | {1,6} | {2,-20} |",
                    "",
                    existingPlayer.Rating,
                    existingPlayer.Name);
                if (existingPlayer.PlayerId == currentPlayer.PlayerId)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    if (existingPlayer.Rating > 20)
                    {
                        Console.WriteLine(String.Format("\t   {0,-2} | {1,6} | {2,-20} |", "", "...", "..."));
                    }
                    str = "\t +-----------------------------------------+\n" + 
                        String.Format("\t | {0,-2} | {1,6} | {2,-20} | ВЫ |",
                        "ВЫ",
                        existingPlayer.Rating,
                        existingPlayer.Name) +
                        "\n\t +-----------------------------------------+";
                }
                Console.WriteLine(str);
                Console.ResetColor();
            }
            Console.WriteLine("\t      +--------+----------------------+");
            Console.ReadKey();
        }
    }
}
