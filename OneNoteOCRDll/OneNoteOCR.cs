using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;
using System.Collections.Generic;

namespace OneNoteOCRDll
{
    /// <summary>
    /// ocr with one node
    /// </summary>
    public class OneNoteOCR
    {

        /// <summary>
        /// Gets or sets the one note application.
        /// </summary>
        /// <value>
        /// The one note application.
        /// </value>
        public Application OneNoteApp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneNoteOCR"/> class.
        /// </summary>
        public OneNoteOCR()
        {
            try
            {
                OneNoteApp = new Application();
            }
            catch (Exception)
            {
                Console.WriteLine("OneNote 15 must be installed for this user");
                return;
            }
        }

        /// <summary>
        /// Releases the object.
        /// </summary>
        public void ReleaseObject()
        {
            Console.WriteLine("Releasing Marshall Objec(t inside Recognize Image");
            Marshal.ReleaseComObject(OneNoteApp);
        }

        /// <summary>
        /// verify one note exists on pc
        /// </summary>
        public void Verify()
        {
            OneNoteApp = new Application();
            Marshal.ReleaseComObject(OneNoteApp);
            OneNoteApp = null;
            Thread.Sleep(2000);
        }

        /// <summary>
        /// Recognize text in image
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public Tuple<XDocument, Image> RecognizeImage(Image imageWanted)
        {
            string sections;
            OneNoteApp.GetHierarchy(null, HierarchyScope.hsSections, out sections);
            var doc = XDocument.Parse(sections);
            var ns = doc.Root.Name.Namespace;
            var node = doc.Descendants(ns + "Section").First();
            var sectionId = node.Attribute("ID").Value;
            string pageId;
            OneNoteApp.CreateNewPage(sectionId, out pageId);
            var imageCreated = InsertImage(imageWanted, pageId, OneNoteApp);
            //update the note page 
            Thread.Sleep(2000);
            string str = "";
            OneNoteApp.GetPageContent(pageId, out str, PageInfo.piBasic, XMLSchema.xsCurrent);
            doc = XDocument.Parse(str);
            OneNoteApp.DeleteHierarchy(pageId, deletePermanently: true);
            return new Tuple<XDocument, Image>(doc, imageCreated);
        }

        /// <summary>
        /// Inserts the image.
        /// </summary>
        /// <param name="imageWanted">The image wanted.</param>
        /// <param name="existingPageId">The existing page identifier.</param>
        /// <param name="oneNoteApp">The one note application.</param>
        /// <returns></returns>
        Image InsertImage(Image imageWanted, string existingPageId, Application oneNoteApp)
        {
            string strNamespace = "http://schemas.microsoft.com/office/onenote/2013/onenote";
            string m_xmlImageContent =
                "<one:Image><one:Size width=\"{1}\" height=\"{2}\" isSetByUser=\"true\" /><one:Data>{0}</one:Data></one:Image>";
            string m_xmlNewOutline =
                "<?xml version=\"1.0\"?><one:Page xmlns:one=\"{2}\" ID=\"{1}\"><one:Title><one:OE><one:T><![CDATA[{3}]]></one:T></one:OE></one:Title>{0}</one:Page>";
            string pageToBeChange = "RecognizeImage" + DateTime.Now.ToString("yyyyMMddHHmmss");
            string fileString;

            Image baseScreenshot = imageWanted;
            using (var bitmap = new Bitmap(baseScreenshot))
            {
                var stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Png);
                fileString = Convert.ToBase64String(stream.ToArray());

                string imageXmlStr = string.Format(m_xmlImageContent, fileString, bitmap.Width, bitmap.Height);
                string pageChangesXml = string.Format(m_xmlNewOutline,
                    new object[] { imageXmlStr, existingPageId, strNamespace, pageToBeChange });

                oneNoteApp.UpdatePageContent(pageChangesXml);

            }
            return baseScreenshot;
        }
    }
}
