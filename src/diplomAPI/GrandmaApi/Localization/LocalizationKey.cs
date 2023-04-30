using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrandmaApi.Localization
{
    public static class LocalizationKey
    {
        public static class Shared
        {
            public const string NotEmpty = "shared.NotEmpty";
            public const string MaxLength = "shared.MaxLength";
            public const string FutureDate = "shared.FutureDate";
            public const string CannotDeserialize = "shared.CannotDeserialize";
            public const string InternalServerError = "shared.InternalServerError";
        }
        public static class Device
        {
            public const string InventoryNumberIsUsed = "device.InventoryNumberIsUsed";
            public const string NotFound = "device.NotFound";
            public const string NotBooked = "device.NotBooked";
            public const string Booked = "device.Booked";
            public const string InventoryNumberNotFound = "device.InventoryNumberNotFound";
            public const string InventoryNumberTemplate = "device.InventoryNumberTemplate";
        }
        public static class File
        {
            public const string FileNotFound = "file.NotFound";
            public const string MaxSize = "file.MaxSize";
            public const string AllowedExtensions = "file.AllowedExtensions";
        }
        public static class User
        {
            public const string NotFound = "user.NotFound";
            public const string NameTemplate = "user.NameTemplate";
            public const string Email = "user.Email";
            public const string MattermostNameTemplate = "user.MattermostNameTemplate";
            public const string IsNull = "user.IsNull";
            public const string NotSynchronized = "user.NotSynchronized";
            public const string NotRecognized = "user.NotRecognized";
        }
        public static class Booking
        {
            public const string NotFound = "booking.NotFound";
            public const string TakeAtBeforeReturnAt = "booking.TakeAtBeforeReturnAt";
        }
    }
}