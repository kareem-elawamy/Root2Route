using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Service.Services.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> UploadImageAsync(string location, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            // 1. Security: تحديد حجم الملف (مثلاً 2 ميجا بايت)
            var maxFileSizeInBytes = 2 * 1024 * 1024;
            if (file.Length > maxFileSizeInBytes)
            {
                throw new ArgumentException("File size exceeds the maximum limit of 2MB.");
            }

            // 2. Security: السماح بامتدادات محددة فقط
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Unsupported file format.");
            }

            // 3. Security: التحقق من "توقيع الملف" (Magic Numbers)
            // هذا يمنع رفع ملف exe تم تغيير اسمه لـ jpg
            using (var stream = file.OpenReadStream())
            {
                if (!IsValidImageSignature(stream, fileExtension))
                {
                    throw new ArgumentException("File content does not match its extension (Potential Malicious File).");
                }
            }

            // 4. Security: تنظيف المسار لمنع Path Traversal
            // يمنع المستخدم من إرسال location بقيمة "../System32"
            var safeLocation = string.Join("_", location.Split(Path.GetInvalidFileNameChars()));

            var path = _webHostEnvironment.WebRootPath;
            var filePath = Path.Combine("uploads", safeLocation);
            var fullPath = Path.Combine(path, filePath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // استخدام Guid لضمان عدم تكرار الاسم وعدم استخدام اسم المستخدم الأصلي
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            var finalPath = Path.Combine(fullPath, fileName);

            using (var fileStream = new FileStream(finalPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // إرجاع المسار بـ Forward Slashes عشان الـ URL
            return Path.Combine("/", filePath, fileName).Replace("\\", "/");
        }

        // دالة مساعدة للتحقق من الماجيك نمبر (Hex Signature)
        private bool IsValidImageSignature(Stream stream, string extension)
        {
            stream.Position = 0;
            using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true))
            {
                // قراءة أول سطور في الـ Header الخاص بالملف
                var headerBytes = reader.ReadBytes(8);

                // تحويل البايتات لنص Hex للمقارنة
                var headerHex = BitConverter.ToString(headerBytes).Replace("-", "").ToUpper();

                // مقارنة التوقيع بناءً على الامتداد
                return extension switch
                {
                    ".jpg" or ".jpeg" => headerHex.StartsWith("FFD8"),
                    ".png" => headerHex.StartsWith("89504E470D0A1A0A"),
                    ".webp" => headerHex.StartsWith("52494646") && headerHex.Substring(16, 8) == "57454250", // RIFF....WEBP
                    _ => false
                };
            }
        }
    }
}