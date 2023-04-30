using Domain.Models;
using Singularis.Internal.Domain.Specification;

namespace GrandmaApi.Database.Specifications
{
    public class InventoryNumberSpecification : Specification<DeviceModel>
    {
        public readonly string InventoryNumber;
        public InventoryNumberSpecification(string inventoryNumber)
        {
            InventoryNumber = inventoryNumber;
            Query = Source().Where(d => d.InventoryNumber == inventoryNumber);
        }
    }
}