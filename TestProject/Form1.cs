using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    public partial class Form1 : Form
    {
        // картинка, которая будет фоном формы, чтобы не было мерцания
        Bitmap bitmap;

        // экземпляр класса Menu
        Form menuF;
        // шар
        Ball ball;
        // позиции мыши
        Point mouseLocation1;
        Point mouseLocation2;
        // нажата ли кнопка мыши
        bool mouseDown;
        // массив кегель
        Ball[] arrPins = new Ball[10];
        // рисовка графики вне Paint
        Graphics gg;
        // шрифт и его размеры
        Font font;
        // переменная для понимания кто ходит: человек или бот
        int at = 1;
        Random rnd;

        // переменные отвечающие за границы игрового поля
        int leftGameWallX1;
        int leftGameWallY1;
        int rightGameWallX1;
        int rightGameWallY1;
        int leftGameWallX2;
        int leftGameWallY2;
        int rightGameWallX2;
        int rightGameWallY2;

        // для черных линий 
        int blackLineHeight;
        int dHeight;



        // передача экземляра формы, от которой создалась эта форма(т.е. форма Menu)
        public Form1(Menu menuForm)
        {
            // двойная буфферизация - создание следующего изображения(подгрузка) до отображения
            // помогает устранить мерцание, но тут мерцание устранено благодаря:
            //  двойной буфферизации
            //  фоном является Bitmap
            //  использование массива кегль, а не списка
            //  работа с таймером, как компонентом формы
            this.DoubleBuffered = true;

            // инициализация
            InitializeComponent();

            // экзмепляр класса Menu, нужен, чтобы потом вернуться в предыдущее окно и
            //      сделать предыдущее окно невидимым во время активного окна(текущего), т.е. Form1
            menuF = menuForm;

            // утсановка размеров, цвета фона, расположения, отключение рамок окна
            // задание размеров именно окна(без учета рамок), ибо this.Size = new Size(); это задание размеров окна с учетом рамок
            this.ClientSize = new Size(800, 700);
            // задание цвета фона у формы
            this.BackColor = ColorTranslator.FromHtml("#FFFFF0");
            // расположение по центру экрана устройства
            this.StartPosition = FormStartPosition.CenterScreen;
            // отсутсвие рамок
            this.FormBorderStyle = FormBorderStyle.None;

            // инициализация переменных
            // рисование на форме в этом методе
            this.Paint += Form1_Paint;
            // объявление компонентов и переменных в этом методе
            this.Load += Form1_Load;
            // сохранение отношений приложения во время изменения размеров окна в этом методе
            this.Resize += Form1_Resize;
            // методы управления мышью: отжата кнопка, зажата кнопка, курсор передвигается
            this.MouseUp += Form1_MouseUp;
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
        }

        // вернуться в главное меню
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            menuF.Visible = true;


        }

        // Принудительно закрыть игру
        private void button2_Click(object sender, EventArgs e)
        {

            Application.Exit();
        }

        // ДЛЯ ТЕБЯ
        // так как окно в фикс. размере, то этот метод не применяется
        //
        // перерисовка окна в случае изменения размеров окна
        private void Form1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        // загрузка компонентов формы
        private void Form1_Load(object sender, EventArgs e)
        {
            // ДЛЯ ТЕБЯ
            // то, что написал в коменте тут это вроде точно, но не точно
            //
            // работа с графикой в некоторых местах, где это происходит вне метода Paint
            gg = this.CreateGraphics();

            // инициализация экземпляра класса Random(), будет использоваться для задания направления и коэф. скорости шара во время хода бота
            rnd = new Random();
            // ДЛЯ ТЕБЯ
            // вся форма является объектом Bitmap, т.е. картинка, чтобы избежать мерцания хехехехе
            //
            bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            // ДЛЯ ТЕБЯ
            // этот блок был в Paint, но тут вроде пологичнее и работает)
            // я чет глянул, вроде больше нигде не юзается, а менять мне лень, так что тут останется
            // а вру, юзается еще же, так что все ок
            //
            // переменные которые хранят координаты 2 зеленых вертикальных линий(начало и конец)
            leftGameWallX1 = this.ClientSize.Width / 4;
            leftGameWallY1 = 0;
            leftGameWallX2 = this.ClientSize.Width / 4;
            leftGameWallY2 = this.ClientSize.Height;
            rightGameWallX1 = this.ClientSize.Width - this.ClientSize.Width / 4;
            rightGameWallY1 = 0;
            rightGameWallX2 = this.ClientSize.Width - this.ClientSize.Width / 4;
            rightGameWallY2 = this.ClientSize.Height;

            // переменные для горизонтальных линий черного цвета и дельта, чтобы ровно их распологать отн-но друг друга
            blackLineHeight = this.ClientSize.Height / 7;
            dHeight = 50;

            // ДЛЯ ТЕБЯ
            // это было в Paint, см выше че писал
            //

            // шрифт и размер слов в окне игры(имя, очки и тд)
            font = new Font("Times New Roman", 13.0f);

            // кнопки вернуться в меню и выйти из игры(закрыть приложение)
            button1.Size = new Size(140, 50);
            button2.Size = new Size(140, 50);

            // будущее расположение кнопок, задается отн-но экрана 
            Point p1 = new Point((this.ClientSize.Width / 4 - button1.Width) / 2, 10);
            Point p2 = new Point((this.ClientSize.Width / 4 - button2.Width) / 2 +
                                    (this.ClientSize.Width - this.ClientSize.Width / 4), 10);
            // фон кнопок в цвет фона
            button1.BackColor = ColorTranslator.FromHtml("#FFFFF0");
            button2.BackColor = ColorTranslator.FromHtml("#FFFFF0");
            // задание расположения кнопкам
            button1.Location = p1;
            button2.Location = p2;

            // инициализация шара, задание расположения
            ball = new Ball(60, 10, new Vector2D(400, 560));

            //инициализация списка кеглей, задание расположения
            for (int i = 0; i < 10; i++)
            {
                if (i < 4)
                {
                    Ball b = new Ball(35, 2, new Vector2D(new Point(295 + i * 70, 100)));
                    arrPins[i] = b;
                }
                else if (i < 7)
                {
                    Ball b = new Ball(35, 2, new Vector2D(new Point(330 + (i - 4) * 70, 150)));
                    arrPins[i] = b;   
                }
                else if (i > 6 && i < 9)
                {
                    Ball b = new Ball(35, 2, new Vector2D(new Point(365 + (i - 7) * 70, 200)));
                    arrPins[i] = b;
                }
                else
                {
                    Ball b = new Ball(35, 2, new Vector2D(new Point(400, 250)));
                    arrPins[i] = b;
                }
            }

            // интервал и активация таймера
            timer1.Interval = 10;
            timer1.Enabled = true;
        }

        // отрисовка компонентов формы 
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // зеленые линии разметки поля
            e.Graphics.DrawLine(new Pen(Color.Green, 4), new Point(leftGameWallX1, leftGameWallY1), new Point(leftGameWallX2, leftGameWallY2));
            e.Graphics.DrawLine(new Pen(Color.Green, 4), new Point(rightGameWallX1, rightGameWallY1), new Point(rightGameWallX2, rightGameWallY2));

            // черные линии разметки для текста
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(0, blackLineHeight), new Point(leftGameWallX1, blackLineHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(rightGameWallX1, blackLineHeight), new Point(ClientSize.Width, blackLineHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(0, blackLineHeight + dHeight), new Point(leftGameWallX1, blackLineHeight + dHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(rightGameWallX1, blackLineHeight + dHeight), new Point(ClientSize.Width, blackLineHeight + dHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(0, blackLineHeight + 2 * dHeight), new Point(leftGameWallX1, blackLineHeight + 2 * dHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(rightGameWallX1, blackLineHeight + 2 * dHeight), new Point(ClientSize.Width, blackLineHeight + 2 * dHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(0, blackLineHeight + 3 * dHeight), new Point(leftGameWallX1, blackLineHeight + 3 * dHeight));
            e.Graphics.DrawLine(new Pen(Color.Black, 4), new Point(rightGameWallX1, blackLineHeight + 3 * dHeight), new Point(ClientSize.Width, blackLineHeight + 3 * dHeight));

            // полосы черной дыры и границ со стороны игрока
            e.Graphics.DrawLine(new Pen(Color.Chocolate, 4), new Point(leftGameWallX1, 60), new Point(rightGameWallX1, 60));
            e.Graphics.DrawLine(new Pen(Color.Chocolate, 4), new Point(leftGameWallX1, 650), new Point(rightGameWallX1, 650));

            // лунка в конце полосы
            e.Graphics.FillRectangle(SystemBrushes.WindowText, new Rectangle(new Point(202, 0), new Size(396, 58)));

            // ДЛЯ ТЕБЯ
            // следующие 3 блока частично надо было бы в Load, но оставил, чтобы не терялась целостность их между собой
            // но это не оч правильно и красиво)
            //
            // отвечают за создание строк и их отрисовку(имя, очки, раунд)
            String scorePlayer = "SCORE: " + GameStatus.playerScore;
            String roundPlayer = "Timer: " + GameStatus.playerRound/100;
            String scoreBot = "SCORE: " + GameStatus.botScore;
            String roundBot = "ROUND: " + GameStatus.botRound;

            // размеры полей
            SizeF playerName = e.Graphics.MeasureString(GameStatus.playerName, font);
            SizeF scorePl = e.Graphics.MeasureString(scorePlayer, font);
            SizeF roundPl = e.Graphics.MeasureString(roundPlayer, font);
            SizeF botName = e.Graphics.MeasureString(GameStatus.botName, font);
            SizeF scoreB = e.Graphics.MeasureString(scoreBot, font);
            SizeF roundB = e.Graphics.MeasureString(roundBot, font);

            // расположение полей
            PointF pPlayerName = new PointF((leftGameWallX1 - playerName.Width) / 2, ((blackLineHeight + dHeight - playerName.Height) / 2) + dHeight);
            PointF pPlayerScore = new PointF((leftGameWallX1 - scorePl.Width) / 2, ((blackLineHeight + dHeight - scorePl.Height) / 2) + 2 * dHeight);
            PointF pPlayerRound = new PointF((leftGameWallX1 - roundPl.Width) / 2, ((blackLineHeight + dHeight - roundPl.Height) / 2) + 3 * dHeight);
            PointF pBotName = new PointF((this.ClientSize.Width - rightGameWallX1 - botName.Width) / 2 + rightGameWallX1, ((blackLineHeight + dHeight - botName.Height) / 2) + dHeight);
            PointF pBotScore = new PointF((this.ClientSize.Width - rightGameWallX1 - scoreB.Width) / 2 + rightGameWallX1, ((blackLineHeight + dHeight - scoreB.Height) / 2) + 2 * dHeight);
            PointF pBotRound = new PointF((this.ClientSize.Width - rightGameWallX1 - roundB.Width) / 2 + rightGameWallX1, ((blackLineHeight + dHeight - roundB.Height) / 2) + 3 * dHeight);

            // имя игроков, счет, раунд отрисовывается
            e.Graphics.DrawString(GameStatus.playerName, font, SystemBrushes.WindowText, pPlayerName);
            e.Graphics.DrawString(scorePlayer, font, SystemBrushes.WindowText, pPlayerScore);
            e.Graphics.DrawString(roundPlayer, font, SystemBrushes.WindowText, pPlayerRound);
            e.Graphics.DrawString(GameStatus.botName, font, SystemBrushes.WindowText, pBotName);
            e.Graphics.DrawString(scoreBot, font, SystemBrushes.WindowText, pBotScore);
            e.Graphics.DrawString(roundBot, font, SystemBrushes.WindowText, pBotRound);

            // на фон задаем изображение, созданное ранее на всю площадь формы
            this.BackgroundImage = bitmap;

            // отрисовка шара
            ball.DrawBall(e.Graphics);
   
            // отрисовка кегель
            for (int i = 0; i < 10; i++)
            {
                arrPins[i].Drawpin(e.Graphics);
            }
        }

        // возникает при нажатии кнопки мыши
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //кнопка мыши зажата
            mouseDown = true;
            mouseLocation1 = e.Location;
        }

        // возникает при перемещении курсора с зажатой кнопкой
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            // если кнопка мыши зажата
            if (mouseDown)
            {
                mouseLocation2 = e.Location;

                // рисуем полоску от шара до курсора
                gg.DrawLine(new Pen(Color.Black, 4), mouseLocation2, new Point((int)ball.VLOCATION.X, (int)ball.VLOCATION.Y));
            }
        }

        // возникает при отпускании кнопки мыши
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            // кнопка отжата
            mouseDown = false;
            mouseLocation2 = e.Location;
            Vector2D v;

            // проверка на недопустимость перемещения вниз
            if (e.Location.Y < ball.VLOCATION.Y)
            {
                v = new Vector2D(0f, 0f);
            }
            else
            {
                v = new Vector2D(ball.VLOCATION.X - e.Location.X, ball.VLOCATION.Y - e.Location.Y);

                // задание скорости в зависимости от отдаления мыши от шара
                if (v.Length() < 15)
                {
                    ball.SetSpeedKoef(3f);
                }
                else if (v.Length() > 15 & v.Length() < 30)
                {
                    ball.SetSpeedKoef(6f);
                }
                else if (v.Length() > 30 & v.Length() < 60)
                {
                    ball.SetSpeedKoef(9f);
                }
                else if (v.Length() > 60 & v.Length() < 90)
                {
                    ball.SetSpeedKoef(12f);
                }
                else
                {
                    ball.SetSpeedKoef(20f);
                }
                // нормализация вектора направления
                v = v.Unit();
                // тех. данные
                // замеры скорости шара
                //Console.WriteLine(v.X + " " + v.Y); 
            }  
            ball.VSPEED = v;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // ДЛЯ ТЕБЯ
            // тута код с ботом, код отсчета таймера, код подсчета сбитых кегель
            //
            // GameStatus.playerRound - отсчитывет время, в методе Load стоит /100, а не на /1000, потому что
            // интервал таймера равен 10 (10*100=1000мс=1сек)
            //тут можно менять число в сравнении, пока тесты, тут стоит 600(6сек +-), но потом ставь 1000-1500 значение равное 15-30сек в реальности
            // лучше будет что-то около 10сек = 1000 в коде сравнения ниже
            if (GameStatus.playerRound >= 600)
            {
                // at - переменная, которая в случае 1 означает, что ход игрока, в случае -1, что ход бота
                if (at == 1)
                {
                    // пробег по циклу кегль и те, кто окрашены в красный(после удара), идут как сбитые в счетчик(в данном случае к игроку)
                    for (int i = 0; i < 10; i++)
                    {
                        if (arrPins[i].COLOR == Color.Red)
                        {
                            GameStatus.playerScore++;
                        }
                    }
                    // смена хода, след. будет ходить бот
                    at *= -1;
                    // очистка формы(на самом деле перезагрузка вроде как) после этого все кегли и шар встанут на позицию по умолчанию
                    Form1_Load(null, null);
                    // обновление таймера
                    GameStatus.playerRound = 0;
                    // тут переменные берутся рандомно из диапазона для броска бота, направление и коэф. скорости передаются шару
                    float x = rnd.Next(-4, 4);
                    float y = rnd.Next(-4, -2);
                    float k = rnd.Next(3, 8);
                    ball.VSPEED = new Vector2D(x, y);
                    ball.SetSpeedKoef(k);                 
                }
                else
                {
                    // аналогично выше написано
                    for (int i = 0; i < 10; i++)
                    {
                        if (arrPins[i].COLOR == Color.Red)
                        {
                            GameStatus.botScore++;
                        }
                    }
                    at *= -1;
                    Form1_Load(null, null);
                    GameStatus.playerRound = 0;
                    GameStatus.botRound++;                  
                }
            }
            // если таймер еще не дошел до указанного времени, идет прибавление времени в таймер(счетчик по сути)
            else { GameStatus.playerRound++;}
            

            this.Invalidate();
            
            // проверка столкновения шара и стены
            if (ball.IsCollosoinWithWall(leftGameWallX1, rightGameWallX1))
            {
                ball.BallAndWallCollisionPhysics();
            }

            // находится ли шар в лунке
            if (ball.IsInHole())
            {
                ball.VSPEED = new Vector2D(0f, 0f);
            }

            //проверка в цикле кегль на:
            //      столкновение с шаром
            //      столкновение со стеной
            //      проверка в лунке ли
            for (int i = 0; i < 10; i++)
            {
                if (ball.IsCollisoinWithBall(arrPins[i]))
                {
                    arrPins[i].COLOR = Color.Red;
                    ball.BallAndBallCollisionPhysics(arrPins[i]);
                }
                if (arrPins[i].IsCollosoinWithWall(leftGameWallX1, rightGameWallX1))
                {
                    arrPins[i].BallAndWallCollisionPhysics();
                }
                if (arrPins[i].IsInHole())
                {
                    arrPins[i].VSPEED = new Vector2D(0f, 0f);
                }

                // цикл в цикле, чтобы проверить столкновение кегель между собой
                for (int j = 0; j < 10; j++)
                {
                    if (j == i)
                    {
                    }
                    else
                    {
                        if (arrPins[i].IsCollisoinWithBall(arrPins[j]))
                        {
                            arrPins[j].COLOR = Color.Red;
                            arrPins[i].BallAndBallCollisionPhysics(arrPins[j]);                          
                        }
                    }
                }
            }

            // ВНИМАНИЕ ДЛЯ ТЕБЯ
            // тут тупой код на замедление, который работает через жопу, иногда баги тупые выдает
            // мб поправлю еще, если буду тыкаться в нем, но пока хз
            // если что, то это НЕ БАГ А ФИЧА!!!!
            //
            // для каждой кегли вызывается метод MovePin() чтобы сделать смещение
            // происходит замедление скорости
            for (int i = 0; i < 10; i++)
            {
                arrPins[i].MovePin();
                if (arrPins[i].VSPEED.X != 0 || arrPins[i].VSPEED.Y != 0)
                {
                    if (arrPins[i].KSPEEDPIN > 0)
                    {
                        arrPins[i].UpdateStopSpeedPinKoef();
                    }
                    else if ((arrPins[i].VSPEED.X == 0 || arrPins[i].VSPEED.Y == 0) && arrPins[i].KSPEEDPIN == 0)
                    {
                        arrPins[i].VSPEED = new Vector2D(0f, 0f);
                    }
                    

                }
            }

            // смещение для шара и физика замедления
            ball.MoveBall();

            if (ball.VSPEED.X != 0 || ball.VSPEED.Y != 0)
            {
                if (ball.KPEEDBALL > 0)
                {
                    ball.UpdateStopSpeedBallKoef();

                }

            }
            else if ((ball.VSPEED.X == 0 || ball.VSPEED.Y == 0) && ball.KPEEDBALL == 0)
            {
                ball.VSPEED = new Vector2D(0f, 0f);
                
            }

        }
        
    }
}




















