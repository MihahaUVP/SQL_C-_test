using System;
using Microsoft.Data.SqlClient;
namespace pr3_1_4
{
    class Program
    {
        static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=MinionsDB;Trusted_Connection=True";
        static void Main(string[] args)
        {
            //string minionsInfo;
            string minionsName;
            int age;
            string town;
            string villainsName;
            string input = Console.ReadLine();
            string[] split_arr;
            split_arr = input.Split(" ");
            minionsName = split_arr[1];
            age = Convert.ToInt32(split_arr[2]);
            town = split_arr[3];
            input = Console.ReadLine();
            split_arr = input.Split(" ");
            villainsName = split_arr[1];
            int townId = CheckTown(town);

            if(townId == 0)
            {
                townId = InsertTown(town);
                Console.WriteLine("Город " +town+" был успешно добавлен в базу данных.");
            }
            int villainId;
            villainId = CheckVillain(villainsName);
            if(villainId == 0)
            {
                villainId = InsertVillain(villainsName);
                Console.WriteLine("Злодей " + villainsName +" был успешно добавлен в базу данных");
            }
            //проверка на злодея(функция) или фикция
            //добавление миньонов
            AddMinion(minionsName,age,townId,villainId);
            Console.WriteLine("Успешно добавлен " + minionsName+" чтобы быть миньоном "+ villainsName);
        }
        static int CheckTown(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectTownString = "SELECT Id FROM Towns WHERE Name =@name";
                SqlCommand command = new SqlCommand(selectTownString, connection);
                command.Parameters.AddWithValue("@name", name);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader[0].ToString());
                    }
                }
                return 0;///временно!!! ne vremenno!
            }    
        }
        static int InsertTown(string name)
        {
            int id = 0;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertTownString = "INSERT INTO Towns " +
                    "(Name) VALUES (@name)";
                SqlCommand command = new SqlCommand(insertTownString, connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
                id = CheckTown(name);
            }
            return id;
        }
        static int CheckVillain(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectVillainsString = "SELECT Id FROM Villains WHERE Name =@name";
                SqlCommand command = new SqlCommand(selectVillainsString, connection);
                command.Parameters.AddWithValue("@name", name);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    if (reader.Read())
                    {
                        return Convert.ToInt32(reader[0].ToString());
                    }
                }
            }
            return 0;
        }
        static int InsertVillain(string name)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            int id = 0;
            using (connection)
            {
                string insertVillainsString = "INSERT INTO Villains " +
                    "(Name, EvilnessFactorId) VALUES (@name, 2)";
                SqlCommand command = new SqlCommand(insertVillainsString, connection);
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
                id = CheckVillain(name);
             }
            return id;
        }
        static void AddMinion(string name,int age,int townId,int villainId)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string insertMinionString = "INSERT INTO Minions " +
                    "(Name, Age, TownId) VALUES (@name,@age,@townId)";
                SqlCommand command = new SqlCommand(insertMinionString, connection);
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@age",age);
                command.Parameters.AddWithValue("@townId", townId);
                command.ExecuteNonQuery();
                string selectMinionIdString = "SELECT Id FROM Minions WHERE Name = @name";
                SqlCommand command2 = new SqlCommand(selectMinionIdString, connection);
                command2.Parameters.AddWithValue("@name", name);
                SqlDataReader reader = command2.ExecuteReader();
                int minionId;
                using (reader)
                {
                    reader.Read();
                    minionId =Convert.ToInt32((reader[0].ToString()));
                }
                // вставка информации о связи миньона и злодея
                string insertMinionsVillainsString= "INSERT INTO MinionsVillains " +
                    "(MinionId, VillainId) VALUES (@minionId,@villainId)";
                SqlCommand command3 = new SqlCommand(insertMinionsVillainsString,connection);
                command3.Parameters.AddWithValue("@minionId", minionId);
                command3.Parameters.AddWithValue("@VillainId", villainId);
                command3.ExecuteNonQuery();
            }
        }
    }
}
