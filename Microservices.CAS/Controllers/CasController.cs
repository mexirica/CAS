// using Microservices.CAS.Business;
// using Microservices.CAS.Db;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Microservices._cas.Controllers;
//
// [ApiController]
// public class _casController(ContentAddressableStorage _cas, CASFileRepository repository)
//     : ControllerBase
// {
//     [HttpPost("upload/{filename}")]
//     public async Task<IActionResult> UploadFile(string filename)
//     {
//         try
//         {
//             using var memoryStream = new MemoryStream();
//             await Request.Body.CopyToAsync(memoryStream);
//             var content = memoryStream.ToArray();
//             var _ = await _cas.Store(content, filename);
//             return Created();
//         }
//         catch (Exception ex)
//         {
//             return Problem(detail: "Error uploading file", statusCode: 500);
//         }
//     }
//     
//     [HttpGet("download/{filename}")]
//     public async Task<IActionResult> DownloadFile(string filename)
//     {
//         try
//         {
//             var retrievedContent = await _cas.RetrieveBytes(filename);
//             return retrievedContent != null
//                 ? File(retrievedContent, "application/octet-stream", filename)
//                 : NotFound($"File '{filename}' not found.");
//         }
//         catch (ArgumentException ex)
//         {
//             return NotFound($"File '{filename}' not found.");
//
//         }
//         catch (Exception e)
//         {
//             return Problem(detail: e.Message, statusCode: 500);
//         }
//     }
//
//     [HttpGet("stream/{filename}")]
//     public async Task<IActionResult> GetStream(string filename)
//     {
//         try
//         {
//             var retrievedContent = await _cas.RetrieveStream(filename);
//             return File(retrievedContent, "media/mp4", enableRangeProcessing: true);
//         }
//         catch (ArgumentException ex)
//         {
//             return NotFound($"File '{filename}' not found.");
//
//         }
//         catch (Exception e)
//         {
//             return Problem(detail: e.Message, statusCode: 500);
//         }
//     }
// }
