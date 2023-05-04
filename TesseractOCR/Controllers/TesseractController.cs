using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace TesseractOCR.Controllers
{
    public class TesseractController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public TesseractController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(IFormFile file)
        {
            var result = string.Empty;
            string imageBase64 = null;
            string folderPath = Path.Combine(_env.WebRootPath, "tessdata");

            using (var engine = new TesseractEngine(folderPath, "eng", EngineMode.Default))
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using (var img = Pix.LoadFromMemory(stream.ToArray()))
                    {
                        using (var page = engine.Process(img))
                        {
                            result = page.GetText();
                        }
                    }
                }
            }

            // Convert the image to a base64-encoded string
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                var bytes = stream.ToArray();
                imageBase64 = Convert.ToBase64String(bytes);
            }

            // Set the OCR result and image in the ViewBag or a ViewModel
            ViewBag.Result = result;
            ViewBag.ImageBase64 = imageBase64;

            return View();
        }

    }
}
