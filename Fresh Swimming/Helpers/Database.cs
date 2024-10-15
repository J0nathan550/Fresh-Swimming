using Dapper;
using Fresh_Swimming.Models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Fresh_Swimming.Helpers;

public static class Database
{
    private static readonly string MYSQL_CONNECTION_STRING = "Server=127.0.0.1;Database=fresh_swimming;Uid=root;Pwd=;";
    private static readonly SolidColorBrush ColorBrushWhite = new(Colors.White);
    private static readonly SolidColorBrush ColorBrushBlack = new(Colors.Black);

    public static async Task CreateUserAsync(string textBoxName, string? textBoxEmail, string? textBoxPhoneNumber, byte skill, string colorPicker)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "INSERT INTO users (Name, PhoneNumber, Email, Skill, Color) VALUES (@Name, @PhoneNumber, @Email, @Skill, @Color)";
            await dbConnection.ExecuteAsync(sqlQuery, new { Name = textBoxName, PhoneNumber = textBoxPhoneNumber, Email = textBoxEmail, Skill = skill, Color = colorPicker });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await CreateUserAsync(textBoxName, textBoxEmail, textBoxPhoneNumber, skill, colorPicker);
        }
    }

    public static async Task CreateLaneAsync(string textBoxName, float costPerHour, float length, float depth)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "INSERT INTO lane (Name, CostPerHour, Length, Depth) VALUES (@Name, @CostPerHour, @Length, @Depth)";
            await dbConnection.ExecuteAsync(sqlQuery, new { Name = textBoxName, CostPerHour = costPerHour, Length = length, Depth = depth });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await CreateLaneAsync(textBoxName, costPerHour, length, depth);
        }
    }

    public static async Task<ObservableCollection<User>> GetUsersAsync()
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM users";
            List<User> convert = (await dbConnection.QueryAsync<User>(sqlQuery)).ToList();
            ObservableCollection<User> users = [.. convert];
            return users;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await GetUsersAsync();
        }
    }

    public static async Task<ObservableCollection<Lane>> GetLanesAsync()
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM lane";
            List<Lane> convert = (await dbConnection.QueryAsync<Lane>(sqlQuery)).ToList();
            ObservableCollection<Lane> lanes = [.. convert];
            return lanes;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await GetLanesAsync();
        }
    }

    public static async Task<bool> CheckUserAsync(string name, string? email)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT COUNT(id) FROM users WHERE name = @Name AND email = @Email";
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Name = name, Email = email }) > 0;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CheckUserAsync(name, email);
        }
    }

    public static async Task<bool> CheckLaneAsync(string name)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT COUNT(id) FROM lane WHERE name = @Name";
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Name = name }) > 0;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CheckLaneAsync(name);
        }
    }

    public static async Task<ObservableCollection<Reservation>> GetReservationsAsync(DateTime date)
    {
        try
        {
            DateTime dateTwo = new(date.Year, date.Month, date.Day);
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM lane";
            List<Lane> lanes = (await dbConnection.QueryAsync<Lane>(sqlQuery)).ToList();
            Dictionary<string, Reservation> hashMapReservations = [];
            ObservableCollection<Reservation> reservations = [];
            foreach (Lane lane in lanes)
            {
                sqlQuery = $"SELECT reservation.StartHour, reservation.EndHour, users.Color, users.Name FROM reservation INNER JOIN users ON users.ID = reservation.FK_User WHERE FK_Lane = @Lane AND Date = @Date";
                List<dynamic> listReservation = (await dbConnection.QueryAsync(sqlQuery, new { Lane = lane.ID, Date = dateTwo })).ToList();

                Reservation reservation;

                string laneName = lane.Name ?? "";
                if (hashMapReservations.TryGetValue(laneName, out Reservation? value))
                {
                    reservation = value;
                }
                else
                {
                    reservation = new Reservation
                    {
                        LaneName = laneName,
                        CostPerHour = lane.CostPerHour + "$"
                    };
                    hashMapReservations[laneName] = reservation;
                }

                foreach (dynamic item in listReservation)
                {
                    dynamic name = item.Name;
                    dynamic start = item.StartHour;
                    dynamic end = item.EndHour;
                    if (reservation.Colors == null || reservation.Colors.Length == 0)
                    {
                        reservation.Colors =
                        [
                            ColorBrushWhite, ColorBrushWhite, ColorBrushWhite, ColorBrushWhite, ColorBrushWhite,
                        ColorBrushWhite, ColorBrushWhite, ColorBrushWhite, ColorBrushWhite, ColorBrushWhite,
                        ColorBrushWhite, ColorBrushWhite, ColorBrushWhite
                        ];
                    }
                    if (reservation.ColorsForeground == null || reservation.ColorsForeground.Length == 0)
                    {
                        reservation.ColorsForeground =
                        [
                            ColorBrushBlack, ColorBrushBlack, ColorBrushBlack, ColorBrushBlack, ColorBrushBlack,
                        ColorBrushBlack, ColorBrushBlack, ColorBrushBlack, ColorBrushBlack, ColorBrushBlack,
                        ColorBrushBlack, ColorBrushBlack, ColorBrushBlack
                        ];
                    }
                    if (reservation.Hours == null || reservation.Hours.Length == 0) reservation.Hours = new string[13];
                    for (int i = start; i <= end; i++)
                    {
                        if (start <= i && end >= i)
                        {
                            reservation.Colors[i - 8] = ColorConverters.GetSolidColorBrush(item.Color);
                            reservation.ColorsForeground[i - 8] = ColorConverters.GetSaturationColorBrush(item.Color);
                            reservation.Hours[i - 8] = name;
                        }
                    }
                }
            }
            foreach (Reservation r in hashMapReservations.Values)
            {
                reservations.Add(r);
            }
            return reservations;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await GetReservationsAsync(date);
        }
    }

    public static async Task<string> GetUserByIDAsync(int userID)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT Name FROM users WHERE id = @Id";
            return await dbConnection.QueryFirstAsync<string>(sqlQuery, new { Id = userID });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await GetUserByIDAsync(userID);
        }
    }

    public static async Task<int> GetLaneIDByNameAsync(string name)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT Id FROM lane WHERE Name = @Name";
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Name = name });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await GetLaneIDByNameAsync(name);
        }
    }

    public static async Task CreateReservationAsync(int userID, DateTime selectedDate, string laneName, int startHour, int endHour)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "INSERT INTO reservation (Date, StartHour, EndHour, FK_User, FK_Lane) VALUES (@Date, @StartHour, @EndHour, @ID_User, @ID_Lane)";
            await dbConnection.ExecuteAsync(sqlQuery, new { Date = selectedDate, StartHour = startHour, EndHour = endHour, ID_User = userID, ID_Lane = await GetLaneIDByNameAsync(laneName) });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await CreateReservationAsync(userID, selectedDate, laneName, startHour, endHour);
        }
    }

    public static async Task<bool> CheckReservationAvailabilityAsync(DateTime selectedDate, int startHour, int endHour, string laneName)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT COUNT(id) FROM reservation WHERE FK_Lane = @ID_Lane AND StartHour >= @StartHour AND EndHour <= @EndHour AND Date = @Date";
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Date = selectedDate, StartHour = startHour, EndHour = endHour, ID_Lane = await GetLaneIDByNameAsync(laneName) }) > 0;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CheckReservationAvailabilityAsync(selectedDate, startHour, endHour, laneName);
        }
    }

    public static async Task<string> CalculateUsageAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;

            if (!showForAllDays) sqlQuery = "SELECT StartHour, EndHour FROM reservation WHERE Date = @Date";
            else sqlQuery = "SELECT StartHour, EndHour FROM reservation";

            IEnumerable<dynamic> query = await dbConnection.QueryAsync(sqlQuery, new { Date = date });
            int total = 0;
            foreach (dynamic item in query)
            {
                total += item.EndHour - item.StartHour + 1;
            }
            return $"Total number of hours {(showForAllDays ? "of all days" : "of the day")}: " + total;
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CalculateUsageAsync(date, showForAllDays);
        }
    }

    public static async Task<string> CalculateAverageSkillAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;
            if (!showForAllDays) sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, users.Skill, users.ID FROM reservation JOIN users ON FK_User = users.ID WHERE reservation.Date = @Date";
            else sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, users.Skill, users.ID FROM reservation JOIN users ON FK_User = users.ID";

            HashSet<int> users = [];
            IEnumerable<dynamic> query = await dbConnection.QueryAsync<dynamic>(sqlQuery, new { Date = date });

            int totalHours = 0;
            int weightedSkill = 0;

            foreach (dynamic item in query)
            {
                if (!users.Contains(item.ID))
                {
                    users.Add(item.ID);
                }

                int hours = item.EndHour - item.StartHour + 1;
                totalHours += hours;
                weightedSkill += hours * item.Skill;
            }

            if (totalHours == 0)
            {
                return "No usage for this date.";
            }

            int averageSkill = weightedSkill / totalHours;
            return $"Average skill for {users.Count} users is: " + SkillConverter.CalculateSkillAsString(averageSkill);
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CalculateAverageSkillAsync(date, showForAllDays);
        }
    }

    public static async Task<List<Tuple<string, double>>> CalculateLaneUsageAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;
            if (!showForAllDays) sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Date = @Date";
            else sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID";

            IEnumerable<dynamic> query = await dbConnection.QueryAsync<dynamic>(sqlQuery, new { Date = date });
            Dictionary<string, int> laneUsage = [];

            foreach (dynamic item in query)
            {
                int hours = item.EndHour - item.StartHour + 1;
                if (laneUsage.ContainsKey(item.Name))
                {
                    laneUsage[item.Name] += hours;
                }
                else
                {
                    laneUsage[item.Name] = hours;
                }
            }

            return laneUsage.Select(kvp => new Tuple<string, double>(kvp.Key, kvp.Value)).ToList();
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CalculateLaneUsageAsync(date, showForAllDays);
        }
    }

    public static async Task<string> CalculateAmountToPayAsync(int userID)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT lane.ID, lane.CostPerHour, reservation.StartHour, reservation.EndHour FROM reservation LEFT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Paid = 0 AND reservation.FK_User = @UserID";

            IEnumerable<dynamic> paymentPerLane = await dbConnection.QueryAsync<dynamic>(sqlQuery, new { UserID = userID });
            HashSet<int> usedLanes = [];
            float amountToPay = 0.0f;
            int totalHours = 0;

            foreach (dynamic item in paymentPerLane)
            {
                int hours = item.EndHour - item.StartHour + 1;
                totalHours += hours;
                if (!usedLanes.Contains(item.ID))
                {
                    usedLanes.Add(item.ID);
                }
                amountToPay += item.CostPerHour * hours;
            }

            if (totalHours == 0)
                return "The user has nothing to pay...";

            return $"User has used {usedLanes.Count} lanes, for a total of: {totalHours} hours, payment is: {amountToPay}$";
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CalculateAmountToPayAsync(userID);
        }
    }

    public static async Task<List<Tuple<string, double>>> CalculateRentabilityOfLanesAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;
            if (!showForAllDays) sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name, lane.CostPerHour FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Date = @Date";
            else sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name, lane.CostPerHour FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID";

            IEnumerable<dynamic> query = await dbConnection.QueryAsync<dynamic>(sqlQuery, new { Date = date });
            Dictionary<string, float> profit = [];

            foreach (dynamic item in query)
            {
                float profitForLane = (item.EndHour - item.StartHour + 1) * item.CostPerHour;
                if (profit.ContainsKey(item.Name))
                {
                    profit[item.Name] += profitForLane;
                }
                else
                {
                    profit[item.Name] = profitForLane;
                }
            }

            Rentability[] rentabilities = [.. profit
            .Select(kvp => new Rentability { Name = kvp.Key, Profit = kvp.Value })
            .OrderByDescending(r => r.Profit)];

            return rentabilities.Select(r => new Tuple<string, double>(r.Name!, r.Profit)).ToList();
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return await CalculateRentabilityOfLanesAsync(date, showForAllDays);
        }
    }

    public static async Task ExecutePaymentAsync(int userID)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "UPDATE reservation SET Paid = 1 WHERE FK_User = @UserID AND Paid = 0";
            await dbConnection.ExecuteAsync(sqlQuery, new { UserID = userID });
        }
        catch
        {
            MessageBox.Show("Looks like there is a problem to connecting to database, click OK to try again.", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            await ExecutePaymentAsync(userID);
        }
    }
}