using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookService.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
    }
}