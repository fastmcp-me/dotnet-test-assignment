namespace Weather.Core;

public sealed class AlertLevel
{
	public static readonly AlertLevel Unknown = new(nameof(Unknown), 1);
	public static readonly AlertLevel Green = new(nameof(Green), 1);
    public static readonly AlertLevel Yellow = new(nameof(Yellow), 2);
    public static readonly AlertLevel Orange = new(nameof(Orange), 3);
    public static readonly AlertLevel Red = new(nameof(Red), 4);

    public string Name { get; }
    public int Value { get; }

    private AlertLevel(string name, int value)
    {
        Name = name;
        Value = value;
    }

    public override string ToString() => Name;

    public static IEnumerable<AlertLevel> List() => [ Green, Yellow, Orange, Red ];

    public static AlertLevel FromName(string name) =>
        List().FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase)) ?? Unknown;

    public static AlertLevel FromValue(int value) =>
        List().FirstOrDefault(x => x.Value == value) ?? Unknown;
}
