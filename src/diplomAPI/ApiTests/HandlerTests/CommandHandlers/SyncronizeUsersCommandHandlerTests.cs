using System;
using System.Collections.Generic;
using Domain.Enums;
using System.Threading;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;
using Domain.Models;
using GrandmaApi.Notification;
using GrandmaApi.SignalR;
using LdapConnector;
using Microsoft.Extensions.Logging;
using Moq;
using Singularis.Internal.Domain.Specifications;
using Xunit;

namespace ApiTests.HandlerTests.CommandHandlers;

public class SyncronizeUsersCommandHandlerTests
{
    private readonly Mock<IGrandmaRepository> _repoMock = new();
    private readonly Mock<ILdapRepository> _ldapMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<INotifier> _notifierMock = new();
    private readonly Mock<ILogger<SyncronizeUsersCommandHandler>> _loggerMock = new(); 
    
    [Fact]
    public void UsersAdded()
    {
        var ldapUser1 = new User()
        {
            CommonName = "User 1",
            Email = "user1@example.com",
            LdapId = 1000,
            UId = "user.first"
        };
        var ldapUser2 = new User()
        {
            CommonName = "User 2",
            Email = "user2@example.com",
            LdapId = 1001,
            UId = "user.second"
        };
        
        
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(new List<MongoUser>());
        _ldapMock.Setup(ldap => ldap.GetUsers()).Returns(new List<User>(){ ldapUser1, ldapUser2});
        _mapperMock.Setup(mapper => mapper.Map<User, MongoUser>(It.IsAny<User>())).Returns<User>(user => new MongoUser()
        {
            CommonName = user.CommonName,
            Email = user.Email,
            LdapId = user.LdapId,
            MattermostName = user.UId
        });

        var command = new SyncronizeUsersCommand();
        var handler = new SyncronizeUsersCommandHandler(_ldapMock.Object, _repoMock.Object, _mapperMock.Object, null,
            _notifierMock.Object, _loggerMock.Object);
        handler.Handle(command, CancellationToken.None);
        
        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<MongoUser>(), CancellationToken.None), Times.Exactly(2));
        
        
    }
    
    [Fact]
    public void UsersDeleted()
    {
        var user1 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = 1002,
            Email = "first@example.com",
            CommonName = "User 1",
            MattermostName = "user.first"
        };
        var user2 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = 1003,
            Email = "second@example.com",
            CommonName = "User 2",
            MattermostName = "user.second"
        };
        
        var ldapUser1 = new User()
        {
            CommonName = "Ldap User 1",
            Email = "ldapUser1@example.com",
            LdapId = 1000,
            UId = "ldap.first"
        };
        var ldapUser2 = new User()
        {
            CommonName = "Ldap User 2",
            Email = "ldapUser2@example.com",
            LdapId = 1001,
            UId = "ldap.second"
        };
        
        
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(new List<MongoUser>(){user1, user2});
        _ldapMock.Setup(ldap => ldap.GetUsers()).Returns(new List<User>(){ ldapUser1, ldapUser2});
        _mapperMock.Setup(mapper => mapper.Map<User, MongoUser>(It.IsAny<User>())).Returns<User>(user => new MongoUser()
        {
            CommonName = user.CommonName,
            Email = user.Email,
            LdapId = user.LdapId,
            MattermostName = user.UId
        });

        var command = new SyncronizeUsersCommand();
        var handler = new SyncronizeUsersCommandHandler(_ldapMock.Object, _repoMock.Object, _mapperMock.Object, null,
            _notifierMock.Object, _loggerMock.Object);
        handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<MongoUser>(), CancellationToken.None), Times.Exactly(4));
    }
    
    [Fact]
    public void NotifiedAboutDeleted()
    {
        var user1 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = 1001,
            Email = "first@example.com",
            CommonName = "User 1",
            MattermostName = "user.first"
        };
        
        var user2 = new MongoUser()
        {
            Id = Guid.NewGuid(),
            LdapId = 1002,
            Email = "second@example.com",
            CommonName = "User 2",
            MattermostName = "user.second"
        };
        
        var device1 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.001",
            Name = "Device 1"
        };
        
        var device2 = new DeviceModel()
        {
            Id = Guid.NewGuid(),
            InventoryNumber = "d.00.002",
            Name = "Device 2"
        };
        
        var booking1 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device1.Id,
            UserId = user1.Id,
            State = DeviceStates.Booked,
            ReturnAt = DateTime.Now.AddDays(3),
            TakeAt = DateTime.Now.AddDays(-3)
        };
        
        var booking2 = new BookingModel()
        {
            Id = Guid.NewGuid(),
            DeviceId = device2.Id,
            UserId = user2.Id,
            State = DeviceStates.Booked,
            ReturnAt = DateTime.Now.AddDays(5),
            TakeAt = DateTime.Now.AddDays(-5)
        };
        
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<MongoUser, Guid>>(), CancellationToken.None).Result)
            .Returns(new List<MongoUser>(){user1, user2});
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<DeviceModel, Guid>>(), CancellationToken.None).Result)
            .Returns(new List<DeviceModel>(){device1, device2});
        _repoMock.Setup(repo => repo.ListAsync(It.IsAny<NotDeleted<BookingModel, Guid>>(), CancellationToken.None).Result)
            .Returns(new List<BookingModel>(){booking1, booking2});
        
        _ldapMock.Setup(ldap => ldap.GetUsers()).Returns(new List<User>());
        _mapperMock.Setup(mapper => mapper.Map<User, MongoUser>(It.IsAny<User>())).Returns<User>(user => new MongoUser()
        {
            CommonName = user.CommonName,
            Email = user.Email,
            LdapId = user.LdapId,
            MattermostName = user.UId
        });

        var command = new SyncronizeUsersCommand();
        var handler = new SyncronizeUsersCommandHandler(_ldapMock.Object, _repoMock.Object, _mapperMock.Object, null,
            _notifierMock.Object, _loggerMock.Object);
        handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(repo => repo.SaveAsync(It.IsAny<MongoUser>(), CancellationToken.None), Times.Exactly(2));
        _notifierMock.Verify(notifier => notifier.NotifyAdminAboutDeletedUsersWithBookingsByEmail(It.IsAny<IEnumerable<AdminNotificationDto>>(), It.IsAny<IEnumerable<string>>()));
        _notifierMock.Verify(notifier => notifier.NotifyAdminAboutDeletedUsersWithBookingsByMattermost(It.IsAny<IEnumerable<AdminNotificationDto>>(), It.IsAny<IEnumerable<string>>()));
    }
}