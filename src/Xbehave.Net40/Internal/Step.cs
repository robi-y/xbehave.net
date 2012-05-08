﻿// <copyright file="Step.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System;
    using System.Collections.Generic;
    using Xbehave.Infra;

    internal class Step
    {
        private readonly string name;
        private readonly Func<IDisposable> body;
        private readonly bool inIsolation;
        private readonly string skipReason;

        public Step(string prefix, string message, Func<IDisposable> body, bool inIsolation, string skipReason)
        {
            Require.NotNull(prefix, "prefix");
            Require.NotNull(message, "message");
            Require.NotNull(body, "body");

            this.name = message.ToSentenceStartingWith(prefix);
            this.body = body;
            this.inIsolation = inIsolation;
            this.skipReason = skipReason;
        }

        public Step(string prefix, string message, Action body, bool inIsolation, string skipReason)
            : this(prefix, message, DisposableFunctionFactory.Create(body), inIsolation, skipReason)
        {
        }

        public Step(string prefix, string message, Func<IEnumerable<IDisposable>> body, bool inIsolation, string skipReason)
            : this(prefix, message, DisposableFunctionFactory.Create(body), inIsolation, skipReason)
        {
        }

        public Step(string prefix, string message, Action body, Action dispose, bool inIsolation, string skipReason)
            : this(prefix, message, DisposableFunctionFactory.Create(body, dispose), inIsolation, skipReason)
        {
        }

        public string Name
        {
            get { return this.name; }
        }

        public string SkipReason
        {
            get { return this.skipReason; }
        }

        public bool InIsolation
        {
            get { return this.inIsolation; }
        }

        public int MillisecondsTimeout { get; set; }

        public IDisposable Execute()
        {
            if (this.MillisecondsTimeout > 0)
            {
                var result = this.body.BeginInvoke(null, null);

                // NOTE: we do not call the WaitOne(int) overload because it wasn't introduced until .NET 3.5 SP1 and we want to support pre-SP1
                if (!result.AsyncWaitHandle.WaitOne(this.MillisecondsTimeout, false))
                {
                    throw new Xunit.Sdk.TimeoutException(this.MillisecondsTimeout);
                }

                return this.body.EndInvoke(result);
            }

            return this.body();
        }
    }
}