using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MoodBoardApp;


namespace MoodBoardApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double canvasWidth = 0;
        private double canvasHeight = 0;
        private bool leftMouseBtnDown = false;
        private MoodBoard moodBoard;

        /// <summary>
        /// Den bild man högerklickat på och ämnar ta bort från moodboarden
        /// </summary>
        private Image highlightedImage = null;

        /// <summary>
        /// Värde som bestämmer om en bild ska vara i förgrunden eller bakgrunden. 
        /// </summary>
        private int highestPriority;

        public MainWindow()
        {
            InitializeComponent();
            moodBoard = new MoodBoard();
        }

        /// <summary>
        /// Tar bort den valda bilden från moodboarden och sänker värdet på higestpriority med 1
        /// </summary>
        private void Remove()
        {
            if (highlightedImage == null)
            {
                MessageBox.Show("No image selected");
                return;
            }

            moodBoard.Remove(highlightedImage);
            canvas.Children.Remove(highlightedImage);
            highlightedImage = null;
            highestPriority--;
        }


        /// <summary>
        /// Resnart moodboard och sätter higestpriority till 0
        /// </summary>
        private void Reset()
        {
            moodBoard.Clear();
            canvas.Children.Clear();
            highestPriority = 0;
        }

        /// <summary>
        /// Laddar en tidigare sparad moodboard
        /// </summary>
        private void Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "ser Files(*.ser)|*.ser;";
            if (dialog.ShowDialog() == true)
            {
                Reset();
                moodBoard.Open(dialog.FileName);

                //Skapa en Image för varje ImageFile, koplla listeners och lägg den till canvas
                foreach (ImageFile imagefile in moodBoard.ImageFiles)
                {
                    Image img = ImageGenerator.GetImage(imagefile);
                    AddListenersAndContexMenu(img);
                    canvas.Children.Add(img);
                    highestPriority++;
                }
            }                
        }


        private void SaveAsImage()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG Files(*.png)|*.png;";
            if(dialog.ShowDialog() == true)
            {
                //Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
                //canvas.Measure(size);

                var rtb = new RenderTargetBitmap((int)canvasWidth, (int)canvasHeight, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(canvas);
                SaveRTBAsPNG(rtb, dialog.FileName);
            }

        }

        private void SaveRTBAsPNG(RenderTargetBitmap bmp, string filename)
        {
            var enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));

            using (var stm = System.IO.File.Create(filename))
            {
                enc.Save(stm);
            }
        }

        /// <summary>
        /// Lägger till en bild till moodboarden och kopplar event listeners till den
        /// </summary>
        private void Add()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            if (dialog.ShowDialog() == true)
            {
                ImageFile imageFile = new ImageFile();
                imageFile.Path = dialog.FileName;
                imageFile.Left = 0;
                imageFile.Top = 0;
                imageFile.Width = 100;
                imageFile.Height = 100;
                moodBoard.AddFile(imageFile);

                //Image to add to canvas.children
                Image img = ImageGenerator.GetImage(imageFile);
                AddListenersAndContexMenu(img);
                canvas.Children.Add(img);

                highestPriority++;
            }        
        }

        /// <summary>
        /// Sparar en moodboard i dess aktuella tillstånd
        /// </summary>
        private void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "ser Files(*.ser)|*.ser;";
            if (dialog.ShowDialog() == true)
                moodBoard.Save(dialog.FileName);            
        }

        private void ChangeImageSize()
        {
            if (highlightedImage == null)
            {
                MessageBox.Show("No image selected");
                return;
            }

            SizeWindow window = new SizeWindow();
            if (window.ShowDialog() == true)
            {
                double width = window.NewWidth;
                double height = window.NewHeight;
                if (width > 0 && height > 0)
                {
                    ImageFile imageFile = moodBoard.GetImageFile(highlightedImage.Name);
                    imageFile.Width = width;
                    imageFile.Height = height;
                    highlightedImage.Width = width;
                    highlightedImage.Height = height;
                }
            }

            highlightedImage = null;
        }


        /// <summary>
        /// Ändrar moodboardens bilders ZIndex, dvs det värde som avgör om de ska ligga i förgunden eller bakgrunden. Den Image som skickas som parameter ges högsta prioritet,
        /// och alla andra Images ges prioritet -= 1.
        /// </summary>
        private void SetImagePriorities(Image currentlySelectedImage)
        {
            Canvas.SetZIndex(currentlySelectedImage, highestPriority);

            foreach (Image image in canvas.Children)
            {
                if (image.Equals(currentlySelectedImage))
                    continue;

                int priority = Canvas.GetZIndex(image);
                if (priority > 0)
                    priority--;

                Canvas.SetZIndex(image, priority);
            }
        }

        /// <summary>
        /// Lägger till event listeners och popupmeny till en Image
        /// </summary>
        private void AddListenersAndContexMenu(Image img)
        {
            //listeners
            img.MouseLeftButtonDown += Img_MouseLeftButtonDown;
            img.MouseLeftButtonUp += Img_MouseLeftButtonUp;
            img.MouseRightButtonDown += Img_MouseRightButtonDown;
            img.MouseMove += Img_MouseMove;

            //popup menu
            ContextMenu popupMenu = new ContextMenu();
            popupMenu.Items.Add(new MenuItem() { Header = "Ta bort" });
            popupMenu.Items.Add(new MenuItem() { Header = "Ändra storlek" });
            foreach (MenuItem menuItem in popupMenu.Items)
                menuItem.Click += DropdownMenuItem_Click;

            img.ContextMenu = popupMenu;
        }


        //Event listeners
        private void DropdownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Ta bort":
                    Remove();
                    break;

                case "Ändra storlek":
                    ChangeImageSize();
                    break;
            }
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftMouseBtnDown)
            {
                Image currentlySelectedImage = (sender as Image);
                //Get corresponding ImageFile
                ImageFile imageFile = moodBoard.GetImageFile(currentlySelectedImage.Name);

                //Räkna ut bildens nya position och lagra den i ImageFilen. Sätt sedan currentlySelectedImage att ha den positionen.
                if (imageFile != null)
                {
                    Point mousePos = e.GetPosition(canvas);
                    double halfWidth = currentlySelectedImage.Width / 2;
                    double halfHeight = currentlySelectedImage.Height / 2;
                    double left = mousePos.X - halfWidth;
                    double top = mousePos.Y - halfHeight;
                    imageFile.Left = left;
                    imageFile.Top = top;
                    Canvas.SetLeft(currentlySelectedImage, imageFile.Left);
                    Canvas.SetTop(currentlySelectedImage, imageFile.Top);

                    //Console.WriteLine(imageFile.Name + " " + Canvas.GetLeft(currentlySelectedImage) + " " + Canvas.GetTop(currentlySelectedImage));
                }
            }
        }

        private void Img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            leftMouseBtnDown = false;
        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            leftMouseBtnDown = true;
            highlightedImage = null;
            SetImagePriorities(sender as Image);
        }

        private void Img_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            highlightedImage = (sender as Image);
        }

        private void MainMenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header.ToString())
            {
                case "Spara moodboard":
                    Save();
                    break;

                case "Lägg till en bild":
                    Add();
                    break;

                case "Öppna en moodboard":
                    Open();
                    break;

                case "Spara moodboard som bild":
                    SaveAsImage();
                    break;
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasWidth = canvas.ActualWidth;
            canvasHeight = canvas.ActualHeight;
        }
    }
}
