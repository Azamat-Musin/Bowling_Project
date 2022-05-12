using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    /*
     * класс ведущий статистику игры: имена играющих, очки, таймер, показывающий время
     * и номер раунду
     */
     class GameStatus
    {
        static public String playerName = "Player";
        static public String botName = "Bot";

        static public int playerScore = 0;
        static public int botScore = 0;

        // эта переменная используется как счетчик времени - таймер, в методе таймера в классе Form1
        static public int playerRound = 0;
        // эта переменная служит общим для всех номером раунда
        static public int botRound = 1;

        


        
    }
}
