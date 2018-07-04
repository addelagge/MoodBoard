using MoodBoardApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UtilitiesLib;

namespace MoodBoardApp
{
    public class MoodBoard
    {
        /// <summary>
        /// De representationer av Images som lagts till moodboarden
        /// </summary>
        public List<ImageFile> ImageFiles { get; private set; }

        /// <summary>
        /// Löpnummer som används i en ImageFile's 'name'
        /// </summary>
        private int id = 0;

        public MoodBoard()
        {
            ImageFiles = new List<ImageFile>();
        }

        /// <summary>
        /// Tar bort alla ImageFiles
        /// </summary>
        public void Clear()
        {
            ImageFiles.Clear();
        }

        /// <summary>
        /// Tar bort vald ImageFile
        /// </summary>
        /// <param name="imageToRemove"></param>
        public void Remove(Image imageToRemove)
        {
            //TODO ändra så att parameter är ImageFile?
            foreach (ImageFile imageFile in ImageFiles)
            {
                if (imageToRemove.Name.Equals(imageFile.Name))
                {
                    ImageFiles.Remove(imageFile);
                    break;
                }
            }
        }


        /// <summary>
        /// Skapar en ny ImageFile och lägger den till moodbarden
        /// </summary>
        public void AddFile(String fileName)
        {
            ImageFile imageFile = new ImageFile();
            imageFile.Path = fileName;
            imageFile.Left = 0;
            imageFile.Top = 0;
            imageFile.Width = 100;
            imageFile.Height = 100;
            imageFile.Name = "image" + ++id;
            ImageFiles.Add(imageFile);
        }

        /// <summary>
        /// Lägger till en ImageFile till moodboarden
        /// </summary>
        public void AddFile(ImageFile imageFile)
        {
            if (imageFile != null)
            {
                imageFile.Name = "image" + ++id;
                ImageFiles.Add(imageFile);
            }                
        }

        /// <summary>
        /// Returnerar den ImageFile som matchar parametern. Null returneras ifall ingen match finns
        /// </summary>
        public ImageFile GetImageFile(String name)
        {
            foreach(ImageFile imageFile in ImageFiles)
            {
                if (imageFile.Name.Equals(name))
                    return imageFile;
            }

            return null;
        }

        /// <summary>
        /// Sparar moodboarden genom att serialisera listan med ImageFiles
        /// </summary>
        public void Save(String fileName)
        {
            if (ImageFiles.Count == 0)
                return;

            Serializer.XmlFileSerialize<List<ImageFile>>(fileName, ImageFiles);
        }

        /// <summary>
        /// Laddar en tidigare sparad moodboard genom att deserialisera innehållet i filen till listan med ImageFiles
        /// </summary>
        public void Open(String fileName)
        {
            ImageFiles = Serializer.XmlFileDeserialize<List<ImageFile>>(fileName);
            id = ImageFiles.Count;
        }
    }
    
}
