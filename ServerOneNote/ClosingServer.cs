using OneNoteOCRDll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerOneNote
{
    public class ClosingServer
    {
        private  OneNoteOCR ocr { get; set; }

        public ClosingServer(OneNoteOCR OneNote)
        {
            ocr = OneNote;
        }

      
    }
}
