using Microsoft.AspNetCore.Mvc;
using Solid.Ecommerce.Caching;

namespace Solid.Ecommerce.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;

    }
    /*Return all products*/
    //GET: api/products
    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Product>))]
    public async Task<IEnumerable<Product>> GetAllProducts()
        => await _productService.GetAllAsync();


    /*Return a product by ID*/
    //GET: api/products/[id]  => localhost:3000/api/products/1

    [HttpGet("{id}", Name = nameof(GetProductById))] //name for route
    [ProducesResponseType(200, Type = typeof(Product))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProductById(int? id)
    {
        Product? p = await _productService.GetOneAsync(id);
        if (p == null)
        {
            return NotFound(); //return 404
        }
        else
        {
            return Ok(p);
        }
    }

    /*Create a product*/
    //POST: api/products/[product]

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Product))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Product p)
    {
        try
        {
            if (p == null)
            {
                return BadRequest();
            }

            //add p to db
            await _productService.AddAsync(p);
            // Sau khi insert p vao DB xong -> return chi tiet cua san pham p do
            return CreatedAtRoute(
                routeName: nameof(GetProductById),
                routeValues: new { id = p.ProductId, },
                value: p

                );

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
            //Exception handling
        }

    }

    /*Update a product by ID*/
    //PUT/PATCH (update): api/products/[id]
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int? id, [FromBody] Product p)
    {
        if (p == null || p.ProductId != id)
        {
            return BadRequest(); //400 Badrequest
        }
        //log at here
        //Get product by id from db
        Product? existing = await _productService.GetOneAsync(id);
        if (existing == null)
            return NotFound(); //404 code error

        //execute
        await _productService.UpdateAsync(p);
        return new NoContentResult(); //204

    }

    /*Delete a product by ID*/
    //DELETE: api/products/[id] : remove by ID (clean)
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Delete(int? id)
    {
        try
        {
            Product? existing = await _productService.GetOneAsync(id);
            if (existing == null) //khong co product nao trong db ma co id
            {
                //ProblemDetails: la doi tuong chua cac noi dung mo ta va ta muon return
                ProblemDetails problemDetail = new()
                {
                    
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://mydomain/api/products/failed-to-delete",
                    Title = $"Product Id {id} found but failed to delete.",
                    Detail = "Detail for....",
                    Instance = HttpContext.Request.Path,
                   

                };
                return BadRequest(problemDetail);
            }
            //if found
            await _productService.DeleteAsync(id);
            return new NoContentResult();//204 no contents

        }
        catch (Exception ex)
        {
            return BadRequest($"Product Id {id} was not found but failed to delete....");//400 code
        }

    }
}
