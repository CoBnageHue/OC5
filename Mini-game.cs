using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OC5
{
    public partial class Mini_game : Form
    {
        private Form1 mainForm;
        public Mini_game(Form1 form)
        {
            this.mainForm = form;
            InitializeComponent();
            GameReset();
        }

        bool jumping = false; // boolean to check if player is jumping or not
        int jumpSpeed = 10; // integer to set jump speed
        int force = 12; // force of the jump in an integer
        int obstacleSpeed = 10; // the default speed for the obstacles
        Random rand = new Random(); // create a new random class
        bool isGameOver = false;
        int position;
            
        
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            trex.Top += jumpSpeed;
            
            if (jumping && force < 0)
            {
                jumping = false;
            }
            if (jumping)
            {
                jumpSpeed = -12;
                force -= 1;
            }
            else
            {
                jumpSpeed = 12;
            }

            if (trex.Top >= 138 && !jumping)
            {
                force = 12;
                trex.Top = 139;
                jumpSpeed = 0;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (String)x.Tag == "obstracle")
                {
                    x.Left -= obstacleSpeed;
                    if (x.Left < -100)
                    {
                        x.Left = this.ClientSize.Width + rand.Next(200, 500) + (x.Width * 15);
                    }
                    if (trex.Bounds.IntersectsWith(x.Bounds))
                    {
                        gameTimer.Stop();
                        trex.Image = Properties.Resources.dead;
                        label1.Text = "Вы проиграли, нажмите кнопку или пробел, чтобы начать заного";
                        isGameOver = true;
                    }
                }
            }
        }

        private void keyisdown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space && !jumping)
            {
                jumping = true;
            }
            if (e.KeyCode == Keys.Space && isGameOver)
            {
                GameReset();
            }

        }
        private void keyisup(object sender, KeyEventArgs e)
        {
            if(jumping)
            {
                jumping = false;
            }
        }

        public void GameReset()
        {
            force = 12;
            jumpSpeed = 0;
            jumping = false;
            obstacleSpeed = 10;
            label1.Text = "Нажимайте пробел или кнопку, чтобы прыгать";
            trex.Image = Properties.Resources.running;
            isGameOver = false;
            trex.Top = 139;


            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && (String)x.Tag == "obstracle")
                {
                    position = this.ClientSize.Width + rand.Next(100, 270) + (x.Width * 10);
                    x.Left = position;

                }
            }
            gameTimer.Start();
        }

        private void Mini_game_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.ButtonStartMinigame.Enabled = true;
        }
        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!jumping)
            {
                jumping = true;
            }
            if (isGameOver)
            {
                GameReset();
            }
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            if (jumping)
            {
                jumping = false;
            }
        }
    }
}
