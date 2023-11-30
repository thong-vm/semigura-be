using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Interfaces;
using semigura.DBContext.Entities;
using Template;

namespace GraphQL
{
    public class Mutation
    {
       
    }

    public class TMutateResolver<TEntity>
    where TEntity : class, IEntity, new()
    {
        public async Task<TEntity?> Add([Service] TRepository<TEntity, DBEntities> repo, [Service] ITopicEventSender sender, TEntity input)
        {
            try
            {
                TEntity newObj = new();
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    var inputValue = property.GetValue(input);
                    var defaultValue = property.GetValue(newObj);
                    property.SetValue(newObj, inputValue ?? defaultValue);
                }
                var added = await repo.Add(newObj);
                if (added != null)
                {
                    var topic = Subscription.GetAddedTopic<TEntity>();
                    await sender.SendAsync(topic, added);
                    return added;
                }

                throw new Exception($"Add {typeof(TEntity)} error!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        public async Task<TEntity?> Update([Service] TRepository<TEntity, DBEntities> repo, [Service] ITopicEventSender sender, TEntity input, string id)
        {
            try
            {
                var found = await repo.Update(input);
                if (found != null)
                {
                    var topic = Subscription.GetUpdatedTopic<TEntity>();
                    await sender.SendAsync(topic, found);
                }
                return found;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        // if not found id then return null
        // else return deleted obj
        public async Task<TEntity?> Delete([Service] TRepository<TEntity, DBEntities> repo, [Service] ITopicEventSender sender, string id)
        {
            try
            {
                var found = await repo.Delete(id);
                if (found != null)
                {
                    var topic = Subscription.GetDeletedTopic<TEntity>();
                    await sender.SendAsync(topic, found);
                }
                return found;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }

    }

    public class TMutateTypeExtension<TEntity> : ObjectTypeExtension
        where TEntity : class, IEntity, new()
    {
        enum MutateAction
        {
            Create, Update, Delete
        }
        protected override void Configure(IObjectTypeDescriptor descriptor)
        {
            descriptor.Authorize();
            descriptor.Name(nameof(Mutation));

            var addAction = nameof(MutateAction.Create) + typeof(TEntity).Name;
            descriptor
                .Field(addAction.LowerFirstChar())
                .Argument("input", a => a.Type<NonNullType<InputObjectType<TEntity>>>())
                .ResolveWith<TMutateResolver<TEntity>>(f => f.Add(default!, default!, default!));

            var updateAction = nameof(MutateAction.Update) + typeof(TEntity).Name;
            descriptor
                .Field(updateAction.LowerFirstChar())
                .Argument("input", a => a.Type<NonNullType<InputObjectType<TEntity>>>())
                .Argument("id", _ => _.Type<NonNullType<StringType>>())
                .ResolveWith<TMutateResolver<TEntity>>(f => f.Update(default!, default!, default!, default!));

            var deleteAction = nameof(MutateAction.Delete) + typeof(TEntity).Name;
            descriptor
                .Field(deleteAction.LowerFirstChar())
                .Argument("id", _ => _.Type<NonNullType<StringType>>())
                .ResolveWith<TMutateResolver<TEntity>>(f => f.Delete(default!, default!, default!));
        }
    }
}
