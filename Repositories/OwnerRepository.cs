﻿using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;

        // The constructor accepts an IConfiguration object as a parameter. This class comes from the ASP.NET framework and is useful for retrieving things out of the appsettings.json file like connection strings.
        public OwnerRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Email, Name AS OwnerName, Address, NeighborhoodId, Phone 
                        FROM Owner";



                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new List<Owner>();
                        while (reader.Read())
                        {
                            Owner owner = new Owner
                            {

                                Id = reader.GetInt32(reader.GetOrdinal("Id")),


                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone"))
                            };

                            owners.Add(owner);
                        }

                        return owners;
                    }
                }
            }
        }

        public Owner GetOwnerById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                           SELECT o.Id, o.Email, o.Name AS OwnerName, o.Address, o.NeighborhoodId, o.Phone, d.Name AS DogName, d.Id AS DogId
                        FROM Owner o
                        LEFT JOIN Dog d ON d.OwnerId = o.Id
                        WHERE o.Id = @Id";


                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Owner owner = null;
                        while (reader.Read())
                        {



                            if (owner == null)
                            {
                                owner = new Owner
                                {

                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    Name = reader.GetString(reader.GetOrdinal("OwnerName")),
                                    Dogs = new List<Dog>(),
                                    Address = reader.GetString(reader.GetOrdinal("Address")),
                                    NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                    Phone = reader.GetString(reader.GetOrdinal("Phone"))
                                };

                            }
                            if (!reader.IsDBNull(reader.GetOrdinal("DogId")))
                            {
                                owner.Dogs.Add(new Dog
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),

                                });

                            }
                        }
                        return owner;


                    }
                }
            }
        }


        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], Email, Address, Phone, NeighborhoodId
                        FROM Owner
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Owner owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId"))
                            };

                            return owner;
                        }

                        return null;
                    }
                }
            }
        }

            public void AddOwner(Owner owner)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID 
                    
                    
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId)
                ";

                        cmd.Parameters.AddWithValue("@name", owner.Name);
                        cmd.Parameters.AddWithValue("@email", owner.Email);
                        cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                        cmd.Parameters.AddWithValue("@address", owner.Address);
                        cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                        int id = (int)cmd.ExecuteScalar();
                        

                        owner.Id = id;
                        
                    }
                }
            }

            public void UpdateOwner(Owner owner)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                        cmd.Parameters.AddWithValue("@name", owner.Name);
                        cmd.Parameters.AddWithValue("@email", owner.Email);
                        cmd.Parameters.AddWithValue("@address", owner.Address);
                        cmd.Parameters.AddWithValue("@phone", owner.Phone);
                        cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                        cmd.Parameters.AddWithValue("@id", owner.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            public void DeleteOwner(int ownerId)
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                        cmd.Parameters.AddWithValue("@id", ownerId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }




