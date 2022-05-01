using ApplicationServices.Implementation.ChangeTracker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace ECom.Expansion.UnitTests.Domain.ChangeTracker
{
    public class EntityChangeTrackerTests
    {
        [Fact]
        public async Task Can_GetEntityChanges()
        {
            var sourceUpdateEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 1,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };

            var sourceNewEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source new",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 3,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };

            var targetUpdateEntity = new TestEntity()
            {
                Id = 1,
            };

            var targetDeleteEntity = new TestEntity()
            {
                Id = 5,
            };

            var sources = new List<TestEntity>(new[] { sourceUpdateEntity, sourceNewEntity });

            var targets = new List<TestEntity>(new[] { targetUpdateEntity, targetDeleteEntity });

            await sources.NavigateEntityList(targets, onEntityChanged: OnEntityChanged, searchInSourcesByCurrentTarget:
                SearchInSourcesByCurrentTarget, cancellationToken: CancellationToken.None);

        }

        [Fact]
        public async Task Can_TrackChangeByChangeByEntityChangeTracker()
        {
            var cancellationToken = CancellationToken.None;
            var sourceUpdateEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 1,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };

            var sourceNewEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source new",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 3,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };

            var targetUpdateEntity = new TestEntity()
            {
                Id = 1,
            };

            var targetDeleteEntity = new TestEntity()
            {
                Id = 5,
            };

            var sources = new List<TestEntity>(new[] { sourceUpdateEntity, sourceNewEntity });

            var targets = new List<TestEntity>(new[] { targetUpdateEntity, targetDeleteEntity });


            var changeTracker = new TestChangeByChangeEntityTracker();

            var handler = new TestEntityChangesHandler("entity change tracker");

            handler.Subscribe(changeTracker);

            await changeTracker.TrackChanges(sources, targets, cancellationToken)
                .ContinueWith(c => changeTracker.Done(cancellationToken), cancellationToken);
        }

        const bool UseChangeByChangeEntityTracker = true;

        [Theory]
        [InlineData(!UseChangeByChangeEntityTracker)]
        //[InlineData(UseChangeByChangeEntityTracker)]
        public async Task Can_TrackChangesEntityAndProperties(bool useChangeByChangeEntityTracker)
        {
            var cancellationToken = CancellationToken.None;
            var sourceOfChangesEntity = new TestEntity()
            {
                Id = 1,
                CreatedOn = DateTime.Now,
                Name = "The Name",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,

                ModifiedOn = DateTime.Now,
                Price = 150.2m
            };

            var targetUpdateEntity = new TestEntity()
            {
                Id = sourceOfChangesEntity.Id,
                CreatedOn = sourceOfChangesEntity.CreatedOn,
                Name = sourceOfChangesEntity.Name,
                Duration = sourceOfChangesEntity.Duration,
                ExternalId = sourceOfChangesEntity.ExternalId,

                ModifiedOn = DateTime.Now.AddDays(-1),
                Price = 120.5m,
            };



            var propertiesChangeTracker = useChangeByChangeEntityTracker
                ? new ChangeByChangePropertyTracker<TestEntity, TestEntity>()
                : new GetAllChangedProperiesThenProcessTracker<TestEntity, TestEntity>();

            var externalIdChangeHandler = new ExternalIdProperyChangeHandler();
            var priceChangeHandler = new PricePropertyChangeHandler();
            var manyPropertiesHandler = new PriceAndExternalIdPropertiesChangeHandler();

            externalIdChangeHandler.Subscribe(propertiesChangeTracker);
            priceChangeHandler.Subscribe(propertiesChangeTracker);
            manyPropertiesHandler.Subscribe(propertiesChangeTracker);

            var changeByChangeTracker = new TestChangeByChangeEntityTracker();

            var allChangesTracker = new TestAllChangesEntityTracker();


            var entityHandler = new TestEntityChangesHandler("entity change tracker with properties",
                propertiesChangeTracker);

            var callCount = 0;
            var maxCoallCount = 10000;
            if (useChangeByChangeEntityTracker)
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (callCount++ < maxCoallCount)
                {
                    entityHandler.Subscribe(changeByChangeTracker);
                    await changeByChangeTracker.TrackChanges(sourceOfChangesEntity, targetUpdateEntity, cancellationToken)
                        .ContinueWith(c => changeByChangeTracker.Done(cancellationToken), cancellationToken);
                }
                Debug.WriteLine($"useChangeByChangeEntityTracker: {useChangeByChangeEntityTracker}");
                Debug.WriteLine(sw.ElapsedMilliseconds);
            }
            else
            {
                Stopwatch sw = Stopwatch.StartNew();
                while (callCount++ < maxCoallCount)
                {
                    entityHandler.Subscribe(allChangesTracker);
                    await allChangesTracker.TrackChanges(sourceOfChangesEntity, targetUpdateEntity, cancellationToken)
                        .ContinueWith(c => allChangesTracker.Done(cancellationToken), cancellationToken);
                }
                Debug.WriteLine($"useChangeByChangeEntityTracker: {useChangeByChangeEntityTracker}");
                Debug.WriteLine(sw.ElapsedMilliseconds);
            }
        }
    


       

        private Task OnEntityChanged(ChangeType changeType, TestEntity? source, TestEntity? target,
            CancellationToken cancellationToken)
        {
            if (changeType == ChangeType.Add)
            {
                Assert.Equal(3, source.Id);
            }

            if(changeType == ChangeType.Remove)
            {
                Assert.Equal(5, target.Id);
            }

            if(changeType == ChangeType.Update)
            {
                Assert.Equal(1, source.Id);
            }

            return Task.CompletedTask;
        }

        private Task<TestEntity?> SearchInSourcesByCurrentTarget(
            IEnumerable<TestEntity> sources, TestEntity target, CancellationToken cancellationToken) 
                => Task.FromResult(sources.FirstOrDefault(x => x.Id == target.Id));

    }

}
