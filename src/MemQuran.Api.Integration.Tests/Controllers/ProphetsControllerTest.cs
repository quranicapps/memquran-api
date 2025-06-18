using System.Net;
using System.Net.Http.Json;
using MemQuran.Api.Integration.Tests.Shared;
using Microsoft.AspNetCore.Mvc;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit.Abstractions;

namespace MemQuran.Api.Integration.Tests.Controllers;

[Trait("Category", "Integration"), Collection(nameof(WireMockCollection))]
public class ProphetsControllerTest(CustomApiFactory customApiFactory, ITestOutputHelper testOutputHelper) 
    : IClassFixture<CustomApiFactory>
{
    
}