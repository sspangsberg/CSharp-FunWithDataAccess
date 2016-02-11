using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace FunWithDataAccess
{
    class GZippy
    {
        public static long SqueezeIt(string unCompressedFilePath)
        {
            StringBuilder sb = new StringBuilder();
            long squeezedSize = 0;

            using (StreamReader sr = File.OpenText(unCompressedFilePath))
            {
                while (!sr.EndOfStream)
                    sb.Append(sr.ReadLine());
            }

            byte[] dataToCompress = Encoding.UTF8.GetBytes(sb.ToString());
            
            
            using (FileStream fsCompressed = File.Create(unCompressedFilePath + ".gz"))
            {
                using (GZipStream compressionStream = new GZipStream(fsCompressed, CompressionMode.Compress))
                {
                    compressionStream.Write(dataToCompress, 0, dataToCompress.Length);
                    squeezedSize = fsCompressed.Length;
                }
            }

            return squeezedSize;
        }


        public static void BlowItBackUp(FileInfo compressedFile)
        {
            using (FileStream fsCompressed = compressedFile.OpenRead())
            {
                string existingFile = compressedFile.FullName;
                string newFileName = existingFile.Remove(existingFile.Length - compressedFile.Extension.Length);  
                using (FileStream fsExpanded = File.Create(newFileName))
                {
                    using (GZipStream fsExpandedZip = new GZipStream(fsCompressed, CompressionMode.Decompress))
                    {
                        fsExpandedZip.CopyTo(fsExpanded);
                    }
                }
            }
        }

    }
}
