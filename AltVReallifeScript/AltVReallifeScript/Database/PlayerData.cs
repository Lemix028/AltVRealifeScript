using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltVReallifeScript.Entitys;
using AltVReallifeScript.Structs;
using AltVReallifeScript.Utils;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AltVReallifeScript.Database
{
    public class PlayerData : IScript
    {
        public static void RegisterPlayer(MyPlayer player, string password)
        {
            string saltedPassword = PasswordDerivation.Derive(password);
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO users (username, password, adminlevel, socialclub) VALUES (@username, @password, @adminlevel, @socialclub); SELECT LAST_INSERT_ID();";
                    command.Parameters.AddWithValue("@username", player.Username);
                    command.Parameters.AddWithValue("@password", saltedPassword);
                    command.Parameters.AddWithValue("@adminlevel", player.AdminLevel);
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);

                    player.DBId = (uint)(ulong)command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} RegisterAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} RegisterAccount: {e.StackTrace}");
                }
            }
        }
        public static void RegisterDiscordPlayer(MyPlayer player)
        {


            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "INSERT INTO users (username, discord_id, discord_avatar, discord_discriminator, adminlevel, socialclub, discord_auth_object) VALUES (@username, @discord_id, @discord_avatar, @discord_discriminator, @adminlevel, @socialclub, @discord_auth_object); SELECT LAST_INSERT_ID();";
                    command.Parameters.AddWithValue("@username", player.Username);
                    command.Parameters.AddWithValue("@discord_id", player.DiscordId);
                    command.Parameters.AddWithValue("@discord_avatar", player.DiscordAvatar);
                    command.Parameters.AddWithValue("@discord_discriminator", player.DiscordId);
                    command.Parameters.AddWithValue("@adminlevel", player.AdminLevel);
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);
                    command.Parameters.AddWithValue("@discord_auth_object", JsonConvert.SerializeObject(player.DiscordAuth));
                    player.DBId = (uint)(ulong)command.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} RegisterAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} RegisterAccount: {e.StackTrace}");
                }
            }
        }
        public static void LoadPlayer(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE username=@username LIMIT 1";
                    command.Parameters.AddWithValue("@username", player.Username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            player.AdminLevel = reader.GetInt16("adminlevel");
                            player.DBId = reader.GetUInt32("id");
                            if (reader.IsDBNull(reader.GetOrdinal("discord_auth_object")))
                                player.DiscordAuth = JsonConvert.DeserializeObject<DiscordAuthObject>(reader.GetString("discord_auth_object"));
                            if (reader.IsDBNull(reader.GetOrdinal("discord_id")))
                                player.DiscordId = reader.GetString("discord_id");
                            if (reader.IsDBNull(reader.GetOrdinal("discord_avatar")))
                                player.DiscordAvatar = reader.GetString("discord_avatar");
                            if (reader.IsDBNull(reader.GetOrdinal("discord_discriminator")))
                                player.DiscordDiscriminator = reader.GetString("discord_discriminator");
                        }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} LoadPlayer: {e.Message}");
                    Alt.Log($"{Variables.Prefix} LoadPlayer: {e.StackTrace}");
                }
            }
        }

        public static void LoadPlayerSocialClub(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE socialclub=@socialclub LIMIT 1";
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            player.AdminLevel = reader.GetInt16("adminlevel");
                            player.DBId = reader.GetUInt32("id");
                            if (!reader.IsDBNull(reader.GetOrdinal("discord_auth_object")))
                                player.DiscordAuth = JsonConvert.DeserializeObject<DiscordAuthObject>(reader.GetString("discord_auth_object"));
                            if (!reader.IsDBNull(reader.GetOrdinal("discord_id")))
                                player.DiscordId = reader.GetString("discord_id");
                            if (!reader.IsDBNull(reader.GetOrdinal("discord_avatar")))
                                player.DiscordAvatar = reader.GetString("discord_avatar");
                            if (!reader.IsDBNull(reader.GetOrdinal("discord_discriminator")))
                                player.DiscordDiscriminator = reader.GetString("discord_discriminator");
                        }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} LoadPlayerSocialClub: {e.Message}");
                    Alt.Log($"{Variables.Prefix} LoadPlayerSocialClub: {e.StackTrace}");
                }
            }
        }

        public static Dictionary<ulong, DiscordAuthObject> LoadAllDiscordAuthenfications()
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                Dictionary<ulong, DiscordAuthObject> ret = new Dictionary<ulong, DiscordAuthObject>();
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE discord_auth_object IS NOT NULL";

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (!reader.IsDBNull(reader.GetOrdinal("discord_auth_object")))
                            {
                                ret.Add(reader.GetUInt64("socialclub"), JsonConvert.DeserializeObject<DiscordAuthObject>(reader.GetString("discord_auth_object")));
                            }
                                 

                        }
                    }
                    connection.Close();
                    return ret;
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} LoadPlayerSocialClub: {e.Message}");
                    Alt.Log($"{Variables.Prefix} LoadPlayerSocialClub: {e.StackTrace}");
                }
            }
            return null;
        }
        public static void UpdatePlayer(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "UPDATE users SET adminlevel=@adminlevel WHERE username=@username";
                    command.Parameters.AddWithValue("@username", player.Username);
                    command.Parameters.AddWithValue("@adminlevel", player.AdminLevel);

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

        public static void UpdateDiscordData(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "UPDATE users SET username=@username, discord_id=@discord_id, discord_avatar=@discord_avatar, discord_discriminator=@discord_discriminator, discord_auth_object=@discord_auth_object WHERE socialclub=@socialclub";
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);
                    command.Parameters.AddWithValue("@adminlevel", player.AdminLevel);
                    command.Parameters.AddWithValue("@discord_id", player.DiscordId);
                    command.Parameters.AddWithValue("@discord_avatar", player.DiscordAvatar);
                    command.Parameters.AddWithValue("@discord_discriminator", player.DiscordDiscriminator);
                    command.Parameters.AddWithValue("@discord_auth_object", JsonConvert.SerializeObject(player.DiscordAuth));
                    command.Parameters.AddWithValue("@username", player.Username);

                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} UpdateDiscordData: {e.Message}");
                    Alt.Log($"{Variables.Prefix} UpdateDiscordData: {e.StackTrace}");
                }
            }
        }
        public static void ClearDiscordData(MyPlayer player)
        {
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "UPDATE users SET username=@username, discord_id=@discord_id, discord_avatar=@discord_avatar, discord_discriminator=@discord_discriminator, discord_auth_object=@discord_auth_object WHERE socialclub=@socialclub";
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);
                    command.Parameters.AddWithValue("@discord_id", null);
                    command.Parameters.AddWithValue("@discord_avatar", null);
                    command.Parameters.AddWithValue("@discord_discriminator", null);
                    command.Parameters.AddWithValue("@discord_auth_object", null);
                    command.Parameters.AddWithValue("@username", "");

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
        public static bool CheckPassword(string username, string input)
        {
            string password = "";
            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT password FROM users WHERE username=@username LIMIT 1";
                    command.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (!reader.IsDBNull(reader.GetOrdinal("password")))
                            {
                                password = reader.GetString("password");
                            }
                            
                        }
                    }
                    connection.Close();
                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} CheckPwAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} CheckPwAccount: {e.StackTrace}");
                }
            }
            if (password == "" || input == "")
                return false;
            if (PasswordDerivation.Verify(password, input))
                return true;
            else
                return false;
        }
        public static bool DoesPlayerUsernameExists(string username)
        {

            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE username=@username LIMIT 1";
                    command.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} DPlayerNameExistsAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} DPlayerNameExistsAccount: {e.StackTrace}");
                }
            }
            return false;

        }
        public static bool DoesDiscordPlayerExists(string discordId)
        {

            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE discord_id=@discord_id LIMIT 1";
                    command.Parameters.AddWithValue("@discord_id", discordId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} DPlayerNameExistsAccount: {e.Message}");
                    Alt.Log($"{Variables.Prefix} DPlayerNameExistsAccount: {e.StackTrace}");
                }
            }
            return false;

        }

        public static bool DoesSocialClubUserExists(MyPlayer player)
        {

            using (MySqlConnection connection = new MySqlConnection(Variables.MySqlConnectionString))
            {
                try
                {
                    connection.Open();
                    MySqlCommand command = connection.CreateCommand();

                    command.CommandText = "SELECT * FROM users WHERE socialclub=@socialclub LIMIT 1";
                    command.Parameters.AddWithValue("@socialclub", player.SocialClubId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            connection.Close();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
                catch (Exception e)
                {
                    Alt.Log($"{Variables.Prefix} DoesSocialClubUserExists: {e.Message}");
                    Alt.Log($"{Variables.Prefix} DoesSocialClubUserExists: {e.StackTrace}");
                }
            }
            return false;

        }


    }
}
