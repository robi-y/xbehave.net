﻿// <copyright file="ITestNameFactory.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Internal
{
    using System.Collections.Generic;

    internal interface ITestNameFactory
    {
        string Create(IEnumerable<Step> steps);
    }
}