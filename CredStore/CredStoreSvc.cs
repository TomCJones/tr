// CredStore copyright 2020 tomjones
    
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Grpc.Net.Client;
using Grpc.Core;

namespace CredStore
{
    public class CredStoreSvc : ICredStoreSvc, IHostedService
    {
        private object keyRequstLock = new object();
        private readonly ILogger _logger;
        private readonly GrpcChannel gChannel;
 //       private readonly string gRpcServer = @"SimpleService.exe";
        private readonly string requestAddr = "https://localhost:5033";
        private readonly string gRpcArgs = "";
  //      private InitialReply result = null;
        private SHA256 sha256 = null;
        public CredStoreSvc(ILogger<CredStoreSvc> logger)
        {
            /*        gChannel = GrpcChannel.ForAddress(requestAddr);
                     try
                     {
               //          Gryffin.GryffinClient grClient = new Gryffin.GryffinClient (gChannel);
              //           result = grClient.SayHello(new InitialScream { Name = "CredStore Startup" });
               //          logger.LogInformation("CSS1 constructor retured: " + result.Message + "  Version: " + result.AppVersion);
                     }
                     catch (Exception ex)
                     {
                         logger.LogInformation ("CSS2 gRPC setup failed with exception: " + ex.Message);
                     }
                    sha256 = SHA256.Create();   */
             _logger = logger;
        }
        /// <summary>
        ///  returns status of the service and the underlying creds
        /// </summary>
        /// <returns></returns>
        public async Task<string> StatusAsync()
        {
            string strOut = "failed";
            try
            {
     //       Gryffin.GryffinClient gClient = new Gryffin.GryffinClient (gChannel);
     //       var result = await gClient.SayHelloAsync(
     //                         new InitialScream { Name = "CredStore status request" });
                strOut = ""; // result.Message;
            }
            catch (Exception ex)
            {
                strOut = "gRPC failed with Exception: " + ex.Message;
            }
            _logger.LogInformation("CredStoreSvc Status has returned: " + strOut);
            return strOut;
        }
        /// <summary>
        /// Add a key request to the queue
        /// </summary>gRPC 
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        public async Task<CrResponse> Proc(CrRequest grq)
        {
            //       Gryffin.GryffinClient gClient = new Gryffin.GryffinClient(gChannel);
            //        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(grq.Request));
            grq.data = ""; //  ByteString.CopyFrom(hash);
            CrResponse grOut = null;  // await gClient.SimpleRequestAsync(grq);
            return grOut;
        }
        public async Task<CrResponder> Proc(string keyName, XferBlock oneRequest)
        {
            XferBlock[] strOut = new XferBlock[] { oneRequest };
            return await Proc(keyName, strOut);
        }
        public async Task<CrResponder>  Proc(string keyname, XferBlock[] requests)
        {
            string[] strOut = new string[] { "err", "no Key", "" };

            XferBlock xb = requests[0];
    //        Gryffin.GryffinClient gClient = new Gryffin.GryffinClient(gChannel);
     //       CrResponse grOut = await gClient.SimpleRequestAsync(
     //             new CrRequest { Request = xb.request, Action = (grqAct) xb.action }) ;

            _logger.LogInformation( String.Format("added {0} key requests", requests.Length));
            xb.result = ""; //  grOut.data;
            CrResponder grs = new CrResponder() {xb};
            return grs;
        }

        private static string find(byte[] request)
        {
            return "result of search";
        }
        private static string sign(byte[] request)
        {
            return Convert.ToBase64String(request).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            //      RSACng rsa = new RSACng(cngKey);
            //     return rsa.SignHash(request);
        }
        private static string decrypt(byte[] request)
        {
            return "result of search";
        }
        public static string Encode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cred Store Service is starting");
            await Task.Delay(100);   // give other processes a chance to start
            var request = new HttpRequestMessage(HttpMethod.Get, requestAddr)
            {
                Version = new Version(2, 0)
            };
            request.Headers.Add("User-Agent", "Mozilla TopCat");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cred Store Service is doing stoping");
        }
    }
}
