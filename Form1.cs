using System;
using System.Drawing;
using System.Windows.Forms;

namespace test_c_sharp
{
    public partial class Form1 : Form
    {
        /*
        мяч всегда летит в сторону того, кто забил последний шар
        начальное направление и сторона мяча рандомные
        со временем мяч начинает ускоряться до след. забитого шара
        кнопка рестарта
        
        управление player 1 - A D
                   player 2 - 4 6 (стрелки вообще не обрабатываются в WinForms)
        */

        // настройки игры
        int platformsWidth = 120, platformsHeight = 10;
        int ballRadius = 10;
        int moveSpeed = 30;
        int ballSpeed = 3;
        uint winScore = 3;
        ////////////////////

        Random rand = new Random();

        Point platformScale;
        Point platform1, platform2, ball;
        Rectangle platf1, platf2, rectBall;

        int moveX, moveY;
        uint blueScore = 0, redScore = 0;
        float acceleration = 1f;

        public Form1()
        {
            InitializeComponent();

            platformScale = new Point(platformsWidth, platformsHeight);

            platform1 = new Point(pictureBox1.Width / 2, pictureBox1.Height * 7 / 8);
            platform2 = new Point(pictureBox1.Width / 2, pictureBox1.Height * 1 / 8);

            int platform1LeftX = platform1.X - platformScale.X / 2,
                platform1TopY = platform1.Y - platformScale.Y / 2;
            platf1 = new Rectangle(platform1LeftX, platform1TopY, platformScale.X, platformScale.Y);

            int platform2LeftX = platform2.X - platformScale.X / 2,
                platform2TopY = platform2.Y - platformScale.Y / 2;
            platf2 = new Rectangle(platform2LeftX, platform2TopY, platformScale.X, platformScale.Y);

            moveX = rand.Next(0, 2) * 2 - 1; // -1 или 1
            moveY = rand.Next(0, 2) * 2 - 1; // -1 или 1

            ball = new Point(pictureBox1.Width / 2, pictureBox1.Height * (moveY == 1 ? 1 : 3) / 4);            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int platform1LeftX = platform1.X - platformScale.X / 2,
                platform1TopY = platform1.Y - platformScale.Y / 2;
            platf1 = new Rectangle(platform1LeftX, platform1TopY, platformScale.X, platformScale.Y);

            e.Graphics.FillRectangle(Brushes.Red, platf1);

            int platform2LeftX = platform2.X - platformScale.X / 2,
                platform2TopY = platform2.Y - platformScale.Y / 2;
            platf2 = new Rectangle(platform2LeftX, platform2TopY, platformScale.X, platformScale.Y);

            e.Graphics.FillRectangle(Brushes.Blue, platf2);

            rectBall = new Rectangle(ball.X - ballRadius, ball.Y - ballRadius, 2 * ballRadius, 2 * ballRadius);
            e.Graphics.FillEllipse(Brushes.Yellow, rectBall);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            blueScore = redScore = 0;

            button1.Visible = false;
            label2.Visible = false;

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ball = new Point(
                 ball.X + (int)(moveX * ballSpeed * acceleration),
                 ball.Y + (int)(moveY * ballSpeed * acceleration));

            acceleration += 0.0005f;

            int ballLeftX = ball.X - ballRadius,
                ballRightX = ball.X + ballRadius,
                ballBottomY = ball.Y + ballRadius,
                ballTopY = ball.Y - ballRadius;

            if (ballInPlatform1(rectBall))
                moveY = -1;
            else if (ballInPlatform2(rectBall))
                moveY = 1;

            if (ballLeftX <= 1 || ballRightX >= pictureBox1.Width)
                moveX = -moveX;

            if (ballBottomY >= pictureBox1.Height)
            {
                blueScore++;
                ball = new Point(pictureBox1.Width / 2, pictureBox1.Height * 3 / 4);
                moveY = -1;
                acceleration = 1f;
            }
            else if (ballTopY <= 0)
            {
                redScore++;
                ball = new Point(pictureBox1.Width / 2, pictureBox1.Height * 1 / 4);
                moveY = 1;
                acceleration = 1f;
            }

            label1.Text = $"Score: {blueScore} : {redScore}";


            if (blueScore == winScore || redScore == winScore)
            {
                label2.Visible = true;
                button1.Visible = true;

                if (blueScore == winScore)
                    label2.Text = "Синий игрок победил!";
                else
                    label2.Text = "Красный игрок победил!";

                timer1.Stop();
            }

            pictureBox1.Refresh();
        }

        private bool ballInPlatform1(Rectangle ball)
        {
            return dotInPlatform1(new Point(ball.Left, ball.Top)) || dotInPlatform1(new Point(ball.Left, ball.Bottom))
                || dotInPlatform1(new Point(ball.Right, ball.Top)) || dotInPlatform1(new Point(ball.Right, ball.Bottom));
        }

        private bool ballInPlatform2(Rectangle ball)
        {
            return dotInPlatform2(new Point(ball.Left, ball.Top)) || dotInPlatform2(new Point(ball.Left, ball.Bottom))
                || dotInPlatform2(new Point(ball.Right, ball.Top)) || dotInPlatform2(new Point(ball.Right, ball.Bottom));
        }

        private bool dotInPlatform1(Point point)
        {
            return point.X > platf1.Left && point.X < platf1.Right && point.Y > platf1.Top && point.Y < platf1.Bottom;
        }

        private bool dotInPlatform2(Point point)
        {
            return point.X > platf2.Left && point.X < platf2.Right && point.Y > platf2.Top && point.Y < platf2.Bottom;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'a')
                platform1 = new Point(Clamp(platform1.X - moveSpeed,   0, pictureBox1.Width), platform1.Y );
            else if (e.KeyChar == 'd')
                platform1 = new Point(Clamp(platform1.X + moveSpeed,   0, pictureBox1.Width), platform1.Y);

            if (e.KeyChar == '4')
                platform2 = new Point(Clamp(platform2.X - moveSpeed,   0, pictureBox1.Width), platform2.Y);
            else if (e.KeyChar == '6')
                platform2 = new Point(Clamp(platform2.X + moveSpeed,   0, pictureBox1.Width), platform2.Y);
        }


        private int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else 
                return value;
        }
    }
}
