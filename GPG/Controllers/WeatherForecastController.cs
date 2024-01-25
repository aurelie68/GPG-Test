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
                // Chemin du fichier chiffré 
                string fileName = "Blagues.txt";
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // Chemin du dossier où enregistrer les fichiers déchiffrés
                string decryptedFolderPath = Path.Combine(documentsPath, "DecryptFiles");

                // Si le dossier DecryptFiles existe, sinon créer
                if (!Directory.Exists(decryptedFolderPath))
                {
                    Directory.CreateDirectory(decryptedFolderPath);
                }

                string encryptedFilePath = Path.Combine(documentsPath, fileName + ".gpg");
                string decryptedFilePath = Path.Combine(decryptedFolderPath, fileName + "_decrypted.txt");

                // Commande pour déchiffrer le fichier avec le mot de passe
                string decryptCommand = $"--batch --yes --passphrase {passphrase} --output {decryptedFilePath} --decrypt {encryptedFilePath}";

                // Lancer le processus avec la commande GnuPG
                RunGpgProcess(decryptCommand);

                return Ok($"Fichier déchiffré avec succès. Chemin du fichier déchiffré : {decryptedFilePath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur lors du déchiffrement : {ex.Message}");
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
