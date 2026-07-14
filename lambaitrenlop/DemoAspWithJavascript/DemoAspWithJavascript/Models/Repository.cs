namespace DemoAspWithJavascript.Models;

public class Repository : IRepository
{
    private Dictionary<int, Reservation> items;

    public Repository()
    {
        items = new Dictionary<int, Reservation>();
        new List<Reservation>
        {
            new Reservation { Id = 1, Name = "Steven", StartLocation = "New York", EndLocation = "Beijing" },
            new Reservation { Id = 2, Name = "John", StartLocation = "New Jersey", EndLocation = "Boston" },
            new Reservation { Id = 3, Name = "Martin", StartLocation = "London", EndLocation = "Paris" }
        }.ForEach(r => AddReservation(r));
    }

    public Reservation this[int id] => items.ContainsKey(id) ? items[id] : null!;

    public IEnumerable<Reservation> Reservations => items.Values;

    public Reservation AddReservation(Reservation reservation)
    {
        if (reservation.Id == 0)
        {
            reservation.Id = items.Count > 0 ? items.Keys.Max() + 1 : 1;
        }

        items[reservation.Id] = reservation;
        return reservation;
    }

    public Reservation UpdateReservation(Reservation reservation)
    {
        if (items.ContainsKey(reservation.Id))
        {
            items[reservation.Id] = reservation;
            return reservation;
        }

        return null!;
    }

    public void DeleteReservation(int id)
    {
        if (items.ContainsKey(id))
        {
            items.Remove(id);
        }
    }
}
