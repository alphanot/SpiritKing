using System.Collections.Generic;

namespace SpiritKing.Components.Interfaces;
public interface INode
{
    List<INode> Children { get; set; }
}
