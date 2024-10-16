using Dapper;
using Fresh_Swimming.Models;
using MySqlConnector;
using ScottPlot.Colormaps;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Media;

namespace Fresh_Swimming.Helpers;

public static class Database
{
    private static readonly string MYSQL_CONNECTION_STRING = "Server=127.0.0.1;Database=fresh_swimming;Uid=root;Pwd=;";
    private static readonly SolidColorBrush ColorBrushRed = new(Colors.Red);
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
            MessageBoxResult result = MessageBox.Show("Failed to create user. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await CreateUserAsync(textBoxName, textBoxEmail, textBoxPhoneNumber, skill, colorPicker);
            }
            else
            {
                MessageBox.Show("User creation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task<ObservableCollection<User>> GetUsersAsync()
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM users";
            List<User> convert = (await dbConnection.QueryAsync<User>(sqlQuery)).ToList();
            ObservableCollection<User> users = new(convert);
            return users;
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to retrieve users. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await GetUsersAsync();
            }
            else
            {
                MessageBox.Show("User retrieval aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty collection if aborted
            }
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
            MessageBoxResult result = MessageBox.Show("Failed to check user. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await CheckUserAsync(name, email);
            }
            else
            {
                MessageBox.Show("User check aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }
    }

    public static async Task DeleteUserAsync(int id)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);

            dbConnection.Open();

            using IDbTransaction transaction = dbConnection.BeginTransaction();
            try
            {
                // Step 1: Delete dependent records from 'reservation' table
                string sqlQueryDeleteReservations = "DELETE FROM reservation WHERE FK_User = @Id";
                await dbConnection.ExecuteAsync(sqlQueryDeleteReservations, new { Id = id }, transaction);

                // Step 2: Delete the user record itself
                string sqlQueryDeleteLane = "DELETE FROM users WHERE ID = @Id";
                await dbConnection.ExecuteAsync(sqlQueryDeleteLane, new { Id = id }, transaction);

                // Commit the transaction if both operations succeed
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("An error occurred while trying to delete the users. Would you like to try again?",
                                         "Database Error",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteUserAsync(id);
            }
            else
            {
                MessageBox.Show("The user deletion was not completed.", "Deletion Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task UpdateUserAsync(int userId, string name, string email, string phoneNumber, byte skill, string color)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = @"
            UPDATE users 
            SET Name = @Name, Email = @Email, PhoneNumber = @PhoneNumber, Skill = @Skill, Color = @Color 
            WHERE ID = @UserId";

            await dbConnection.ExecuteAsync(sqlQuery, new { UserId = userId, Name = name, Email = email, PhoneNumber = phoneNumber, Skill = skill, Color = color });
        }
        catch
        {
            if (MessageBox.Show("Would you like to retry the update?", "Retry", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                await UpdateUserAsync(userId, name, email, phoneNumber, skill, color);
            }
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
            // Show message box and offer retry option
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await GetUserByIDAsync(userID);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return string.Empty; // Return an empty string or handle as needed to signify the operation failed
            }
        }
    }

    public static async Task<User> GetUserActualByIDAsync(int userID)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM users WHERE id = @Id";
            return await dbConnection.QueryFirstAsync<User>(sqlQuery, new { Id = userID });
        }
        catch
        {
            // Show message box and offer retry option
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await GetUserActualByIDAsync(userID);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return new();
            }
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
            MessageBoxResult result = MessageBox.Show("Failed to create lane. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await CreateLaneAsync(textBoxName, costPerHour, length, depth);
            }
            else
            {
                MessageBox.Show("Lane creation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task<ObservableCollection<Lane>> GetLanesAsync()
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM lane";
            List<Lane> convert = (await dbConnection.QueryAsync<Lane>(sqlQuery)).ToList();
            ObservableCollection<Lane> lanes = new(convert);
            return lanes;
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to retrieve lanes. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await GetLanesAsync();
            }
            else
            {
                MessageBox.Show("Lane retrieval aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty collection if aborted
            }
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
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await GetLaneIDByNameAsync(name);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return -1; // Return -1 or another sentinel value to indicate the operation was aborted or failed
            }
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
            MessageBoxResult result = MessageBox.Show("Failed to check lane. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await CheckLaneAsync(name);
            }
            else
            {
                MessageBox.Show("Lane check aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }
    }

    public static async Task UpdateLaneAsync(int laneId, string name, float costPerHour, float length, float depth)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = @"UPDATE lane SET Name = @Name, CostPerHour = @CostPerHour, Length = @Length, Depth = @Depth WHERE ID = @LaneId";
            await dbConnection.ExecuteAsync(sqlQuery, new
            {
                LaneId = laneId,
                Name = name,
                CostPerHour = costPerHour,
                Length = length,
                Depth = depth
            });
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to update lane. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await UpdateLaneAsync(laneId, name, costPerHour, length, depth); // Retry the operation
            }
            else
            {
                MessageBox.Show("Lane update aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task DeleteLaneAsync(int id)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);

            dbConnection.Open();

            using IDbTransaction transaction = dbConnection.BeginTransaction();
            try
            {
                // Step 1: Delete dependent records from 'reservation' table
                string sqlQueryDeleteReservations = "DELETE FROM reservation WHERE FK_Lane = @Id";
                await dbConnection.ExecuteAsync(sqlQueryDeleteReservations, new { Id = id }, transaction);

                // Step 2: Delete the lane record itself
                string sqlQueryDeleteLane = "DELETE FROM lane WHERE ID = @Id";
                await dbConnection.ExecuteAsync(sqlQueryDeleteLane, new { Id = id }, transaction);

                // Commit the transaction if both operations succeed
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("An error occurred while trying to delete the lane. Would you like to try again?",
                                         "Database Error",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteLaneAsync(id);
            }
            else
            {
                MessageBox.Show("The lane deletion was not completed.", "Deletion Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task<ObservableCollection<Holiday>> GetHolidaysAsync()
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT * FROM holidays";
            List<Holiday> convert = (await dbConnection.QueryAsync<Holiday>(sqlQuery)).ToList();
            ObservableCollection<Holiday> holidays = new(convert);
            return holidays;
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to retrieve holidays. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await GetHolidaysAsync();
            }
            else
            {
                MessageBox.Show("Holiday retrieval aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty collection if aborted
            }
        }
    }

    public static async Task<bool> CheckHolidayAsync(string name)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT COUNT(id) FROM holidays WHERE name = @Name";
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Name = name }) > 0;
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to check holiday. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await CheckHolidayAsync(name);
            }
            else
            {
                MessageBox.Show("Holiday check aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }
    }

    public static async Task CreateHolidayAsync(string textBoxName, bool? allowToEnterCheckBox, DateTime selectedDateCalendar, float resultPrice)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "INSERT INTO holidays (Name, Date, AllowToEnter, PricePerEntry) VALUES (@Name, @Date, @AllowToEnter, @PricePerEntry)";
            await dbConnection.ExecuteAsync(sqlQuery, new { Name = textBoxName, Date = selectedDateCalendar, AllowToEnter = allowToEnterCheckBox, PricePerEntry = resultPrice });
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to create holiday. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await CreateHolidayAsync(textBoxName, allowToEnterCheckBox, selectedDateCalendar, resultPrice);
            }
            else
            {
                MessageBox.Show("Holiday creation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task DeleteHolidayAsync(int id)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "DELETE FROM holidays WHERE ID = @Id";
            await dbConnection.ExecuteAsync(sqlQuery, new { ID = id });
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to delete holiday. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await DeleteHolidayAsync(id);
            }
            else
            {
                MessageBox.Show("Holiday deletion aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task UpdateHolidayAsync(int holidayID, string? name, DateTime date, byte allowToEnter, float pricePerEntry)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = @"UPDATE holidays SET Name = @Name, Date = @Date, AllowToEnter = @AllowToEnter, PricePerEntry = @PricePerEntry WHERE ID = @HolidayID";
            await dbConnection.ExecuteAsync(sqlQuery, new
            {
                HolidayID = holidayID,
                Name = name,
                Date = date,
                AllowToEnter = allowToEnter,
                PricePerEntry = pricePerEntry
            });
        }
        catch
        {
            MessageBoxResult result = MessageBox.Show("Failed to update holiday. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                await UpdateHolidayAsync(holidayID, name, date, allowToEnter, pricePerEntry); // Retry the operation
            }
            else
            {
                MessageBox.Show("Holiday update aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task<ObservableCollection<Reservation>> GetReservationsAsync(DateTime date)
    {
        try
        {
            DateTime dateTwo = new(date.Year, date.Month, date.Day);
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);

            // Get all lanes
            string sqlQuery = "SELECT * FROM lane";
            List<Lane> lanes = (await dbConnection.QueryAsync<Lane>(sqlQuery)).ToList();

            sqlQuery = "SELECT * FROM holidays WHERE Date = @Date";
            Holiday? holiday = await dbConnection.QueryFirstOrDefaultAsync<Holiday>(sqlQuery, new { Date = dateTwo });

            Dictionary<string, Reservation> hashMapReservations = [];
            ObservableCollection<Reservation> reservations = [];

            foreach (Lane lane in lanes)
            {
                // Get reservations for each lane
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
                    if (holiday != null)
                    {
                        string laneCost = holiday.AllowToEnter == 1 ? holiday.PricePerEntry + "$" : "N/A";

                        reservation = new Reservation
                        {
                            LaneName = laneName,
                            CostPerHour = laneCost,
                            IsHoliday = holiday.AllowToEnter == 0,
                            Length = lane.Length.ToString(),
                            Depth = lane.Depth.ToString()
                        };
                        if (holiday != null && holiday.AllowToEnter == 0)
                        {
                            reservation.Hours =
                            [
                                "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A",
                            ];
                            reservation.Colors =
                            [
                                ColorBrushRed, ColorBrushRed, ColorBrushRed, ColorBrushRed, ColorBrushRed,
                                ColorBrushRed, ColorBrushRed, ColorBrushRed, ColorBrushRed, ColorBrushRed,
                                ColorBrushRed, ColorBrushRed, ColorBrushRed
                            ];
                        }
                    }
                    else
                    {
                        reservation = new Reservation
                        {
                            LaneName = laneName,
                            CostPerHour = lane.CostPerHour + "$",
                            IsHoliday = false,
                            Length = lane.Length.ToString(),
                            Depth = lane.Depth.ToString()
                        };
                    }
                    hashMapReservations[laneName] = reservation;
                }


                foreach (dynamic item in listReservation)
                {
                    dynamic name = item.Name;
                    dynamic start = item.StartHour;
                    dynamic end = item.EndHour;

                    // Initialize Colors and ColorsForeground if null
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
                    if (reservation.Hours == null || reservation.Hours.Length == 0)
                    {
                        reservation.Hours = new string[13];
                    }
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

            // Add all reservations to the ObservableCollection
            foreach (Reservation r in hashMapReservations.Values)
            {
                reservations.Add(r);
            }

            return reservations;
        }
        catch
        {
            // Show message box and offer retry option
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);
            if (result == MessageBoxResult.Yes)
            {
                return await GetReservationsAsync(date);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty collection if aborted
            }
        }
    }

    public static async Task CreateReservationAsync(int userID, DateTime selectedDate, string laneName, int startHour, int endHour)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "INSERT INTO reservation (Date, StartHour, EndHour, FK_User, FK_Lane) VALUES (@Date, @StartHour, @EndHour, @ID_User, @ID_Lane)";
            int laneID = await GetLaneIDByNameAsync(laneName);
            await dbConnection.ExecuteAsync(sqlQuery, new { Date = selectedDate, StartHour = startHour, EndHour = endHour, ID_User = userID, ID_Lane = laneID });
        }
        catch
        {
            // Show message box and offer retry option
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                await CreateReservationAsync(userID, selectedDate, laneName, startHour, endHour);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public static async Task<bool> CheckReservationAvailabilityAsync(DateTime selectedDate, int startHour, int endHour, string laneName)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT COUNT(id) FROM reservation WHERE FK_Lane = @ID_Lane AND StartHour >= @StartHour AND EndHour <= @EndHour AND Date = @Date";
            int laneID = await GetLaneIDByNameAsync(laneName);
            return await dbConnection.QueryFirstAsync<int>(sqlQuery, new { Date = selectedDate, StartHour = startHour, EndHour = endHour, ID_Lane = laneID }) > 0;
        }
        catch
        {
            // Show message box and offer retry option
            MessageBoxResult result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CheckReservationAvailabilityAsync(selectedDate, startHour, endHour, laneName);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return false; // Return false to indicate the operation was not completed successfully
            }
        }
    }

    public static async Task<string> CalculateUsageAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;

            if (!showForAllDays)
                sqlQuery = "SELECT StartHour, EndHour FROM reservation WHERE Date = @Date";
            else
                sqlQuery = "SELECT StartHour, EndHour FROM reservation";

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
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CalculateUsageAsync(date, showForAllDays);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return "Operation aborted by user."; // Optional return message
            }
        }
    }

    public static async Task<string> CalculateAverageSkillAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;

            if (!showForAllDays)
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, users.Skill, users.ID FROM reservation JOIN users ON FK_User = users.ID WHERE reservation.Date = @Date";
            else
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, users.Skill, users.ID FROM reservation JOIN users ON FK_User = users.ID";

            HashSet<int> users = []; // Initialize the HashSet
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
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CalculateAverageSkillAsync(date, showForAllDays);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return "Operation aborted by user."; // Optional return message
            }
        }
    }

    public static async Task<List<Tuple<string, double>>> CalculateLaneUsageAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;

            if (!showForAllDays)
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Date = @Date";
            else
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID";

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
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CalculateLaneUsageAsync(date, showForAllDays);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty list if operation is aborted
            }
        }
    }

    public static async Task<string> CalculateAmountToPayAsync(int userID)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery = "SELECT lane.ID, lane.CostPerHour, reservation.StartHour, reservation.EndHour, reservation.Date FROM reservation LEFT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Paid = 0 AND reservation.FK_User = @UserID";
            IEnumerable<dynamic> paymentPerLane = await dbConnection.QueryAsync<dynamic>(sqlQuery, new { UserID = userID });
            sqlQuery = "SELECT * FROM holidays";
            IEnumerable<dynamic> holidays = await dbConnection.QueryAsync<dynamic>(sqlQuery);
            Dictionary<DateTime, float> holidaysPayment = [];
            HashSet<int> usedLanes = [];
            float amountToPay = 0.0f;
            int totalHours = 0;

            foreach (var holiday in holidays)
            {
                holidaysPayment[holiday.Date] = holiday.AllowToEnter ? holiday.PricePerEntry : 0;
            }

            foreach (dynamic item in paymentPerLane)
            {
                int hours = item.EndHour - item.StartHour + 1;
                totalHours += hours;
                if (!usedLanes.Contains(item.ID))
                {
                    usedLanes.Add(item.ID);
                }
                float cost = (holidaysPayment.ContainsKey(item.Date) ? holidaysPayment[item.Date] : item.CostPerHour) ?? 0;
                amountToPay += cost * hours;
            }

            if (totalHours == 0)
                return "The user has nothing to pay...";

            return $"User has used {usedLanes.Count} lanes, for a total of: {totalHours} hours, payment is: {amountToPay}$";
        }
        catch
        {
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CalculateAmountToPayAsync(userID);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return "Operation aborted by user."; // Return an abort message
            }
        }
    }

    public static async Task<List<Tuple<string, double>>> CalculateRentabilityOfLanesAsync(DateTime date, bool showForAllDays)
    {
        try
        {
            using IDbConnection dbConnection = new MySqlConnection(MYSQL_CONNECTION_STRING);
            string sqlQuery;

            if (!showForAllDays)
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name, lane.CostPerHour FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID WHERE reservation.Date = @Date";
            else
                sqlQuery = "SELECT reservation.StartHour, reservation.EndHour, lane.Name, lane.CostPerHour FROM reservation RIGHT JOIN lane ON reservation.FK_Lane = lane.ID";

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

            var rentabilities = profit
                .Select(kvp => new Rentability { Name = kvp.Key, Profit = kvp.Value })
                .OrderByDescending(r => r.Profit)
                .ToArray();

            return rentabilities.Select(r => new Tuple<string, double>(r.Name!, r.Profit)).ToList();
        }
        catch
        {
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                return await CalculateRentabilityOfLanesAsync(date, showForAllDays);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
                return []; // Return an empty list if operation is aborted
            }
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
            var result = MessageBox.Show("Looks like there is a problem connecting to the database. Would you like to try again?", "Database Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

            if (result == MessageBoxResult.Yes)
            {
                await ExecutePaymentAsync(userID);
            }
            else
            {
                MessageBox.Show("Operation aborted.", "Operation Aborted", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}