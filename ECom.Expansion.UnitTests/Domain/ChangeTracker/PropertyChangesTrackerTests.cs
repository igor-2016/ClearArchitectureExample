using ApplicationServices.Implementation.ChangeTracker;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace ECom.Expansion.UnitTests.Domain
{
    public class PropertyChangesTrackerTests
    {
        [Fact]
        public async Task Can_GetPropertiesChanges()
        {
            var cancellationToken = CancellationToken.None;
            var sourceEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 1,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };
            var targetNewEntity = new TestEntity();

            targetNewEntity.Id = sourceEntity.Id;
            targetNewEntity.Name = sourceEntity.Name;
            await targetNewEntity.NavigatePropertiesAsync(sourceEntity, onPropertyChanged: OnPropertyChanged,
                cancellationToken: cancellationToken);

            object a = 1;
            object b = 1;

            Assert.False(a == b); // !
            Assert.True(a.Equals(b));

            int? c = null;
            int? d = null;
            Assert.True(c == d);
            Assert.True(c.Equals(d));

            c = 1;
            d = 1;
            Assert.True(c == d);
            Assert.True(c.Equals(d));
        }

        [Fact]
        public async Task Can_GetChangesFromDifferentTypePropertiesSameName()
        {
            var cancellationToken = CancellationToken.None;
            var sourceEntity = new TestEntity()
            {
                Id = 1,
            };
            var targetEntity = new TestEntityIdGuid();

            await targetEntity.NavigatePropertiesAsync(sourceEntity, onPropertyChanged: OnPropertyChangedGuid,
                cancellationToken: cancellationToken);

            object a = 1;
            object b = Guid.NewGuid();

            Assert.False(a == b); 
            Assert.False(a.Equals(b));
        }

        [Fact]
        public async Task Can_TrackChangeByChangeByPropertiesChangeTracker()
        {
            var cancellationToken = CancellationToken.None;
            var sourceEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = null,
                Id = 1,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };
            var targetNewEntity = new TestEntity();

            var changeTracker = new ChangeByChangePropertyTracker<TestEntity, TestEntity>();
            
            var externalIdChangeHandler = new ExternalIdProperyChangeHandler();
            var priceChangeHandler = new PricePropertyChangeHandler();
            var manyPropertiesHandler = new PriceAndExternalIdPropertiesChangeHandler();

            externalIdChangeHandler.Subscribe(changeTracker);
            priceChangeHandler.Subscribe(changeTracker);
            manyPropertiesHandler.Subscribe(changeTracker);

            await changeTracker.TrackChanges(sourceEntity, targetNewEntity, cancellationToken)
                .ContinueWith(c => changeTracker.Done(cancellationToken), cancellationToken);
        }

        [Fact]
        public async Task Can_TrackAllChangeThenProcessPropertiesChangeTracker()
        {
            var cancellationToken = CancellationToken.None;
            var sourceEntity = new TestEntity()
            {
                CreatedOn = DateTime.Now,
                Name = "Source",
                Duration = TimeSpan.FromSeconds(1),
                ExternalId = 56,
                Id = 1,
                ModifiedOn = DateTime.Now,
                Price = 100.1m
            };
            var targetNewEntity = new TestEntity();

            var changeTracker = new GetAllChangedProperiesThenProcessTracker<TestEntity, TestEntity>();

            var externalIdChangeHandler = new ExternalIdProperyChangeHandler();
            var priceChangeHandler = new PricePropertyChangeHandler();
            var manyPropertiesHandler = new PriceAndExternalIdPropertiesChangeHandler();

            externalIdChangeHandler.Subscribe(changeTracker);
            priceChangeHandler.Subscribe(changeTracker);
            manyPropertiesHandler.Subscribe(changeTracker);

            await changeTracker.TrackChanges(sourceEntity, targetNewEntity, cancellationToken)
                .ContinueWith(c => changeTracker.Done(cancellationToken), cancellationToken);


        }



        private Task OnPropertyChanged(TestEntity source,
            PropertyInfo sourcePropertyInfo, object? sourceValue,
            TestEntity target, PropertyInfo targetPropertyInfo, object? targetValue,
            CancellationToken cancellationToken)
        {
            Debug.WriteLine($"{sourcePropertyInfo.Name} : {sourceValue} -> {targetPropertyInfo.Name} : {targetValue}");
            return Task.CompletedTask;
        }

        private Task OnPropertyChangedGuid(TestEntity source,
            PropertyInfo sourcePropertyInfo, object? sourceValue,
            TestEntityIdGuid target, PropertyInfo targetPropertyInfo, object? targetValue,
            CancellationToken cancellationToken)
        {
            Debug.WriteLine($"{sourcePropertyInfo.Name} : {sourceValue} -> {targetPropertyInfo.Name} : {targetValue}");
            return Task.CompletedTask;
        }
    }
}
