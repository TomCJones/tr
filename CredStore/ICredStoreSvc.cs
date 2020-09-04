// ICredStoreSvc.cs  copyright 2020 tomjones

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CredStore
{
    public interface ICredStoreSvc
    {
        Task<CrResponder> Proc(string friendly, XferBlock[] data);
        Task<CrResponder> Proc(string friendly, XferBlock data);
        Task<CrResponse> Proc(CrRequest grq);
        Task<string> StatusAsync();
    }

    public class CrResponder : List<XferBlock>
    {
    }

    public enum Acts { find, sign, decrypt }

    public class XferBlock
    {
        public XferBlock(Acts inta, string rq, string rs)
        {
            action = inta;
            request = rq;
            result = rs;
        }
        public Acts action;
        public string request;
        public string result;
    }

    public class CrRequest
    {
        public Acts action { get; set; }
        public string request { get; set; }
        public string data { get; set; }
    }

    public class CrResponse
    {
        public Acts action { get; set; }
        public string request { get; set; }
        public string data { get; set; }
    }

    public class CredRequester
    {
        public CredRequester(Acts ia, byte[] rq, string rs)
        {
            action = ia;
            bytes = rq;
            result = rs;
        }
        public Acts action;
        public byte[] bytes;
        public string result;
    }
}
