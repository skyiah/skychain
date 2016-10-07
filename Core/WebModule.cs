﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Greatbone.Core
{
    /// <summary>
    /// A module consists of sub controllers and/or variable-key hub controller. It represents an independent business realm.
    /// </summary>
    public abstract class WebModule : WebSub, ILogger
    {
        // the added sub controllers, if any
        private Roll<WebSub> subs;

        // the attached variable-key multiplexer, if any
        private WebVarHub varhub;

        protected WebModule(IScope scope) : base(scope)
        {
        }

        public long LastModified { get; set; }

        public T AddSub<T>(string key, bool auth) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Roll<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(IScope) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebScope scope = new WebScope()
            {
                key = key,
                Parent = this,
                Service = Service,
                IsVar = false
            };
            T sub = (T)ci.Invoke(new object[] { scope });

            subs.Add(sub);

            return sub;
        }

        public Roll<WebSub> Subs => subs;

        public WebVarHub VarHub => varhub;

        public T SetVarHub<T>(bool auth) where T : WebVarHub
        {
            // create instance
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(IScope) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebScope scope = new WebScope
            {
                key = "var",
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T hub = (T)ci.Invoke(new object[] { scope });

            this.varhub = hub;

            return hub;
        }

        public override void Do(string rsc, WebContext wc)
        {
            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                base.Do(rsc, wc);
            }
            else // not local then sub & mux
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub))
                {
                    sub.Do(rsc.Substring(slash + 1), wc);
                }
                else if (varhub == null)
                {
                    wc.StatusCode = 501; // Not Implemented
                }
                else
                {
                    varhub.Do(rsc.Substring(slash + 1), wc, dir); // var = dir
                }
            }
        }

        //
        // LOGGING
        //

        const string FileName = "module.log";

        // opened writer on the log file
        StreamWriter writer;

        public IDisposable BeginScope<T>(T state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<T>(LogLevel logLevel, EventId eventId, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (formatter != null)
            {
                var message = formatter(state, exception);
            }
            throw new NotImplementedException();
        }


    }
}