using System.Text.Json;
using AutoMapper;
using DTO;
using GrandmaApi.Database;
using Singularis.Internal.Domain.Specifications;
using GrandmaApi.Mediatr.Commands;
using GrandmaApi.Models.Configs;
using Domain.Models;
using Domain.Enums;
using GrandmaApi.Notification;
using LdapConnector;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;

namespace GrandmaApi.Mediatr.Handlers.HttpRequestsHandlers.CommandHandlers;

public class SyncronizeUsersCommandHandler : IRequestHandler<SyncronizeUsersCommand, HttpCommandResponse<Unit>>
{
    private readonly ILdapRepository _ldapRepository;
    private readonly IGrandmaRepository _grandmaRepository;
    private readonly INotifier _notifier;
    private readonly ILogger<SyncronizeUsersCommandHandler> _logger;
    private readonly IOptions<ImagesConfig> _config;
    private readonly IMapper _mapper;

    public SyncronizeUsersCommandHandler(ILdapRepository ldapRepository, 
                                         IGrandmaRepository grandmaRepository,
                                         IMapper mapper, 
                                         IOptions<ImagesConfig> config,
                                         INotifier notifier,
                                         ILogger<SyncronizeUsersCommandHandler> logger)
    {
        _ldapRepository = ldapRepository;
        _grandmaRepository = grandmaRepository;
        _mapper = mapper;
        _config = config;
        _notifier = notifier;
        _logger = logger;
    }

    public async Task<HttpCommandResponse<Unit>> Handle(SyncronizeUsersCommand request, CancellationToken cancellationToken)
    {
        var ldapUsers = _ldapRepository.GetUsers();
        var mongoUsers = await _grandmaRepository.ListAsync(new NotDeleted<MongoUser, Guid>());

        var toDelete = mongoUsers.ExceptBy(ldapUsers.Select(lu => lu.LdapId), mu => mu.LdapId);
        var toAdd = ldapUsers.ExceptBy(mongoUsers.Select(mu => mu.LdapId), lu => lu.LdapId);
        
        _logger.LogDebug($"{toDelete.Count()} will be remove");
        _logger.LogDebug($"{toAdd.Count()} will be add");
        
        
        var bookings = await _grandmaRepository.ListAsync(new NotDeleted<BookingModel, Guid>());
        var devices = await _grandmaRepository.ListAsync(new NotDeleted<DeviceModel, Guid>());
        var adminNotifications = new List<AdminNotificationDto>();
        foreach (var entry in toDelete)
        {
            var photoId = entry.PhotoId;
            entry.IsDeleted = true;
            await _grandmaRepository.SaveAsync(entry);
            if (photoId.HasValue)
            {
                var file = await _grandmaRepository.GetAsync(new NotDeleted<FileModel, Guid>(photoId.Value));
                file.IsDeleted = true;
                await _grandmaRepository.SaveAsync(file);
                File.Delete(Path.Combine(_config.Value.Path, file.StorredFileName));
                _logger.LogDebug($"File with id {file.Id} and name {file.StorredFileName} removed. ");
            }

            if (bookings is null)
            {
                continue;
            }
            var userBookings = bookings.Where(b => b.UserId == entry.Id);
            
            if (userBookings.Any())
            {
                _logger.LogDebug($"Found {userBookings.Count()} bookings from deleted users");
                var bookDeviceInfo = userBookings.Join(devices, b => b.DeviceId, d => d.Id, (model, deviceModel) => new BookingInfoDto()
                {
                    Name = deviceModel.Name,
                    InventoryNumber = deviceModel.InventoryNumber,
                    ReturnAt = model.ReturnAt.Value,
                    TakeAt = model.TakeAt.Value
                });
                adminNotifications.Add(new AdminNotificationDto()
                {
                    Name = entry.CommonName,
                    Devices = bookDeviceInfo.ToList()
                });
                foreach (var userBook in userBookings)
                {
                    _logger.LogDebug($"Device with id {userBook.DeviceId} become free");
                    userBook.ReturnAt = null;
                    userBook.TakeAt = null;
                    userBook.State = DeviceStates.Free;
                    userBook.UserId = null;
                    await _grandmaRepository.SaveAsync(userBook);
                }
            }
        }
        if (adminNotifications.Any())
        {
            _logger.LogDebug($"{adminNotifications.Count()} will be sent");
            var admins = _ldapRepository.GetUsers().Where(u => u.Role == Roles.Admin);
            var adminEmails = admins.Select(a => a.Email);
            var adminMattermosts = admins.Select(a => a.UId);
            await _notifier.NotifyAdminAboutDeletedUsersWithBookingsByEmail(adminNotifications, adminEmails);
            await _notifier.NotifyAdminAboutDeletedUsersWithBookingsByMattermost(adminNotifications, adminMattermosts);
        }

        foreach (var entry in toAdd)
        {
            var entity = _mapper.Map<User, MongoUser>(entry);
            entity.Id = Guid.NewGuid();
            entity.MattermostName = entry.UId;
            await _grandmaRepository.SaveAsync(entity);
            _logger.LogDebug($"Added new user: {JsonSerializer.Serialize(entity)}");
        }

        return new HttpCommandResponse<Unit>(Unit.Value);
    }
}