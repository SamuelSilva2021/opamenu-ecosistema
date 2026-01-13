using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpaMenu.Application.Common.Interfaces;
using OpaMenu.Application.Common.Models;
using OpaMenu.Application.DTOs;
using OpaMenu.Domain.DTOs;

namespace OpaMenu.Web.UserEntry;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : BaseController
{
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(
        IFileStorageService fileStorageService,
        ILogger<FilesController> logger)
    {
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<FileUploadResult>> UploadFile(
        IFormFile file,
        [FromForm] string folder = "products")
    {
        var serviceResponse = await _fileStorageService.UploadImageAsync(file, folder);
        return BuildResponse(serviceResponse);
    }

    [HttpPost("upload-multiple")]
    public async Task<ActionResult<List<FileUploadResult>>> UploadMultipleFiles(
        List<IFormFile> files,
        [FromForm] string folder = "products")
    {
        if (files is null || files.Count == 0)
        {
            return BadRequest(new { error = "Nenhum arquivo foi fornecido." });
        }

        if (files.Count > 10)
        {
            return BadRequest(new { error = "Máximo de 10 arquivos por vez." });
        }

        var uploadTasks = files
        .Where(f => f.Length > 0)
        .Select(file => _fileStorageService.UploadImageAsync(file, folder));

        // Aguarda todos os uploads terminarem simultaneamente
        var results = await Task.WhenAll(uploadTasks);

        var successfulUploads = results
       .Where(r => r.Succeeded && r.Data is not null)
       .Select(r => r.Data!)
       .ToList();

        var errors = results
        .Where(r => !r.Succeeded)
        .SelectMany(r => r.Errors)
        .ToList();

        if (successfulUploads.Count == 0)
        {
            return errors.Any()
                ? BuildResponse(new ResponseDTO<List<FileUploadResult>> { Succeeded = false, Errors = errors, Code = StatusCodes.Status400BadRequest })
                : BadRequest(new { error = "Falha ao realizar upload dos arquivos." });
        }

        var response = new ResponseDTO<List<FileUploadResult>>
        {
            Succeeded = true,
            Data = successfulUploads,
            Code = StatusCodes.Status200OK,
        };

        _logger.LogInformation("Upload múltiplo concluído: {Count} arquivos salvos com sucesso.", successfulUploads.Count);

        return BuildResponse(response);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteFile([FromQuery] string filePath)
    {
        var serviceResponse = await _fileStorageService.DeleteFileAsync(filePath);
        return BuildResponse(serviceResponse);
    }

    [HttpGet("url")]
    [AllowAnonymous]
    public ActionResult<string> GetFileUrl([FromQuery] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new { error = "Caminho do arquivo é obrigatório." });

        var serviceResponse = _fileStorageService.GetFileUrl(filePath);
        var response = new ResponseDTO<string>
        {
            Succeeded = true,
            Data = serviceResponse,
            Code = StatusCodes.Status200OK
        };
        return BuildResponse(response);
    }

    [HttpGet("variants")]
    [AllowAnonymous]
    public async Task<ActionResult<List<string>>> GetImageVariants([FromQuery] string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return BadRequest(new { error = "Caminho do arquivo é obrigatório." }); 

        var serviceResponse = await _fileStorageService.GetImageVariantsAsync(filePath);
        return BuildResponse(serviceResponse);
    }
}
