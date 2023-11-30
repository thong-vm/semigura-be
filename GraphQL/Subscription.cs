using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using Interfaces;
using System.Runtime.CompilerServices;

namespace GraphQL
{
    public class Subscription
    {
        enum SubscriptionAction
        {
            Created, Updated, Deleted
        }

        public static string GetAddedTopic<TEntity>() => "on" + typeof(TEntity).Name + nameof(SubscriptionAction.Created);
        public static string GetUpdatedTopic<TEntity>() => "on" + typeof(TEntity).Name + nameof(SubscriptionAction.Updated);
        public static string GetDeletedTopic<TEntity>() => "on" + typeof(TEntity).Name + nameof(SubscriptionAction.Deleted);
    }

    public class TSubScriptonResolver<TEntity>
        where TEntity : class, IEntity, new()
    {
        public async IAsyncEnumerable<TEntity> SubscribeToStream(
            [Service] ITopicEventReceiver receiver,
            [EnumeratorCancellation] CancellationToken cancellationToken,
            string topic
        )
        {
            var srcStream = await receiver.SubscribeAsync<string, TEntity>(topic, cancellationToken);

            await foreach (var entity in srcStream.ReadEventsAsync())
            {
                yield return entity;
            }
        }
    }

    public class TSubscriptionTypeExtension<TEntity> : ObjectTypeExtension
    where TEntity : class, IEntity, new()
    {
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Name(nameof(Subscription));

            var addedTopic = Subscription.GetAddedTopic<TEntity>();
            descriptor
                .Field(addedTopic.LowerFirstChar())
                .Resolve(context => context.GetEventMessage<TEntity>())
                .Subscribe(async context =>
                {
                    var receiver = context.Service<ITopicEventReceiver>();

                    ISourceStream stream =
                        await receiver.SubscribeAsync<string, TEntity>(addedTopic);

                    return stream;
                });

            var updatedTopic = Subscription.GetUpdatedTopic<TEntity>();
            descriptor
                .Field(updatedTopic.LowerFirstChar())
                .Resolve(context => context.GetEventMessage<TEntity>())
                .Subscribe(async context =>
                {
                    var receiver = context.Service<ITopicEventReceiver>();

                    ISourceStream stream =
                        await receiver.SubscribeAsync<string, TEntity>(updatedTopic);

                    return stream;
                });

            var deletedTopic = Subscription.GetDeletedTopic<TEntity>();
            descriptor
                .Field(deletedTopic.LowerFirstChar())
                .Resolve(context => context.GetEventMessage<TEntity>())
                .Subscribe(async context =>
                {
                    var receiver = context.Service<ITopicEventReceiver>();

                    ISourceStream stream =
                        await receiver.SubscribeAsync<string, TEntity>(deletedTopic);

                    return stream;
                });
        }
    }
}
