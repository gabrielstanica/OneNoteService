using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OneNoteOCRDll
{
    public class ActionNote
    {
        /// <summary>
        /// The one note
        /// </summary>
        private OneNoteOCR OneNote;

        /// <summary>
        /// The get dpi
        /// </summary>
        private GetDeviceDpi GetDpi;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionNote"/> class.
        /// </summary>
        public ActionNote()
        {
            OneNote = new OneNoteOCR();
        }


        /// <summary>
        /// Finds the text.
        /// </summary>
        /// <param name="imageWanted">The image wanted.</param>
        /// <param name="wantedText">The wanted text.</param>
        /// <returns></returns>
        public TextFound FindText(Image imageWanted, string wantedText)
        {
            List<TextFound> result = new List<TextFound>();

            var foundItems = OneNote.RecognizeImage(imageWanted);
            var xmlDocument = foundItems.Item1;
            var imageCreated = foundItems.Item2;

            GetDpi = new GetDeviceDpi(imageCreated);

            IEnumerable<XElement> tokenArray = null;
            var wantedTextLength = wantedText.Length;
            string textValue = string.Empty;
            try
            {
                var textArray = xmlDocument.Descendants().First(t => t.Name.LocalName == "OCRText");
                textValue = textArray.Value;
                tokenArray = xmlDocument.Descendants().Where(t => t.Name.LocalName == "OCRToken").ToList();

                foreach (var elementToken in tokenArray)
                {
                    bool checkExists = false;
                    var startingTokenPosition = int.Parse(elementToken.Attribute("startPos").Value);
                    var stillToSearch = textValue.Length - startingTokenPosition - wantedTextLength;

                    if (stillToSearch > 0)
                    {
                        var wantedSubstring = textValue.Substring(startingTokenPosition, wantedTextLength);
                        checkExists = wantedSubstring.ToLower().Equals(wantedText.ToLower());
                    }

                    float x = 0, y = 0, width = 0, height = 0;

                    if (checkExists)
                    {
                        var widthPoint = float.Parse(elementToken.Attribute("width").Value);

                        int count = 0;
                        var elementIndex = elementToken.ElementsBeforeSelf().Count();
                        if (elementToken.ElementsBeforeSelf().Count() < (elementToken.ElementsBeforeSelf().Count() + count))
                        {
                            var s = tokenArray.ElementAt(elementToken.ElementsBeforeSelf().Count() + count);
                            var positionOfSecondElement = int.Parse(s.Attribute("startPos").Value);
                            int foundLength = positionOfSecondElement - startingTokenPosition - 1;

                            while (wantedTextLength - foundLength >= 0)
                            {
                                var WidthToAdd = float.Parse(s.Attribute("width").Value);
                                widthPoint += WidthToAdd;
                                count++;
                                s = tokenArray.ElementAt(elementToken.ElementsBeforeSelf().Count() + count);
                                foundLength = int.Parse(s.Attribute("startPos").Value) - startingTokenPosition;
                            }
                        }
                        var xPoint = float.Parse(elementToken.Attribute("x").Value);
                        var yPoint = float.Parse(elementToken.Attribute("y").Value);
                        var heightPoint = float.Parse(elementToken.Attribute("height").Value);

                        GetDpi.TransformToPixels(xPoint, out x);
                        GetDpi.TransformToPixels(yPoint, out y);
                        GetDpi.TransformToPixels(widthPoint, out width);
                        GetDpi.TransformToPixels(heightPoint, out height);
                        result.Add(new TextFound(x, y, width, height, wantedText, textValue));
                    }
                }
                result.Add(new TextFound(0, 0, 0, 0, wantedText, textValue));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception thrown, look over");
                Console.WriteLine(ex);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                result.Add(new TextFound(0, 0, 0, 0, wantedText, textValue));
            }

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Finds the text.
        /// </summary>
        /// <param name="imagePath">The image path.</param>
        /// <param name="wantedText">The wanted text.</param>
        /// <returns></returns>
        public TextFound FindText(string imagePath, string wantedText)
        {
            Image imageWanted = Image.FromFile(imagePath);
            return FindText(imageWanted, wantedText);
        }

    }
}
