using AltV.Net;
using AltV.Net.Data;
using AltVReallifeScript.Classes;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Structs;
using AltVReallifeScript.Utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AltVReallifeScript.Database
{
    public class PlayerCharacterData : IScript
    {


        public static void RegisterNewCharacter(PlayerCharacter character)
        {

            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO characters (ownerid, firstname, lastname, birthday, health, armor, cash, gender, position, appearence) VALUES (@ownerid, @firstname, @lastname, @birthday, @health, @armor, @cash, @gender, @position, @appearence); SELECT LAST_INSERT_ID();";
                    command.Parameters.AddWithValue("@ownerid", character.ownerId);
                    command.Parameters.AddWithValue("@firstname", character.firstname);
                    command.Parameters.AddWithValue("@lastname", character.lastname);
                    command.Parameters.AddWithValue("@birthday", character.birthday.ToString("dd-MM-yyyy"));
                    command.Parameters.AddWithValue("@health", 100);
                    command.Parameters.AddWithValue("@armor", 100);
                    command.Parameters.AddWithValue("@cash", character.cash);
                    command.Parameters.AddWithValue("@gender", (int)character.gender);
                    command.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(character.pos));
                    command.Parameters.AddWithValue("@appearence", JsonConvert.SerializeObject(character.bodyappearence));
                    character.DBId = (uint)(ulong)command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} RegisterNewCharacter: {e.Message}");
                    Alt.Log($"{Variables.Prefix} RegisterNewCharacter: {e.StackTrace}");
                }
            }
        }

        public static void LoadCharacter(PlayerCharacter character, int CharacterId)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM characters WHERE id=@id LIMIT 1";
                    command.Parameters.AddWithValue("@id", CharacterId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            character.DBId = reader.GetUInt32("id");
                            character.firstname = reader.GetString("firstname");
                            character.lastname = reader.GetString("lastname");
                            character.birthday = DateTime.ParseExact(reader.GetString("birthday"), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            character.health = reader.GetUInt16("health");
                            character.armor = reader.GetUInt16("armor");
                            character.cash = reader.GetUInt64("cash");
                            if (!reader.IsDBNull(reader.GetOrdinal("appearence")))
                                character.bodyappearence = JsonConvert.DeserializeObject<BodyAppearence>(reader.GetString("appearence"));
                            if (!reader.IsDBNull(reader.GetOrdinal("position")))
                                character.pos = JsonConvert.DeserializeObject<Position>(reader.GetString("position"));
                            else
                                character.pos = new Position();
                            character.gender = (Enums.Genders)reader.GetUInt64("gender");
                        }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} LoadCharacter: {e.Message}");
                    Alt.Log($"{Variables.Prefix} LoadCharacter: {e.StackTrace}");
                }
            }
        }

        public static void DeleteCharacter(int CharacterId)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {

                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "DELETE FROM characters WHERE id=@id";
                    command.Parameters.AddWithValue("@id", CharacterId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} DeleteCharacter: {e.Message}");
                    Alt.Log($"{Variables.Prefix} DeleteCharacter: {e.StackTrace}");
                }
            }
        }

        public static void UpdateCharacter(PlayerCharacter character)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "UPDATE characters SET health=@health, armor=@armor, cash=@cash, position=@position, appearence=@appearence WHERE id=@id";
                    command.Parameters.AddWithValue("@id", character.DBId);
                    command.Parameters.AddWithValue("@health", character.health);
                    command.Parameters.AddWithValue("@armor", character.armor);
                    command.Parameters.AddWithValue("@cash", character.cash);
                    command.Parameters.AddWithValue("@position", JsonConvert.SerializeObject(character.pos));
                    command.Parameters.AddWithValue("@appearence", JsonConvert.SerializeObject(character.bodyappearence));
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} UpdateAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} UpdateAccount: {e.StackTrace}");
                }
            }
        }

        public static List<PlayerCharacter> GetAllCharactersFromPlayer(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM characters WHERE ownerid=@ownerid";
                    command.Parameters.AddWithValue("@ownerid", player.DBId);
                    List<PlayerCharacter> chars = new List<PlayerCharacter>();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlayerCharacter newchar = new PlayerCharacter();
                            newchar.DBId = reader.GetUInt32("id");
                            newchar.ownerId = reader.GetUInt32("ownerid");
                            newchar.firstname = reader.GetString("firstname");
                            newchar.lastname = reader.GetString("lastname");
                            newchar.birthday = DateTime.ParseExact(reader.GetString("birthday"), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            newchar.health = reader.GetUInt16("health");
                            newchar.armor = reader.GetUInt16("armor");
                            newchar.cash = reader.GetUInt32("cash");
                            if (!reader.IsDBNull(reader.GetOrdinal("appearence")))
                                newchar.bodyappearence = JsonConvert.DeserializeObject<BodyAppearence>(reader.GetString("appearence"));
                            if (!reader.IsDBNull(reader.GetOrdinal("position")))
                                newchar.pos = JsonConvert.DeserializeObject<Position>(reader.GetString("position"));
                            else
                                newchar.pos = new Position();
                            newchar.gender = (Enums.Genders)reader.GetUInt64("gender");
                            chars.Add(newchar);
                        }
                        reader.Close();
                    }
                    connection.Close();
                    return chars;
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} GetAllCharactersFromPlayer: {e.Message}");
                    Alt.Log($"{Variables.Prefix} GetAllCharactersFromPlayer: {e.StackTrace}");
                }
                return null;
            }
        }

        public static int CountAllCharactersFromPlayer(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM characters WHERE ownerid=@ownerid";
                    command.Parameters.AddWithValue("@ownerid", player.DBId);
                    int i = 0;
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            i++;
                        }
                        reader.Close();
                    }
                    connection.Close();
                    return i;
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} CountAllCharactersFromPlayer: {e.Message}");
                    Alt.Log($"{Variables.Prefix} CountAllCharactersFromPlayer: {e.StackTrace}");
                }
                return 0;
            }
        }
    }
}
