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
        private readonly string passphrase = "toto123";

        [HttpGet("decrypt")]
        public IActionResult Decrypt()
        {
            try
            {
                // Chemin du fichier chiffr� 
                string fileName = "Blagues.txt";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Chemin du dossier o� enregistrer les fichiers d�chiffr�s
                string decryptedFolderPath = Path.Combine(documentsPath, "DecryptFiles");

                // Si le dossier DecryptFiles existe, sinon cr�er
                if (!Directory.Exists(decryptedFolderPath))
                {
                    Directory.CreateDirectory(decryptedFolderPath);
                }

                string encryptedFilePath = Path.Combine(documentsPath, fileName + ".gpg");
                string decryptedFilePath = Path.Combine(decryptedFolderPath, fileName + "_decrypted.txt");

                // Commande pour d�chiffrer le fichier avec le mot de passe
                string decryptCommand = $"--batch --yes --passphrase {passphrase} --output {decryptedFilePath} --decrypt {encryptedFilePath}";

                // Lancer le processus avec la commande GnuPG
                RunGpgProcess(decryptCommand);

                return Ok($"Fichier d�chiffr� avec succ�s. Chemin du fichier d�chiffr� : {decryptedFilePath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors du d�chiffrement : {ex.Message}");
            }
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
