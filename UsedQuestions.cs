using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZ
{
    public class UsedQuestions
    {
        public int PlayerId { get; set; }
        public int QuestionId { get; set; }
        public UsedQuestions() { }
        public UsedQuestions(int playerId, int questionId)
        {
            PlayerId = playerId;
            QuestionId = questionId;
        }

        public List<int> readUsedQuestions(int currentPlayerId)//читаем файл б.у. вопросов - возвращаем список записей(объектов) для текущего игрока
        {
            List<int> usedQuestionsIdList = new List<int>();
            string usedQuestionsJson = File.ReadAllText("ТипаБазаДанных\\usedQuestions.txt");
            List<UsedQuestions> usedQuestionsList = JsonConvert.DeserializeObject<List<UsedQuestions>>(usedQuestionsJson);
            foreach(UsedQuestions used in usedQuestionsList.Where(used => used.PlayerId == currentPlayerId))
            {
                usedQuestionsIdList.Add(used.QuestionId);
            }
            return usedQuestionsIdList;
        }

        public List<UsedQuestions> readUsedQuestionsList()//читаем файл б.у. вопросов - возвращаем список записей(объектов)
        {
            string usedQuestionsListJson = File.ReadAllText("ТипаБазаДанных\\usedQuestions.txt");
            return JsonConvert.DeserializeObject<List<UsedQuestions>>(usedQuestionsListJson); ;
        }

        public void writeUsedQuestion(UsedQuestions usedQuestion)//добавляем в файл запись (об увиденном вопросе для игрока)
        {
            string usedQuestionJson = JsonConvert.SerializeObject(usedQuestion);
            string usedQuestionsJson = File.ReadAllText("ТипаБазаДанных\\usedQuestions.txt");//?????????NIcht????
            string usedQuestionsJsonTemp = usedQuestionsJson.Replace("]", "," + usedQuestionJson + "]");
            File.WriteAllText("ТипаБазаДанных\\usedQuestions.txt", usedQuestionsJsonTemp);
        }
        public void delUsedQuestionsByPlayerId(int currentPlayerId)//переписываем файл с данными о б.у. вопросах:
                                                                    //удаляем все записи для текущего игрока
        {
            List<UsedQuestions> usedQuestionsList = readUsedQuestionsList();//получили всю таблицу с записями
            List<UsedQuestions> usedQuestionsListFresh = usedQuestionsList.Where(used => used.PlayerId != currentPlayerId).ToList();//получили список записей
                                                                                                                 //исключив записи про одного игрока по Id
            File.WriteAllText("ТипаБазаДанных\\usedQuestions.txt", JsonConvert.SerializeObject(usedQuestionsListFresh));//записали обновленный список в файл
        }
    }
}
