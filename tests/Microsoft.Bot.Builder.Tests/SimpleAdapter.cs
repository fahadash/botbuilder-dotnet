﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Bot.Builder.Tests
{
    public class SimpleAdapter : BotAdapter
    {
        private readonly Action<Activity[]> _callOnSend = null;
        private readonly Action<Activity> _callOnUpdate = null;
        private readonly Action<ConversationReference> _callOnDelete = null;

        public SimpleAdapter() { }
        public SimpleAdapter(Action<Activity[]> callOnSend) { _callOnSend = callOnSend; }
        public SimpleAdapter(Action<Activity> callOnUpdate) { _callOnUpdate = callOnUpdate; }
        public SimpleAdapter(Action<ConversationReference> callOnDelete) { _callOnDelete = callOnDelete; }

        public override Task DeleteActivityAsync(ITurnContext context, ConversationReference reference, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(reference, "SimpleAdapter.deleteActivity: missing reference");
            _callOnDelete?.Invoke(reference);
            return Task.CompletedTask;
        }

        public override Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext context, Activity[] activities, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(activities, "SimpleAdapter.deleteActivity: missing reference");
            Assert.IsTrue(activities.Count() > 0, "SimpleAdapter.sendActivities: empty activities array.");

            _callOnSend?.Invoke(activities);
            List<ResourceResponse> responses = new List<ResourceResponse>();
            foreach(var activity in activities)
            {
                responses.Add(new ResourceResponse(activity.Id));
            }

            return Task.FromResult(responses.ToArray());
        }

        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext context, Activity activity, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(activity, "SimpleAdapter.updateActivity: missing activity");
            _callOnUpdate?.Invoke(activity);
            return Task.FromResult(new ResourceResponse(activity.Id)); // echo back the Id
        }

        public async Task ProcessRequest(Activity activty, Func<ITurnContext, Task> callback, CancellationToken cancellationToken)
        {
            using (var ctx = new TurnContext(this, activty))
            {
                await this.RunPipelineAsync(ctx, callback, cancellationToken);
            }
        }
    }

}
