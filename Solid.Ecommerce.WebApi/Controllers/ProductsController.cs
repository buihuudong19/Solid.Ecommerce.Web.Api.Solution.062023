namespace Solid.Ecommerce.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController:ControllerBase
{
    private readonly IProductService _productService;
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    /*Return all products*/
    //GET: api/products
    [HttpGet]
    [ProducesResponseType(200,Type = typeof(IEnumerable<Product>))]
    public async Task<IEnumerable<Product>> GetAllProducts()
        => await _productService.GetAllAsync();
}
