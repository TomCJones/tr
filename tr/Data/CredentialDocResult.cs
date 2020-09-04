// CredentialDocResult.cs copyright tomjones

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using CredStore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace tr.Data
{
    public class CredentialDocResult 
    {
        public string encodedFirst2 = "";
        public Dictionary<string, object> Entries { get; }
        public int? MaxAge { get; }
        /// <summary>
        /// Initialize an instance of the Doc
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="maxage"></param>
        public CredentialDocResult(string header, string json, int? maxage)
        {
            encodedFirst2 = Base64UrlCoder.Encode(header) + "." + Base64UrlCoder.Encode(json); ;
            MaxAge = maxage;  
        }
        /// <summary>
        /// Initialize an instance of the Doc - endode as a jose string and add a signature
        /// </summary>
        /// <param name="json"> is a json string</param>
        /// <param name="maxage"></param>
        public async Task<string[]> SignCDR(ICredStoreSvc kss)
        {
            string[] jsonOut = new string[2];
            try
            {

                CrRequest grq = new CrRequest { request = encodedFirst2, action = Acts.sign };
                CrResponse grs = await kss.Proc(grq);

                //           CrResponse grOut = await kss.gClient.SimpleRequestAsync(
                //                new CrRequest { Request = encodedFirst2, Action = grqAct.Sign });
                jsonOut[1] = grs.request;    // the public key in json format
                jsonOut[0] = encodedFirst2 + "." + grs.data;  // TODO null
            }
            catch (Exception ex)
            {
                Base.JsonError jerr = new Base.JsonError
                {
                    operation = "SignCDR in CredentialDocResult",
                    error = "Exeception on Client.SimpleRequestAsync " + ex.Message,
                    error_description = "TargetSite: " + ex.TargetSite.Name + "  Source: " + ex.Source + "  InnerEx: " + ex.InnerException
                };
                jsonOut[0] = JsonSerializer.Serialize(jerr);
            }

            return jsonOut;
        }

    }
}
