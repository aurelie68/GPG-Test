using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly string gpgPath = @"C:\Program Files (x86)\GnuPG\bin\gpg.exe";

        [HttpPost("decrypt")]
        public IActionResult DecryptAndDownload()
        {
            try
            {
                string fileName = "SPRING.png"; // Nom de mon fichier PNG chiffré
                string decryptedFolderPath = @"C:\Users\8301681S\Documents\DecryptedFiles"; // Chemin spécifié pour les fichiers déchiffrés

                // Chemin du fichier chiffré
                string encryptedFilePath = Path.Combine(@"C:\Users\8301681S\Pictures\Screenshots", fileName + ".gpg");

                // Chemin du fichier déchiffré
                string decryptedFilePath = Path.Combine(decryptedFolderPath, fileName.Replace(".gpg", "_decrypted.png"));

                // Commande pour déchiffrer le fichier avec le mot de passe
                string decryptCommand = $"--batch --yes --output {decryptedFilePath} --decrypt {encryptedFilePath}";

                // Lancer le processus avec la commande GnuPG
                RunGpgProcess(decryptCommand);

                // Renvoyer le fichier déchiffré pour téléchargement avec choix de l'emplacement
                return DownloadFile(decryptedFilePath, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors du déchiffrement : {ex.Message}");
            }
        }

        private IActionResult DownloadFile(string filePath, string contentType)
        {
            var fileInfo = new FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return File(fileStream, contentType, fileInfo.Name);
        }

        private void RunGpgProcess(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = gpgPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                Arguments = command
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
