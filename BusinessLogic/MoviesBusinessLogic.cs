using DataLayer.DataContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic
{
    public class MoviesBusinessLogic
    {
        private readonly MovieDbContext _context;
        public MoviesBusinessLogic(MovieDbContext context) 
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies.ToListAsync();
        }

        public async Task<Movie> AddMovieAsync(Movie movie)
        {
            try
            {
                Movie m = _context.Movies.FirstOrDefault(x => x.id == movie.id);
                if(m != null) 
                {
                    return movie;
                }
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
             
                return movie;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<Movie> getMovieByIdAsync(int id)
        {
            try
            {
                return _context.Movies.FirstOrDefault(x => x.id == id);
            }
            catch(Exception ex)
            {
                return null;
                //throw ex;
            }
        }
    }
}
