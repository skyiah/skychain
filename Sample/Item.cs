﻿using Greatbone;

namespace Samp
{
    /// <summary>
    /// An item data object that represents a product or service.
    /// </summary>
    public class Item : IData, IKeyable<string>
    {
        public static readonly Item Empty = new Item();

        public const byte PK = 1, LATER = 2;

        // sorts
        public static readonly Map<short, string> Cats = new Map<short, string>
        {
            {1, "粉面类"},
            {11, "谷物类"},
            {21, "其他类"},
        };

        // status
        public static readonly Map<short, string> Statuses = new Map<short, string>
        {
            {0, "下架"},
            {1, "上架"},
            {2, "推荐"},
        };

        internal string name;
        internal string descr;
        internal string remark;
        internal string mov; // movie url
        internal short cat;
        internal string unit;
        internal decimal price;
        internal decimal giverp; // giver price
        internal decimal grperp; // group cost
        internal decimal shipperp; // deliver cost
        internal short min;
        internal short step;
        internal bool refrig;
        internal short demand;
        internal Giver[] givers;
        internal short status;

        public void Read(ISource s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Get(nameof(name), ref name);
            }
            s.Get(nameof(descr), ref descr);
            s.Get(nameof(remark), ref remark);
            s.Get(nameof(mov), ref mov);
            s.Get(nameof(cat), ref cat);
            s.Get(nameof(unit), ref unit);
            s.Get(nameof(price), ref price);
            s.Get(nameof(giverp), ref giverp);
            s.Get(nameof(grperp), ref grperp);
            s.Get(nameof(shipperp), ref shipperp);
            s.Get(nameof(min), ref min);
            s.Get(nameof(step), ref step);
            s.Get(nameof(refrig), ref refrig);
            s.Get(nameof(demand), ref demand);
            s.Get(nameof(givers), ref givers);
            s.Get(nameof(status), ref status);
        }

        public void Write(ISink s, byte proj = 0x0f)
        {
            if ((proj & PK) == PK)
            {
                s.Put(nameof(name), name);
            }
            s.Put(nameof(descr), descr);
            s.Put(nameof(remark), remark);
            s.Put(nameof(mov), mov);
            s.Put(nameof(cat), cat);
            s.Put(nameof(unit), unit);
            s.Put(nameof(price), price);
            s.Put(nameof(giverp), giverp);
            s.Put(nameof(grperp), grperp);
            s.Put(nameof(shipperp), shipperp);
            s.Put(nameof(min), min);
            s.Put(nameof(step), step);
            s.Put(nameof(refrig), refrig);
            s.Put(nameof(demand), demand);
            s.Put(nameof(givers), givers);
            s.Put(nameof(status), status);
        }

        public string Key => name;

        public short[] Cap7Of(int uid)
        {
            if (givers != null)
            {
                for (int i = 0; i < givers.Length; i++)
                {
                    return givers[i].cap7;
                }
            }
            return null;
        }
    }

    public struct Giver : IData
    {
        internal int uid;
        internal string uname;
        internal short[] cap7;

        public void Read(ISource s, byte proj = 15)
        {
            s.Get(nameof(uid), ref uid);
            s.Get(nameof(uname), ref uname);
            s.Get(nameof(cap7), ref cap7);
        }

        public void Write(ISink s, byte proj = 15)
        {
            s.Put(nameof(uid), uid);
            s.Put(nameof(uname), uname);
            s.Put(nameof(cap7), cap7);
        }
    }
}