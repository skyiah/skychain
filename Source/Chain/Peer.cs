using System;

namespace Skyiah.Chain
{
    public class Peer : IData, IKeyable<string>
    {
        public static readonly Peer Empty = new Peer();

        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "Disabled"},
            {1, "Enabled"}
        };

        internal string id;

        internal string name;

        internal string raddr; // remote address

        internal DateTime stamp;

        internal short status;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(id), ref id);
            s.Get(nameof(name), ref name);
            s.Get(nameof(raddr), ref raddr);
            s.Get(nameof(stamp), ref stamp);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(id), id);
            s.Put(nameof(name), name);
            s.Put(nameof(raddr), raddr);
            s.Put(nameof(stamp), stamp);
            s.Put(nameof(status), status);
        }

        public string Key => id;
    }
}