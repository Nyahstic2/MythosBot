using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MythosBot
{
    internal class MimeToString
    {
        public static string PegarOTipoDoArquivo(string MimeType)
        {
            return MimeType switch
            {
                // Tipos de Texto
                MediaTypeNames.Text.Plain => "de Texto Simples",
                MediaTypeNames.Text.Html => "de HTML",
                MediaTypeNames.Text.RichText => "de Texto Rico (RTF)",
                MediaTypeNames.Text.Xml => "de Documento XML",
                "text/csv" => "de Dados CSV (Comma-Separated Values)",
                "text/css" => "de Arquivo CSS",
                "text/javascript" => "de JavaScript",

                // Tipos de Aplicação
                MediaTypeNames.Application.Pdf => "de Documento PDF",
                MediaTypeNames.Application.Octet => "de Arquivo Binário",
                MediaTypeNames.Application.Rtf => "de Documento RTF",
                "application/vnd.ms-excel" => "de Planilha Excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => "de Planilha Excel (XLSX)",
                "application/msword" => "de Documento Word",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => "de Documento Word (DOCX)",
                "application/x-www-form-urlencoded" => "de Formulário Codificado",
                "application/x-java-archive" => "de Arquivo Java (JAR)",
                "application/x-msdownload" => "de Executável Windows (EXE)",
                "application/x-sh" => "de Script Shell",
                "application/x-bat" => "de Script Batch (BAT)",

                // Tipos de Imagem
                MediaTypeNames.Image.Jpeg => "de Imagem JPEG",
                MediaTypeNames.Image.Gif => "de Imagem GIF",
                MediaTypeNames.Image.Tiff => "de Imagem TIFF",
                MediaTypeNames.Image.Png => "de Imagem PNG",
                "image/bmp" => "de Imagem BMP",
                "image/webp" => "de Imagem WebP",
                "image/svg+xml" => "de Imagem SVG",

                // Tipos de Áudio
                "audio/basic" => "de Áudio",
                "audio/wav" => "de Áudio WAV",
                "audio/wave" => "de Áudio WAV",
                "audio/mpeg" => "de Áudio MPEG",
                "audio/ogg" => "de Áudio OGG",

                // Tipos de Vídeo
                "video/mp4" => "de Vídeo MP4",
                "video/x-msvideo" => "de Vídeo AVI",
                "video/webm" => "de Vídeo WebM",
                "video/mpeg" => "de Vídeo MPEG",

                // Outros
                "application/json" => "de Dados JSON",
                "application/zip" => "de Arquivo Compactado (ZIP)",
                "application/x-tar" => "de Arquivo TAR",
                "application/x-7z-compressed" => "de Arquivo Compactado (7z)",
                "application/vnd.rar" => "de Arquivo Compactado (RAR)",
                "application/x-executable" => "de Executável Genérico",

                _ => "de um tipo desconhecido"
            };
        }
    }
}
