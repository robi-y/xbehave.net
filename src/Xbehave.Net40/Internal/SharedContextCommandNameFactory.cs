﻿// <copyright file="SharedContextCommandNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;
    using Xbehave.Infra;

    internal class SharedContextCommandNameFactory : ISharedContextCommandNameFactory
    {
        private readonly ICommandNameFactory testNameFactory;

        public SharedContextCommandNameFactory(ICommandNameFactory testNameFactory)
        {
            this.testNameFactory = testNameFactory;
        }

        public string CreateContext(IEnumerable<Step> steps)
        {
            return string.Concat(this.testNameFactory.Create(steps), " { (shared context)");
        }

        public string Create(IEnumerable<Step> contextSteps, Step then)
        {
            return string.Concat(this.testNameFactory.Create(contextSteps), " | ", this.testNameFactory.Create(then.AsEnumerable()));
        }

        public string CreateDisposal(IEnumerable<Step> steps)
        {
            return string.Concat(this.testNameFactory.Create(steps), " } (disposal)");
        }
    }
}