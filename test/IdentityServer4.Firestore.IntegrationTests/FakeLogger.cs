﻿using Microsoft.Extensions.Logging;
using System;

namespace IdentityServer4.Firestore.IntegrationTests
{
    public class FakeLogger<T> : FakeLogger, ILogger<T>
    {
        public static ILogger<T> Create()
        {
            return new FakeLogger<T>();
        }
    }

    public class FakeLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}
