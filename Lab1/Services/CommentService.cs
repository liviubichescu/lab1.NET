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

        public IEnumerable<CommentGetModel> GetAllComments(string text)
        {
            IQueryable<CommentGetModel> result = dbContext.Comments.Select(x => new CommentGetModel() { 

                Id = x.Id,
                Text = x.Text,
                Important = x.Important,
                MovieId = (from movies in dbContext.Movies
                           where movies.Comments.Contains(x)
                           select movies.Id).FirstOrDefault()
            });

            if (text != null)
            {
                result = result.Where(comment => comment.Text.Contains(text));
            }

            return result;
        }
    }
}
