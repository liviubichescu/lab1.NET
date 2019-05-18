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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<MovieGetModel> GetAllMovies(DateTime? from = null, DateTime? to = null);
        Movie GetById(int id);
        Movie Create(MoviePostModel movie);
        MoviePostModel Upsert(int id, MoviePostModel movie);
        Movie Delete(int id);
    }

    public class MovieService : IMovieService
    {
        private MovieDbContext dbContext;

        public MovieService(MovieDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Movie Create(MoviePostModel movieDTO)
        {
            Movie addMovie = MoviePostModel.ToMovie(movieDTO);
            dbContext.Movies.Add(addMovie);
            dbContext.SaveChanges();
            return addMovie;
        }

        public Movie Delete(int id)
        {
            Movie movie = dbContext.Movies.Find(id);
            if (movie == null)
            {
                return null;
            }
            dbContext.Remove(movie);
            dbContext.SaveChanges();

            return movie;
        }

        public IEnumerable<MovieGetModel> GetAllMovies(DateTime? from = null, DateTime? to = null)
        {
            IQueryable<Movie> result = dbContext.Movies.Include(f => f.Comments).OrderByDescending(f => f.ReleseYear);

            if (from == null && to == null)
            {
                return result.Select(movie => MovieGetModel.FromMovie(movie));
            }
            if (from != null)
            {
                result = result.Where(f => f.DateAdded > from);
            }
            if (to != null)
            {
                result = result.Where(f => f.DateAdded < to);
            }


            return result.Select(movie => MovieGetModel.FromMovie(movie));
        }

        public Movie GetById(int id)
        {
            return dbContext.Movies.Include(c=> c.Comments).FirstOrDefault(m => m.Id == id);
        }

        public MoviePostModel Upsert(int id, MoviePostModel movie)
        {
            var existing = dbContext.Movies.FirstOrDefault(c => c.Id == id);

            if (existing == null)
            {
                dbContext.Movies.Add(MoviePostModel.ToMovie(movie));
                dbContext.SaveChanges();
                return movie;
            }

            dbContext.Movies.Update(MoviePostModel.ToMovie(movie));
            dbContext.SaveChanges();

            return movie;
        }
    }
}
