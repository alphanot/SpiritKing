using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpiritKing.Controllers.Models
{
    public class Input
    {
        public string Name { get; set; }

        public GamePadState state { get; set; }
    }
}
