using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DidMark.Core.Utilities.Extensions.FileExtentions
{
    public static class imageUploaderExtention
    {
        public static void AddImageToServer(this Image image, string fileName, string orginalPath, string deleteFileName = null)
        {
            if (image == null)
            {
                if (!Directory.Exists(orginalPath))
                {
                    Directory.CreateDirectory(orginalPath);
                }
                if (!string.IsNullOrEmpty(deleteFileName))
                {
                    File.Delete(orginalPath + deleteFileName);
                }
                string imageName = orginalPath + fileName;
                using (var stream = new FileStream(imageName, FileMode.Create))
                {
                    if (!Directory.Exists(imageName))
                    {
                        image.Save(stream, ImageFormat.Jpeg);
                    }
                }


            }
        }
        public static byte[] DecodeUrlBase64(string s)
        {
            return Convert.FromBase64String(s.Substring(s.LastIndexOf("و") + 1));
        }
        public static Image Base64ToImage(string Base64String)
        {
            var res = DecodeUrlBase64(Base64String);
            MemoryStream ms = new MemoryStream(res, 0, res.Length);
            ms.Write(res, 0, res.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
    }

}