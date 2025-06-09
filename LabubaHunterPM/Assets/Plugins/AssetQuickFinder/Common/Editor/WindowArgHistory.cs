using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuickFinder.Editor
{
    public class WindowArgHistory
    {
        private List<object> args = new List<object>();

        private int current = -1;

        public void Enqueue(object arg)
        {
            args.Insert(0, arg);
            if (current == -1)
            {
                current = 0;
            }
        }

        public System.Object Dequeue()
        {
            if(args.Count == 0)
            { return null; }
            var arg = args[args.Count - 1];
            args.RemoveAt(args.Count - 1);
            return arg;
        }

        public System.Object Latest()
        {
            if (args.Count == 0)
                return null;
            return args[0];
        }
        public TArg Latest<TArg>()
        {
            return (TArg)Latest();
        }

        public System.Object Oldest()
        {
            if (args.Count == 0)
                return null;
            return args[args.Count - 1];
        }
        public TArg Oldest<TArg>()
        {
            return (TArg)Oldest();
        }

        public System.Object Current()
        {
            if (current == -1)
            { return null; }
            return args[current];
        }
        public TArg Current<TArg>()
        {
            return (TArg)Current();
        }

        public System.Object Forward()
        {
            if (current == -1)
            { return null; }
            current--;
            current = System.Math.Max(0, current);
            return args[current];
        }
        public TArg Forward<TArg>()
        {
            return (TArg)Forward();
        }

        public System.Object Backward()
        {
            if (current == -1)
            { return null; }
            current++;
            current = System.Math.Min(current, args.Count - 1);
            return args[current];
        }
        public TArg Backward<TArg>()
        {
            return (TArg)Backward();
        }

        public System.Object ToLatest()
        {
            if (current == -1)
            { return null; }
            current = 0;
            return args[current];
        }
        public TArg ToLatest<TArg>()
        {
            return (TArg)ToLatest();
        }

        public System.Object ToOldest()
        {
            if (current == -1)
            { return null; }
            current = args.Count - 1;
            return args[current];
        }
        public TArg ToOldest<TArg>()
        {
            return (TArg)ToOldest();
        }

        public bool DeleteCurrent<TArg>(out TArg currentArg)
        {
            if(current < 0 || args.Count <= 1)
            {
                currentArg = default;
                return false; 
            }

            args.RemoveAt(current);
            current--;
            if (current < 0)
                current = 0;
            currentArg = (TArg)(args[current]);
            return true;
        }

        public void Clear() { args.Clear(); }


    }
}

