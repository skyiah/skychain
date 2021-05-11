using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static SkyChain.Db.ChainUtility;

namespace SkyChain.Db
{
    /// <summary>
    /// A chain client connector to a specific remote peer..
    /// </summary>
    public class ChainClient : HttpClient, IKeyable<short>
    {
        const int REQUEST_TIMEOUT = 3;

        const short
            NORMAL = 1,
            NETWORK_ERROR = 2,
            DATA_ERROR = 3,
            PEER_ERROR = 4,
            INTERNAL_ERROR = 5;

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, null},
            {NORMAL, "normal"},
            {NETWORK_ERROR, "network error"},
            {DATA_ERROR, "data error"},
            {PEER_ERROR, "peer error"},
            {INTERNAL_ERROR, "internal error"},
        };

        // when a chain connector
        readonly ChainPeer info;

        // acceptable remote addresses
        readonly IPAddress[] addrs;

        Task<(short, Archival[])> polltsk;

        // the status showing what is happening 
        short status;

        string err;


        /// <summary>
        /// To construct a chain client. 
        /// </summary>
        internal ChainClient(ChainPeer info, ChainClientHandler handler = null) : base(handler ?? new ChainClientHandler())
        {
            this.info = info;
            var baseuri = info.uri;
            BaseAddress = new Uri(baseuri);
            try
            {
                addrs = Dns.GetHostAddresses(BaseAddress.Host);
            }
            catch (Exception e)
            {
                // ServerEnviron.WAR(e.Message);
            }
            Timeout = TimeSpan.FromSeconds(REQUEST_TIMEOUT);
        }

        public short Key => info.id;

        public ChainPeer Info => info;

        public string Err => err;

        ///
        /// <returns>0) void, 1) remoting, 200) ok, 204) no content, 500) server error</returns>
        /// 
        public short TryReap(out Archival[] block)
        {
            block = null;
            if (polltsk == null)
            {
                return 0;
            }
            if (!polltsk.IsCompleted)
            {
                return 1;
            }
            var (code, cnt) = polltsk.Result;

            if (code == 500)
            {
                status = PEER_ERROR;
            }
            else if (code == 200 || code == 204)
            {
                status = NORMAL;
            }

            block = cnt;
            return code;
        }

        public bool IsRemoteAddr(IPAddress addr)
        {
            for (int i = 0; i < addrs.Length; i++)
            {
                if (addrs[i].Equals(addr))
                {
                    return true;
                }
            }
            return false;
        }

        internal void SetDataError(string err)
        {
            this.status = DATA_ERROR;
            this.err = err;
        }

        internal void SetDataError(long seq)
        {
            this.status = DATA_ERROR;
            this.err = seq.ToString();
        }

        internal void SetInternalError(string seq)
        {
            status = INTERNAL_ERROR;
            // this.seq = seq;
        }

        public void ScheduleRemotePoll(int blockid)
        {
            if (status != DATA_ERROR)
            {
                // schedule to execute
                if (polltsk == null || polltsk.IsCompleted)
                {
                    (polltsk = RemotePollAsync(blockid)).Start();
                }
            }
        }

        //
        // RPC
        //

        async Task<(short, Archival[])> RemotePollAsync(int blockid)
        {
            try
            {
                // request
                var req = new HttpRequestMessage(HttpMethod.Get, "/onpoll");

                req.Headers.TryAddWithoutValidation(X_FROM, ChainEnviron.Info.id.ToString());
                req.Headers.TryAddWithoutValidation(X_PEER_ID, info.id.ToString());
                req.Headers.TryAddWithoutValidation(X_BLOCK_ID, blockid.ToString());

                // response
                var rsp = await SendAsync(req, HttpCompletionOption.ResponseContentRead);
                if (rsp.StatusCode != HttpStatusCode.OK)
                {
                    return ((short) rsp.StatusCode, null);
                }
                var hdrs = rsp.Content.Headers;

                var bytea = await rsp.Content.ReadAsByteArrayAsync();
                var arr = (JArr) new JsonParser(bytea, bytea.Length).Parse();
                var dat = arr.ToArray<Archival>();
                return (200, dat);
            }
            catch
            {
                status = NETWORK_ERROR;
                return (0, null);
            }
        }

        const string
            CONTENT_TYPE = "Content-Type",
            CONTENT_LENGTH = "Content-Length",
            AUTHORIZATION = "Authorization";
    }
}