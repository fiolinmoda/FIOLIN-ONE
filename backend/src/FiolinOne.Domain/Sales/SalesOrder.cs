using FiolinOne.Domain.Common;

namespace FiolinOne.Domain.Sales;

public sealed class SalesOrder : AuditableEntity
{
    private readonly List<SalesOrderItem> items = [];

    private SalesOrder()
    {
    }

    public SalesOrder(
        Guid id,
        string salesOrderNumber,
        string customerName,
        DateTime orderDate,
        string status,
        string? notes,
        IReadOnlyList<SalesOrderItem> orderItems)
    {
        Id = id;
        SalesOrderNumber = salesOrderNumber;
        CustomerName = customerName;
        OrderDate = orderDate;
        Status = status;
        Notes = notes;
        items.AddRange(orderItems);
        RecalculateTotal();
    }

    public string SalesOrderNumber { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public DateTime OrderDate { get; private set; }
    public string Status { get; private set; } = SalesOrderStatuses.Draft;
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }
    public string? DeletedBy { get; private set; }
    public uint RowVersion { get; private set; }
    public IReadOnlyCollection<SalesOrderItem> Items => items;
    public DateTime CreatedAt => CreatedAtUtc;
    public DateTime? UpdatedAt => UpdatedAtUtc;

    public void Update(
        string customerName,
        DateTime orderDate,
        string status,
        string? notes,
        IReadOnlyList<SalesOrderItem> orderItems)
    {
        CustomerName = customerName;
        OrderDate = orderDate;
        Status = status;
        Notes = notes;
        TotalAmount = orderItems.Where(item => !item.IsDeleted).Sum(item => item.TotalAmount);
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void RecalculateTotal()
    {
        TotalAmount = items.Where(item => !item.IsDeleted).Sum(item => item.TotalAmount);
    }

    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        DeletedBy = deletedBy;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public static class SalesOrderStatuses
{
    public const string Draft = "Draft";
    public const string Approved = "Approved";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}
