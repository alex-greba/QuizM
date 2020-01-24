using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZ
{
    public class History
    {
        public int IdPlayer { get; set; }
        public int SessionNumber { get; set; }  //номер игры (порядковый для данного игрока)
        public string Result { get; set; }      //типа "8 из 10"
        //можно добавить и вопрос (id), и ответ игрока 
        public History() { }
        public History(int idPlayer, int sessionNumber, string result)
        {
            IdPlayer = idPlayer;
            SessionNumber = sessionNumber;
            Result = result;
        }

        public string readHistoryList()//читаем Базу, возвращаем стринг (по всем игрокам)
        {
            string historyListJson = File.ReadAllText("ТипаБазаДанных\\History.txt");
            return historyListJson;
        }
        public void addEntryToHistoryFile(History newEntry)//добавляем новую запись(объект) в Файл 
        {
            string addNewEntryJson = readHistoryList().Replace("]", "," + JsonConvert.SerializeObject(newEntry) + "]");
            File.WriteAllText("ТипаБазаДанных\\History.txt", addNewEntryJson);
        }

        public void viewHistory(int currentPlayerId, string currentPlayerName)//смотрим записи для определенного игрока
        {
            Console.Clear();
            List<History> entryOfHistoryList = JsonConvert.DeserializeObject<List<History>>(readHistoryList());//все записи
            string str;
            Console.WriteLine("\t\tИгрок: " + currentPlayerName + "\n");
            Console.WriteLine("\t\t+---------+-------------+");
            Console.WriteLine("\t\t|  № ИГРЫ |  РЕЗУЛЬТАТ  |");
            Console.WriteLine("\t\t+---------+-------------+");
            foreach (History entry in entryOfHistoryList.Where(entry => entry.IdPlayer == currentPlayerId))//отобрали записи по Id игрока
            {
                str = String.Format("\t\t| {0,7} |  {1,9}  |",
                entry.SessionNumber,
                entry.Result);
                Console.WriteLine(str);
            }
            Console.WriteLine("\t\t+---------+-------------+");
            Console.ReadKey();
        }
    }
}
