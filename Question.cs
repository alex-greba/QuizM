using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZ
{
    [Serializable]
    public class Question
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public Question() { }

        public int rndQuestionId()
        {
            Random rnd = new Random();
            return rnd.Next(1, 81);
        }

        public List<Question> choiceQuestion(int currentPlayerId)//подбираем вопросы для игрока на текущий сеанс
        {
            int needToAdd = 10;//количество вопросов, которые НАДО отобрать в список на текущий сеанс (на входе все 10 вопросов - сеанс игры)
            int canToAdd;//количество вопросов, которые МОЖНО отобрать в список на текущий сеанс (доступных)
            UsedQuestions u = new UsedQuestions();//Б.у. вопрос 
            List<int> usedQuestionsIdList = u.readUsedQuestions(currentPlayerId);//получаем список Id вопросов, заблокированных для выбора в текущем сеансе
                                                                                 //на которые уже отвечал ЭТОТ игрок (по Id этого игрока)
                                                                                 //в FORе будет пополняться вопросами на текущий сеанс
            List<Question> currentQuestionList = new List<Question>();//создаем список вопросов НА ТЕКУЩИЙ СЕАНС (сюда будем добавлять подходящие вопросы)
            canToAdd = totalNumberQuestions() - usedQuestionsIdList.Count;//количество всех вопросов в базе - количество заблокированных для игрока
            if (canToAdd <= needToAdd)//если в базе недостаточно доступных вопросов на текущий сеанс (или осталось как раз столько - сколько надо)
                                      //добавим в первую очередь оставшиеся (что бы ни один вопрос не остался без внимания), а далее - далее..
            {
                List<int> allQuestionsIdList = new List<int>();//создаем список Id всех вопросов из базы
                readQuestions().ForEach(q => allQuestionsIdList.Add(q.QuestionId));//наполняем список Id-шниками
                //List<int> allQuestionsIdList = new List<int>(readQuestions().Select(q => q.QuestionId));//наверное, то же cработает
                List<int> availableQuestionsIdList = allQuestionsIdList.Except(usedQuestionsIdList).ToList();//создаем <список Id> оставшихся доступных вопросов =
                                                                                                             //из списка всех исключаем список использованных
                usedQuestionsIdList.Clear();//очищаем список <Id> заблокированных вопросов для текущего игроку, полученный из ФАЙЛА
                foreach (int availableQuestionId in availableQuestionsIdList)//добавляем Все доступные вопросы без рандома
                                                                            //в список вопросов НА ТЕКУЩИЙ СЕАНС
                                                                            //(всё равно попадут все оставшиеся) - так быстрее..., кажется
                {
                    currentQuestionList.Add(readQuestionById(availableQuestionId));//добавляем валидный вопрос в список вопросов НА ТЕКУЩИЙ СЕАНС для этого игрока
                    usedQuestionsIdList.Add(availableQuestionId);//пополняем список Id вопросов (но не файл), заблокированных для выбора в текущем сеансе
                }
                Console.WriteLine("\t\t\t\tЗакончились уникальные вопросы...\n" +
                    "\t\t\t\tВопросы, которые вы уже видели будут снова доступны.\n" +
                    "\t\t\t\tПродолжить.. > any key <");
                Console.ReadKey();
                u.delUsedQuestionsByPlayerId(currentPlayerId);//удаляем в файле б.у. вопросов все записи для данного игрока
                needToAdd = needToAdd - canToAdd;//определяем, сколько ещё необходимо добавить вопросов на текущий сеанс 
            }

            //собираем доступные вопросы для текущего сеанса ИЛИ ДОсобираем (рандомом)...
            for (int i = 1; i <= needToAdd; i++)
            {
            InvalidQuestion://____________________________________________________________________________________________________________________GOTO_1
                int tempQuestionId = rndQuestionId();//рандомный номер (Id) вопроса = (генерируем)
                foreach (int usedQuestionsId in usedQuestionsIdList)//проверяем сгенерированный Id на наличие в уже заблокированных вопросов 
                {
                    if (tempQuestionId == usedQuestionsId)//рандомный Id = Id заблокированного:
                    {
                        goto InvalidQuestion;// генерируем новый и снова проверяем_________________________________________________________________GOTO_1
                    }
                }//вышли - вопрос подходит
                currentQuestionList.Add(readQuestionById(tempQuestionId));//добавляем валидный вопрос в список вопросов НА ТЕКУЩИЙ СЕАНС для этого игрока
                usedQuestionsIdList.Add(tempQuestionId);//пополняем список Id вопросов (но не файл), заблокированных для выбора в текущем сеансе
            }
            return currentQuestionList;//отправляем список с 10-ю готовыми вопросами для текущего сеанса
        }
        public List<Question> readQuestions()//получаем список всех вопросов в базе
        {
            string QuestionsJson = File.ReadAllText("ТипаБазаДанных\\Questions.txt");
            return JsonConvert.DeserializeObject<List<Question>>(QuestionsJson);

        }
        public Question readQuestionById(int temp)//получаем вопрос из базы по его Id
        {
            return readQuestions().Find(question => question.QuestionId == temp);
        }
        public int totalNumberQuestions()//получаем количество всех вопросов в базе (Id последнего)
        {
            return readQuestions().Last().QuestionId;
        }



    }
}
