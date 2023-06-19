﻿using BulletProve.Hooks;
using BulletProve.Http.Models;

namespace BulletProve.Http.Interfaces
{
    /// <summary>
    /// A service that will be run after server Http request was sent.
    /// </summary>
    public interface IAfterHttpRequestHook : IHook
    {
        /// <summary>
        /// Will be run after server Http request was sent.
        /// </summary>
        /// <param name="context">Http request context.</param>
        Task AfterHttpRequestAsync(HttpRequestContext context);
    }
}