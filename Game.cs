using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json;
using System.Threading;

namespace QUIZ
{
    public class Game
    {
        public static string playerAnswer { get; set; }
        public int currentScore { get; set; } = 0;
        public Player launchGame(Player currentPlayer)//
        {
            Console.Clear();
            Console.WriteLine("\n\n\n\n\n\n\n\n\n\t\t\t\t\t    > НОВАЯ ИГРА <\n\n\t\t\t\t\t       ПОЕХАЛИ!\n\n");
            Console.ReadKey();
            playNow(currentPlayer);
            return currentPlayer;
        }

        public static void countdown()//таймер на 7 сек
        {
            int i = 7;
            do
            {
                Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight / 4) + 2);
                Console.WriteLine(i);
                Thread.Sleep(1000);
                i--;
            }
            while (i > 0);
            Console.SetCursorPosition(Console.WindowWidth / 2, (Console.WindowHeight / 4) + 2);
            Console.WriteLine(i + " - время вышло(");
        }

        public static void waitAnswer()
        {
            playerAnswer = "";//  ответ по умолчанию либо стартовое состояние текста ответа
            ConsoleKeyInfo symbol;//символ-ключ принимаемый в процессе ответа на вопрос
            do
            {
                symbol = Console.ReadKey(true);//читаем клаву
                if (symbol.Key != ConsoleKey.Enter)//  если не - enter -, может...
                {
                    if (symbol.Key == ConsoleKey.Backspace)//- Backspace - и тут понятно..
                    {
                        if (playerAnswer.Length > 0)
                            playerAnswer = playerAnswer.Substring(0, playerAnswer.Length - 1);
                    }
                    else//если не - enter - и не - Backspace - ,тогда к ответу плюсуем символ
                            playerAnswer = playerAnswer + symbol.KeyChar;
                }
                    
            }
            while (symbol.Key != ConsoleKey.Enter && playerAnswer.Length < 11);//пока не нажали кл enter  или длина ответа не превысит 10 символов
        }

        public void playNow(Player currentPlayer)
        {
            int number = 1;//номер вопроса в текущем сеансе
            Question q = new Question();
            List<Question> currentQuestionList = q.choiceQuestion(currentPlayer.PlayerId);//дай список доступных вопросов НА ТЕКУЩИЙ СЕАНС для ЭТОГО игрока

            foreach (Question question in currentQuestionList)//перебираем вопросы для текущего сеанса: собственно -> игра
            {
                Console.Clear();
                Thread timer = new Thread(countdown);
                Thread wait = new Thread(waitAnswer);
                Console.SetCursorPosition((Console.WindowWidth / 2) - 4, (Console.WindowHeight / 4));
                Console.WriteLine("Вопрос " + number);
                Console.SetCursorPosition((Console.WindowWidth / 2) - 5, (Console.WindowHeight / 4) + 4);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(question.Text);// выводим текст вопроса и 1!, 2!, 3!
                Console.ResetColor();
                timer.Start();// 1 - стартовал отсчет времени
                wait.Start();// 2 - стартовал поток ожидания ввода ответа
                // 3 - УВИДЕЛИ вопрос -> записываем:
                UsedQuestions usedQuestion = new UsedQuestions(currentPlayer.PlayerId, question.QuestionId);//создали запись(объект) в использованных вопросах
                usedQuestion.writeUsedQuestion(usedQuestion);                                                     //пишем её(его) в ФАЙЛ б.у. вопросов
                do //процесс ответа: либо время истекло(нет ответа), либо есть ввод(неважно, правильно/неправильно, подтверждён или время истекло)
                {
                    Console.SetCursorPosition((Console.WindowWidth / 2) + 5, (Console.WindowHeight / 4) + 4);
                    Console.WriteLine("__________");//для - Backspace - (затирание)
                    Console.SetCursorPosition((Console.WindowWidth / 2) + 5, (Console.WindowHeight / 4) + 4);
                    Console.WriteLine(playerAnswer);
                    bool waitIsAlive = wait.IsAlive;
                    if (waitIsAlive == false)//если поток ожидания ввода ответа умер(введен ответ c подтверждением ч-з Enter), тогда..
                    {
                        timer.Abort();//убиваем таймер
                    }
                }
                while (timer.IsAlive == true);//следим(проверяем) за потоком ожидания ввода ответа, пока таймер не умер естественной смертью

                if (playerAnswer == "")//для универсального вывода на экран "нет ответа", и прочего
                {
                    playerAnswer = "нет ответа";
                }

                Console.SetCursorPosition((Console.WindowWidth / 2) - (("Ваш ответ: " + playerAnswer).Length / 2), (Console.WindowHeight / 4) + 4);
                Console.Write("Ваш ответ: " + playerAnswer + "  - ");

                if (playerAnswer == question.Answer)    //определение результата по вопросу 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Правильно");
                    currentScore++;// - поле-переменная Game - при создании объека Game - =0
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write("Неправильно");
                    if (playerAnswer.Length > 10)//залипание клавиши либо осознанный вред  при вводе ответа
                    {
                        string notBot = null;
                        do
                        {
                            Console.WriteLine("! Не тратьте время - ответ ГОООРАЗДО короче... ;)\n");
                            Console.Write("Введите \"-1\" чтобы продолжить: ");
                            notBot = Console.ReadLine();
                            Console.Clear();
                        }
                        while (notBot != "-1");
                        Console.Clear();
                    }
                }
                Console.ResetColor();
                Console.SetCursorPosition((Console.WindowWidth / 2) - 5 - 4, (Console.WindowHeight / 4) + 6);
                Console.WriteLine(currentScore + " - правильно из " + number); number++;
                Console.ReadKey();
            }
            //результат ИГРЫ:
            currentPlayer.Sessions++;//текущему игроку плюсуем общее кол-во проведенных игр
            currentPlayer.Points = currentPlayer.Points + currentScore;//текущему игроку плюсуем общее кол-во набранных очков
            currentPlayer.Performance = ((double)currentPlayer.Points / (currentPlayer.Sessions * 10)) * 100;//ИТОГОВАЯ производительность(кпд) текущего игрока
            currentPlayer.Rating = currentPlayer.calculateRating(currentPlayer);//спрашиваем ИТОГОВЫЙ рейтинг (место) в таблице результатов
            History newEntry = new History(currentPlayer.PlayerId, currentPlayer.Sessions, currentScore.ToString() + " из 10");//создаем запись для Истории
                                                                                                                               //об этом игроке (id), о номере (по счету) его игры, и о результате этой игры
            newEntry.addEntryToHistoryFile(newEntry);//добавляем запись в Файл History
            Console.WriteLine(
                "\n      Игрок: " + currentPlayer.Name +
                "\nВсего очков: " + currentPlayer.Points +
                "\n  Всего игр: " + currentPlayer.Sessions +
                "\nСредний КПД: " + currentPlayer.Performance +
                "\n      Место: " + currentPlayer.Rating);
            Console.WriteLine("Игра завершена.");
            Console.ReadKey();
        }
    }
}
