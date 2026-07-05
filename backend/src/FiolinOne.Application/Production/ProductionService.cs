using FiolinOne.Application.Common.Models;
using FiolinOne.Application.Fabric;
using FiolinOne.Domain.Production;

namespace FiolinOne.Application.Production;

public sealed class ProductionService(
    IProductionRepository productionRepository,
    IFabricService fabricService) : IProductionService
{
    public Task<ProductionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        return productionRepository.GetDashboardAsync(cancellationToken);
    }

    public async Task<PagedResult<ProductionOrderDto>> GetOrdersAsync(ProductionQuery query, CancellationToken cancellationToken)
    {
        var result = await productionRepository.GetOrdersAsync(query.ToParameters(), cancellationToken);
        return new PagedResult<ProductionOrderDto>(
            result.Items.Select(ToDto).ToList(),
            result.Page,
            result.PageSize,
            result.TotalItems,
            result.TotalPages);
    }

    public async Task<ProductionOrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await productionRepository.GetOrderByIdAsync(id, cancellationToken);
        return order is null ? null : ToDto(order);
    }

    public async Task<ProductionOrderDto> CreateOrderAsync(CreateProductionOrderRequest request, CancellationToken cancellationToken)
    {
        await EnsureOrderIsValidAsync(request.ProductionNumber, null, request.ProductId, request.Items, cancellationToken);

        var order = new ProductionOrder(
            request.ProductionNumber.Trim(),
            request.ProductId,
            request.PlannedQuantity,
            request.ProductionReason.Trim(),
            NormalizeOptional(request.Notes),
            request.Status.Trim());

        await productionRepository.AddOrderAsync(order, cancellationToken);
        await productionRepository.ReplaceItemsAsync(order.Id, ToItems(order.Id, request.Items), cancellationToken);
        await AddTimelineAsync(order.Id, "Production Created", $"Production order {order.ProductionNumber} created.", DateTime.UtcNow, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);

        order = await productionRepository.GetOrderByIdAsync(order.Id, cancellationToken) ?? order;
        return ToDto(order);
    }

    public async Task<ProductionOrderDto?> UpdateOrderAsync(Guid id, UpdateProductionOrderRequest request, CancellationToken cancellationToken)
    {
        var order = await productionRepository.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return null;
        }

        await EnsureOrderIsValidAsync(request.ProductionNumber, id, request.ProductId, request.Items, cancellationToken);
        order.Update(request.ProductionNumber.Trim(), request.ProductId, request.PlannedQuantity, request.ProductionReason.Trim(), NormalizeOptional(request.Notes), request.Status.Trim());
        await productionRepository.ReplaceItemsAsync(id, ToItems(id, request.Items), cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);

        order = await productionRepository.GetOrderByIdAsync(id, cancellationToken) ?? order;
        return ToDto(order);
    }

    public async Task<bool> DeleteOrderAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await productionRepository.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return false;
        }

        order.SoftDelete();
        foreach (var item in order.Items)
        {
            item.SoftDelete();
        }

        await productionRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<ProductionOrderDto?> UpdateStatusAsync(Guid id, UpdateProductionStatusRequest request, CancellationToken cancellationToken)
    {
        var order = await productionRepository.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.SetStatus(request.Status.Trim());
        await AddTimelineAsync(id, request.Status.Trim(), $"Production status changed to {request.Status.Trim()}.", DateTime.UtcNow, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return ToDto(order);
    }

    public async Task<CuttingRecordDto> CreateCuttingAsync(CreateCuttingRecordRequest request, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(request.ProductionOrderId, cancellationToken);
        await fabricService.ConsumeFabricAsync(new CreateFabricConsumptionRequest(
            request.FabricId,
            request.ConsumedWeightKg,
            order.ProductionNumber,
            request.CuttingDate,
            request.Notes),
            cancellationToken);

        order.SetStatus(ProductionStatuses.Cutting);
        var record = new CuttingRecord(request.ProductionOrderId, request.FabricId, request.ConsumedWeightKg, request.WasteWeightKg, request.CuttingDate, NormalizeOptional(request.OperatorName), NormalizeOptional(request.Notes));
        await productionRepository.AddCuttingRecordAsync(record, cancellationToken);
        await AddTimelineAsync(request.ProductionOrderId, "Fabric Consumed", $"{request.ConsumedWeightKg} Kg consumed. Waste: {request.WasteWeightKg} Kg.", request.CuttingDate, cancellationToken);
        await AddTimelineAsync(request.ProductionOrderId, "Cutting Finished", "Cutting record created.", request.CuttingDate, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return new CuttingRecordDto(record.Id, record.ProductionOrderId, record.FabricId, string.Empty, record.ConsumedWeightKg, record.WasteWeightKg, record.CuttingDate, record.OperatorName, record.Notes);
    }

    public async Task<WorkshopShipmentDto> CreateWorkshopShipmentAsync(CreateWorkshopShipmentRequest request, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(request.ProductionOrderId, cancellationToken);
        order.SetStatus(ProductionStatuses.AtWorkshop);
        var shipment = new WorkshopShipment(request.ProductionOrderId, request.Workshop.Trim(), request.ShipmentDate, request.ExpectedReturnDate, request.SentQuantity, NormalizeOptional(request.Notes), request.Status.Trim());
        await productionRepository.AddWorkshopShipmentAsync(shipment, cancellationToken);
        await AddTimelineAsync(request.ProductionOrderId, "Workshop Shipment", $"{request.SentQuantity} sent to {request.Workshop.Trim()}.", request.ShipmentDate, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return ToDto(shipment);
    }

    public async Task<WorkshopReturnDto> CreateWorkshopReturnAsync(CreateWorkshopReturnRequest request, CancellationToken cancellationToken)
    {
        await GetRequiredOrderAsync(request.ProductionOrderId, cancellationToken);
        var workshopReturn = new WorkshopReturn(request.ProductionOrderId, request.WorkshopShipmentId, request.ReturnedQuantity, request.ExtraQuantity, request.MissingQuantity, request.ReturnDate, NormalizeOptional(request.Notes));
        await productionRepository.AddWorkshopReturnAsync(workshopReturn, cancellationToken);
        await AddTimelineAsync(request.ProductionOrderId, "Workshop Return", $"Returned {request.ReturnedQuantity}, extra {request.ExtraQuantity}, missing {request.MissingQuantity}.", request.ReturnDate, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return ToDto(workshopReturn);
    }

    public async Task<ProductionOrderDto?> SendToIroningPackagingAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await productionRepository.GetOrderByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return null;
        }

        order.SetStatus(ProductionStatuses.AtIroningPackaging);
        await AddTimelineAsync(id, "Ironing & Packaging", "Production sent to ironing and packaging.", DateTime.UtcNow, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return ToDto(order);
    }

    public async Task<WarehouseEntryDto> CreateWarehouseEntryAsync(CreateWarehouseEntryRequest request, CancellationToken cancellationToken)
    {
        var order = await GetRequiredOrderAsync(request.ProductionOrderId, cancellationToken);
        order.SetStatus(ProductionStatuses.Completed);
        var entry = new WarehouseEntry(request.ProductionOrderId, request.ActualQuantity, request.WarehouseDate, NormalizeOptional(request.Notes));
        await productionRepository.AddWarehouseEntryAsync(entry, cancellationToken);
        await AddTimelineAsync(request.ProductionOrderId, "Warehouse Entry", $"{request.ActualQuantity} finished products received.", request.WarehouseDate, cancellationToken);
        await productionRepository.SaveChangesAsync(cancellationToken);
        return ToDto(entry);
    }

    public async Task<IReadOnlyList<ProductionTimelineDto>> GetTimelineAsync(Guid productionOrderId, CancellationToken cancellationToken)
    {
        var timeline = await productionRepository.GetTimelineAsync(productionOrderId, cancellationToken);
        return timeline.Select(ToDto).ToList();
    }

    private async Task EnsureOrderIsValidAsync(string productionNumber, Guid? excludedId, Guid productId, IReadOnlyList<ProductionOrderItemRequest> items, CancellationToken cancellationToken)
    {
        if (await productionRepository.ProductionNumberExistsAsync(productionNumber.Trim(), excludedId, cancellationToken))
        {
            throw new InvalidOperationException("Production number already exists.");
        }

        if (!await productionRepository.ProductExistsAsync(productId, cancellationToken))
        {
            throw new InvalidOperationException("Product does not exist.");
        }

        foreach (var item in items)
        {
            if (!await productionRepository.VariantExistsAsync(item.ProductVariantId, productId, cancellationToken))
            {
                throw new InvalidOperationException("Selected variant does not belong to the selected product.");
            }
        }
    }

    private async Task<ProductionOrder> GetRequiredOrderAsync(Guid productionOrderId, CancellationToken cancellationToken)
    {
        return await productionRepository.GetOrderByIdAsync(productionOrderId, cancellationToken)
            ?? throw new InvalidOperationException("Production order does not exist.");
    }

    private async Task AddTimelineAsync(Guid orderId, string eventType, string description, DateTime eventDate, CancellationToken cancellationToken)
    {
        await productionRepository.AddTimelineAsync(new ProductionTimelineEntry(orderId, eventType, description, eventDate), cancellationToken);
    }

    private static IReadOnlyList<ProductionOrderItem> ToItems(Guid orderId, IReadOnlyList<ProductionOrderItemRequest> items)
    {
        return items.Select(item => new ProductionOrderItem(orderId, item.ProductVariantId, item.PlannedQuantity)).ToList();
    }

    private static ProductionOrderDto ToDto(ProductionOrder order)
    {
        return new ProductionOrderDto(
            order.Id,
            order.ProductionNumber,
            order.ProductId,
            order.Product?.ProductCode ?? string.Empty,
            order.Product?.ProductName ?? string.Empty,
            order.PlannedQuantity,
            order.ProductionReason,
            order.Notes,
            order.Status,
            order.Items.Where(item => !item.IsDeleted).Select(ToDto).ToList(),
            order.CreatedAt,
            order.UpdatedAt);
    }

    private static ProductionOrderItemDto ToDto(ProductionOrderItem item)
    {
        var variant = item.ProductVariant;
        var variantName = variant is null ? string.Empty : $"{variant.Color?.Name ?? string.Empty} / {variant.Size?.Name ?? string.Empty}";
        return new ProductionOrderItemDto(item.Id, item.ProductVariantId, variantName, item.PlannedQuantity, item.BarcodeGenerated, item.BarcodePrinted, item.BarcodeValue);
    }

    private static WorkshopShipmentDto ToDto(WorkshopShipment shipment) => new(shipment.Id, shipment.ProductionOrderId, shipment.Workshop, shipment.ShipmentDate, shipment.ExpectedReturnDate, shipment.SentQuantity, shipment.Notes, shipment.Status);
    private static WorkshopReturnDto ToDto(WorkshopReturn workshopReturn) => new(workshopReturn.Id, workshopReturn.ProductionOrderId, workshopReturn.WorkshopShipmentId, workshopReturn.ReturnedQuantity, workshopReturn.ExtraQuantity, workshopReturn.MissingQuantity, workshopReturn.ReturnDate, workshopReturn.Notes);
    private static WarehouseEntryDto ToDto(WarehouseEntry entry) => new(entry.Id, entry.ProductionOrderId, entry.ActualQuantity, entry.WarehouseDate, entry.Notes);
    private static ProductionTimelineDto ToDto(ProductionTimelineEntry entry) => new(entry.Id, entry.ProductionOrderId, entry.EventType, entry.Description, entry.EventDate, entry.CreatedAt);

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
