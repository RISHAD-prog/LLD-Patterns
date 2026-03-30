using System;
public enum Vehicle
{
    Car,
    Bike,
    Truck
}
public enum Spot
{
    Small,
    Medium,
    Large
}

public interface IParkingLot
{
    //ParkingSpot IsFull();
    void HandleRequest(Vehicle vehicle, bool isEnter);
}

public class ParkingLot : IParkingLot
{
    public List<ParkingFloor> parkingFloors;
    //public bool isCarEnter { get; set; }
    //public bool isCarExit { get; set; }
    public ParkingLot() {
    }
    public void HandleRequest(Vehicle vehicle, bool isEnter)
    {
        if(isEnter)
        {
            foreach (var floor in parkingFloors)
            {
                if (floor.AssignParkingSpot(vehicle))
                {
                    Console.WriteLine("Parking spot assigned successfully.");
                    return;
                }
                else
                {
                    continue;
                }
            }
        }
        else if (!isEnter)
        {
            foreach (var floor in parkingFloors)
            {
                floor.UnAssignParkingSpot(vehicle);
            }
        }
    }

    //public ParkingSpot IsFull(Vehicle vehicle, Spot spotType)
    //{
    //    foreach (var floor in parkingFloors)
    //    {
    //        var spot = floor.IsAvailableSpot(vehicle, spotType);
    //        if (spot == null)
    //        {
    //            return spot;
    //        }
    //    }
    //    return null;
    //}
}

public interface IParkingSpot
{
    void Assign(Vehicle vehicle);
    void Unassign(Vehicle vehicle);

    bool CanFit(Vehicle vehicle);
}

public class ParkingSpot : IParkingSpot
{
    public int id;
    public Spot spotType;
    public bool isAvailable;
    public Vehicle assignedVehicle;
    private IParkingTicket _parkingTicket;

    public ParkingSpot(IParkingTicket parkingTicket)
    {
        this._parkingTicket = parkingTicket;
    }

    public void Assign(Vehicle vehicle)
    {
        this.assignedVehicle = vehicle;
        this.isAvailable = false;
        this.id = Guid.NewGuid().GetHashCode();
        _parkingTicket.CreateTicket(this.assignedVehicle, this.spotType);
    }

