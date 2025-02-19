﻿using System.Security.Cryptography.X509Certificates;
using DevEx.Core;
using DevEx.Core.Helpers;
using DevEx.Core.Storage;
using DevEx.Modules.SSL.Helpers;

namespace DevEx.Modules.SSL.Handlers
{
    internal class SslInstallCertificateHandler : ICommandHandler
    {
        public void Execute(Dictionary<string, string> options)
        {
            var sslConfigPath = CertificateHelper.GetSslConfigPath();
            var sslCertificatePassword = UserStorageManager.GetDecryptedValue("SslCertificatePassword");

            //Delete existing certificates
            CertificateHelper.DeleteCertificate($"DevExLead Root CA", new X509Store(StoreName.Root, StoreLocation.LocalMachine));
            CertificateHelper.DeleteCertificate($"DevExLead Wildcard", new X509Store(StoreName.My, StoreLocation.LocalMachine));

            //Add to the stores
            TerminalHelper.Run(TerminalHelper.ConsoleMode.Powershell, $"certutil -addstore root ca.crt", sslConfigPath);
            TerminalHelper.Run(TerminalHelper.ConsoleMode.Powershell, $"certutil -addstore my wildcard.crt", sslConfigPath);
            TerminalHelper.Run(TerminalHelper.ConsoleMode.Powershell, $"Import-PfxCertificate -FilePath 'wildcard.pfx' -Password (ConvertTo-SecureString -String '{sslCertificatePassword}' -AsPlainText -Force) -CertStoreLocation 'Cert:\\LocalMachine\\My'", sslConfigPath);
        }
    }
}
