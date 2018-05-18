﻿// Copyright © 2018, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Tests.DbContextClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace MySql.Data.EntityFrameworkCore.Tests
{
  public class ModelingTests
  {
    public ModelingTests()
    {
      using (SakilaLiteContext context = new SakilaLiteContext())
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Populate
        context.PopulateData();
      }
    }

    [Fact]
    public void TableSplitting()
    {
      // Inserting data
      using (SakilaLiteTableSplittingContext context = new SakilaLiteTableSplittingContext())
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.Films.Add(new FilmLite
        {
          FilmId = 1,
          Title = "ACADEMY DINOSAUR",
          Description = "A Epic Drama of a Feminist And a Mad Scientist who must Battle a Teacher in The Canadian Rockies",
          Details = new FilmDetails
          {
            FilmId = 1,
            ReleaseYear = 2006,
            LanguageId = 1,
            OriginalLanguageId = null,
            RentalDuration = 6,
            RentalRate = 0.99m,
            Length = 86,
            ReplacementCost = 20.99m,
            Rating = "PG",
            SpecialFeatures = "Deleted Scenes,Behind the Scenes",
            LastUpdate = DateTime.Parse("2006-02-15 05:03:42")
          }
        });
        context.Films.Add(new FilmLite
        {
          FilmId = 2,
          Title = "ACE GOLDFINGER",
          Description = "A Astounding Epistle of a Database Administrator And a Explorer who must Find a Car in Ancient China"
        });
        context.FilmDetails.Add(new FilmDetails
        {
          FilmId = 2,
          ReleaseYear = 2006,
          LanguageId = 1,
          OriginalLanguageId = null,
          RentalDuration = 3,
          RentalRate = 4.99m,
          Length = 48,
          ReplacementCost = 12.99m,
          Rating = "G",
          SpecialFeatures = "Trailers,Deleted Scenes",
          LastUpdate = DateTime.Parse("2006-02-15 05:03:42")
        });

        context.SaveChanges();
      }

      // Validating data
      using (SakilaLiteTableSplittingContext context = new SakilaLiteTableSplittingContext())
      {
        var films = context.Films.Include(e => e.Details).ToList();

        Action<FilmLite, int, string, short> validate = (film, id, title, releaseYear) =>
        {
          Assert.Equal(id, film.FilmId);
          Assert.NotNull(film.Details);
          Assert.NotNull(film.Details.Film);
          Assert.Equal(id, film.Details.FilmId);
          Assert.Equal(title, film.Title);
          Assert.Equal(releaseYear, film.Details.ReleaseYear);
        };

        Assert.Collection(films,
          film => validate(film, 1, "ACADEMY DINOSAUR", 2006),
          film => validate(film, 2, "ACE GOLDFINGER", 2006)
        );
      }
    }
  }
}
