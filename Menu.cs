using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QUIZ
{
    public class Menu
    {
        public Player CurrentPlayer { get; set; }
        public void StartMenu()
        {
            Player p = new Player();
        Registration:
            CurrentPlayer = p.registration();
            ConsoleKeyInfo k;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n\n\n\n\t\t\t\t\tПРИВЕТ, " + CurrentPlayer.Name);
                Console.WriteLine("\n\n" +
                    "\t\t\t\t\t0   - СМЕНА ПРОФИЛЯ\n\n" +
                    "\t\t\t\t\t1   - НОВАЯ ИГРА\n\n" +
                    "\t\t\t\t\t2   - ИСТОРИЯ\n\n" +
                    "\t\t\t\t\t3   - ТОП-20\n\n" +
                    "\t\t\t\t\tESC - ВЫХОД\n\n");
                k = Console.ReadKey();
                switch (k.KeyChar)
                {
                    case '0': goto Registration;
                    case '1': Game newGame = new Game();
                        newGame.launchGame(CurrentPlayer);
                        break;
                    case '2': History h = new History();
                        h.viewHistory(CurrentPlayer.PlayerId, CurrentPlayer.Name);
                        break;
                    case '3': CurrentPlayer.top20(CurrentPlayer);
                        break;
                    default: break;
                }

            }
            while (k.Key != ConsoleKey.Escape);
            Console.WriteLine("\t\t\tВЫХОД. СПАСИБО ЗА ИГРУ.");
        }
    }
}
