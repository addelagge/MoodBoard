using MoodBoardApp;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MoodBoardApp
{
    public class ImageGenerator
    {
        /// <summary>
        /// Returnerar en Image utifrån den ImageFile som skickas som parameter
        /// </summary>
        public static Image GetImage(ImageFile imageFile)
        {
            Image img = new Image();
            img.Width = imageFile.Width;
            img.Height = imageFile.Height;
            img.Source = new BitmapImage(new Uri(imageFile.Path));
            img.Name = imageFile.Name;
            Canvas.SetLeft(img, imageFile.Left);
            Canvas.SetTop(img, imageFile.Top);

            return img;
        }
    }
}
