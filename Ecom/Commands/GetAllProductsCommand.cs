namespace Ecom.Commands
{
    public interface GetAllProductsCommand
    {
        int CorrelationId { get; }
        int UserId { get; }
    }

    public interface GetAllProductsResult
    {
        int CorrelationId { get; set; }
        int UserId { get; set; }
        bool isSuccessful { get; set; }
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        decimal Price { get; set; }
        int Stock { get; set; }
        string Image {  get; set; }
        decimal Discount { get; set; }
        string ProductCode { get; set; }
        List<Reviews> Reviews { get; set; }
    }

    public interface Reviews
    {
        int Id { get; set; }
        int Name { get; set; }
        string? Description { get; set; }
        int Rating { get; set; }
    }
}
