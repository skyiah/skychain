﻿using System;
using System.IO;

namespace Greatbone.Core
{
    /// <summary>
    /// The creation environment for a particular work instance.
    /// </summary>
    public class WorkConfig
    {
        // either the identifying name for a fixed work or the constant var for a variable work
        readonly string name;

        public WorkConfig(string name)
        {
            this.name = name;
        }

        public string Name => name;

        public Service Service { get; internal set; }

        public Work Parent { get; internal set; }

        public int Level { get; internal set; }

        public bool IsVar { get; internal set; }

        public string Directory { get; internal set; }

        public string GetFilePath(string file)
        {
            return Path.Combine(Directory, file);
        }

        // to obtain a string key from a data object.
        public Delegate Keyer { get; internal set; }
    }
}