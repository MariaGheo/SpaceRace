using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace SpaceRace
{
    public partial class Form1 : Form
    {
        Rectangle player1 = new Rectangle(260, 370, 20, 20);
        Rectangle player2 = new Rectangle(420, 370, 20, 20);
        const int playerSpeed = 2;

        List<Rectangle> obstacle = new List<Rectangle>();
        List<int> obstacleSpeeds = new List<int>();

        Rectangle timer = new Rectangle(345, 0, 10, 400);
        int counter = 0;

        bool wDown = false;
        bool sDown = false;
        bool upDown = false;
        bool downDown = false;

        SolidBrush whiteBrush = new SolidBrush(Color.White);

        Random randGen = new Random();
        int randValue = 0;

        int p1Score = 0;
        int p2Score = 0;

        string gameState = "waiting";

        public Form1()
        {
            InitializeComponent();

            p1ScoreLabel.Visible = false;
            p2ScoreLabel.Visible = false;
        }

        public void GameInitialize()
        {
            titleLabel.Visible = false;
            subtitleLabel.Visible = false;
            p1ScoreLabel.Visible = true;
            p2ScoreLabel.Visible = true;
            p1ScoreLabel.Text = "0";
            p2ScoreLabel.Text = "0";

            gameTimer.Enabled = true;
            gameState = "running";
            p1Score = 0;
            p2Score = 0;

            obstacle.Clear();

            player1.Location = new Point(260, 370);
            player2.Location = new Point(420, 370);
            timer.Location = new Point(345, 0);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = true;
                    break;
                case Keys.S:
                    sDown = true;
                    break;
                case Keys.Up:
                    upDown = true;
                    break;
                case Keys.Down:
                    downDown = true;
                    break;
                case Keys.Space:
                    if (gameState == "waiting" || gameState == "over")
                    {
                        SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);
                        confirmBeep.Play();
                        GameInitialize();
                    }
                    break;
                case Keys.Escape:
                    if (gameState == "waiting" || gameState == "over")
                    {
                        Application.Exit();
                    }
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    wDown = false;
                    break;
                case Keys.S:
                    sDown = false;
                    break;
                case Keys.Up:
                    upDown = false;
                    break;
                case Keys.Down:
                    downDown = false;
                    break;
            }
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            //move player 1 
            if (wDown == true)
            {
                player1.Y -= playerSpeed;
            }

            if (sDown == true && player1.Y < this.Height - player1.Height)
            {
                player1.Y += playerSpeed;
            }

            //move player 2
            if (upDown == true)
            {
                player2.Y -= playerSpeed;
            }
            
            if (downDown == true && player2.Y < this.Height - player2.Height)
            {
                player2.Y += playerSpeed;
            }

            //move obstacles 
            for (int i = 0; i < obstacle.Count(); i++)
            {
                //find the new postion of x based on speed 
                int x = obstacle[i].X + obstacleSpeeds[i];

                //replace the rectangle in the list with updated one using new x
                obstacle[i] = new Rectangle(x, obstacle[i].Y, 10, 5);
            }

            //check to see if a new obstacle should be created 
            randValue = randGen.Next(0, 101);

            if (randValue < 6) //4% chance of a new obstacle from the left 
            {
                int y = randGen.Next(5, 361);
                obstacle.Add(new Rectangle(-10, y, 10, 5));
                obstacleSpeeds.Add(randGen.Next(2, 5));

            }
            else if (randValue < 12) //4% chance of a new obstacle from the right
            {
                int y = randGen.Next(5, 361);
                obstacle.Add(new Rectangle(700, y, 10, 5));
                obstacleSpeeds.Add(randGen.Next(-5, -2));
            }

            //check if obstacle is out of play area and remove it if it is 
            for (int i = 0; i < obstacle.Count(); i++)
            {
                if (obstacle[i].X < -10 || obstacle[i].X > 700)
                {
                    obstacle.RemoveAt(i);
                    obstacleSpeeds.RemoveAt(i);
                }
            }

            //check for collision of obstacles and players
            for (int i = 0; i < obstacle.Count(); i++)
            {
                //check for collision of obstacle and player 1
                if (player1.IntersectsWith(obstacle[i]))
                {
                    player1.Location = new Point(260, 370); //move player 1 back to start
                    obstacle.RemoveAt(i); //remove obstacle from list
                    obstacleSpeeds.RemoveAt(i); //remove obstacle speed from list
                    SoundPlayer blip = new SoundPlayer(Properties.Resources.blip);
                    blip.Play();
                }
                else if (player2.IntersectsWith(obstacle[i])) //check for collision of obstacle and player 2d
                {
                    player2.Location = new Point(420, 370); //move player 2 back to start
                    obstacle.RemoveAt(i); //remove obstacle from list
                    obstacleSpeeds.RemoveAt(i); //remove obstacle speed from list 
                    SoundPlayer blip = new SoundPlayer(Properties.Resources.blip);
                    blip.Play();
                }
            }

            //counter for timer
            counter++;
            
            //update timer
            if (counter == 5)
            {
                timer.Y += 1;
                counter = 0;
            }

            //check if player 1 got to the top
            if (player1.Y < 0)
            {
                player1.Location = new Point(260, 370);
                p1Score++;
                SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);

                p1ScoreLabel.Text = $"{p1Score}";
                confirmBeep.Play();
            }

            //check if player 2 got to the top
            if (player2.Y < 0)
            {
                player2.Location = new Point(420, 370);
                p2Score++;
                SoundPlayer confirmBeep = new SoundPlayer(Properties.Resources.confirmBeep);

                p2ScoreLabel.Text = $"{p2Score}";
                confirmBeep.Play();
            }

            //check if time is up
            if (timer.Y == 400)
            {
                gameState = "over";
                gameTimer.Enabled = false;
            }

            Refresh();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (gameState == "waiting")
            {
                titleLabel.Text = "SPACERACE";
                subtitleLabel.Text = "Press Space Bar to Start or Escape to Exit";
            }
            else if (gameState == "running")
            {
                //draw players
                e.Graphics.FillRectangle(whiteBrush, player1);
                e.Graphics.FillRectangle(whiteBrush, player2);

                //draw timer
                e.Graphics.FillRectangle(whiteBrush, timer);

                //draw obstacles
                for (int i = 0; i < obstacle.Count; i++)
                {
                    e.Graphics.FillRectangle(whiteBrush, obstacle[i]);
                }
            }
            else if (gameState == "over")
            {
                if (p1Score > p2Score)
                {
                    titleLabel.Text = "PLAYER 1 WON!!!";
                }
                else if (p2Score > p1Score)
                {
                    titleLabel.Text = "PLAYER 2 WON!!!";
                }
                else
                {
                    titleLabel.Text = "TIE!!!";
                }

                subtitleLabel.Text = "Press Space Bar to Play Again or Escape to Exit";

                p1ScoreLabel.Visible = false;
                p2ScoreLabel.Visible = false;
                titleLabel.Visible = true;
                subtitleLabel.Visible = true;
            }
        }
    }
}