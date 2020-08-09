using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FytSoa.Core.Model.Music
{
    public class SearchInput
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Author { get; set; }

        [Range(1, 50)]
        public int Top { get; set; }

        public SearchInput()
        {
            Top = 10;
        }
    }
}
