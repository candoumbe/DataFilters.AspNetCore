// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.UnitTests
{
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using Xunit.Categories;

    using static Moq.MockBehavior;
    using static Moq.It;
    using Xunit;
    using FsCheck.Xunit;
    using FsCheck;
    using System;
    using DataFilters.Casing;
    using Microsoft.AspNetCore.Builder;
    using FluentAssertions;

    [UnitTest]
    public class DataFiltersExtensionsTests
    {
        private readonly Mock<IServiceCollection> _serviceCollectionMock;
        private readonly Mock<IApplicationBuilder> _applicationBuilderMock;

        public DataFiltersExtensionsTests()
        {
            _serviceCollectionMock = new(Strict);
            _applicationBuilderMock = new(Strict);
        }

        /// <summary>
        /// Tests for <see cref="DataFiltersExtensions.AddDataFilters(IServiceCollection, Action{DataFilterOptions})"/>
        /// </summary>
        [Fact]
        public void Given_no_configuration_AddDataFilterService_should_add_instance_with_default_options()
        {
            // Arrange
            _serviceCollectionMock.Setup(mock => mock.Add(IsAny<ServiceDescriptor>()));

            // Act
            _serviceCollectionMock.Object.AddDataFilters();

            // Assert
            _serviceCollectionMock.Verify(mock => mock.Add(IsAny<ServiceDescriptor>()), Times.Once);
            _serviceCollectionMock.Verify(mock => mock.Add(Is<ServiceDescriptor>(sd => sd.ServiceType == typeof(IDataFilterService)
                                                                                       && sd.Lifetime == ServiceLifetime.Singleton
                                                                                       && sd.ImplementationInstance != null)),
                                          Times.Once);
            _serviceCollectionMock.VerifyNoOtherCalls();
        }
    }
}
