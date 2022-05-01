using ApplicationServices.Implementation.ChangeTracker;
using BenchmarkDotNet.Attributes;
using ECom.Expansion.UnitTests.Domain;
using ECom.Expansion.UnitTests.Domain.ChangeTracker;

namespace ECom.Expansion.Benchmarks
{
    public class TestChangeTrackers
    {
        [Benchmark]
        public async Task TestChangeByChangeEntityTracker()
        {
            await Do(true);
        }

        [Benchmark]
        public async Task TestAllChangesEntityTracker()
        {
            await Do(false);
        }

        public async Task Do(bool useChangeByChangeEntityTracker)
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
                //Stopwatch sw = Stopwatch.StartNew();
                while (callCount++ < maxCoallCount)
                {
                    entityHandler.Subscribe(changeByChangeTracker);
                    await changeByChangeTracker.TrackChanges(sourceOfChangesEntity, targetUpdateEntity, cancellationToken)
                        .ContinueWith(c => changeByChangeTracker.Done(cancellationToken), cancellationToken);
                }
                //Debug.WriteLine($"useChangeByChangeEntityTracker: {useChangeByChangeEntityTracker}");
                //Debug.WriteLine(sw.ElapsedMilliseconds);
            }
            else
            {
                //Stopwatch sw = Stopwatch.StartNew();
                while (callCount++ < maxCoallCount)
                {
                    entityHandler.Subscribe(allChangesTracker);
                    await allChangesTracker.TrackChanges(sourceOfChangesEntity, targetUpdateEntity,
                        cancellationToken)
                        .ContinueWith(c => allChangesTracker.Done(cancellationToken), cancellationToken);
                }
                //Debug.WriteLine($"useChangeByChangeEntityTracker: {useChangeByChangeEntityTracker}");
                //Debug.WriteLine(sw.ElapsedMilliseconds);
            }
        }
    }

}
