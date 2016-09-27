﻿using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    /// <summary>
    /// JjavaScript Object Binary
    /// </summary>
    public class JsobContent : DynamicContent, IOutput
    {
        // stack of json knots in processing
        readonly int[] starts;

        // current level in stack
        int level;

        // current position
        int pos;


        public JsobContent(byte[] buf, int count) : base(buf, count)
        {
            starts = new int[8];
            level = -1;
            pos = -1;
        }


        public JsobContent(int capacity) : base(capacity)
        {
        }


        public override string Type => "application/jsob";



        public void Put(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, short value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, char[] value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void Put(string name, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, List<T> value)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string name, Dictionary<string, T> value)
        {
            throw new NotImplementedException();
        }

        public void Arr(Action a)
        {
            throw new NotImplementedException();
        }

        public void Obj(Action a)
        {
            throw new NotImplementedException();
        }

        public new void Put<T>(string name, T value)
        {
            throw new NotImplementedException();
        }
    }
}