    public bool CanFit(Vehicle vehicle)
    {
        if (this.isAvailable)
        {
            if (Vehicle.Car == vehicle && (spotType == Spot.Medium || spotType == Spot.Large))
            {
                return true;
            }
            else if (this.isAvailable && Vehicle.Bike == vehicle && (spotType == Spot.Medium || spotType == Spot.Small || spotType == Spot.Large))
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    

    public void Unassign(Vehicle vehicle)
    {
        this.isAvailable = true;
    }
}

public interface IParkingFloor
{
    bool AssignParkingSpot(Vehicle vehicle);
    void UnAssignParkingSpot(Vehicle vehicle);
    //bool IsAvailableSpot(Vehicle vehicle);
}

public class ParkingFloor : IParkingFloor
{
    private List<ParkingSpot> parkingSpotList;
    private IParkingSpot _parkingSpot;

    public ParkingFloor(IParkingSpot parkingSpot)
    {
        _parkingSpot = parkingSpot;
    }

    //public ParkingFloor IsAvailableSpot(Vehicle vehicle)
    //{
    //    foreach (var item in parkingSpotList)
    //    {
    //        if(item.CanFit(vehicle))
    //        {
    //            return item;   
    //        }
    //    }
    //    return item;
    //}

    public bool AssignParkingSpot(Vehicle vehicle)
    {
        foreach (var item in parkingSpotList)
        {
            if (item.CanFit(vehicle))
            {
                item.Assign(vehicle);
                return item.isAvailable;
            }
            else
            {
                continue;
            }
        }
        return false;
    }

    public void UnAssignParkingSpot(Vehicle vehicle)
    {
        _parkingSpot.Unassign(vehicle);
    }
}

public interface IParkingTicket
{
    void CreateTicket(Vehicle vehicle, Spot spotType);
    //void CalculateFee();
}

public class ParkingTicket: IParkingTicket
{
    private int ticketId;
    private Vehicle vehicle;
    private Spot spot;
    private DateTime entryTime;
    private DateTime exitTime;
    private decimal fee;
    //private IPayment _payment;
    private ICalculator _calculator;
    public ParkingTicket(ICalculator calculator) {
        this._calculator = calculator;
    }

    //public void CalculateFee()
    //{
    //    TimeSpan duration = exitTime - entryTime;
    //    Console.WriteLine($"Parking Duration: {duration.TotalHours} hours");
    //    this._payment.ProcessPayment(this);
    //}

    public void CreateTicket(Vehicle vehicle, Spot spotType)
    {
        this.ticketId = Guid.NewGuid().GetHashCode();
        this.entryTime = DateTime.Now;
        this.exitTime = DateTime.Now;
        this.vehicle = vehicle;
        this.spot = spotType;
        this.fee = this._calculator.CalculateFee(entryTime, exitTime);
    }
}

public interface ICalculator
{
    decimal CalculateFee(DateTime entryTime, DateTime ExitTime);
}

public class FeeCalculator : ICalculator
{
    private IParkingTicket _parkingTicket;
    public FeeCalculator(IParkingTicket _parkingTicket) {
        this._parkingTicket = _parkingTicket;
    }
    public decimal CalculateFee(DateTime entryTime, DateTime ExitTime)
    {
        TimeSpan duration = ExitTime - entryTime;
        return (decimal)duration.TotalHours * 10; // Example fee calculation: $10 per hour
    }
}

public interface IPaymentFactory
{
    IPaymentStrategy CreatePayment(string paymentType);
}

public class PaymentFactory : IPaymentFactory
{
    public IPaymentStrategy CreatePayment(string paymentType)
    {
        if (paymentType == "CreditCard")
        {
            return new CreditCardPayment();
        }
        else if (paymentType == "Cash")
        {
            return new CashPayment();
        }
        else if (paymentType == "MobilePayment")
        {
            return new MobilePayment();
        }
        else
        {
            throw new ArgumentException("Invalid payment type");
        }
    }
}

//public interface IPayment
//{
//    void ProcessPayment(ParkingTicket parkingTicket);
//}

//public class Payment : IPayment
//{
//    private IPaymentStrategy _paymentStrategy;
//    public Payment(IPaymentStrategy paymentStrategy)
//    {
//        _paymentStrategy = paymentStrategy;
//    }
//    public void ProcessPayment(ParkingTicket parkingTicket)
//    {
//        _paymentStrategy.Pay(parkingTicket);
//    }
//}

public interface IPaymentStrategy
{
    void Pay(ParkingTicket parkingTicket);
}

public class CreditCardPayment : IPaymentStrategy
{
    public void Pay(ParkingTicket parkingTicket)
    {
        Console.WriteLine("Payment made using Credit Card.");
    }
}

public class CashPayment : IPaymentStrategy
{
    public void Pay(ParkingTicket parkingTicket)
    {
        Console.WriteLine("Payment made using Cash.");
    }
}

public class MobilePayment : IPaymentStrategy
{
    public void Pay(ParkingTicket parkingTicket)
    {
        Console.WriteLine("Payment made using Mobile Payment.");
    }
}

public interface IParkingAttendant
{
    void HandleRequest(Vehicle vehicle, bool isEnter);
}

public class ParkingAttendant : IParkingAttendant
{
    private IParkingLot _parkingLot;
    public ParkingAttendant(IParkingLot parkingLot)
    {
        _parkingLot = parkingLot;
    }
    public void HandleRequest(Vehicle vehicle, bool isEnter)
    {
        _parkingLot.HandleRequest(vehicle, isEnter);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        IParkingLot parkingLot = new ParkingLot();
        IParkingAttendant parkingAttendant = new ParkingAttendant(parkingLot);
        parkingAttendant.HandleRequest(Vehicle.Car, true);
        parkingAttendant.HandleRequest(Vehicle.Bike, true);
        parkingAttendant.HandleRequest(Vehicle.Truck, true);
        parkingAttendant.HandleRequest(Vehicle.Car, false);
        parkingAttendant.HandleRequest(Vehicle.Bike, false);
        parkingAttendant.HandleRequest(Vehicle.Truck, false);
    }
}