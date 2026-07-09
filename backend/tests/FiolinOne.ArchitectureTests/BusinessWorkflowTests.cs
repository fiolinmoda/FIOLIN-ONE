using FiolinOne.Domain.Products;
using FiolinOne.Domain.Production;
using Xunit;

namespace FiolinOne.ArchitectureTests;

public sealed class BusinessWorkflowTests
{
    [Fact]
    public void Product_variant_stock_increases_when_finished_goods_enter_warehouse()
    {
        var variant = new ProductVariant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "BC-001", null, 10, "Active");

        variant.IncreaseStock(15);

        Assert.Equal(25, variant.Stock);
    }

    [Fact]
    public void Product_variant_stock_cannot_be_increased_with_negative_quantity()
    {
        var variant = new ProductVariant(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "BC-002", null, 10, "Active");

        Assert.Throws<InvalidOperationException>(() => variant.IncreaseStock(-1));
    }

    [Fact]
    public void Workshop_shipment_status_tracks_return_progress()
    {
        var shipment = new WorkshopShipment(Guid.NewGuid(), "Atolye A", DateTime.UtcNow, null, 100, null, WorkshopShipmentStatuses.Sent);

        shipment.SetStatus(WorkshopShipmentStatuses.PartialReturn);
        Assert.Equal(WorkshopShipmentStatuses.PartialReturn, shipment.Status);

        shipment.SetStatus(WorkshopShipmentStatuses.Returned);
        Assert.Equal(WorkshopShipmentStatuses.Returned, shipment.Status);
    }

    [Fact]
    public void Production_order_item_is_barcode_ready_by_default()
    {
        var item = new ProductionOrderItem(Guid.NewGuid(), Guid.NewGuid(), 12);

        Assert.False(item.BarcodeGenerated);
        Assert.False(item.BarcodePrinted);
        Assert.Null(item.BarcodeValue);
    }
}
