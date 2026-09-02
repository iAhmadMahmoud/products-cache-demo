using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductsCacheDemo.Features.Products.Commands;
using ProductsCacheDemo.Features.Products.Dtos;
using ProductsCacheDemo.Features.Products.Queries;

namespace ProductsCacheDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _sender;

        public ProductsController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _sender.Send(new GetAllProductsQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _sender.Send(new GetProductByIdQuery(id));
            if (result is null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]int Id, [FromBody] UpdateProductDto dto) {
            var updateProduct = await _sender.Send(new UpdateProductCommand(Id, dto));

            if (updateProduct is null)
            {
                return NotFound($"Product with ID {Id} not found.");
            }
            return Ok(updateProduct);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _sender.Send(new DeleteProductCommand(id));
            if (!deleted)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return NoContent();
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategoryId(int categoryId)
        {
            var result = await _sender.Send(new GetProductsByCategoryIdQuery(categoryId));
            return Ok(result);
        }
         
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var createdProduct = await _sender.Send(new CreateProductCommand(dto));
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
        }
    }
}
