// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Xunit.Categories;

namespace DataFilters.AspNetCore.UnitTests;

[UnitTest]
public class DataFiltersExtensionsTests
{
    private readonly IServiceCollection _serviceCollectionMock;
    public DataFiltersExtensionsTests()
    {
        _serviceCollectionMock = Substitute.For<IServiceCollection>();
    }

    /// <summary>
    /// Tests for <see cref="ServiceCollectionExtensions.AddDataFilters(IServiceCollection, Action{DataFilterOptions})"/>
    /// </summary>
    [Fact]
    public void Given_no_configuration_AddDataFilterService_should_add_instance_with_default_options()
    {
        // Arrange

        // Act
        _serviceCollectionMock.AddDataFilters();

        // Assert
        _serviceCollectionMock.Received(1).Add(Arg.Any<ServiceDescriptor>());
        _serviceCollectionMock.Received(1).Add(Arg.Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IDataFilterService)
                                                                               && sd.Lifetime == ServiceLifetime.Singleton
                                                                               && sd.ImplementationInstance != null));
    }
}