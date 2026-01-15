using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpaMenu.Application.Common.Interfaces;
using OpaMenu.Application.Common.Models;
using OpaMenu.Application.DTOs;
using OpaMenu.Commons.Api.Commons;
using OpaMenu.Commons.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Infrastructure.Services
{
    public class CloudinaryFileStorageService(
        Cloudinary cloudinary,
        ILogger<CloudinaryFileStorageService> logger
        ) : IFileStorageService
    {
        private readonly Cloudinary _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));

        private readonly ILogger<CloudinaryFileStorageService> _logger = logger;

        public Task<ResponseDTO<bool>> DeleteFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public string GetFileUrl(string filePath)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseDTO<List<string>>> GetImageVariantsAsync(string originalPath)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDTO<FileUploadResult>> UploadImageAsync(IFormFile file, string folder)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return StaticResponseBuilder<FileUploadResult>
                        .BuildError("Nenhum arquivo foi fornecido para upload.");
                }

                var validationResult = ValidateImageFile(file);
                if (!validationResult.IsSuccess)
                {
                    return StaticResponseBuilder<FileUploadResult>
                        .BuildError(validationResult.ErrorMessage!);
                }

                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = $"opamenu/{folder}",
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false,
                    Transformation = new Transformation()
                        .Quality("auto")
                        .FetchFormat("auto")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != HttpStatusCode.OK)
                {
                    return StaticResponseBuilder<FileUploadResult>
                        .BuildError("Erro ao enviar imagem para o storage.");
                }

                var publicId = uploadResult.PublicId;

                // Geração de variantes (Cloudinary way)
                var thumbnailUrl = _cloudinary.Api.UrlImgUp
                    .Transform(
                        new Transformation()
                            .Width(150)
                            .Height(150)
                            .Crop("fill")
                            .Quality("auto")
                            .FetchFormat("auto")
                    )
                    .BuildUrl(publicId);

                var mediumUrl = _cloudinary.Api.UrlImgUp
                    .Transform(
                        new Transformation()
                            .Width(400)
                            .Quality("auto")
                            .FetchFormat("auto")
                    )
                    .BuildUrl(publicId);

                var largeUrl = _cloudinary.Api.UrlImgUp
                    .Transform(
                        new Transformation()
                            .Width(800)
                            .Quality("auto")
                            .FetchFormat("auto")
                    )
                    .BuildUrl(publicId);

                var result = new FileUploadResult
                {
                    IsSuccess = true,
                    FileName = publicId,
                    OriginalFileName = file.FileName,
                    FileUrl = uploadResult.SecureUrl.ToString(),
                    ThumbnailUrl = thumbnailUrl,
                    MediumUrl = mediumUrl,
                    LargeUrl = largeUrl,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    UploadDate = DateTime.UtcNow
                };

                _logger.LogInformation(
                    "Image uploaded successfully. PublicId: {PublicId}",
                    publicId);

                return StaticResponseBuilder<FileUploadResult>
                    .BuildCreated(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return StaticResponseBuilder<FileUploadResult>
                    .BuildErrorResponse(ex);
            }
        }
        private (bool IsSuccess, string? ErrorMessage) ValidateImageFile(IFormFile file)
        {
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                return (false, "Tipo de arquivo inválido. Apenas imagens são permitidas.");
            }
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
            {
                return (false, "O tamanho do arquivo excede o limite máximo de 5 MB.");
            }
            return (true, null);
        }
    }
}
