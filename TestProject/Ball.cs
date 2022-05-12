using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestProject
{
    /*
     * класс который описывает шар для боулинга и кеглю
     */
    public class Ball
    {
        float radius;
        float diametr;
        float weight;
        // переменная является направлением и составляющей скоростью одновременно
        Vector2D vSpeed;
        Vector2D vlocation;
        Color color = Color.White;
        // тянем мышку и этот коэф меняется
        // для шара, нужен для скорости
        private float kSpeedBall = 6f;
        //для кегли, нужен для скорости
        private float kSpeedPin = 1f;
        //эти коэф. для замедления шара и кегли соотв.
        private float kStopSpeedBall = 0.1f;
        private float kStopSpeedPin = 0.1f;

        public Color COLOR
        {
            get { return color; }
            set { color = value; }
        }
        public float KPEEDBALL
        {
            get { return kSpeedBall; }
        }

        public float KSPEEDPIN
        {
            get { return kSpeedPin; }
        }
        public float RADIUS
        {
            get { return radius; }
            set { radius = value; }
        }
        public float DIAMETR
        {
            get { return diametr; }
            set { diametr = value; }
        }
        public float WEIGHT
        {
            get { return weight; }
            set { weight = value; }
        }
        public Vector2D VSPEED
        {
            get { return vSpeed; }
            set { vSpeed = value; }
        }
        public Vector2D VLOCATION
        {
            get { return vlocation; }
            set { vlocation = value; }
        }



        public Ball(float diametr, float weight, Vector2D vSpeed, Vector2D location)
        {
            this.diametr = diametr;
            this.weight = weight;
            this.radius = diametr / 2;
            this.vSpeed = vSpeed;
            this.vlocation = location;
        }

        public Ball(float diametr, float weight, Vector2D location)
        {
            this.diametr = diametr;
            this.weight = weight;
            this.radius = diametr / 2;
            this.vSpeed = new Vector2D(0, 0);
            this.vlocation = location;
        }

        // вычисляет следующую позицию шара из текущей локации + скорости + направления


        // проверка столкновения между шаром и стеной                   - DONE
        // проверка столкновения между шаром и шаром                    - DONE 
        // проверка выхода в край полосы - черная зона(яма)             - DONE
        // проверка невозможности бросить шар в сторону игрока(вниз)    - DONE (проверку сделать во время обработки мыши)

        // физика при столкновении между шарами                         - DONE
        // физика физика столкновения между шарами и стеной             - DONE
        // физика замедления скорости                                   - DONE +-
        // физика скорости в зависимости от удаления курсора от мяча    - DONE (сделать во время обработки мыши)
                                          



        // обновляет координаты
        // с учетом скорости шара и коэф. шара
        public void MoveBall()
        {
            this.vlocation += this.vSpeed * kSpeedBall;
           
        }

        // перемещение кегли с учетом скорости и коэф. скорости
        public void MovePin()
        {
            this.vlocation += this.vSpeed * kSpeedPin;
        }


        // проверяет столкновение между шаром и и кеглями/ кеглями и кеглями
        // проверяет расстояние между центрами объектов и суммой их радиусов
        // тут остался неэффективный код, который много ест, но пусть пока будет просто так
        public bool IsCollisoinWithBall(Ball b2)
        {
            /*Vector2D v = new Vector2D(this.VLOCATION.X, this.VLOCATION.Y, b2.VLOCATION.X, b2.VLOCATION.Y);
            float l = v.Length();
            if (l > this.RADIUS + b2.RADIUS)
            {
                return false;
            }
            else 
            {
                return true;
            }*/
            /*else if (Math.Abs(this.VLOCATION.Y - b2.VLOCATION.Y) < this.RADIUS + b2.RADIUS)
                        {
                            return true;
                        }*/
            if (Math.Abs(this.VLOCATION.X - b2.VLOCATION.X) < this.RADIUS + b2.RADIUS &
                    Math.Abs(this.VLOCATION.Y - b2.VLOCATION.Y) < this.RADIUS + b2.RADIUS)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // проверяет столкновение между шаром(кеглей) и боковыми стенками поля
        // проверяет расстояние между разницей координаты центра(Х) и радиуса и левой координатой поля(0 по Х)
        // проверяет расстояние между суммой координаты центра(Х) и радиуса и правой координатой поля(.Width)
        public bool IsCollosoinWithWall(float left, float right)
        {
            if (this.vlocation.X - this.radius <= left)
            {
                return true;
            }
            else if (this.vlocation.X + this.radius >= right)
            {
                return true;
            }
            else { 
                return false;
            }
        }

        // проверяет расположение шара(кегли) на нахождение в дыре в конце полосы
        // проверяет расположение центра координаты(Y) отн-но линии начала дыры
        // т.е. шар может краем зайти на черное поле, но он не пропадет, а если его центр
        // окажется в зоне дыры, то он пропадет 
        // тут используется фиксированное число, которое означает границу начала черной области(дыры)
        public bool IsInHole()
        {
            if (this.vlocation.Y > 58)
            {
                return false;
            }
            else
            {
                this.VLOCATION = new Vector2D(this.VLOCATION.X, 29);
                return true;
            }
            
        }

        // результат столкновения шара/кегли с кеглей или шаром
        public void BallAndBallCollisionPhysics(Ball b2)
        {
            Vector2D v1 = new Vector2D(this.VLOCATION.X, this.VLOCATION.Y, b2.VLOCATION.X, b2.VLOCATION.Y);
            Vector2D v2 = new Vector2D(b2.VLOCATION.X, b2.VLOCATION.Y, this.VLOCATION.X, this.VLOCATION.Y);

            v1 = v1.Unit();
            v2 = v2.Unit();

            this.VSPEED = this.VSPEED + v2;
            b2.VSPEED = b2.VSPEED + v1;

           /* this.VSPEED.Unit();
            b2.VSPEED.Unit();*/
            

        }

        // результат столкновения со стеной
        public void BallAndWallCollisionPhysics()
        {
            this.VSPEED.X *= -1;
        }

        // обновление заливки - цвета круга(кегли), чтобы понять, что было столкновение, заливка красным
        public void BackColorSwitch(Graphics g)
        {
            SolidBrush brush = new SolidBrush(Color.Red);
            
            g.FillEllipse(brush, this.vlocation.X - this.radius, this.vlocation.Y - this.radius,
                          this.diametr, this.diametr);
        }

        // задает значение полю, полученное от мыши
        public void SetSpeedKoef(float a)
        {
            this.kSpeedBall = a;
        }

        // обновление скорости шара - замедление
        public void UpdateStopSpeedBallKoef()
        {
            this.kSpeedBall -= this.kStopSpeedBall;
        }

        // обновление скорости кегли - замедление
        public void UpdateStopSpeedPinKoef()
        {
            this.kSpeedPin -= this.kStopSpeedPin;
        }




        // рисует шар
        public void DrawBall(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 2); 
            SolidBrush brush = new SolidBrush(Color.Blue); 
            g.DrawEllipse(pen, this.vlocation.X - this.radius, this.vlocation.Y - this.radius,
                          this.diametr, this.diametr);

            g.FillEllipse(brush, this.vlocation.X - this.radius, this.vlocation.Y - this.radius,
                          this.diametr, this.diametr);

        }

        // рисует кеглю
        public void Drawpin(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 2); 
            SolidBrush brush = new SolidBrush(color); 
            g.DrawEllipse(pen, this.vlocation.X - this.radius, this.vlocation.Y - this.radius,
                          this.diametr, this.diametr);

            g.FillEllipse(brush, this.vlocation.X - this.radius, this.vlocation.Y - this.radius,
                          this.diametr, this.diametr);

        }

      
        
    }
}
