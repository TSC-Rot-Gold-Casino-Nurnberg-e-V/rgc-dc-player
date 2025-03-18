using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentDJ.Model
{
    public class Tag
    {
        [Key]
        public string Name { get; set; }
        //Represents the Colors ARGB Value
        public int Color { get; set; }

        public static readonly Color DefaultColor = System.Drawing.Color.Gray;

        public Tag()
        {
            Name = "New Tag";
            Color = DefaultColor.ToArgb();
        }
    }
}
