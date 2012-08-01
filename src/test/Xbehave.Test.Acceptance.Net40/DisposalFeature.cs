﻿// <copyright file="DisposalFeature.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave.Test.Acceptance
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xbehave.Test.Acceptance.Infrastructure;
    using Xunit.Sdk;

    // In order to release allocated resources
    // As a developer
    // I want to register objects for disposal after a scenario has run
    public static class DisposalFeature
    {
        private enum LifeTimeEventType
        {
            Constructed,
            Disposed,
        }

        [Scenario]
        public static void RegisteringManyDisposableObjectsInASingleStep()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a step which registers many disposable objects followed by a step which uses the objects"
                .Given(() => feature = typeof(SingleStep));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => Disposable.ClearRecordedEvents());

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringManyDisposableObjectsInSeperateSteps()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given many steps which each register a disposable object followed by a step which uses the objects"
                .Given(() => feature = typeof(ManySteps));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => Disposable.ClearRecordedEvents());

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been disposed once in reverse order"
                .And(() => DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder());
        }

        [Scenario]
        public static void RegisteringADisposableObjectInManyContexts()
        {
            var feature = default(Type);
            var results = default(MethodResult[]);

            "Given a scenario with a step which registers a disposable object followed by steps which use the disposable object and generate two contexts"
                .Given(() => feature = typeof(SingleStepTwoContexts));

            "When running the scenario"
                .When(() => results = TestRunner.Run(feature).ToArray())
                .Teardown(() => Disposable.ClearRecordedEvents());

            "Then there should be no failures"
                .Then(() => results.Should().NotContain(result => result is FailedResult));

            "And some disposable objects should have been created"
                .And(() => SomeDisposableObjectsShouldHaveBeenCreated());

            "And the disposable objects should each have been created and disposed one before the other"
                .And(() =>
                {
                    var @events = Disposable.RecordedEvents.ToArray();
                    (@events.Length % 2).Should().Be(0);
                    for (var index = 0; index < @events.Length; index = index + 2)
                    {
                        var event0 = events[index];
                        var event1 = events[index + 1];

                        event0.ObjectId.Should().Be(event1.ObjectId);
                        event0.EventType.Should().Be(LifeTimeEventType.Constructed);
                        event1.EventType.Should().Be(LifeTimeEventType.Disposed);

                        @events.Where(@event => @event.ObjectId == event0.ObjectId).Count().Should().Be(2);
                    }
                });
        }

        private static AndConstraint<FluentAssertions.Assertions.GenericCollectionAssertions<LifetimeEvent>> SomeDisposableObjectsShouldHaveBeenCreated()
        {
            return Disposable.RecordedEvents.Where(@event => @event.EventType == LifeTimeEventType.Constructed).Should().NotBeEmpty();
        }

        private static AndConstraint<FluentAssertions.Assertions.BooleanAssertions> DisposableObjectsShouldEachHaveBeenDisposedOnceInReverseOrder()
        {
            return Disposable.RecordedEvents.SkipWhile(@event => @event.EventType != LifeTimeEventType.Disposed)
                .Reverse()
                .SequenceEqual(
                    Disposable.RecordedEvents.TakeWhile(@event => @event.EventType == LifeTimeEventType.Constructed),
                    new CustomEqualityComparer<LifetimeEvent>((x, y) => x.ObjectId == y.ObjectId, x => x.ObjectId))
                .Should().BeTrue();
        }

        private static class SingleStep
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given some disposables"
                    .Given(() =>
                    {
                        disposable0 = new Disposable().Using();
                        disposable1 = new Disposable().Using();
                        disposable2 = new Disposable().Using();
                    });

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class ManySteps
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);
                var disposable1 = default(Disposable);
                var disposable2 = default(Disposable);

                "Given a disposable"
                    .Given(() => disposable0 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable1 = new Disposable().Using());

                "And another disposable"
                    .Given(() => disposable2 = new Disposable().Using());

                "When using the disposables"
                    .When(() =>
                    {
                        disposable0.Use();
                        disposable1.Use();
                        disposable2.Use();
                    });
            }
        }

        private static class SingleStepTwoContexts
        {
            [Scenario]
            public static void Scenario()
            {
                var disposable0 = default(Disposable);

                "Given a disposable"
                    .Given(() => disposable0 = new Disposable().Using());

                "When using the disposable"
                    .When(() => disposable0.Use());

                "Then something"
                    .Then(() => { })
                    .InIsolation();

                "And something"
                    .Then(() => { });
            }
        }

        private sealed class Disposable : IDisposable
        {
            private static readonly ConcurrentQueue<LifetimeEvent> Events = new ConcurrentQueue<LifetimeEvent>();

            private bool isDisposed;

            public Disposable()
            {
                Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Constructed, ObjectId = this.GetHashCode() });
            }

            public static IEnumerable<LifetimeEvent> RecordedEvents
            {
                get { return Disposable.Events.Select(_ => _); }
            }

            public static void ClearRecordedEvents()
            {
                LifetimeEvent ignored;
                while (Events.TryDequeue(out ignored))
                {
                }
            }

            public void Use()
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(this.GetType().FullName);
                }
            }

            public void Dispose()
            {
                Events.Enqueue(new LifetimeEvent { EventType = LifeTimeEventType.Disposed, ObjectId = this.GetHashCode() });
                this.isDisposed = true;
            }
        }

        private class LifetimeEvent
        {
            public LifeTimeEventType EventType { get; set; }

            public int ObjectId { get; set; }
        }
    }
}