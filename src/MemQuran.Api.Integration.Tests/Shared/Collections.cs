namespace MemQuran.Api.Integration.Tests.Shared;

// This class has no code, and is never created. Its purpose is simply to be the place
// to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.

// Ensure this test runs in the same collection (group of tests across the project)
// injecting type T in ICollectionFixture injected once as singleton for all tests in the same collection
// as opposed to a new instance created for each test class, including base classes in the tests.

// Each collection runs in parallel with other collections, but not within the same collection.

// Having one collection for all tests in the project, SharedFixture is created once for all tests in the same collection.
// This single SharedFixture is responsible for setting up resources for all tests in the collection

[CollectionDefinition(nameof(AudioCollection))]
public class AudioCollection : ICollectionFixture<SharedFixture>;

[CollectionDefinition(nameof(ConfigurationCollection))]
public class ConfigurationCollection : ICollectionFixture<SharedFixture>;

[CollectionDefinition(nameof(DuaCollection))]
public class DuaCollection : ICollectionFixture<SharedFixture>;

[CollectionDefinition(nameof(ProphetsCollection))]
public class ProphetsCollection : ICollectionFixture<SharedFixture>;