﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Integration.AspNet.Core.Handlers
{
    public class BotMessageHandler : BotMessageHandlerBase
    {
        protected override async Task<InvokeResponse> ProcessMessageRequestAsync(HttpRequest request, BotFrameworkAdapter botFrameworkAdapter, Func<ITurnContext, Task> botCallbackHandler, CancellationToken cancellationToken)
        {
            var activity = default(Activity);

            using (var bodyReader = new JsonTextReader(new StreamReader(request.Body, Encoding.UTF8)))
            {
                activity = BotMessageHandlerBase.BotMessageSerializer.Deserialize<Activity>(bodyReader);
            }

            var invokeResponse = await botFrameworkAdapter.ProcessActivityAsync(
                    request.Headers["Authorization"],
                    activity,
                    botCallbackHandler,
                    cancellationToken);

            return invokeResponse;
        }
    }
}