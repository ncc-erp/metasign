using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NccCore.Extension
{
    public static class FuncEx
    {
        [DebuggerStepThrough]
        public static Func<TRes> Create<TRes>(Func<TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TRes> Create<TArg1, TRes>(Func<TArg1, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TRes> Create<TArg1, TArg2, TRes>(Func<TArg1, TArg2, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TRes> Create<TArg1, TArg2, TArg3, TRes>(Func<TArg1, TArg2, TArg3, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TRes> Create<TArg1, TArg2, TArg3, TArg4, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes> f) { return f; }

        [DebuggerStepThrough]
        public static TRes Invoke<TRes>(Func<TRes> f) { return f(); }
    }
}
