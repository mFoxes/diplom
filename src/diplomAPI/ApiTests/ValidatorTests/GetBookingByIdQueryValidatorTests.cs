using System;
using System.Linq;
using System.Threading;
using GrandmaApi.Database;
using GrandmaApi.Localization;
using GrandmaApi.Mediatr.Queries;
using Domain.Models;
using GrandmaApi.Validators;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.ValidatorTests;

public class GetBookingByIdQueryValidatorTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new Mock<IGrandmaRepository>();
    private readonly Mock<ILocalizationService> _localizationMock = new Mock<ILocalizationService>();

    public GetBookingByIdQueryValidatorTests()
    {
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>(), It.IsAny<object>()))
            .Returns("invalid");
        _localizationMock.Setup(localization => localization.GetString(It.IsAny<string>())).Returns("invalid");
    }

    [Fact]
    public void BookingDoesntExist()
    {
        var id = Guid.NewGuid();
        var query = new GetBookingByIdQuery(id);

        _repoMock.Setup(repo => repo.Get(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(id))))
            .Returns((BookingModel)null);

        var validator = new GetBookingByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.Equal(1, result.Errors.Count);
        Assert.True(result.Errors.Exists(e => e.PropertyName == nameof(GetBookingByIdQuery.Id)));
    }
    
    [Fact]
    public void ValidModel()
    {
        var id = Guid.NewGuid();
        var query = new GetBookingByIdQuery(id);

        _repoMock.Setup(repo => repo.GetAsync(It.Is<NotDeleted<BookingModel, Guid>>(s => s.Ids.Contains(id)), CancellationToken.None).Result)
            .Returns(new BookingModel());

        var validator = new GetBookingByIdQueryValidator(_repoMock.Object, _localizationMock.Object);
        var result = validator.ValidateAsync(query).Result;
        
        Assert.True(result.IsValid);
    }
}