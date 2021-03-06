﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeSurveyGenerator.Domain.SeedWork;
using FakeSurveyGenerator.Infrastructure.Persistence;
using MediatR;

namespace FakeSurveyGenerator.Infrastructure
{
    internal static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, SurveyContext ctx, CancellationToken cancellationToken)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent, cancellationToken);
        }
    }
}