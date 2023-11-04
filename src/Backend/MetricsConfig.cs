using System.Diagnostics.Metrics;

namespace Backend;

public static class MetricsConfig
{
    public static readonly Meter Meter = new(DiagnosticsConfig.Name);

    // example metrics we can add values to (change these to match your app's needs)
    public static readonly Histogram<double> SalesValue = Meter
        .CreateHistogram<double>(
            name: "sales.value",
            unit: "currency",
            description: "the value of sales");

    public static readonly Counter<int> Users = Meter
        .CreateCounter<int>(
            name: "users.count",
            unit: "users",
            description: "the number of users registered");

    public static readonly Counter<int> Orders = Meter
        .CreateCounter<int>(
            name: "sales.orders.count",
            unit: "orders",
            description: "the number of orders made");
}