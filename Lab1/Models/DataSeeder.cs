﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Models
{
    public class DataSeeder
    {
        public static void Initialize(MovieDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Movies.Any())
            {
                return;   // DB has been seeded
            }

            context.Movies.AddRange(
                new Movie
                {
                    Title = "Avengers",
                    Rating = 9,

                },
                new Movie
                {
                    Title = "Captain Marvel",
                    Rating = 7,

                });
            context.SaveChanges();
        }

    }
}
