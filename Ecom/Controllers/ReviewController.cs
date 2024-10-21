using Dapper;
using Ecom.Data;
using Ecom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Ecom.Controllers
{
    [ApiController]
    [Route("/Api/[controller]")]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ReviewController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("AddReview")]
        public async Task<ActionResult<List<Reviews>>> AddReview(ReviewInputModel reviewInput)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                string query = "Insert into Reviews (EmailId, ProductId, Rating, Review, Name) values (@email, @pid, @rating, @review, @name)";
                await conn.QueryAsync(query, new { email = reviewInput.EmailId, pid = reviewInput.ProductId, rating = reviewInput.Rating, review = reviewInput.Review, name = reviewInput.Name });
                int prevUsersCount = await conn.QueryFirstOrDefaultAsync<int>("select ratedUsers from Products where Id = @pid", new { pid = reviewInput.ProductId });
                var prevRating = await conn.QueryFirstOrDefaultAsync<decimal>("select rating from Products where Id = @pid", new { pid = reviewInput.ProductId });

                decimal newRating = (prevRating * prevUsersCount + reviewInput.Rating.GetValueOrDefault()) / (prevUsersCount + 1);
                string ratingQuery = "update Products set rating = @newrating, ratedUsers = @newRatedUsers where Id = @pid";
                await conn.QueryAsync(ratingQuery, new { newrating = newRating, newRatedUsers = prevUsersCount + 1, pid = reviewInput.ProductId});
                List<Reviews> result = (List<Reviews>)await conn.QueryAsync<Reviews>("select * from Reviews where ProductId = @pid", new { email = reviewInput.EmailId, pid = reviewInput.ProductId });
                return Ok(result);
            }
        }

        [HttpPost("DeleteReview")]
        public async Task<ActionResult<List<Reviews>>> DeleteReview (ReviewInputModel ReviewInput)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                await conn.QueryAsync("delete from Reviews where id = @Id", new {Id = ReviewInput.ReviewID});
                List<Reviews> result = (List<Reviews>)await conn.QueryAsync<Reviews>("select * from Reviews where ProductId = @pid", new { email = ReviewInput.EmailId, pid = ReviewInput.ProductId });
                return Ok(result);
            }
        }


        [HttpPost("GetReviews")]
        public async Task<ActionResult<List<Reviews>>> GetReviews(ReviewInputModel ReviewInput)
        {
            using (var conn = new SqlConnection(_config.GetConnectionString("DefaultConnectionString")))
            {
                await conn.OpenAsync();
                List<Reviews> result = (List<Reviews>)await conn.QueryAsync<Reviews>("select * from Reviews where ProductId = @pid", new {pid = ReviewInput.ProductId });
                return Ok(result);
            }
        }
    }
}
