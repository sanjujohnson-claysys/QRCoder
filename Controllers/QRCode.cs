using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using Serilog.Sinks.RollingFile;
using Serilog;

using Serilog.Sinks.File;
using Serilog.Extensions.Logging;
using Serilog.Sinks.RollingFile;

[ApiController]
[Route("api/[controller]")]
public class QRCodeController : ControllerBase
{
    [HttpPost]
    [Route("GenerateQRCode")]
    public IActionResult GenerateQRCode([FromBody] string qrCodeString)
    {
        try
        {
            // Configure Serilog logger
            // Specify the full path for the log file
            string logFilePath = "C:\\Users\\sanju.johnson\\source\\repos\\QRCoder\\logs.txt";

            // Configure Serilog logger to write logs to the specified file and the console
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Infinite)
                .WriteTo.Console()
                .CreateLogger();

            var writer = new QRCodeWriter();
            // Generate QR Code
            var resultBit = writer.encode(qrCodeString, BarcodeFormat.QR_CODE, 200, 200);
            // Get Bitmatrix result
            var matrix = resultBit;

            // Convert bitmatrix into image
            int scale = 2;

            Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int x = 0; x < matrix.Height; x++)
            {
                for (int y = 0; y < matrix.Width; y++)
                {
                    System.Drawing.Color pixel = matrix[x, y] ? System.Drawing.Color.Black : System.Drawing.Color.White;
                    for (int i = 0; i < scale; i++)
                    {
                        for (int j = 0; j < scale; j++)
                        {
                            result.SetPixel(x * scale + i, y * scale + j, pixel);
                        }
                    }
                }
            }

            // Convert Bitmap to bytes
            using (var stream = new MemoryStream())
            {
                result.Save(stream, ImageFormat.Png);
                var imageBytes = stream.ToArray();

                // Log successful QR code generation
                Log.Information("QR code generated successfully for {QRCodeString}", qrCodeString);

                // Return the image bytes as response
                return File(imageBytes, "image/png");

            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Log.Error(ex, "An error occurred while generating QR code");

            // Handle the exception and return an appropriate response
            return BadRequest("QR code generation failed: " + ex.Message);
        }
    }

}
