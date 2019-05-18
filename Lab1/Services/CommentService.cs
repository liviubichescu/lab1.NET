using Lab1.Models;
using Lab1.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Services
{
    public interface ICommentService
    {

        IEnumerable<CommentGetModel> GetAllComments(string filter);

    }

    public class CommentService : ICommentService
    {
        private MovieDbContext dbContext;

        public CommentService(MovieDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<CommentGetModel> GetAllComments(string filter)
        {

            bool filterComment = true;
            if (string.IsNullOrEmpty(filter))
                filterComment = false;

            var qry = GetCommentAndMovie().Where(com => filterComment ? com.Text.Contains(filter) : true
           );

            return qry.ToList();

        }


        private IQueryable<CommentGetModel> GetCommentAndMovie()
        {
            var commentAndMovie = from comment in dbContext.Comments
                                  join movie in dbContext.Movies
                                  on comment.MovieId equals movie.Id
                                  select new CommentGetModel()
                                  {
                                      Id = comment.Id,
                                      Text = comment.Text,
                                      Important = comment.Important,
                                      MovieId = comment.MovieId
                                  };

            return dbContext.Comments.Select(comment => CommentGetModel.FromComment(comment));
        }
    }
}
