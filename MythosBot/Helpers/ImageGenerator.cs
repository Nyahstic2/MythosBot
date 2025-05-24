using System.IO;
using SkiaSharp;

namespace MythosBot.Helpers
{
    public static class ImageGenerator
    {
        // Gera uma imagem JPG com texto centralizado usando SkiaSharp
        public static void GenerateJpg(string filePath, string text, int width = 400, int height = 200)
        {
            using var bitmap = new SKBitmap(width, height);
            using var canvas = new SKCanvas(bitmap);
            canvas.Clear(SKColors.White);

            // Configura fonte
            using var typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold);
            using var font = new SKFont(typeface, 32);
            using var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };

            // Centraliza o texto
            var textWidth = font.MeasureText(text, paint);
            var metrics = font.Metrics;
            float x = width / 2f - textWidth / 2f;
            float y = height / 2f - (metrics.Ascent + metrics.Descent) / 2f;
            canvas.DrawText(text, x, y, font, paint);

            // Salva como JPG
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 90);
            using var stream = File.OpenWrite(filePath);
            data.SaveTo(stream);
        }
    }
}