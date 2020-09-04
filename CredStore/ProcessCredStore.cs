// ProcessCredStore.cs Copyright 2020 tomjones

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CredStore
{
    /// <summary>
    /// Process requests to the CredStore as tasks that can be created and awaiting in the main thread
    /// </summary>
  public class ProcessCredStore : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IHostEnvironment _env;
        private bool busy = false;
        public IServiceProvider Services { get; }
        private CngKey cngKey = null;
        private CngAlgorithm keyType = null;
        private X509Certificate2 cert1 = null;
        private SHA256 sha = null;

        public ProcessCredStore(IServiceProvider services,
            ILogger<ProcessCredStore> logger,
            IHostEnvironment env)
        {
            Services = services;
            _logger = logger;
            _env = env;
            sha = SHA256.Create();
    //        rsa = new RSACng(cngKey);
            _logger.LogInformation("Process Cred Store Hosted Service is constructed.");
        }

        private void DoWork()
        {
            _logger.LogInformation("Process Cert Store is doing work");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessService =
                    scope.ServiceProvider.GetRequiredService<ICredStoreSvc>();
   //             scopedProcessService.DoWork();
            }
        }

        ///  this process runs in its own thread for the the full time of the hosting process.
        private void RunProcess(string keyName)
        {
            int threadID = Thread.GetCurrentProcessorId();
            string whoami = Thread.CurrentPrincipal?.Identity?.ToString();
            if (String.IsNullOrEmpty(whoami))
            {
                whoami = Environment.GetEnvironmentVariable("USERNAME");
            }

            _logger.LogInformation("Process Cred Store Service is runing on processor {0} and user {1}.", threadID, whoami);
            Thread.Sleep(200);   // ensure the rest of the web server starts up

            bool bExists = CngKey.Exists(keyName);
            RSACng rsaKey;
            if (bExists)
            {
                CngKeyHandleOpenOptions ckhoo = new CngKeyHandleOpenOptions { };
                cngKey = CngKey.Open(keyName);
                _logger.LogInformation("Found Key {0}.", keyName);
                X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certRes = certStore.Certificates.Find(X509FindType.FindBySubjectName, "Trustregistry", false);
                //                    X509Certificate2Enumerator certEnum = certRes.GetEnumerator();
                foreach (X509Certificate2 x2 in certRes)
                {
                    if (cert1 == null)
                    {
                        cert1 = x2;  // TODO see if there are better choices than the first when i have some criteria
                    }
                }
                certStore.Close();
            }
            else
            {
                try
                {
                    rsaKey = new RSACng(CngKey.Create(CngAlgorithm.Rsa, keyName, new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowExport }));
                    cngKey = rsaKey.Key;
                    RSAParameters rsaP = rsaKey.ExportParameters(true);
                    X500DistinguishedName dn = new X500DistinguishedName("CN=Trustregistry.US", new X500DistinguishedNameFlags { });
                    CertificateRequest cr = new CertificateRequest(dn, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    _logger.LogInformation("Need to create a new Key named {0}.", keyName);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Exception trying to create a new Key named {0}, exception {1}.", keyName, ex.Message);
                }
            }
            if (cngKey != null)
            {
                try
                {
                    keyType = cngKey.Algorithm;
                    rsaKey = new RSACng(cngKey);
                    X500DistinguishedName dnx = new X500DistinguishedName("CN=Trustregistry.US", new X500DistinguishedNameFlags { });
                    CertificateRequest crx = new CertificateRequest(dnx, rsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    X509Certificate2 cert2 = crx.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow + TimeSpan.FromHours(24 * 365.2 * 5)); // TODO get value from config IN HOURS
                                                                                                                                                      // put that sert in the store
                    X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    certStore.Open(OpenFlags.ReadOnly);
                    X509Certificate2Collection certRes = certStore.Certificates.Find(X509FindType.FindBySubjectName, "Trustregistry", false);
//                    X509Certificate2Enumerator certEnum = certRes.GetEnumerator();
                    foreach (X509Certificate2 x2 in certRes)
                    {
                        if (cert1 == null)
                        {
                            cert1 = x2;  // TODO see if there are better choices than the first when i have some criteria
                        }
                    }
//                    certStore.Add(cert2);
                    certStore.Close();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Exception trying to create a new cert for key named {0}, exception {1}.", keyName, ex.Message);
                }
            }
            while (true)
            {
                if (busy)
                {

                }
                Thread.Sleep(60000);  // TODO make this event driven

                _logger.LogInformation("Process Cred Store Service still runing.");
            }
        }
        public string[] RunRequests(string friendly, CredRequester[] requests)
        {
            busy = true;
            string[] strOut = new string[] { "err", "Could not process requests", "" };

            _logger.LogInformation("Process Cred Store Service proccessing {0} requests.", requests.Length);
            int cntRequests = 0;
            foreach (CredRequester kr in requests)
            {
                cntRequests++;
                string[] response = RunRequest(friendly, kr);
                if (response[0] == "err")
                {
                    strOut = response;
                    strOut[1] = String.Format("Cound not process request {0}", cntRequests);
                }
            }
            busy = false;
            return strOut;
        }
        public string[] RunRequest(string friendly, CredRequester request)
        {
            string[] strOut = new string[] { "err", "Could not process requests", "" };
            Acts choice = request.action;
            if (choice == Acts.find)
            {
                strOut[0] = "ok";
                string pub = cert1.PublicKey.Key.ToXmlString(false);
                request.result = pub;
            }
            else if (choice == Acts.sign)
            {
                strOut[0] = "ok";
                string pub = cert1.PublicKey.Key.ToXmlString(false);
                request.result = pub;
            }
            else if (choice == Acts.decrypt)
            {
                strOut[0] = "ok";
                string pub = cert1.PublicKey.Key.ToXmlString(false);
                request.result = pub;
            }
            return strOut;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Process Cred Store Hosted Service is starting (ExecuteAsync.");

            cancellationToken.Register(() =>
                _logger.LogInformation("Process Cred Store Hosted Service is stopping(ExecuteAsync."));

            // this process assures that keystore has the referenced key and then just waits for keystorage requests and processes them.
            RunProcess("SignKey");  // TODO get this name from config or startup

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Process Cred Store Hosted Service has received a cancellation. (ExecuteAsync.");
                if (busy)
                    await Task.Delay(200);

                await Task.Delay(100, cancellationToken);
            }
            _logger.LogInformation("Process Cred Store Hosted Service is stopped(ExecuteAsync.");
        }
    }
}
