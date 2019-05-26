using Lab1.Models;
using Lab1.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Services
{
    public interface IMovieService
    {   
        IEnumerable<MovieGetModel> GetAllMovies(DateTime? from = null, DateTime? to = null);
        Movie GetById(int id);
        Movie Create(MoviePostModel movie);
        Movie Upsert(int id, Movie movie);
        Movie Delete(int id);
    }

    public class MovieService : IMovieService
    {
        private DatabaseContext Context;

        public MovieService(DatabaseContext Context)
        {
            this.Context = Context;
        }

        public Movie Create(MoviePostModel movie)
        {
            Movie convertedMovie = MoviePostModel.ToMovie(movie);
            Context.Movies.Add(convertedMovie);
            Context.SaveChanges();
            return convertedMovie;
        }

        public Movie Delete(int id)
        {
            Movie foundMovie = Context.Movies.Include(comment => comment.Comments).FirstOrDefault(movie => movie.Id == id);
            if (foundMovie == null)
            {
                return null;
            }
            Context.Remove(foundMovie);
            Context.SaveChanges();

            return foundMovie;
        }

        public IEnumerable<MovieGetModel> GetAllMovies(DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Movie> result = Context.Movies.Include(movie => movie.Comments).OrderByDescending(movie => movie.ReleseYear);

            if (from == null && to == null)
            {
                return result.Select(movie => MovieGetModel.FromMovie(movie));
            }
            if (from != null)
            {
                result = result.Where(movie => movie.DateAdded > from);
            }
            if (to != null)
            {
                result = result.Where(movie => movie.DateAdded < to);
            }


            return result.Select(movie => MovieGetModel.FromMovie(movie));
        }

        public Movie GetById(int id)
        {
            return Context.Movies.Include(movie => movie.Comments).FirstOrDefault(movie => movie.Id == id);
        }

        public Movie Upsert(int id, Movie movie)
        {
            var existing = Context.Movies.AsNoTracking().FirstOrDefault(c => c.Id == id);

            if (existing == null)
            {
                Context.Movies.Add(movie);
                Context.SaveChanges();
                return movie;
            }
            movie.Id = id;
            Context.Movies.Update(movie);
            Context.SaveChanges();

            return movie;
        }
    }
}
