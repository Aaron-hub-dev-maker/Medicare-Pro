using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Utilities
{
    public class ImageOperations
    {
         IWebHostEnvironment _env;

        // Allowed extensions and max size (e.g., 2 MB)
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

        public ImageOperations(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string ImageUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            if (file.Length > MaxFileSize)
                return string.Empty;

            string filename = null;

            if (file != null)
            {
                string fileDirectory = Path.Combine(_env.WebRootPath, "Images");
                filename = Guid.NewGuid() + "-" + file.FileName;
                string filepath = Path.Combine(fileDirectory, filename);
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    file.CopyToAsync(fs);
                }
            }
            return filename;
        }
    }
}
