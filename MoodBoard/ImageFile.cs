using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoodBoardApp
{
    /// <summary>
    /// Klass som används för att representera en bildfil. 
    /// </summary>
    [Serializable]
    public class ImageFile
    {
        public String Path { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public String Name { get; set; }
    }
}
