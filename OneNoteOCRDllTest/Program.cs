using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneNoteOCRDll;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace OneNoteOCRDllTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var ocr = new OneNoteOCR();
            var findText = new ActionNote();
            string imagePath = String.Empty;
            string textToFind = String.Empty;

            try
            {
                ocr.Verify();
            }
            catch (Exception)
            {
                Console.WriteLine("you do not have OneNote 15 ");
                return;
            }

            if (Debugger.IsAttached)
            {
                imagePath = @"C:\Users\gastanica\Desktop\serverPASS4.png";
                textToFind = "daily";
            }
            else
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("please add argument = path to the image file");
                    return;
                }
                imagePath = args[0];
                textToFind = args[1];
            }

            TextFound ocrText = new TextFound(0, 0, 0, 0, "", "");
            ocrText = findText.FindText(imagePath, textToFind);
            Console.WriteLine(imagePath);
            Console.WriteLine(textToFind);

            Console.WriteLine(ocrText.TextArea);
            Console.WriteLine(ocrText.Center());
            Console.WriteLine(ocrText.ImageText);
            Console.WriteLine(ocrText.SearchText);
        }

        public static void CreateImage(Rectangle findArea, string imagePath)
        {
            var wantedRegion = new Rectangle(findArea.X, findArea.Y, findArea.Width, findArea.Height);

            var screenshotToBitmap = new Bitmap(imagePath);
            var cloneRectangle = screenshotToBitmap.Clone(wantedRegion, PixelFormat.DontCare);

            //cloneRectangle.Save(@"args[0].\rectangle.png", ImageFormat.Png);
            screenshotToBitmap.Dispose();
        }

    }
}
