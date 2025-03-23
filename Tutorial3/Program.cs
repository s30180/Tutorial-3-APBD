using System;
using System.Collections.Generic;


interface IHazardNotifier
{
    void NotifyHazard(string containerNumber);
}

// Custom exception for overfilling
class OverfillException : Exception
{
    public OverfillException(string message) : base(message) { }
}

// Container class
abstract class Container
{
    private static int idCounter = 1;
    public string SerialNumber { get; private set; }
    public int Height { get; set; }
    public int Depth { get; set; }
    public double TareWeight { get; set; }
    public double MaxPayload { get; set; }
    public double CurrentLoad { get; protected set; } = 0;

    public Container(int height, int depth, double tareWeight, double maxPayload, string typeCode)
    {
        Height = height;
        Depth = depth;
        TareWeight = tareWeight;
        MaxPayload = maxPayload;
        SerialNumber = $"KON-{typeCode}-{idCounter++}";
    }

    public abstract void Load(double weight, bool isHazardous);
    public abstract void Unload();
    public abstract void PrintInfo();
}

class LiquidContainer : Container, IHazardNotifier
{
    private bool isHazardous;

    public LiquidContainer(int height, int depth, double tareWeight, double maxPayload)
        : base(height, depth, tareWeight, maxPayload, "L") { }

    public override void Load(double weight, bool isHazardous)
    {
        this.isHazardous = isHazardous;
        double limit = isHazardous ? 0.5 : 0.9;
        if (weight > MaxPayload * limit)
        {
            NotifyHazard(SerialNumber);
            throw new OverfillException("Overfilled hazardous liquid container");
        }
        CurrentLoad = weight;
    }

    public override void Unload()
    {
        CurrentLoad = 0;
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation in container {containerNumber}");
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"[LiquidContainer] {SerialNumber} Load: {CurrentLoad}");
    }
}

class GasContainer : Container, IHazardNotifier
{
    private double pressure;

    public GasContainer(int height, int depth, double tareWeight, double maxPayload, double pressure)
        : base(height, depth, tareWeight, maxPayload, "G")
    {
        this.pressure = pressure;
    }

    public override void Load(double weight, bool isHazardous)
    {
        if (weight > MaxPayload)
        {
            NotifyHazard(SerialNumber);
            throw new OverfillException("Overfilled gas container");
        }
        CurrentLoad = weight;
    }

    public override void Unload()
    {
        CurrentLoad *= 0.05;
    }

    public void NotifyHazard(string containerNumber)
    {
        Console.WriteLine($"Hazardous situation in gas container {containerNumber}");
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"[GasContainer] {SerialNumber} Load: {CurrentLoad}, Pressure: {pressure}");
    }
}

class RefrigeratedContainer : Container
{
    private string productType;
    private double requiredTemp;
    private double containerTemp;

    public RefrigeratedContainer(int height, int depth, double tareWeight, double maxPayload, string productType, double requiredTemp, double containerTemp)
        : base(height, depth, tareWeight, maxPayload, "C")
    {
        this.productType = productType;
        this.requiredTemp = requiredTemp;
        this.containerTemp = containerTemp;
    }

    public override void Load(double weight, bool isHazardous)
    {
        if (containerTemp < requiredTemp)
        {
            throw new OverfillException("Temperature too low for product type");
        }
        if (weight > MaxPayload)
        {
            throw new OverfillException("Overfilled refrigerated container");
        }
        CurrentLoad = weight;
    }

    public override void Unload()
    {
        CurrentLoad = 0;
    }

    public override void PrintInfo()
    {
        Console.WriteLine($"[RefrigeratedContainer] {SerialNumber} Load: {CurrentLoad} Product: {productType}");
    }
}

class ContainerShip
{
    private string name;
    private double maxSpeed;
    private int maxContainerNum;
    private double maxWeight;
    private List<Container> containers = new List<Container>();

    public ContainerShip(string name, double maxSpeed, int maxContainerNum, double maxWeight)
    {
        this.name = name;
        this.maxSpeed = maxSpeed;
        this.maxContainerNum = maxContainerNum;
        this.maxWeight = maxWeight;
    }

    public bool LoadContainer(Container c)
    {
        double totalWeight = 0;
        foreach (var ct in containers)
        {
            totalWeight += ct.TareWeight + ct.CurrentLoad;
        }

        if (containers.Count < maxContainerNum && totalWeight + c.TareWeight + c.CurrentLoad <= maxWeight * 1000)
        {
            containers.Add(c);
            return true;
        }
        return false;
    }

    public bool RemoveContainer(string serial)
    {
        return containers.RemoveAll(c => c.SerialNumber == serial) > 0;
    }

    public void PrintShipInfo()
    {
        Console.WriteLine($"Ship: {name} Speed: {maxSpeed}kn Max weight: {maxWeight}t");
        foreach (var c in containers)
        {
            c.PrintInfo();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var lc = new LiquidContainer(250, 300, 2000, 10000);
            lc.Load(4000, true);

            var gc = new GasContainer(250, 300, 1500, 8000, 10);
            gc.Load(7000, false);

            var rc = new RefrigeratedContainer(250, 300, 2200, 9000, "Bananas", 5, 6);
            rc.Load(8000, false);

            var ship = new ContainerShip("Poseidon", 20, 100, 40000);
            ship.LoadContainer(lc);
            ship.LoadContainer(gc);
            ship.LoadContainer(rc);

            ship.PrintShipInfo();
        }
        catch (OverfillException e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}
