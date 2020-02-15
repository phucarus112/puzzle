using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Puzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Fluent.RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        const int startX = 140;
        const int startY = 140;
        int[,] lines = new int[3,3];
        int[] FirstSample = new int[8];
        Image[,] cropImage = new Image[3, 3];

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        bool _isDragging = false;
        Image _selectedBitmap = null;
        Point _lastPosition;
        int first = 0, firsttime = 0;
        Point src, des;
        int posX = 2, locationX = 380, posY = 2, locationY = 380;

        private void CropImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (firsttime != 0)
            {
                _isDragging = false;
                var position = e.GetPosition(this);

                int x = (int)(position.Y - startX) / 120 * 120 + startX;
                int y = (int)(position.X - startY) / 120 * 120 + startY;

                des.X = (x - startX) / 120;
                des.Y = (y - startY) / 120;



                if (des.X < 3 && des.Y < 3)
                {
                    if (isOverride(des) || !IsValidDes(src, des)) //neu hinh nay ghi đè len hình kia hoac diem di khong phu hợp
                    {
                        if (_selectedBitmap != null)
                        {
                            Canvas.SetLeft(_selectedBitmap, src.Y * 120 + startY);
                            Canvas.SetTop(_selectedBitmap, src.X * 120 + startX);
                            firsttime = 0;

                        }

                    }
                    else
                    {
                        if (_selectedBitmap != null)
                        {

                            Canvas.SetLeft(_selectedBitmap, y);
                            Canvas.SetTop(_selectedBitmap, x);

                        }
                        Image temp = new Image();
                        temp = cropImage[(int)des.X, (int)des.Y];
                        cropImage[(int)des.X, (int)des.Y] = cropImage[(int)src.X, (int)src.Y];
                        cropImage[(int)src.X, (int)src.Y] = temp;


                        int tam = lines[(int)src.X, (int)src.Y];
                        lines[(int)src.X, (int)src.Y] = lines[(int)des.X, (int)des.Y];
                        lines[(int)des.X, (int)des.Y] = tam;
                        //MessageBox.Show($"{src.X},{src.Y} -> {des.X},{des.Y}");

                        string str = "";
                        for (int m = 0; m < 3; m++)
                        {
                            for (int n = 0; n < 3; n++)
                            {
                                str += lines[m, n].ToString();
                            }
                        }
                        // MessageBox.Show(str);
                        checkGame();
                        firsttime = 0;

                    }

                }
                else //neu kéo thả vượt khỏi pham vi
                {
                    if (_selectedBitmap != null)
                    {
                        Canvas.SetLeft(_selectedBitmap, src.Y * 120 + startY);
                        Canvas.SetTop(_selectedBitmap, src.X * 120 + startX);
                        //MessageBox.Show($"{src.Y * 120 + startY},{src.X * 120 + startX}");
                        firsttime = 0;
                    }

                }
            }

        }

        private bool isOverride(Point des)
        {
            if (lines[(int)des.X, (int)des.Y] != -1) return true;
            return false;
        }

        private List<Point> findValidDes(Point src)
        {
            List<Point> list = new List<Point>();

            //neu la 4 ô ở ria
            if ((src.X == 0 && src.Y == 0) || (src.X == 0 && src.Y == 2) || (src.X == 2 && src.Y == 0)
                || (src.X == 2 && src.Y == 2))
            {
                if (src.X == 0)
                {
                    list.Add(new Point(src.X + 1, src.Y));
                    if (src.Y == 0)
                        list.Add(new Point(src.X, src.Y + 1));
                    else
                        list.Add(new Point(src.X, src.Y - 1));
                }
                else
                {
                    list.Add(new Point(src.X - 1, src.Y));
                    if (src.Y == 0)
                        list.Add(new Point(src.X, src.Y + 1));
                    else
                        list.Add(new Point(src.X, src.Y - 1));
                }
            }
            else if ((src.X == 0 && src.Y != 0) || (src.X != 0 && src.Y == 0)
                || (src.X == 2 && src.Y != 0) || (src.X != 0 && src.Y == 2)) //neu la cac o cung hàng với 4 o rìa
            {
                if (src.X == 0 && src.Y != 0)
                {
                    list.Add(new Point(src.X + 1, src.Y));
                    list.Add(new Point(src.X, src.Y - 1));
                    list.Add(new Point(src.X, src.Y + 1));
                }
                else if (src.X != 0 && src.Y == 0)
                {
                    list.Add(new Point(src.X - 1, src.Y));
                    list.Add(new Point(src.X, src.Y + 1));
                    list.Add(new Point(src.X + 1, src.Y));
                }
                else if (src.X == 2 && src.Y != 0)
                {
                    list.Add(new Point(src.X, src.Y + 1));
                    list.Add(new Point(src.X, src.Y - 1));
                    list.Add(new Point(src.X - 1, src.Y));
                }
                else if (src.X != 0 && src.Y == 2)
                {
                    list.Add(new Point(src.X - 1, src.Y));
                    list.Add(new Point(src.X + 1, src.Y));
                    list.Add(new Point(src.X, src.Y - 1));
                }
            }
            else //neu la cac o nam ben trong
            {
                list.Add(new Point(src.X - 1, src.Y));
                list.Add(new Point(src.X, src.Y - 1));
                list.Add(new Point(src.X + 1, src.Y));
                list.Add(new Point(src.X, src.Y + 1));
            }

            return list;
        }

        private bool IsValidDes(Point src, Point des)
        {
            List<Point> temp = findValidDes(src);
            
            /*string res = "";
            for(int i=0;i<temp.Count;i++)
            {
                res += temp[i].X + "," + temp[i].Y + "%";
            }
            MessageBox.Show(res);*/

            for(int i=0;i<temp.Count;i++)
            {
                if (temp[i].X == des.X && temp[i].Y == des.Y) return true;
            }

            return false;
        }

        private void CropImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _selectedBitmap = sender as Image;
            _lastPosition = e.GetPosition(this);
            if (firsttime == 0)
            {
                src.X = ((int)(_lastPosition.Y - startX)) / 120;
                src.Y = ((int)(_lastPosition.X - startY)) / 120;
                firsttime++;
            }
        }

        private void CropImage_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            int i = ((int)position.Y - startY) / 120;
            int j = ((int)position.X - startX) /120;

            //this.Title = $"{position.X} - {position.Y}, a[{i}][{j}]";

            if (_isDragging)
            {
                var dx = position.X - _lastPosition.X;
                var dy = position.Y - _lastPosition.Y;
                var lastLeft = Canvas.GetLeft(_selectedBitmap);
                var lastTop = Canvas.GetTop(_selectedBitmap);
                
                Canvas.SetLeft(_selectedBitmap, lastLeft + dx);
                Canvas.SetTop(_selectedBitmap, lastTop + dy);

                _lastPosition = position;

                
                
            }
        
        }

        private void checkGame()
        {
            int valid = 0;


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        if (lines[i, j] == 3 * i + j) valid++;
                        
                    }
                   
                }
            }
            if (valid >= 8)
            {
                MessageBox.Show("win");
                timer.Stop();
            }
                
        }

        string urlImage = "pic1.jpg";

        private void img1Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic1.jpg";
            shuffleBtnClick(sender, e);
        }

        private void img2Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic2.jpg";
            shuffleBtnClick(sender, e);
        }

        private void img3Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic3.jpeg";
            shuffleBtnClick(sender, e);
        }

        private void img4Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic4.png";
            shuffleBtnClick(sender, e);
        }

        private void img5Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic5.jpg";
            shuffleBtnClick(sender, e);
        }

        private void img6Click(object sender, RoutedEventArgs e)
        {
            urlImage = "pic6.png";
            shuffleBtnClick(sender, e);
        }

        private DispatcherTimer timer;
        private int counter = 180;
        bool setUpTime = false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
                     $"{ --counter }");

            int substract;
            if (counter>= 120)
            {
                substract = 60 - (180 - counter);
                if(counter >= 120 && counter <= 129)
                    showTime.Text = "02:0"+ substract.ToString();
                else
                    showTime.Text = "02:" + substract.ToString();
            }
            else if(counter < 120 && counter >= 60)
            {
                substract = 120 - (180 - counter);
                if (counter >= 60 && counter <= 69)
                    showTime.Text = "01:0" + substract.ToString();
                else
                    showTime.Text = "01:" + substract.ToString();
            }
            else
            {
                substract = 180 - (180 - counter);
                if (counter >= 0 && counter <= 9)
                    showTime.Text = "00:0" + substract.ToString();
                else
                    showTime.Text = "00:" + substract.ToString();
            }

            if (counter == 0)
            {
                timer.Stop();
                MessageBox.Show("You lose game!!");
            }
        }

        private void newGameBtnClick(object sender, RoutedEventArgs e)
        {
            urlImage = "pic1.jpg";
            subcanvas.Children.Clear();
            originalImage.Source = new BitmapImage(new Uri("pic1.jpg", UriKind.Relative));

            int count = 0;

            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("pic1.jpg", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.DecodePixelWidth = 360;
            src.DecodePixelHeight = 360;
            src.EndInit();

            TransformedBitmap myRotatedBitmapSource = new TransformedBitmap();
            myRotatedBitmapSource.BeginInit();
            myRotatedBitmapSource.Source = src;
            myRotatedBitmapSource.EndInit();

            var objImg = new CroppedBitmap[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = (int)src.Width / 3;

                    objImg[count++] = new CroppedBitmap(src, new Int32Rect(j * 120, i * 120, 120, 120));
                }
            }

            int temp;
            String[] ansRnd = new string[9];
            Random random = new Random();
            List<int> list = new List<int>();
            for (int i = 0; i < 8; i++)
                list.Add(i);

            //random
            for (int i = 0; i < 8; i++)
            {
                temp = random.Next(list.Count);
                ansRnd[i] = list[temp].ToString();
                list.RemoveAt(temp);
                FirstSample[i] = int.Parse(ansRnd[i]);
            }

            count = 0;
            lines = new int[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        lines[i, j] = FirstSample[3 * i + j];

                    }
                    else
                    {
                        lines[i, j] = -1;
                    }

                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        cropImage[i, j] = new Image();
                        int val = int.Parse(ansRnd[count++]);
                        cropImage[i, j].Source = objImg[val];
                        subcanvas.Children.Add(cropImage[i, j]);
                        var width = startX + 120 * j;
                        var height = startY + 120 * i;

                        Canvas.SetLeft(cropImage[i, j], width);
                        Canvas.SetTop(cropImage[i, j], height);
                        cropImage[i, j].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        cropImage[i, j].MouseLeftButtonUp += CropImage_MouseLeftButtonUp;
                        cropImage[i, j].MouseMove += CropImage_MouseMove;
                        cropImage[i, j].Tag = new Tuple<int, int>(i, j);

                    }
                    else cropImage[i, j] = null;
                }
            }
            DrawLines(3,3, 120);
            
            if(!setUpTime)
            {
                // khởi tạo bộ đếm thời gian
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
                setUpTime = true;
            }
            else
            {
                timer.Stop();
                counter = 180;
                timer.Start();
            }

            imgUp.Source = new BitmapImage(new Uri("Up.png", UriKind.Relative));
            imgDown.Source = new BitmapImage(new Uri("down.png", UriKind.Relative));
            imgLeft.Source = new BitmapImage(new Uri("left.jpg", UriKind.Relative));
            imgRight.Source = new BitmapImage(new Uri("right.jpg", UriKind.Relative));

            first2 = true;
        }

        private void shuffleBtnClick(object sender, RoutedEventArgs e)
        {

            subcanvas.Children.Clear();
            originalImage.Source = new BitmapImage(new Uri(urlImage, UriKind.Relative));

            int count = 0;
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(urlImage, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.DecodePixelWidth = 360;
            src.DecodePixelHeight = 360;
            src.EndInit();

            TransformedBitmap myRotatedBitmapSource = new TransformedBitmap();
            myRotatedBitmapSource.BeginInit();
            myRotatedBitmapSource.Source = src;
            myRotatedBitmapSource.EndInit();

            var objImg = new CroppedBitmap[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = (int)src.Width / 3;

                    objImg[count++] = new CroppedBitmap(src, new Int32Rect(j * 120, i * 120, 120, 120));
                }
            }

            int temp;
            String[] ansRnd = new string[9];
            Random random = new Random();
            List<int> list = new List<int>();
            for (int i = 0; i < 8; i++)
                list.Add(i);

            //random
            for (int i = 0; i < 8; i++)
            {
                temp = random.Next(list.Count);
                ansRnd[i] = list[temp].ToString();
                list.RemoveAt(temp);
                FirstSample[i] = int.Parse(ansRnd[i]);
            }

            string kq = "";
           
            lines = new int[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        lines[i, j] = FirstSample[3 * i + j];
                        
                    }
                    else
                    {
                        lines[i, j] = -1;
                        
                    }

                }
            }

            count = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!((i == 2) && (j == 2)))
                    {
                        cropImage[i, j] = new Image();
                        int val = int.Parse(ansRnd[count++]);
                        cropImage[i, j].Source = objImg[val];
                        subcanvas.Children.Add(cropImage[i, j]);
                        var width = startX + 120 * j;
                        var height = startY + 120 * i;

                        Canvas.SetLeft(cropImage[i, j], width);
                        Canvas.SetTop(cropImage[i, j], height);
                        cropImage[i, j].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        cropImage[i, j].MouseLeftButtonUp += CropImage_MouseLeftButtonUp;
                        cropImage[i, j].MouseMove += CropImage_MouseMove;
                        cropImage[i, j].Tag = new Tuple<int, int>(i, j);

                    }
                }
            }
            DrawLines(3, 3, 120);

            if (!setUpTime)
            {
                // khởi tạo bộ đếm thời gian
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
                setUpTime = true;
            }
            else
            {
                timer.Stop();
                counter = 180;
                timer.Start();
            }
            imgUp.Source = new BitmapImage(new Uri("Up.png", UriKind.Relative));
            imgDown.Source = new BitmapImage(new Uri("down.png", UriKind.Relative));
            imgLeft.Source = new BitmapImage(new Uri("left.jpg", UriKind.Relative));
            imgRight.Source = new BitmapImage(new Uri("right.jpg", UriKind.Relative));

            first2 = true;
        }

        private void btnHelpClick(object sender, RoutedEventArgs e)
        {
            var screen = new About();

            if (screen.ShowDialog() == true)
            {

            }
        }

        private void saveGameBtnClick(object sender, RoutedEventArgs e)
        {
            var writer = new StreamWriter("save.txt");
            writer.WriteLine("3");
            writer.WriteLine(urlImage);
            writer.WriteLine(counter.ToString());

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    writer.Write($"{lines[i, j]}");
                    if (j != 2)
                    {
                        writer.Write(" ");
                    }
                }
                writer.WriteLine("");
            }
            writer.Close();
            MessageBox.Show("Yeah! Your game is saved");

        }

        private void loadGameBtnClick(object sender, RoutedEventArgs e)
        {
            var reader = new StreamReader("save.txt");
            int Rows = int.Parse(reader.ReadLine());
            
            urlImage = reader.ReadLine();
            counter = int.Parse(reader.ReadLine());
            subcanvas.Children.Clear();
            originalImage.Source = new BitmapImage(new Uri(urlImage, UriKind.Relative));
            int count = 0;
            var src = new BitmapImage();

            src.BeginInit();
            src.UriSource = new Uri(urlImage, UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.DecodePixelWidth = 360;
            src.DecodePixelHeight = 360;
            src.EndInit();

            TransformedBitmap myRotatedBitmapSource = new TransformedBitmap();
            myRotatedBitmapSource.BeginInit();
            myRotatedBitmapSource.Source = src;
            myRotatedBitmapSource.EndInit();

            var objImg = new CroppedBitmap[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int x = (int)src.Width / 3;

                    objImg[count++] = new CroppedBitmap(src, new Int32Rect(j * 120, i * 120, 120, 120));
                }
            }
           
            //doc ra gán vào lines[,]
            for (int i = 0; i < 3; i++)
            {
                var tokens = reader.ReadLine().Split(
                new string[] { " " }, StringSplitOptions.None);
                for (int j = 0; j < 3; j++)
                {
                    lines[i, j] = int.Parse(tokens[j]);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (lines[i,j] != -1)
                    {
                        cropImage[i, j] = new Image();
                        cropImage[i, j].Source = objImg[lines[i, j]];
                        subcanvas.Children.Add(cropImage[i, j]);
                        var width = startX + 120 * j;
                        var height = startY + 120 * i;

                        Canvas.SetLeft(cropImage[i, j], width);
                        Canvas.SetTop(cropImage[i, j], height);
                        cropImage[i, j].MouseLeftButtonDown += CropImage_MouseLeftButtonDown;
                        cropImage[i, j].MouseLeftButtonUp += CropImage_MouseLeftButtonUp;
                        cropImage[i, j].MouseMove += CropImage_MouseMove;
                        cropImage[i, j].Tag = new Tuple<int, int>(i, j);
                    }
                }
            }
            reader.Close();
            DrawLines(Rows, Rows, 120);
      
            imgUp.Source = new BitmapImage(new Uri("Up.png", UriKind.Relative));
            imgDown.Source = new BitmapImage(new Uri("down.png", UriKind.Relative));
            imgLeft.Source = new BitmapImage(new Uri("left.jpg", UriKind.Relative));
            imgRight.Source = new BitmapImage(new Uri("right.jpg", UriKind.Relative));
            MessageBox.Show("Yeah! Your Game is loaded");

            if (!setUpTime)
            {
                // khởi tạo bộ đếm thời gian
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
                setUpTime = true;
            }
            else
            {
                timer.Stop();
                counter = 180;
                timer.Start();
            }
        }

        bool first2 = true;

        private void btnUpClick(object sender, RoutedEventArgs e)
        {
            int indexX = 0, indexY = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (lines[i, j] == -1)
                    {
                        indexX = i;
                        indexY = j;
                        break;
                    }
                }
            }
            cropImage[indexX, indexY] = null;
            // MessageBox.Show(indexX.ToString() + indexY.ToString());
            posX = indexX;
            posY = indexY;
            locationX = 140 + 120 * indexX;
            locationY = 140 + 120 * indexY;
            if (first2)
            {
                first2 = false;
            }

            if (posX - 1 >= 0)
            {
                _selectedBitmap = cropImage[posX - 1, posY];
                Canvas.SetLeft(_selectedBitmap, locationY);
                Canvas.SetTop(_selectedBitmap, locationX);
                int tam = lines[posX - 1, posY];
                lines[posX - 1, posY] = lines[posX, posY];
                lines[posX, posY] = tam;

                Image temp = new Image();
                temp = cropImage[posX, posY];
                cropImage[posX, posY] = cropImage[posX - 1, posY];
                cropImage[posX - 1, posY] = temp;

                posX--;
                locationX -= 120;

            }
            checkGame();
        }

        private void btnDownClick(object sender, RoutedEventArgs e)
        {
            int indexX = 0, indexY = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (lines[i, j] == -1)
                    {
                        indexX = i;
                        indexY = j;
                        break;
                    }
                }
            }
            cropImage[indexX, indexY] = null;
            //MessageBox.Show(indexX.ToString() + indexY.ToString());
            posX = indexX;
            posY = indexY;
            locationX = 140 + 120 * indexX;
            locationY = 140 + 120 * indexY;
            if (first2)
            {
               
                first2 = false;
            }

            if (posX + 1 <= 2)
            {
                    _selectedBitmap = cropImage[posX+1, posY];
                Canvas.SetLeft(_selectedBitmap, locationY);
                Canvas.SetTop(_selectedBitmap, locationX);
                int tam = lines[posX + 1, posY];
                lines[posX + 1, posY] = lines[posX, posY];
                lines[posX, posY] = tam;

                Image temp = new Image();
                temp = cropImage[posX, posY];
                cropImage[posX, posY] = cropImage[posX + 1, posY];
                cropImage[posX + 1, posY] = temp;

                posX++;
                locationX += 120;
                
            }
            checkGame();
        }

        private void btnLeftClick(object sender, RoutedEventArgs e)
        {
            int indexX = 0, indexY = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (lines[i, j] == -1)
                    {
                        indexX = i;
                        indexY = j;
                        break;
                    }
                }
            }
            cropImage[indexX, indexY] = null;
            //MessageBox.Show(indexX.ToString() + indexY.ToString());
            posX = indexX;
            posY = indexY;
            locationX = 140 + 120 * indexX;
            locationY = 140 + 120 * indexY;
            if (first2)
            {
                
                first2 = false;
            }

            if (posY - 1 >= 0)
            {
                _selectedBitmap = cropImage[posX, posY - 1];

                Canvas.SetLeft(_selectedBitmap, locationY);
                Canvas.SetTop(_selectedBitmap, locationX);
                int tam = lines[posX, posY - 1];
                lines[posX, posY - 1] = lines[posX, posY];
                lines[posX, posY] = tam;

                Image temp = new Image();
                temp = cropImage[posX, posY];
                cropImage[posX, posY] = cropImage[posX, posY-1];
                cropImage[posX, posY-1] = temp;

                posY--;
                locationY -= 120;
            }
            checkGame();
        }

        private void btnRightClick(object sender, RoutedEventArgs e)
        {
            int indexX = 0, indexY = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (lines[i, j] == -1)
                    {
                        indexX = i;
                        indexY = j;
                        break;
                    }
                }
            }
            cropImage[indexX, indexY] = null;
            //MessageBox.Show(indexX.ToString() + indexY.ToString());
            posX = indexX;
            posY = indexY;
            locationX = 140 + 120 * indexX;
            locationY = 140 + 120 * indexY;
            if (first2)
            {
               
                first2 = false;
            }

            if (posY + 1 <= 2)
            {
                _selectedBitmap = cropImage[posX, posY+1];
                Canvas.SetLeft(_selectedBitmap, locationY);
                Canvas.SetTop(_selectedBitmap, locationX);
                int tam = lines[posX, posY + 1];
                lines[posX, posY + 1] = lines[posX, posY];
                lines[posX, posY] = tam;

                Image temp = new Image();
                temp = cropImage[posX, posY];
                cropImage[posX, posY] = cropImage[posX , posY+1];
                cropImage[posX , posY+1] = temp;

                posY++;
                locationY += 120;
            }
            checkGame();
        }

        

        private void DrawLines(int Rows, int Cols, int x)
        {
            Line[] lineHor = new Line[Rows + 1];
            Line[] lineVer = new Line[Rows + 1];


            for (int i = 0; i < Rows + 1; i++)
            {
                lineHor[i] = new Line();
                lineHor[i].StrokeThickness = 1;
                lineHor[i].Stroke = new SolidColorBrush(Colors.IndianRed);
                canvas.Children.Add(lineHor[i]);

                lineHor[i].X1 = startX + i * x;
                lineHor[i].Y1 = startY;
                lineHor[i].X2 = startX + i * x;
                lineHor[i].Y2 = startY + Rows * x;

                lineVer[i] = new Line();
                lineVer[i].StrokeThickness = 1;
                lineVer[i].Stroke = new SolidColorBrush(Colors.IndianRed);
                canvas.Children.Add(lineVer[i]);

                lineVer[i].X1 = startX;
                lineVer[i].Y1 = startY + i * x;
                lineVer[i].X2 = startX + Rows * x;
                lineVer[i].Y2 = startY + i * x;
            }
        }
         
    }
}
