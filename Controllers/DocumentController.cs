using InstaportApi.Data;
using InstaportApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InstaportApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiderDocumentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiderDocumentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RiderDocuments
        [HttpGet("all")]
        public async Task<IActionResult> GetAllDocuments()
        {
            var documents = await _context.rider_documents
                .AsNoTracking()
                .ToListAsync();

            return Ok(new { error = false, message = "Documents fetched successfully", documents });
        }

        // GET: api/RiderDocuments/rider/{riderId}
        [HttpGet("rider/{riderId}")]
        public async Task<IActionResult> GetDocumentsByRider(Guid riderId)
        {
            var documents = await _context.rider_documents
                .AsNoTracking()
                .Where(d => d.rider_id == riderId)
                .ToListAsync();

            if (documents.Count == 0)
                return Ok(new { error = true, message = "No documents found for this rider", documents = new List<rider_documents>() });

            return Ok(new { error = false, message = "Documents fetched successfully", documents });
        }

        // GET: api/RiderDocuments/{documentId}
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument(Guid documentId)
        {
            var document = await _context.rider_documents
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.document_id == documentId);

            if (document == null)
                return NotFound(new { error = true, message = "Document not found" });

            return Ok(new { error = false, message = "Document fetched successfully", document });
        }

        // POST: api/RiderDocuments/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateDocument([FromBody] rider_documents document)
        {
            document.document_id = Guid.NewGuid();
            _context.rider_documents.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Document created successfully", document });
        }

        // PUT: api/RiderDocuments/update/{documentId}
        [HttpPut("update/{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] rider_documents updatedDocument)
        {
            if (documentId != updatedDocument.document_id)
                return BadRequest(new { error = true, message = "Document ID mismatch" });

            var existingDoc = await _context.rider_documents.FirstOrDefaultAsync(d => d.document_id == documentId);
            if (existingDoc == null)
                return NotFound(new { error = true, message = "Document not found" });

            _context.Entry(existingDoc).CurrentValues.SetValues(updatedDocument);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Document updated successfully", document = updatedDocument });
        }

        // DELETE: api/RiderDocuments/delete/{documentId}
        [HttpDelete("delete/{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId)
        {
            var document = await _context.rider_documents.FirstOrDefaultAsync(d => d.document_id == documentId);
            if (document == null)
                return NotFound(new { error = true, message = "Document not found" });

            _context.rider_documents.Remove(document);
            await _context.SaveChangesAsync();

            return Ok(new { error = false, message = "Document deleted successfully" });
        }
    }
}
