﻿// <copyright file="SpecificationExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xbehave.Fluent;

    /// <summary>
    /// Provides extensions for a fluent specification syntax
    /// </summary>
    [Obsolete("Use StringExtensions instead.")]
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Records a context setup for this specification.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish the context.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IGivenDefinition Context(this string message, Action arrange)
        {
            return message.Given(arrange);
        }

        /// <summary>
        /// Trap for using contexts implementing IDisposable with the wrong overload.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message", Justification = "The member is deprecated and will be removed.")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "arrange", Justification = "The member is deprecated and will be removed.")]
        [Obsolete("Use Given() instead.")]
        public static void Context(this string message, ContextDelegate arrange)
        {
            throw new InvalidOperationException("Use Given() instead.");
        }

        /// <summary>
        /// Records a disposable context for this specification. The context lifecycle will be managed by Xbehave.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IGivenDefinition ContextFixture(this string message, ContextDelegate arrange)
        {
            return message.GivenDisposable(arrange);
        }

        /// <summary>
        /// Records an action to be performed on the context for this specification.
        /// </summary>
        /// <param name="message">A message describing the action.</param>
        /// <param name="act">The action to perform.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use When() instead.")]
        public static IWhenDefinition Do(this string message, Action act)
        {
            return message.When(act);
        }

        /// <summary>
        /// Records an assertion for this specification.
        /// Each assertion is executed on an isolated context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="assert">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenInIsolation() instead.")]
        public static IThenDefinition Assert(this string message, Action assert)
        {
            return message.ThenInIsolation(assert);
        }

        /// <summary>
        /// Records an observation for this specification.
        /// All observations are executed on the same context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="observation">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use Then() instead.")]
        public static IThenDefinition Observation(this string message, Action observation)
        {
            return message.Then(observation);
        }

        /// <summary>
        /// Records a skipped assertion for this specification.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="skippedAction">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStepDefinition"/>.</returns>
        [Obsolete("Use ThenSkip() instead.")]
        public static IThenDefinition Todo(this string message, Action skippedAction)
        {
            return message.ThenSkip(skippedAction);
        }
    }
}