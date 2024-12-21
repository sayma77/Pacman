using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Pacman_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();

        bool goLeft , goRight , goUp , goDown;
        bool noLeft , noRight , noUp , noDown;

        int speed = 8;
        Rect pacmanHitBox;
        int ghostSpeed = 10;
        int ghostMoveStep = 200;
        int currentGhostStep;
        int score = 0;



        public MainWindow()
        {
            InitializeComponent();

            GameSetUp();
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Left && noLeft == false)
            {
                goRight = goUp = goDown = false;
                noRight = noUp = noDown = false;

                goLeft = true;

                pacman.RenderTransform = new RotateTransform(-180 , pacman.Height / 2, pacman.Width / 2);

            }

            if (e.Key == Key.Right && noRight == false)
            {
                goLeft = goUp = goDown = false;
                noLeft = noUp = noDown = false;

                goRight = true;

                pacman.RenderTransform = new RotateTransform(0 , pacman.Height / 2, pacman.Width / 2);

            }

            if (e.Key == Key.Up && noUp == false)
            {
                goRight = goLeft = goDown = false;
                noRight = noLeft = noDown = false;

                goUp = true;

                pacman.RenderTransform = new RotateTransform(-90, pacman.Height / 2, pacman.Width / 2);

            }

            if (e.Key == Key.Down && noDown == false)
            {
                goRight = goUp = goLeft = false;
                noRight = noUp = noLeft = false;

                goDown = true;

                pacman.RenderTransform = new RotateTransform(90, pacman.Height / 2, pacman.Width / 2);

            }

        }

        private void GameSetUp()
        {

            MyCanvas.Focus();

            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pacman.jpg"));
            pacman.Fill = pacmanImage;

            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/red.jpg"));
            redguy.Fill = redGhost;

            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/orange.jpg"));
            orangeguy.Fill = orangeGhost;

            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pink.jpg"));
            pinkguy.Fill = pinkGhost;

            ImageBrush coinBrush = new ImageBrush();
            coinBrush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/coin.gif"));

            foreach (var element in MyCanvas.Children.OfType<Rectangle>())
            {
                if (element is Shape shape && shape.Tag != null && shape.Tag.ToString() == "coin")
                {
                    shape.Fill = coinBrush;
                }
            }




        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;

            if(goRight)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);

            }

            if (goLeft)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);

            }

            if (goUp)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);

            }

            if (goDown)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);

            }

            if(goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height) 
            {
                noDown = true;
                goDown = false;
            } 
            if(goUp && Canvas.GetTop(pacman) < 1)
            {
                noUp = true;
                goUp = false;
            }

            if (goLeft && Canvas.GetLeft(pacman) - 10 < 1)
            {
                noLeft = true;
                goLeft = false;
            }
            if (goRight && Canvas.GetLeft(pacman) - 110 > Application.Current.MainWindow.Height)
            {
                noRight = true;
                goRight = false;
            }

            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman) , pacman.Width , pacman.Height);

            foreach(var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x) , Canvas.GetTop(x) , x.Width , x.Height);

                if((string)x.Tag == "wall")
                {
                    if (goLeft == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }

                    if (goRight == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }

                    if (goDown == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }

                    if (goUp == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }

                if ((string)x.Tag == "coin")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                if((string)x.Tag == "ghost")
                {
                    if(pacmanHitBox.IntersectsWith(hitBox))
                    {
                        GameOver("Haha, you were caught by the ghost! Click ok to play again.");
                    }
                    if(x.Name.ToString() == "orangeguy")
                    {
                        Canvas.SetLeft(x , Canvas.GetLeft(x) - ghostSpeed);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }

                    currentGhostStep--;

                    if(currentGhostStep < 1)
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    }
                }
            }

            if(score == 98)
            {
                GameOver("Congratulation! You Win.");
            }
        }

        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "The Pacman Game");

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
